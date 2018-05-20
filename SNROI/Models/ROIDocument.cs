using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SNROI.Models
{ 
    public class ROIDocument : BaseModel
    {
        public ROIDocument()
        {
        }

        public string DocumentName { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string CompanyLogoImage { get; set; }

        private CultureCurrencyPair cultureCurrencyPair = new CultureCurrencyPair()
            { CultureCode = "yourface", Country = "no"};

        public CultureCurrencyPair CultureCurrencyPair
        {
            get => cultureCurrencyPair;
            set
            {
                cultureCurrencyPair = value;
                FirePropertyChanged(nameof(CurrencySymbol));
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
                    var cultureInfo = new CultureInfo(CultureCurrencyPair.CultureCode);
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
    }
}
