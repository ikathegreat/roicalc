using DevExpress.DataAccess.ObjectBinding;
using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SNROI.Models
{
    [HighlightedClass]
    public class ROIDocument : ObservableObject
    {
        public ROIDocument()
        {
            Units = Units.Inches;
            Language = "en-US";
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
            Notes = string.Empty;
        }

        public ROIDocumentTimeSpans ROIDocumentTimeSpans => new ROIDocumentTimeSpans();

        public string DocumentName
        {
            get => documentName;
            set
            {
                documentName = value;
                RaisePropertyChanged(nameof(DocumentName));
            }
        }

        public string CompanyName
        {
            get => companyName;
            set
            {
                companyName = value;
                RaisePropertyChanged(nameof(CompanyName));
            }
        }

        public string ContactName
        {
            get => contactName;
            set
            {
                contactName = value;
                RaisePropertyChanged(nameof(ContactName));
            }
        }

        public string CompanyLogoImage
        {
            get => companyLogoImage;
            set
            {
                companyLogoImage = value;
                RaisePropertyChanged(nameof(CompanyLogoImage));
            }
        }

        public Units Units
        {
            get => units;
            set
            {
                units = value;
                RaisePropertyChanged(nameof(Units));
            }
        }

        public DateTime DateCreated
        {
            get => dateCreated;
            set
            {
                dateCreated = value;
                RaisePropertyChanged(nameof(DateCreated));
            }

        }

        public DateTime DateModified
        {
            get => dateModified;
            set
            {
                dateModified = value;
                RaisePropertyChanged(nameof(DateModified));
            }
        }

        public string Notes
        {
            get => notes;
            set
            {
                notes = value;
                RaisePropertyChanged(nameof(Notes));
            }
        }

        private ROIDocumentCalculations roiDocumentCalculations;
        private ObservableCollection<Material> _materialsListCollection = new ObservableCollection<Material>();
        private ObservableCollection<HourlyPerson> _peopleListCollection = new ObservableCollection<HourlyPerson>();
        private ObservableCollection<Machine> _machinesListCollection = new ObservableCollection<Machine>();
        private string documentName;
        private string companyName;
        private string contactName;
        private string companyLogoImage;
        private Units units;
        private DateTime dateCreated;
        private DateTime dateModified;
        private string notes;
        private string language;

        private double percentMaterialUsageSavedPerYear= 5.00;

        public double PercentMaterialUsageSavedPerYear
        {
            get { return percentMaterialUsageSavedPerYear; }
            set { percentMaterialUsageSavedPerYear = value; }
        }

        private double machineMinutesSavedPerHour = 1.00;

        public double MachineMinutesSavedPerHour
        {
            get { return machineMinutesSavedPerHour; }
            set { machineMinutesSavedPerHour = value; }
        }
        private double programmingMinutesSavedPerHour = 1.00;

        public double ProgrammingMinutesSavedPerHour
        {
            get { return programmingMinutesSavedPerHour; }
            set { programmingMinutesSavedPerHour = value; }
        }
        private double adminMinutesSavedPerHour = 1.00;

        public double AdminMinutesSavedPerHour
        {
            get { return adminMinutesSavedPerHour; }
            set { adminMinutesSavedPerHour = value; }
        }



        [XmlIgnore]
        public ROIDocumentCalculations ROIDocumentCalculations
        {
            get => roiDocumentCalculations ?? (roiDocumentCalculations = new ROIDocumentCalculations(this));
            set
            {
                roiDocumentCalculations = value;
                RaisePropertyChanged(nameof(ROIDocumentCalculations));
            }
        }


        /// <summary>
        /// plain english language name (e.g. United Kingdom)
        /// </summary>
        public string Language
        {
            get => language;
            set
            {
                language = value;
                RaisePropertyChanged(nameof(Language));
            }
        }

        public ObservableCollection<Material> MaterialsListCollection
        {
            get => _materialsListCollection;
            set
            {
                _materialsListCollection = value;
                ROIDocumentCalculations.RaisePropertyChanged();
                RaisePropertyChanged(nameof(MaterialsListCollection));
            }
        }

        public ObservableCollection<Machine> MachinesListCollection
        {
            get => _machinesListCollection;
            set
            {
                _machinesListCollection = value;
                ROIDocumentCalculations.RaisePropertyChanged();
                RaisePropertyChanged(nameof(MachinesListCollection));
                //Todo: Bug: these FirePropertyChanged's aren't working after EditDialog is accepted.
            }
        }
        public ObservableCollection<HourlyPerson> PeopleListCollection
        {
            get => _peopleListCollection;
            set
            {
                _peopleListCollection = value;
                ROIDocumentCalculations.RaisePropertyChanged();
                RaisePropertyChanged(nameof(PeopleListCollection));
            }
        }

    }
}
