using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.WindowsAPICodePack.Shell;
using SNROI.Models;
using SNROI.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.Wizards;
using GalaSoft.MvvmLight;
using SNROI.Enums;

namespace SNROI.ViewModels
{
    [HighlightedClass]
    public class ROIDocumentViewModel : ViewModelBase
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
                if (!ImageList.Contains(existingFile))
                    ImageList.Add(existingFile);
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
            get
            {
                if (roiDocument == null)
                {

                    var roiDoc = roiDocument ?? (roiDocument = string.IsNullOrEmpty(DocumentPath)
                          ? new ROIDocument()
                          : LoadROIDocumentFile(DocumentPath));
                    //roiDoc.IsDirty = false;
                    return roiDoc;
                }

                return roiDocument;
            }

            set
            {
                roiDocument = value;
                RaisePropertyChanged(nameof(ROIDocument));

            }
        }

        public Machine SelectedMachine { get; set; }
        public Material SelectedMaterial { get; set; }

        public HourlyPerson SelectedHourlyPerson { get; set; }

        public ICommand SaveROIDocumentCommand => new RelayCommand(() => { SaveROIDocument(); }, CanSaveROIDocument);

        private bool CanSaveROIDocument()
        {
            var result = !string.IsNullOrEmpty(ROIDocument.DocumentName)
                         && !(ROIDocument.DocumentName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0);
            return result;

        }

        public bool SaveROIDocument()
        {
            DialogService.Instance.ShowProgressDialog();
            //New report
            if (string.IsNullOrEmpty(DocumentPath))
                if (!string.IsNullOrEmpty(DataDirectory))
                    DocumentPath = Path.Combine(DataDirectory, ROIDocument.DocumentName + ".xml");

            if (File.Exists(DocumentPath) && IsNewReport)
            {
                DialogService.Instance.HideProgressDialog();

                if (!DialogService.Instance.ShowMessageQuestion($"{ROIDocument.DocumentName} already exists. Overwrite?", "File Exists"))
                {
                    DialogService.Instance.CloseProgressDialog();
                    return false;
                }

                DialogService.Instance.UnhideProgressDialog();
            }

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
                DialogService.Instance.CloseProgressDialog();
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
            //if (ROIDocument.IsDirty)
            //{
            //    if (DialogService.Instance.ShowMessageQuestion($"Save changes to {ROIDocument.DocumentName}?",
            //        "Save Changes"))
            //    {
            //        SaveROIDocument();

            //    }
            //}
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
            RaisePropertyChanged(nameof(ImageList));
        }

        public ICommand OpenMachineEditWindowCommand => new RelayCommand<object>(OpenMachineEditWindow);

        private void OpenMachineEditWindow(object o)
        {
            var machine = (o as Machine);
            NewOrEditMachine(machine);

            //  RaisePropertyChanged(nameof(ROIDocument.MachinesListCollection));
        }

        private void NewOrEditMachine(Machine machine)
        {
            var originalmachine = machine;
            DialogService.Instance.ShowMachineSetupWindow(ref machine);

            var indexOfMachine = ROIDocument.MachinesListCollection.IndexOf(originalmachine);
            if (indexOfMachine > -1)
            {
                ROIDocument.MachinesListCollection[indexOfMachine] = machine;
            }
            else
            {
                if (machine != null)
                    ROIDocument.MachinesListCollection.Add(machine);
            }
        }

        public ICommand DeleteSelectedMachineCommand => new RelayCommand(DeleteSelectedMachine);

        private void DeleteSelectedMachine()
        {
            if (SelectedMachine != null)
            {
                ROIDocument.MachinesListCollection.Remove(SelectedMachine);

            }
        }
        public ICommand EditSelectedMachineCommand => new RelayCommand(EditSelectedMachine);

        private void EditSelectedMachine()
        {
            if (SelectedMachine != null)
            {
                NewOrEditMachine(SelectedMachine);
            }
        }
        public ICommand DeleteSelectedMaterialCommand => new RelayCommand(DeleteSelectedMaterial);

        private void DeleteSelectedMaterial()
        {
            if (SelectedMaterial != null)
            {
                ROIDocument.MaterialsListCollection.Remove(SelectedMaterial);

            }
        }
        public ICommand DeleteSelectedPersonCommand => new RelayCommand(DeleteSelectedPerson);

        private void DeleteSelectedPerson()
        {
            if (SelectedHourlyPerson != null)
            {
                ROIDocument.PeopleListCollection.Remove(SelectedHourlyPerson);

            }
        }

        public ReportType ReportType => ReportType.Standard;
    }
}
