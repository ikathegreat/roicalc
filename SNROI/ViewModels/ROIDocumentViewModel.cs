﻿using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.WindowsAPICodePack.Shell;
using SNROI.Models;
using SNROI.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.Wizards;

namespace SNROI.ViewModels
{
    [HighlightedClass]
    public class ROIDocumentViewModel : BaseViewModel
    {
        public ROIDocumentViewModel()
        {
        }

        public void LoadExistingImages()
        {
            var imageDirectory = Path.Combine(DataDirectory, Constants.ImagesDirectoryName);
            if (string.IsNullOrEmpty(imageDirectory))
                return;
            if (!Directory.Exists(imageDirectory))
                return;

            var existingFiles = Directory.GetFiles(imageDirectory);
            foreach (var existingFile in existingFiles)
            {
                var fileNameNoExt = Path.GetFileName(existingFile);
                if (!ImageList.Contains(fileNameNoExt))
                    ImageList.Add(fileNameNoExt);
            }
            //Todo: Bug with removing an image and not removing it from the list
        }

        private ObservableCollection<string> imageList;
        public ObservableCollection<string> ImageList
        {
            get => imageList ?? (imageList = new ObservableCollection<string>());
            set => imageList = value;
        }
        private ObservableCollection<string> languagesList;
        public ObservableCollection<string> LanguagesList =>
            languagesList ?? (languagesList = new ObservableCollection<string> { "United States", "Korea", "Japan", "Germany" });


        public string DocumentPath { get; set; }
        public string DataDirectory { get; set; }
        public bool IsNewReport { get; set; }
        public List<string> CompaniesList { get; set; }

        private ROIDocument roiDocument;

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

        public ICommand SaveROIDocumentCommand => new RelayCommand(SaveROIDocumentAndClose, CanSaveROIDocument);

        private bool CanSaveROIDocument()
        {
            var result = !string.IsNullOrEmpty(ROIDocument.DocumentName)
                         && !(ROIDocument.DocumentName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0);
            return result;

        }
        private void SaveROIDocumentAndClose()
        {
            SaveROIDocument();
            FireCloseRequest();
        }

        public bool SaveROIDocument()
        {
            DialogService.Instance.ShowProgressDialog();
            //New report
            if (string.IsNullOrEmpty(DocumentPath))
                if (!string.IsNullOrEmpty(DataDirectory))
                    DocumentPath = Path.Combine(DataDirectory, ROIDocument.DocumentName + ".xml");

            if (File.Exists(DocumentPath) && IsNewReport)
                if (!DialogService.Instance.ShowMessageQuestion(
                    $"{ROIDocument.DocumentName} already exists. Overwrite?", "File Exists"))
                    return false;

            var directoryName = Path.GetDirectoryName(DocumentPath);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(DocumentPath);

            ROIDocument.DateModified = DateTime.Now;

            TextWriter writer = new StreamWriter(DocumentPath);
            var xmlSerializer = new XmlSerializer(typeof(ROIDocument));

            try
            {
                xmlSerializer.Serialize(writer, ROIDocument);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while writing to XML: {ex.Message}");
                return false;
            }
            finally
            {
                writer.Close();
            }

            DialogService.Instance.CloseProgressDialog();
            return true;
        }

        public string FileSize
        {
            get
            {
                long fileSizeLength = 0;
                if (!string.IsNullOrEmpty(DocumentPath))
                {
                    if (File.Exists(DocumentPath))
                    {
                        try
                        {
                            fileSizeLength = (new FileInfo(DocumentPath)).Length;
                        }
                        catch { }
                    }
                }
                return BytesToFormattedString(fileSizeLength);
            }

        }
        private static string BytesToFormattedString(long value)
        {
            if (value < 0) { return "-" + BytesToFormattedString(-value); }
            if (value == 0) { return "0.0 bytes"; }

            var mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            return $"{adjustedSize:n0} {SizeSuffixes[mag]}";
        }

        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };




        public ICommand EditReportCommand => new RelayCommand(EditReport);

        private void EditReport()
        {
            if (SaveROIDocument())
            {
                var roiDocumentViewModel = new ROIDocumentViewModel
                {
                    DocumentPath = DocumentPath,
                    DataDirectory = DataDirectory
                };
                var tempFSROIDocList = new ObservableCollection<ROIDocumentViewModel> { roiDocumentViewModel };
                DialogService.Instance.ShowReportsDialog(DataDirectory, tempFSROIDocList);
            }
            else
            {
                DialogService.Instance.ShowMessageError("Failed to save document for preview");
            }
        }

        public ICommand CancelCommand => new RelayCommand(CancelDocumentEdit);

        private void CancelDocumentEdit()
        {
            FireCloseRequest();
        }


        public static ROIDocument LoadROIDocumentFile(string documentPath)
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
                DialogService.Instance.ShowMessageError("Can't find this file: " + documentPath);
            }

            return roiDocument;
        }


        public ICommand OpenImagesWindowCommand => new RelayCommand(OpenImagesWindow);

        private void OpenImagesWindow()
        {
            DialogService.Instance.ShowImageBrowserWindow(Path.Combine(DataDirectory, Constants.ImagesDirectoryName));
            LoadExistingImages();
            FirePropertyChanged(nameof(ImageList));
        }

        public ReportType ReportType => ReportType.Standard;
    }
}
