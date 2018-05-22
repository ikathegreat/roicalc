using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI.Models
{
    public class ROIDocumentCalculations : BaseModel
    {
        private readonly ROIDocument RoiDocument;
        public ROIDocumentCalculations(ROIDocument roiDocument)
        {
            RoiDocument = roiDocument;


        }

        #region Materials
        [Category("Materials"), DisplayName("Number of materials"), Description("Number of materials defined")]
        public int NumberOfMaterials => RoiDocument.MaterialsListCollection.Count;



        #endregion

        #region Machines
        [Category("Machines"), DisplayName("Number of machines"), Description("Number of machines available")]
        public int NumberOfMachines => RoiDocument.MachinesListCollection.Count;



        #endregion

        #region Employees



        [Category("Employees"), DisplayName("Number of employees"), Description("Number of employees in manufacturing and fabrication operations")]
        public int NumberOfEmployees => RoiDocument.PeopleListCollection.Count;

        [Category("Employees"), DisplayName("Daily employee cost"), Description("Total cost of all employees per day")]
        public double DailyEmployeeCost
        {
            get
            {
                var people = RoiDocument.PeopleListCollection;
                return people.Sum(x => x.HourlyWage * 8);
            }
        }

        [Category("Employees"), DisplayName("Monthly employee cost"), Description("Total cost of all employees per month")]
        public double MonthlyEmployeeCost => DailyEmployeeCost * 30;

        [Category("Employees"), DisplayName("Annual employee cost"), Description("Total cost of all employees per year")]
        public double AnnualEmployeeCost => MonthlyEmployeeCost * 12;
        #endregion
    }
}
