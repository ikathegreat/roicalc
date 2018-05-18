using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SNROI.Models;

namespace SNROI.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {

        public MainWindowViewModel()
        {
            FSROIDocList = new ObservableCollection<FileSystemROIDocument>();
            GridSelectedFSROIDocList = new ObservableCollection<FileSystemROIDocument>();
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
                FSROIDocList.Clear();
                foreach (var file in Directory.GetFiles(DataDirectory, "*.*", SearchOption.TopDirectoryOnly).ToList())
                {
                    FSROIDocList.Add(new FileSystemROIDocument(file));
                }

                TotalReportsMessage = FSROIDocList.Count + " reports";
            }
            finally
            {
                //DialogService.Instance.CloseProgressDialog();
            }

        }

        public ObservableCollection<FileSystemROIDocument> FSROIDocList { get; set; }

        private ObservableCollection<FileSystemROIDocument> gridSelectedFSROIDocList;

        public ObservableCollection<FileSystemROIDocument> GridSelectedFSROIDocList
        {
            get
            {
                SelectedReportsMessage = gridSelectedFSROIDocList.Count + " selected";
                return gridSelectedFSROIDocList;
            }
            set => gridSelectedFSROIDocList = value;
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


        public ICommand NewROIDocumentCommand => new RelayCommand(NewROIDocument);
        private void NewROIDocument()
        {
            DialogService.Instance.ShowOpenROIDocumentDialog(DataDirectory);
            ScanFileSystemForROIDocuments();
        }

        public ICommand OpenROIDocumentCommand => new RelayCommand(OpenROIDocument, CanOpenROIDocument);
        private bool CanOpenROIDocument() { return GridSelectedFSROIDocList.Count == 1; }
        public void OpenROIDocument()
        {
            DialogService.Instance.ShowOpenROIDocumentDialog(DataDirectory, GridSelectedFSROIDocList[0].FilePath);
        }
        public ICommand DeleteROIDocumentsCommand => new RelayCommand(DeleteROIDocuments, CanDeleteROIDocuments);
        private bool CanDeleteROIDocuments() { return GridSelectedFSROIDocList.Count > 0; }
        private void DeleteROIDocuments()
        {
            var messagePrompt = GridSelectedFSROIDocList.Count == 1
                ? "Are you sure you want to delete " + GridSelectedFSROIDocList[0].ROIDocumentName + "?"
                : "Are you sure you want to delete " + GridSelectedFSROIDocList.Count + " reports?";
            if (!DialogService.Instance.ShowMessageQuestion(messagePrompt, "Delete Reports"))
                return;
            foreach (var report in GridSelectedFSROIDocList)
            {
                File.Delete(report.FilePath);
            }
            ScanFileSystemForROIDocuments();
        }
        public ICommand CloneROIDocumentCommand => new RelayCommand(CloneROIDocument, CanCloneROIDocument);
        private bool CanCloneROIDocument() { return GridSelectedFSROIDocList.Count == 1; }
        private void CloneROIDocument()
        {
            var reportName = DialogService.Instance.InputDialog("Clone Report", "Enter new report name:");
            var newReportFilePath = Path.Combine(Path.GetDirectoryName(GridSelectedFSROIDocList[0].FilePath), reportName + ".xml");
            if (File.Exists(newReportFilePath))
            {
                DialogService.Instance.ShowMessageError("Error: File already exists: " + Environment.NewLine + Environment.NewLine + newReportFilePath);
            }
            else
            {
                File.Copy(GridSelectedFSROIDocList[0].FilePath, newReportFilePath);
                ScanFileSystemForROIDocuments();
            }
        }

        public ICommand OpenAboutDialogCommand => new RelayCommand(OpenAboutDialog);
        private static void OpenAboutDialog()
        {
            DialogService.Instance.ShowAboutDialog();
        }

        public ICommand OpenReportsDialogCommand => new RelayCommand(OpenReportsDialog, CanOpenReportsDialog);
        private bool CanOpenReportsDialog() { return GridSelectedFSROIDocList.Count > 0; }
        private void OpenReportsDialog()
        {
            DialogService.Instance.ShowReportsDialog(DataDirectory);
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
