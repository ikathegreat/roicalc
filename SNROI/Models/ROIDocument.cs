using DevExpress.DataAccess.ObjectBinding;
using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SNROI.Models
{
    public enum Units
    {
        Inches,
        Metric
    }

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
        private ROIDocumentMeasurements roiDocumentMeasurements;
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


        public ROIDocumentMeasurements ROIDocumentMeasurements
        {
            get
            {
                return roiDocumentMeasurements ?? (roiDocumentMeasurements = new ROIDocumentMeasurements());
            }
            set
            {
                roiDocumentMeasurements = value;
                RaisePropertyChanged(nameof(ROIDocumentCalculations));
                RaisePropertyChanged(nameof(ROIDocumentMeasurements));
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
