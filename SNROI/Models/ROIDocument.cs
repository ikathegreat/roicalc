using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
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
        }

        public string DocumentName { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string CompanyLogoImage { get; set; }
        public Units Units { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        private ROIDocumentCalculations roiDocumentCalculations;
        private ObservableCollection<Material> _materialsListCollection = new ObservableCollection<Material>();
        private ObservableCollection<HourlyPerson> _peopleListCollection = new ObservableCollection<HourlyPerson>();
        private ObservableCollection<Machine> _machinesListCollection = new ObservableCollection<Machine>();

        [XmlIgnore]
        public ROIDocumentCalculations ROIDocumentCalculations
        {
            get => roiDocumentCalculations ?? (roiDocumentCalculations = new ROIDocumentCalculations(this));
            set => roiDocumentCalculations = value;
        }


        /// <summary>
        /// plain english language name (e.g. United Kingdom)
        /// </summary>
        public string Language { get; set; }

        public string CultureCodeString
        {
            get
            {
                if (string.IsNullOrEmpty(Language))
                {
                    return "en-US";
                }

                try
                {
                    //Todo: improve displayed language to culture convert
                    var cultureCode = "en-US";
                    switch (Language)
                    {
                        case "United States":
                            cultureCode = "en-US";
                            break;
                        case "Korea":
                            cultureCode = "en-US";
                            break;
                        case "Germany":
                            cultureCode = "en-GB";
                            break;
                        case "Japan":
                            cultureCode = "ja-JP";
                            break;
                    }
                    return cultureCode;
                }
                catch
                {
                    return "en-US";
                }
            }
        }

        public CultureInfo Culture
        {
            get
            {
                if (string.IsNullOrEmpty(CultureCodeString))
                {
                    return new CultureInfo("en-US");
                }
                else
                {
                    try
                    {
                        return new CultureInfo(CultureCodeString);
                    }
                    catch
                    {
                        return new CultureInfo("en-US");
                    }
                }
            }
        }

        [XmlIgnore]
        public string CurrencySymbol
        {
            get
            {
                var result = "$";
                try
                {
                    var cultureInfo = new CultureInfo(CultureCodeString);
                    result = cultureInfo.NumberFormat.CurrencySymbol;
                }
                catch
                {
                    //invalid code
                }
                return result;
            }
        }

        public ObservableCollection<Material> MaterialsListCollection
        {
            get => _materialsListCollection;
            set
            {
                _materialsListCollection = value;
                ROIDocumentCalculations.FirePropertyChanged();
            }
        }

        public ObservableCollection<Machine> MachinesListCollection
        {
            get => _machinesListCollection;
            set
            {
                _machinesListCollection = value;
                ROIDocumentCalculations.FirePropertyChanged();
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
            }
        }
    }
}
