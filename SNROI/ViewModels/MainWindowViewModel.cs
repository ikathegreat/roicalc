using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SNROI.Models;
using SNROI.Tools;
using SNROI.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SNROI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Public Fields

        public readonly string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.DataFolderName);

        #endregion Public Fields

        #region Observable Properties

        private ObservableCollection<ROIDocumentViewModel> selectedRoiDocViewModelList = new ObservableCollection<ROIDocumentViewModel>();
        public ObservableCollection<ROIDocumentViewModel> SelectedRoiDocViewModelList
        {
            get { return selectedRoiDocViewModelList; }
            set { Set(ref selectedRoiDocViewModelList, value); }
        }

        private ObservableCollection<ROIDocumentViewModel> roiDocViewModelList = new ObservableCollection<ROIDocumentViewModel>();
        public ObservableCollection<ROIDocumentViewModel> RoiDocViewModelList
        {
            get { return roiDocViewModelList; }
            set { Set(ref roiDocViewModelList, value); }
        }

        private ROIDocumentViewModel selectedRoiDocViewModel;
        public ROIDocumentViewModel SelectedRoiDocViewModel
        {
            get { return selectedRoiDocViewModel; }
            set { Set(ref selectedRoiDocViewModel, value); }
        }

        #endregion Observable Properties

        #region Constructor

        public MainWindowViewModel()
        {
            if (IsInDesignMode)
            {
                RoiDocViewModelList = new ObservableCollection<ROIDocumentViewModel>
                {
                    new ROIDocumentViewModel(){ROIDocument = new ROIDocument(){DocumentName = "Designer Doc 1"}},
                    new ROIDocumentViewModel(){ROIDocument = new ROIDocument(){DocumentName = "Designer Doc 2"}},
                    new ROIDocumentViewModel(){ROIDocument = new ROIDocument(){DocumentName = "Designer Doc 3"}}
                };
            }
            else
            {
                ScanFileSystemForROIDocuments();
                InstallFactoryReportTemplates();
            }
        }

        #endregion Constructor

        #region Commands

        private RelayCommand cloneROIDocumentCommand;
        public RelayCommand CloneROIDocumentCommand
        {
            get
            {
                return cloneROIDocumentCommand ??
                      (cloneROIDocumentCommand = new RelayCommand(CloneROIDocument, CanCloneROIDocument));
            }
        }

        private RelayCommand deleteROIDocumentsCommand;
        public RelayCommand DeleteROIDocumentsCommand
        {
            get
            {
                return deleteROIDocumentsCommand ??
                      (deleteROIDocumentsCommand = new RelayCommand(DeleteROIDocuments, CanDeleteROIDocuments));
            }
        }

        private RelayCommand newROIDocumentCommand;
        public RelayCommand NewROIDocumentCommand
        {
            get
            {
                return newROIDocumentCommand ??
                      (newROIDocumentCommand = new RelayCommand(NewROIDocument));
            }
        }

        private RelayCommand openAboutDialogCommand;
        public RelayCommand OpenAboutDialogCommand
        {
            get
            {
                return openAboutDialogCommand ??
                      (openAboutDialogCommand = new RelayCommand(OpenAboutDialog));
            }
        }

        private RelayCommand openApplicationDirectoryCommand;
        public RelayCommand OpenApplicationDirectoryCommand
        {
            get
            {
                return openApplicationDirectoryCommand ??
                      (openApplicationDirectoryCommand = new RelayCommand(OpenApplicationDirectory));
            }
        }

        private RelayCommand openReportEdtiorCommandDialogCommand;
        public RelayCommand OpenReportEdtiorCommandDialogCommand
        {
            get
            {
                return openReportEdtiorCommandDialogCommand ??
                      (openReportEdtiorCommandDialogCommand = new RelayCommand(OpenReportEditorDialog));
            }
        }

        private RelayCommand openReportsDialogCommand;
        public RelayCommand OpenReportsDialogCommand
        {
            get
            {
                return openReportsDialogCommand ??
                      (openReportsDialogCommand = new RelayCommand(OpenReportsDialog));
            }
        }

        private RelayCommand openReportsDirectoryCommand;
        public RelayCommand OpenReportsDirectoryCommand
        {
            get
            {
                return openReportsDirectoryCommand ??
                      (openReportsDirectoryCommand = new RelayCommand(OpenReportsDirectory));
            }
        }

        private RelayCommand openROIDocumentCommand;
        public RelayCommand OpenROIDocumentCommand
        {
            get
            {
                return openROIDocumentCommand ??
                      (openROIDocumentCommand = new RelayCommand(OpenROIDocument, CanOpenROIDocument));
            }
        }

        private RelayCommand openROIDocumentSourceCommand;
        public RelayCommand OpenROIDocumentSourceCommand
        {
            get
            {
                return openROIDocumentSourceCommand ??
                      (openROIDocumentSourceCommand = new RelayCommand(OpenROIDocumentSource));
            }
        }

        private RelayCommand resetReportTemplatesCommand;
        public RelayCommand ResetReportTemplatesCommand
        {
            get
            {
                return resetReportTemplatesCommand ??
                      (resetReportTemplatesCommand = new RelayCommand(ResetReportTemplates));
            }
        }

        #endregion Commands

        #region Public Methods

        public List<string> GetCompaniesList()
        {
            var companyList = new List<string>();
            if (RoiDocViewModelList.Count > 0)
            {
                companyList = RoiDocViewModelList.Where(y => !string.IsNullOrWhiteSpace(y.ROIDocument.CompanyName))
                                                 .GroupBy(p => p.ROIDocument.CompanyName)
                                                 .Select(g => g.First())
                                                 .Select(x => x.ROIDocument.CompanyName)
                                                 .ToList();
                companyList.Sort();
            }
            return companyList;
        }

        public void OpenROIDocument()
        {
            if (SelectedRoiDocViewModel == null)
                return;

            if (DialogService.Instance.ShowOpenROIDocumentDialog(DataDirectory, GetCompaniesList(), SelectedRoiDocViewModel.DocumentPath))
                ScanFileSystemForROIDocuments();
        }

        public void OpenROIDocumentSource()
        {
            var doc = SelectedRoiDocViewModelList.FirstOrDefault();

            if (doc != null)
                Process.Start(doc.DocumentPath);
        }

        #endregion Public Methods

        #region Private Methods

        private static void OpenAboutDialog()
        {
            DialogService.Instance.ShowAboutDialog();
        }

        private static void OpenApplicationDirectory()
        {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory);
        }

        private static void OpenReportEditorDialog()
        {
            DXReportHelper.EditReport();
        }

        private bool CanCloneROIDocument()
        {
            return SelectedRoiDocViewModelList.Count == 1;
        }

        private bool CanDeleteROIDocuments()
        {
            return SelectedRoiDocViewModelList.Count > 0;
        }

        private bool CanOpenROIDocument()
        {
            return SelectedRoiDocViewModelList.Count == 1;
        }

        private void CloneROIDocument()
        {
            var selectedROIViewModel = SelectedRoiDocViewModelList[0];
            if (selectedROIViewModel == null)
                return;

            var suggestedFileNameNoExt = Path.GetFileNameWithoutExtension(FileSystemTools.GetNextAvailableFilename(Path.Combine(DataDirectory,
                    Path.GetFileNameWithoutExtension(selectedROIViewModel.DocumentPath) + "-copy.xml")));

            var reportName = DialogService.Instance.InputDialog("Clone Report", "", "Enter new report name:",
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
                File.Copy(SelectedRoiDocViewModelList[0].DocumentPath, newReportFilePath);
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

        private void DeleteROIDocuments()
        {
            var messagePrompt = SelectedRoiDocViewModelList.Count == 1
                ? "Are you sure you want to delete " + SelectedRoiDocViewModelList[0].ROIDocument.DocumentName + "?"
                : "Are you sure you want to delete " + SelectedRoiDocViewModelList.Count + " reports?";

            if (!DialogService.Instance.ShowMessageQuestion(messagePrompt, "Delete Reports"))
                return;

            foreach (var report in SelectedRoiDocViewModelList)
            {
                File.Delete(report.DocumentPath);
            }

            ScanFileSystemForROIDocuments();
        }

        private void InstallFactoryReportTemplates(bool doFactoryReset = false)
        {
            var reportTemplateDirectory = Path.Combine(DataDirectory, Constants.ReportTemplateDirectoryName);
            if (!Directory.Exists(reportTemplateDirectory))
                Directory.CreateDirectory(reportTemplateDirectory);

            if ((Directory.GetFiles(reportTemplateDirectory, "*.repx", SearchOption.TopDirectoryOnly).Length == 0) || doFactoryReset)
            {
                var factoryTemplateFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
                foreach (var file in Directory.GetFiles(factoryTemplateFolder, "*.repx", SearchOption.TopDirectoryOnly).ToList())
                {
                    if (doFactoryReset)
                    {
                        var targetFilePath = Path.Combine(reportTemplateDirectory, Path.GetFileName(file));
                        try
                        {
                            if (DialogService.Instance.ShowMessageQuestion(
                                $"{Path.GetFileName(file)} already exists. Create a backup copy?", "Backup Template"))
                            {
                                var backupTargetFilePath = FileSystemTools.GetNextAvailableFilename(targetFilePath);
                                File.Move(targetFilePath, backupTargetFilePath);
                            }
                            File.Copy(file, targetFilePath, true);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else
                    {
                        File.Copy(file, Path.Combine(reportTemplateDirectory, Path.GetFileName(file)));
                    }
                }
            }
        }

        private void NewROIDocument()
        {
            if (DialogService.Instance.ShowOpenROIDocumentDialog(DataDirectory, GetCompaniesList()))
                ScanFileSystemForROIDocuments();
        }

        private void OpenReportsDialog()
        {
            DialogService.Instance.ShowReportsDialog(DataDirectory, selectedRoiDocViewModelList);
        }

        private void OpenReportsDirectory()
        {
            Process.Start(DataDirectory);
        }

        private void ResetReportTemplates()
        {
            if (DialogService.Instance.ShowMessageQuestion("Are you sure you want to re-install all factory report templates?" + Environment.NewLine + Environment.NewLine + "All custom reports will be preserved.", "Reset Report Templates"))
            {
                InstallFactoryReportTemplates(true);
                DialogService.Instance.ShowMessageInfo("Report templates restored to factory defaults");
            }
        }

        private void ScanFileSystemForROIDocuments()
        {
            try
            {
                if (!Directory.Exists(DataDirectory))
                    Directory.CreateDirectory(DataDirectory);

                var roiDocList = new List<ROIDocumentViewModel>();
                foreach (var file in Directory.GetFiles(DataDirectory, "*.*", SearchOption.TopDirectoryOnly).ToList())
                {
                    var roiDocumentViewModel = new ROIDocumentViewModel
                    {
                        DocumentPath = file,
                        DataDirectory = DataDirectory
                    };
                    roiDocList.Add(roiDocumentViewModel);
                }

                RoiDocViewModelList = new ObservableCollection<ROIDocumentViewModel>(roiDocList);
            }
            catch (Exception ex)
            {
                DialogService.Instance.ShowMessageError(ex);
            }
        }

        #endregion Private Methods
    }
}