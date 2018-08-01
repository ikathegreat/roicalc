using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.Wizards;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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

namespace SNROI.ViewModels
{
    [HighlightedClass]
    public class ROIDocumentViewModel : ViewModelBase
    {
        #region Private Fields

        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        #endregion Private Fields

        #region Observable Properties

        private List<string> companiesList;
        public List<string> CompaniesList
        {
            get { return companiesList; }
            set { Set(ref companiesList, value); }
        }

        private string dataDirectory;
        public string DataDirectory
        {
            get { return dataDirectory; }
            set { Set(ref dataDirectory, value); }
        }

        private string documentPath;
        public string DocumentPath
        {
            get { return documentPath; }
            set { Set(ref documentPath, value); }
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

        private ObservableCollection<string> imageList = new ObservableCollection<string>();
        public ObservableCollection<string> ImageList
        {
            get { return imageList; }
            set
            {
                if (imageList == value)
                    return;

                imageList = value;
                RaisePropertyChanged();
            }
        }

        private bool isNewReport;
        public bool IsNewReport
        {
            get { return isNewReport; }
            set { Set(ref isNewReport, value); }
        }

        private ObservableCollection<string> languagesList = new ObservableCollection<string> { "United States", "Korea", "Japan", "Germany" };
        public ObservableCollection<string> LanguagesList
        {
            get { return languagesList; }
            set
            {
                if (languagesList == value)
                    return;

                languagesList = value;
                RaisePropertyChanged();
            }
        }

        private ReportType reportType = ReportType.Standard;
        public ReportType ReportType
        {
            get { return reportType; }
            set { Set(ref reportType, value); }
        }

        private ROIDocument roiDocument;
        public ROIDocument ROIDocument
        {
            get
            {
                if (roiDocument != null)
                    return roiDocument;

                var roiDoc = roiDocument ?? (roiDocument = string.IsNullOrEmpty(DocumentPath)
                                 ? new ROIDocument()
                                 : LoadROIDocumentFile(DocumentPath));

                return roiDoc;
            }
            set
            {
                if (roiDocument == value)
                    return;

                roiDocument = value;
                RaisePropertyChanged();
            }
        }

        private HourlyPerson selectedHourlyPerson;
        public HourlyPerson SelectedHourlyPerson
        {
            get { return selectedHourlyPerson; }
            set { Set(ref selectedHourlyPerson, value); }
        }

        private Machine selectedMachine;
        public Machine SelectedMachine
        {
            get { return selectedMachine; }
            set { Set(ref selectedMachine, value); }
        }

        private Material selectedMaterial;
        public Material SelectedMaterial
        {
            get { return selectedMaterial; }
            set { Set(ref selectedMaterial, value); }
        }

        #endregion Observable Properties

        #region Commands

        private RelayCommand addNewHourlyPersonCommand;
        public RelayCommand AddNewHourlyPersonCommand
        {
            get
            {
                return addNewHourlyPersonCommand
                    ?? (addNewHourlyPersonCommand = new RelayCommand(
                    () =>
                    {
                        roiDocument.PeopleListCollection.Add(new HourlyPerson() { Name = $"Programmer {roiDocument.PeopleListCollection.Count + 1}" });
                    }));
            }
        }

        private RelayCommand addNewMachineWindowCommand;
        public RelayCommand AddNewMachineWindowCommand
        {
            get
            {
                return addNewMachineWindowCommand
                    ?? (addNewMachineWindowCommand = new RelayCommand(
                    () =>
                    {
                        var machine = new Machine() { Name = $"Plasma {roiDocument.MachinesListCollection.Count + 1}" };

                        if (DialogService.Instance.ShowMachineSetupWindow(machine))
                            ROIDocument.MachinesListCollection.Add(machine);
                    }));
            }
        }

        private RelayCommand addNewMaterialItemCommand;
        public RelayCommand AddNewMaterialItemCommand
        {
            get
            {
                return addNewMaterialItemCommand
                    ?? (addNewMaterialItemCommand = new RelayCommand(
                    () =>
                    {
                        roiDocument.MaterialsListCollection.Add(new Material() { Name = $"Material {roiDocument.MaterialsListCollection.Count + 1}" });
                    }));
            }
        }

        private RelayCommand cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                return cancelCommand
                    ?? (cancelCommand = new RelayCommand(
                    () =>
                    {
                        if (DialogService.Instance.ShowMessageQuestion($"Save changes to {ROIDocument.DocumentName}?", "Save Changes"))
                            SaveROIDocument();
                        else
                            ResetROIDocument();
                    }));
            }
        }

        private RelayCommand deleteSelectedMachineCommand;
        public RelayCommand DeleteSelectedMachineCommand
        {
            get
            {
                return deleteSelectedMachineCommand
                    ?? (deleteSelectedMachineCommand = new RelayCommand(
                    () =>
                    {
                        if (SelectedMachine == null)
                            return;

                        ROIDocument.MachinesListCollection.Remove(SelectedMachine);
                    }));
            }
        }

        private RelayCommand deleteSelectedMaterialCommand;
        public RelayCommand DeleteSelectedMaterialCommand
        {
            get
            {
                return deleteSelectedMaterialCommand
                    ?? (deleteSelectedMaterialCommand = new RelayCommand(
                    () =>
                    {
                        if (SelectedMaterial == null)
                            return;

                        ROIDocument.MaterialsListCollection.Remove(SelectedMaterial);
                    }));
            }
        }

        private RelayCommand deleteSelectedPersonCommand;
        public RelayCommand DeleteSelectedPersonCommand
        {
            get
            {
                return deleteSelectedPersonCommand
                    ?? (deleteSelectedPersonCommand = new RelayCommand(
                    () =>
                    {
                        if (SelectedHourlyPerson != null)
                        {
                            ROIDocument.PeopleListCollection.Remove(SelectedHourlyPerson);
                        }
                    }));
            }
        }

        private RelayCommand editMachineWindowCommand;
        public RelayCommand EditMachineWindowCommand
        {
            get
            {
                return editMachineWindowCommand
                    ?? (editMachineWindowCommand = new RelayCommand(
                    () =>
                    {
                        if (SelectedMachine == null)
                            return;

                        var machineCopy = SelectedMachine.Clone() as Machine;

                        if (DialogService.Instance.ShowMachineSetupWindow(machineCopy))
                        {
                            var machineList = ROIDocument.MachinesListCollection.ToList();
                            var index = machineList.IndexOf(SelectedMachine);
                            if (index > -1)
                                machineList[index] = machineCopy;

                            ROIDocument.MachinesListCollection = new ObservableCollection<Machine>(machineList);
                        }
                    }));
            }
        }

        private RelayCommand editReportCommand;
        public RelayCommand EditReportCommand
        {
            get
            {
                return editReportCommand ??
                      (editReportCommand = new RelayCommand(() =>
                      {
                          if (DialogService.Instance.ShowMessageQuestion($"Save changes to {ROIDocument.DocumentName}?", "Save Changes"))
                              SaveROIDocument();
                          else
                              ResetROIDocument();

                          var roiDocumentViewModel = new ROIDocumentViewModel { DocumentPath = DocumentPath, DataDirectory = DataDirectory };
                          var tempFSROIDocList = new ObservableCollection<ROIDocumentViewModel> { roiDocumentViewModel };

                          DialogService.Instance.ShowReportsDialog(DataDirectory, tempFSROIDocList);
                      }));
            }
        }

        private RelayCommand openImagesWindowCommand;
        public RelayCommand OpenImagesWindowCommand
        {
            get
            {
                return openImagesWindowCommand
                    ?? (openImagesWindowCommand = new RelayCommand(
                    () =>
                    {
                        DialogService.Instance.ShowImageBrowserWindow(Path.Combine(DataDirectory, Constants.ImagesDirectoryName));
                        LoadExistingImages();
                    }));
            }
        }

        private RelayCommand saveROIDocumentCommand;
        public RelayCommand SaveROIDocumentCommand
        {
            get
            {
                return saveROIDocumentCommand
                    ?? (saveROIDocumentCommand = new RelayCommand(SaveROIDocument, CanSaveROIDocument));
            }
        }

        #endregion Commands

        #region Public Methods

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

        public void LoadExistingImages()
        {
            var imageDirectory = Path.Combine(DataDirectory, Constants.ImagesDirectoryName);

            if (string.IsNullOrEmpty(imageDirectory))
                return;

            if (!Directory.Exists(imageDirectory))
                return;

            var existingFiles = Directory.GetFiles(imageDirectory);

            ImageList = new ObservableCollection<string>(existingFiles);
        }

        public void SaveROIDocument()
        {
            DialogService.Instance.ShowProgressDialog();

            if (string.IsNullOrEmpty(DocumentPath) && !string.IsNullOrEmpty(DataDirectory))
                DocumentPath = Path.Combine(DataDirectory, ROIDocument.DocumentName + ".xml");

            if (File.Exists(DocumentPath) && IsNewReport)
            {
                DialogService.Instance.HideProgressDialog();

                if (!DialogService.Instance.ShowMessageQuestion($"{ROIDocument.DocumentName} already exists. Overwrite?", "File Exists"))
                {
                    DialogService.Instance.CloseProgressDialog();
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
            }
            finally
            {
                writer.Close();
            }

            DialogService.Instance.CloseProgressDialog();
        }

        #endregion Public Methods

        #region Private Methods

        private static string BytesToFormattedString(long value)
        {
            if (value < 0) { return "-" + BytesToFormattedString(-value); }
            if (value == 0) { return "0.0 bytes"; }

            var mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            return $"{adjustedSize:n0} {SizeSuffixes[mag]}";
        }

        private bool CanSaveROIDocument()
        {
            var result = !string.IsNullOrEmpty(ROIDocument.DocumentName)
                         && !(ROIDocument.DocumentName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0);
            return result;
        }

        private void ResetROIDocument()
        {
            ROIDocument = LoadROIDocumentFile(DocumentPath);
        }

        #endregion Private Methods
    }
}