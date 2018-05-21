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
        }

        public string DocumentName { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string CompanyLogoImage { get; set; }

        public Units Units { get; set; }

        /// <summary>
        /// plain english language name (e.g. United Kingdom)
        /// </summary>
        public string Language{ get;
            set; }

        public string CultureCodeString
        {
            get
            {
                if (string.IsNullOrEmpty(Language))
                {
                    return "en-US";
                }
                else
                {
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
        public DateTime ReportDate { get; set; }
        public bool IsMetric { get; set; }

        private ObservableCollection<Material> materialsListCollection = new ObservableCollection<Material>();

        public ObservableCollection<Material> MaterialsListCollection
        {
            get { return materialsListCollection; }
            set { materialsListCollection = value; }
        }

        private ObservableCollection<Machine> machinesListCollection = new ObservableCollection<Machine>();

        public ObservableCollection<Machine> MachinesListCollection
        {
            get { return machinesListCollection; }
            set { machinesListCollection = value; }
        }
        private ObservableCollection<HourlyPerson> peopleListCollection = new ObservableCollection<HourlyPerson>();

        public ObservableCollection<HourlyPerson> PeopleListCollection
        {
            get { return peopleListCollection; }
            set { peopleListCollection = value; }
        }

        public ReportType ReportType => ReportType.Standard;

        public string ReportName => DocumentName;

    }
}
