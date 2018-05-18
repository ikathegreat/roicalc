using Microsoft.WindowsAPICodePack.Shell;
using SNROI.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;

namespace SNROI.ViewModels
{
    public class ROIDocumentViewModel : BaseViewModel
    {
        public ROIDocumentViewModel()
        {
            CultureCurrencyPairs = new ObservableCollection<CultureCurrencyPair>
            {
                new CultureCurrencyPair {Country = "United States", CultureCode = "en-US"},
                new CultureCurrencyPair {Country = "Korea", CultureCode = "kr-KO"},
                new CultureCurrencyPair {Country = "Germany", CultureCode = "en-GB"},
                new CultureCurrencyPair {Country = "Japan", CultureCode = "ja-JP"}
            };

        }


        public ObservableCollection<CultureCurrencyPair> CultureCurrencyPairs
        {
            get => cultureCurrencyPairs;
            set
            {
                cultureCurrencyPairs = value;
                //FirePropertyChanged(nameof(ROIDocument.CultureCurrencyPair));
            }
        }

        public string DocumentPath { get; set; }
        public string DataDirectory { get; set; }

        private ROIDocument roiDocument;
        private ObservableCollection<CultureCurrencyPair> cultureCurrencyPairs;

        public ROIDocument ROIDocument
        {
            get => roiDocument ?? (roiDocument = string.IsNullOrEmpty(DocumentPath)
                       ? new ROIDocument()
                       : LoadROIDocumentFile(DocumentPath));
            set
            {
                roiDocument = value;
                FirePropertyChanged(nameof(ROIDocument));

            }
        }

        public ICommand SaveROIDocumentCommand => new RelayCommand(SaveROIDocument, CanSaveROIDocument);

        private bool CanSaveROIDocument()
        {
            var result = !string.IsNullOrEmpty(ROIDocument.ReportName)
                         && !(ROIDocument.ReportName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0);
            return result;

        }
        private void SaveROIDocument()
        {
            //Todo: Check if file already exists
            if (string.IsNullOrEmpty(DocumentPath))
                if (!string.IsNullOrEmpty(DataDirectory))
                    DocumentPath = Path.Combine(DataDirectory, ROIDocument.ReportName + ".xml");

            var directoryName = Path.GetDirectoryName(DocumentPath);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(DocumentPath);

            TextWriter writer = new StreamWriter(DocumentPath);
            var xmlSerializer = new XmlSerializer(typeof(ROIDocument));

            try
            {
                xmlSerializer.Serialize(writer, ROIDocument);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while writing to XML: " + ex.Message);
            }
            finally
            {
                writer.Close();
            }

            //Write Extra file property
            try
            {
                var file = ShellFile.FromFilePath(DocumentPath);
                file.Properties.System.Company.Value = ROIDocument.CompanyName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            FireCloseRequest();
        }



        public ICommand EditReportCommand => new RelayCommand(EditReport);

        private void EditReport()
        {
            var reportrepx = @"C:\Users\paul.ikeda.SIGMATEK\Desktop\Report1.repx";

            DialogService.Instance.ShowReportPreviewDialog(reportrepx);
   
        }

        public ICommand CancelCommand => new RelayCommand(CancelDocumentEdit);

        private void CancelDocumentEdit()
        {
            FireCloseRequest();
        }


        private static ROIDocument LoadROIDocumentFile(string documentPath)
        {
            var roiDocument = new ROIDocument();
            if (File.Exists(documentPath))
            {
                TextReader reader = new StreamReader(documentPath);
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(ROIDocument));
                    roiDocument = (ROIDocument)xmlSerializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    DialogService.Instance.ShowMessageError("Can't load this file: " + documentPath + Environment.NewLine
                                                            + Environment.NewLine + ex.Message);
                }
                finally
                {
                    reader.Close();
                }
            }
            else
            {
                DialogService.Instance.ShowMessageError("Can't file this file: " + documentPath);
            }

            return roiDocument;
        }


        public ICommand OpenImagesWindowCommand => new RelayCommand(OpenImagesWindow);

        private void OpenImagesWindow()
        {
            DialogService.Instance.ShowImageBrowserWindow(ROIDocument.ImageNameList, Path.Combine(DataDirectory, "Images"));
            FirePropertyChanged(nameof(ROIDocument.ImageNameList));
        }

    }
}
