using GalaSoft.MvvmLight.CommandWpf;
using SNROI.Models;
using SNROI.ViewModels.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using SNROI.Tools;

namespace SNROI.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel()
        {
            roiDocViewModelList = new ObservableCollection<ROIDocumentViewModel>();
            GridSelectedROIViewModelList = new ObservableCollection<ROIDocumentViewModel>();
        }

        public MainWindowViewModel(string aDataDirectory) : this()
        {
            DataDirectory = aDataDirectory;
            ScanFileSystemForROIDocuments();
        }

        public string DataDirectory { get; set; }

        private void ScanFileSystemForROIDocuments()
        {
            //DialogService.Instance.ShowProgressDialog();

            try
            {
                if (!Directory.Exists(DataDirectory))
                    Directory.CreateDirectory(DataDirectory);
                roiDocViewModelList.Clear();
                foreach (var file in Directory.GetFiles(DataDirectory, "*.*", SearchOption.TopDirectoryOnly).ToList())
                {
                    var roiDocumentViewModel = new ROIDocumentViewModel
                    {
                        DocumentPath = file,
                        DataDirectory = DataDirectory
                    };
                    roiDocViewModelList.Add(roiDocumentViewModel);
                }

                TotalReportsMessage = roiDocViewModelList.Count + " reports";
            }
            finally
            {
                //DialogService.Instance.CloseProgressDialog();
            }
        }

        public ObservableCollection<ROIDocumentViewModel> roiDocViewModelList { get; set; }

        private ObservableCollection<ROIDocumentViewModel> gridSelectedROIViewModelList;

        public ObservableCollection<ROIDocumentViewModel> GridSelectedROIViewModelList
        {
            get
            {
                SelectedReportsMessage = gridSelectedROIViewModelList.Count + " selected";
                SelectedReportName = gridSelectedROIViewModelList.Count == 1
                    ? gridSelectedROIViewModelList.First().ROIDocument.DocumentName
                    : string.Empty;
                return gridSelectedROIViewModelList;
            }
            set => gridSelectedROIViewModelList = value;
        }

        private string totalReportsMessage;

        public string TotalReportsMessage
        {
            get => totalReportsMessage;
            set
            {
                totalReportsMessage = value;
                FirePropertyChanged(nameof(TotalReportsMessage));
            }
        }

        private string selectedReportsMessage;

        public string SelectedReportsMessage
        {
            get => selectedReportsMessage;
            set
            {
                selectedReportsMessage = value;
                FirePropertyChanged(nameof(SelectedReportsMessage));
            }
        }

        private string selectedReportName;

        public string SelectedReportName
        {
            get => selectedReportName;
            set
            {
                selectedReportName = value;
                FirePropertyChanged(nameof(SelectedReportName));

            }
        }


        public ICommand NewROIDocumentCommand => new RelayCommand(NewROIDocument);

        private void NewROIDocument()
        {
            DialogService.Instance.ShowOpenROIDocumentDialog(DataDirectory);
            ScanFileSystemForROIDocuments();
        }

        public ICommand OpenROIDocumentCommand => new RelayCommand(OpenROIDocument, CanOpenROIDocument);

        private bool CanOpenROIDocument()
        {
            return GridSelectedROIViewModelList.Count == 1;
        }

        public void OpenROIDocument()
        {
            DialogService.Instance.ShowOpenROIDocumentDialog(DataDirectory, GridSelectedROIViewModelList[0].DocumentPath);
            ScanFileSystemForROIDocuments();
        }

        public ICommand DeleteROIDocumentsCommand => new RelayCommand(DeleteROIDocuments, CanDeleteROIDocuments);

        private bool CanDeleteROIDocuments()
        {
            return GridSelectedROIViewModelList.Count > 0;
        }

        private void DeleteROIDocuments()
        {
            var messagePrompt = GridSelectedROIViewModelList.Count == 1
                ? "Are you sure you want to delete " + GridSelectedROIViewModelList[0].ROIDocument.DocumentName + "?"
                : "Are you sure you want to delete " + GridSelectedROIViewModelList.Count + " reports?";
            if (!DialogService.Instance.ShowMessageQuestion(messagePrompt, "Delete Reports"))
                return;
            foreach (var report in GridSelectedROIViewModelList)
            {
                File.Delete(report.DocumentPath);
            }
            ScanFileSystemForROIDocuments();
        }

        public ICommand CloneROIDocumentCommand => new RelayCommand(CloneROIDocument, CanCloneROIDocument);

        private bool CanCloneROIDocument()
        {
            return GridSelectedROIViewModelList.Count == 1;
        }

        private void CloneROIDocument()
        {
            var selectedROIViewModel = GridSelectedROIViewModelList[0];
            if (selectedROIViewModel == null)
                return;
            var suggestedFileNameNoExt = Path.GetFileNameWithoutExtension(FileSystemTools.GetNextAvailableFilename(Path.Combine(DataDirectory,
                    Path.GetFileNameWithoutExtension(selectedROIViewModel.DocumentPath) + "-copy.xml")));
            var reportName = DialogService.Instance.InputDialog("Clone Report", "","Enter new report name:",
                suggestedFileNameNoExt);
            if (string.IsNullOrEmpty(reportName))
                return;
            var newReportFilePath = Path.Combine(Path.GetDirectoryName(selectedROIViewModel.DocumentPath), reportName + ".xml");
            if (File.Exists(newReportFilePath))
            {
                DialogService.Instance.ShowMessageError("Error: File already exists: " + Environment.NewLine + Environment.NewLine + newReportFilePath);
            }
            else
            {
                File.Copy(GridSelectedROIViewModelList[0].DocumentPath, newReportFilePath);
                var roiDocumentViewModel = new ROIDocumentViewModel
                {
                    DocumentPath = newReportFilePath,
                    DataDirectory = DataDirectory,
                    ROIDocument = { DocumentName = reportName, DateCreated = DateTime.Now, DateModified = DateTime.Now }
                };
                roiDocumentViewModel.SaveROIDocument();
                ScanFileSystemForROIDocuments();
            }
        }

        public ICommand OpenAboutDialogCommand => new RelayCommand(OpenAboutDialog);

        private static void OpenAboutDialog()
        {
            DialogService.Instance.ShowAboutDialog();
        }

        public ICommand OpenReportsDialogCommand => new RelayCommand(OpenReportsDialog);

        private void OpenReportsDialog()
        {
            DialogService.Instance.ShowReportsDialog(DataDirectory, gridSelectedROIViewModelList);
        }

        public ICommand OpenReportEdtiorCommandDialogCommand => new RelayCommand(OpenReportEditorDialog);

        private static void OpenReportEditorDialog()
        {
            DialogService.Instance.ShowReportEditorDialog();
        }

        public ICommand OpenReportsDirectoryCommand => new RelayCommand(OpenReportsDirectory);

        private void OpenReportsDirectory()
        {
            Process.Start(DataDirectory);
        }
    }
}