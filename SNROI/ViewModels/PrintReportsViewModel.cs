using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNROI.ViewModels
{
    public class PrintReportsViewModel : BaseViewModel
    {
        public PrintReportsViewModel()
        {

        }

        private string dataDirectory;
        public string DataDirectory
        {
            get => dataDirectory;
            set
            {
                dataDirectory = value;
                //foreach (var file in Directory.GetFiles(Path.Combine(dataDirectory,"Reports")))
                //{
                //    ReportList.Add(file);
                //}
            }
        }


        private ObservableCollection<string> reportList = new ObservableCollection<string>();

        public ObservableCollection<string> ReportList
        {
            get { return reportList; }
            set { reportList = value; }
        }

        private ObservableCollection<string> selectedReportsList = new ObservableCollection<string>();

        public ObservableCollection<string> SelectedReportsList
        {
            get => selectedReportsList;
            set => selectedReportsList = value;
        }


        public ICommand OpenReportEditorCommand => new RelayCommand(OpenReportEditor, CanOpenReportEditor);

        private bool CanOpenReportEditor() { return true/*Can only edit 1 report template at a time*/; }
        private void OpenReportEditor()
        {
            string selectedReportRepxFilePath = "";
            DialogService.Instance.ShowReportEditorDialog(selectedReportRepxFilePath);
        }
        public ICommand PrintReportsCommand => new RelayCommand(PrintReports);
        private static void PrintReports()
        {

        }
        public ICommand ExportReportsCommand => new RelayCommand(ExportReports);
        private static void ExportReports()
        {

        }

        public ICommand CloseWindowCommand => new RelayCommand(CloseWindow);
        private void CloseWindow()
        {
            FireCloseRequest();
        }
    }
}
