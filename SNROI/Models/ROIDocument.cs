using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.Wizards;

namespace SNROI.Models
{
    public enum Units
    {
        Inches,
        Metric
    }

    [HighlightedClass]
    public class ROIDocument : BaseModel
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
                FirePropertyChanged(nameof(DocumentName));
            }
        }

        public string CompanyName
        {
            get => companyName;
            set
            {
                companyName = value;
                FirePropertyChanged(nameof(CompanyName));
            }
        }

        public string ContactName
        {
            get => contactName;
            set
            {
                contactName = value;
                FirePropertyChanged(nameof(ContactName));
            }
        }

        public string CompanyLogoImage
        {
            get => companyLogoImage;
            set
            {
                companyLogoImage = value;
                FirePropertyChanged(nameof(CompanyLogoImage));
            }
        }

        public Units Units
        {
            get => units;
            set
            {
                units = value;
                FirePropertyChanged(nameof(Units));
            }
        }

        public DateTime DateCreated
        {
            get => dateCreated;
            set
            {
                dateCreated = value;
                FirePropertyChanged(nameof(DateCreated));
            }

        }

        public DateTime DateModified
        {
            get => dateModified;
            set
            {
                dateModified = value;
                FirePropertyChanged(nameof(DateModified));
            }
        }

        public string Notes
        {
            get => notes;
            set
            {
                notes = value;
                FirePropertyChanged(nameof(Notes));
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
                FirePropertyChanged(nameof(ROIDocumentCalculations));
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
                FirePropertyChanged(nameof(ROIDocumentCalculations));
                FirePropertyChanged(nameof(ROIDocumentMeasurements));
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
                FirePropertyChanged(nameof(Language));
            }
        }

        public ObservableCollection<Material> MaterialsListCollection
        {
            get => _materialsListCollection;
            set
            {
                _materialsListCollection = value;
                ROIDocumentCalculations.FirePropertyChanged();
                FirePropertyChanged(nameof(MaterialsListCollection));
            }
        }

        public ObservableCollection<Machine> MachinesListCollection
        {
            get => _machinesListCollection;
            set
            {
                _machinesListCollection = value;
                ROIDocumentCalculations.FirePropertyChanged();
                FirePropertyChanged(nameof(MachinesListCollection));
                //Todo: Bug: these FirePropertyChanged's aren't working after EditDialog is accepted.
            }
        }
        public ObservableCollection<HourlyPerson> PeopleListCollection
        {
            get => _peopleListCollection;
            set
            {
                _peopleListCollection = value;
                ROIDocumentCalculations.FirePropertyChanged();
                FirePropertyChanged(nameof(PeopleListCollection));
            }
        }

    }
}
