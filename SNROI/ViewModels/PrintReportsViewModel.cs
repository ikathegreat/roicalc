using GalaSoft.MvvmLight.CommandWpf;
using SNROI.Models;
using SNROI.ViewModels.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace SNROI.ViewModels
{
    public class PrintReportsViewModel : BaseViewModel
    {
        private enum ReportAction
        {
            Preview,
            Print,
            Export
        }

        private string dataDirectory;

        public string DataDirectory
        {
            get => dataDirectory;
            set
            {
                dataDirectory = value;

                PopulateReportTemplatesList();
            }
        }

        private void PopulateReportTemplatesList()
        {
            var reportsDirectory = Path.Combine(dataDirectory, "Reports");
            if (!Directory.Exists(reportsDirectory))
                Directory.CreateDirectory(reportsDirectory);

            ReportTemplateList.Clear();

            var checkedReportTemplates = LoadPreviouslySelectedReportTemplates();

            foreach (var file in Directory.GetFiles(reportsDirectory))
            {
                //Todo: Recall previously checked reported
                ReportTemplateList.Add(new CheckedListItem<string>()
                {
                    IsChecked = checkedReportTemplates.Contains(Path.GetFileNameWithoutExtension(file)),
                    Item = Path.GetFileNameWithoutExtension(file)
                });
            }
        }

        private List<string> LoadPreviouslySelectedReportTemplates()
        {
            var selectedReportsTemplatesFilePath = Path.Combine(dataDirectory, "AppSettings", "SelectedReportsTemplates.txt");
            var checkedReportTemplates = new List<string>();
            if (File.Exists(selectedReportsTemplatesFilePath))
                checkedReportTemplates.AddRange(File.ReadAllLines(selectedReportsTemplatesFilePath));
            return checkedReportTemplates;
        }

        /// <summary>
        /// Selected file system reports from main grid for export or printing
        /// </summary>
        public ObservableCollection<ROIDocumentViewModel> SelectedROIViewModelList { get; set; } = new ObservableCollection<ROIDocumentViewModel>();

        private ObservableCollection<CheckedListItem<string>> reportTemplateList;

        public ObservableCollection<CheckedListItem<string>> ReportTemplateList
        {
            get => reportTemplateList ?? (reportTemplateList = new ObservableCollection<CheckedListItem<string>>());
            set => reportTemplateList = value;
        }

        private ObservableCollection<string> selectedReportsList;

        /// <summary>
        /// Checked report templates which are used for export/preview only
        /// Edit template can only be handled one at a time
        /// </summary>
        public ObservableCollection<string> SelectedReportsList
        {
            get => selectedReportsList ?? new ObservableCollection<string>();
            set => selectedReportsList = value;
        }

        private CheckedListItem<string> selectedReportForEdit;

        /// <summary>
        /// Selected report template used for XtraReport editor
        /// (Single select only)
        /// </summary>
        public CheckedListItem<string> SelectedReportForEdit
        {
            get => selectedReportForEdit ?? new CheckedListItem<string>();
            set
            {
                selectedReportForEdit = value;
                FirePropertyChanged(nameof(SelectedReportForEdit));
            }
        }

        public ICommand OpenNewReportEditorCommand => new RelayCommand(OpenNewReportEditor);

        private void OpenNewReportEditor()
        {
            DialogService.Instance.ShowReportEditorDialog();
            PopulateReportTemplatesList();
            FirePropertyChanged(nameof(ReportTemplateList));
        }

        public ICommand OpenReportEditorCommand => new RelayCommand(OpenReportEditor);

        private void OpenReportEditor()
        {
            var selectedReportRepxFilePath = string.Empty;

            if (SelectedReportForEdit != null)
            {
                var repxFilePath = Path.Combine(dataDirectory, "Reports", SelectedReportForEdit.Item + ".repx");
                if (File.Exists(repxFilePath))
                    selectedReportRepxFilePath = repxFilePath;
            }
            DialogService.Instance.ShowReportEditorDialog(selectedReportRepxFilePath);
        }

        private bool CanPerformReportActions()
        {
            return SelectedROIViewModelList.Count > 0;
        }

        public ICommand PreviewReportsCommand => new RelayCommand(PreviewReports, CanPerformReportActions);

        private void PreviewReports()
        {
            ExecuteReportActionsOnSelectedReports(ReportAction.Preview);
        }

        public ICommand PrintReportsCommand => new RelayCommand(PrintReports, CanPerformReportActions);

        private void PrintReports()
        {
            ExecuteReportActionsOnSelectedReports(ReportAction.Print);
        }

        public ICommand ExportReportsCommand => new RelayCommand(ExportReports, CanPerformReportActions);

        private void ExportReports()
        {
            ExecuteReportActionsOnSelectedReports(ReportAction.Export);
        }

        public ICommand CloseWindowCommand => new RelayCommand(CloseWindow);

        private void CloseWindow()
        {
            SaveSelectedReportTemplates();
            FireCloseRequest();
        }

        private void SaveSelectedReportTemplates()
        {
            //This needs to be optimized.
            var checkedReportTemplates = new List<string>();
            foreach (var checkedListItem in ReportTemplateList)
            {
                if (checkedListItem.IsChecked)
                    checkedReportTemplates.Add(checkedListItem.Item);
            }
            var selectedReportsTemplatesFilePath = Path.Combine(dataDirectory, "AppSettings", "SelectedReportsTemplates.txt");
            File.WriteAllLines(selectedReportsTemplatesFilePath, checkedReportTemplates);
        }

        private void ExecuteReportActionsOnSelectedReports(ReportAction action)
        {
            //Iterate documents, then report template(s) for each doc.

            var checkedTemplatesCount = ReportTemplateList.Count(x => x.IsChecked);

            if (checkedTemplatesCount == 0)
            {
                DialogService.Instance.ShowMessage("Please select a report template in order to " + action.ToString().ToLower(),
                    "No Selected Report Templates");
                return;
            }

            foreach (var roiDocumentViewModel in SelectedROIViewModelList)
            {
                var roiDocument = ROIDocumentViewModel.LoadROIDocumentFile(roiDocumentViewModel.DocumentPath);
                if (roiDocument == null)
                {
                    DialogService.Instance.ShowMessageError("Unexpected error reading " + roiDocumentViewModel.DocumentPath);
                    continue;
                }

                foreach (var reportTemplate in ReportTemplateList)
                {
                    if (!reportTemplate.IsChecked)
                        continue;

                    var repxFilePath = Path.Combine(DataDirectory, "Reports", reportTemplate.Item + ".repx");
                    if (!File.Exists(repxFilePath))
                        continue;

                    DialogService.Instance.ShowMessage(action + " " + reportTemplate.Item + " for " + roiDocument.DocumentName);

                    if (action == ReportAction.Preview)
                    {
                        DialogService.Instance.ShowReportPreviewDialog(repxFilePath, roiDocument);
                    }
                }
            }

            CloseWindow();
        }
    }
}