using GalaSoft.MvvmLight;
using SNROI.Tools;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SNROI.Enums;

namespace SNROI.Models
{
    public class ROIDocumentCalculations : ObservableObject
    {
        private readonly int WorkHoursInADay; //Business Hours
        private readonly double WorkDaysInAMonth; //Average
        private readonly double WorkDaysInAWeek; //Average
        private readonly int WorkDaysInAYear;

        private readonly ROIDocument RoiDocument;
        public ROIDocumentCalculations(ROIDocument roiDocument)
        {
            RoiDocument = roiDocument;

            WorkHoursInADay = 8;

            var curYear = DateTime.Now.Year;
            var startDate = new DateTime(curYear, 1, 1);
            var endDate = new DateTime(curYear, 12, 31);
            WorkDaysInAYear = DateTimeTools.GetDaysBetween(startDate, endDate, GetDaysReturnType.BusinessDaysExcludeHolidays);

            WorkDaysInAMonth = (double)WorkDaysInAYear / 12;

            WorkDaysInAWeek = 5; //Keep is simple, no compensation for holidays
        }

        #region Information
        [Category("Materials"), DisplayName("Number of materials"), Description("Number of materials defined")]
        public int NumberOfMaterials => RoiDocument.MaterialsListCollection.Count;

        [Category("Machines"), DisplayName("Number of machines"), Description("Number of machines available")]
        public int NumberOfMachines => RoiDocument.MachinesListCollection.Count;

        [Category("Employees"), DisplayName("Number of programmers"), Description("Number of employees in manufacturing and fabrication operations")]
        public int NumberOfProgrammers => RoiDocument.PeopleListCollection.Count(x => x.EmployeeKind == EmployeeKind.Programmer);

        [Category("Employees"), DisplayName("Number of admins"), Description("Number of employees in manufacturing and fabrication operations")]
        public int NumberOfAdmins => RoiDocument.PeopleListCollection.Count(x => x.EmployeeKind == EmployeeKind.Admin);

        [Category("Employees"), DisplayName("Number of employees"), Description("Number of employees in manufacturing and fabrication operations")]
        public int NumberOfEmployees => RoiDocument.PeopleListCollection.Count;


        #endregion

        #region Daily
        [Category("Daily Cost"), DisplayName("Daily material cost"), Description("Total cost of materials per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyMaterialCost => MonthlyMaterialCost / WorkDaysInAMonth;

        [Category("Daily Cost"), DisplayName("Daily machine cost"), Description("Total cost of machines per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyMachineCost
        {
            get
            {
                var machines = RoiDocument.MachinesListCollection;

                return machines.Sum(y => y.CostPerHourToRun * WorkHoursInADay); //Todo: Apply utilization %?

            }
        }
        

        [Category("Daily Cost"), DisplayName("Daily employee cost"), Description("Total cost of all employees per day"),
         DisplayFormat(DataFormatString = "0.00")]
        public double DailyEmployeeCost
        {
            get
            {
                var people = RoiDocument.PeopleListCollection;
                return people.Sum(x => x.HourlyWage * WorkHoursInADay);
            }
        }

        [Category("Daily Cost"), DisplayName("Daily total cost"), Description("Total cost per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyTotalCost => DailyMaterialCost + DailyMachineCost + DailyEmployeeCost;

        [Category("Daily Savings"), DisplayName("Daily total material savings"),
         Description("Total savings of all materials per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyTotalMaterialSavings
        {
            get
            {
                var people = RoiDocument.PeopleListCollection;

                //return people.Sum(y => y.HourlyWage / RoiDocument.PercentMaterialUsageSavedPerYear * WorkHoursInADay);
                return 0;
            }
        }

        [Category("Daily Savings"), DisplayName("Daily total machine savings"),
         Description("Total savings of all machines per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyTotalMachineSavings
        {
            //Todo: what
            get { return DailyMachineCost * 1; }
        }

        [Category("Daily Savings"), DisplayName("Daily total employee savings"),
         Description("Total savings of all employees per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyTotalEmployeeSavings
        {
            get
            {
                var people = RoiDocument.PeopleListCollection;
                var total = 0.00;
                foreach (var employeeKind in (EmployeeKind[])Enum.GetValues(typeof(EmployeeKind)))
                {
                    if (employeeKind == EmployeeKind.Admin)
                    {
                        total = total + people.Sum(y =>
                                    (y.HourlyWage / 60 * RoiDocument.AdminMinutesSavedPerHour) * WorkHoursInADay);
                    }
                    else if (employeeKind == EmployeeKind.Programmer)
                    {
                        total = total + people.Sum(y =>
                                    (y.HourlyWage / 60 * RoiDocument.ProgrammingMinutesSavedPerHour) * WorkHoursInADay);

                    } 
                }

                return total;
            }
        }

        [Category("Daily Savings"), DisplayName("Daily total savings"), Description("Total savings per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyTotalSavings => DailyTotalMaterialSavings + DailyTotalMachineSavings + DailyTotalEmployeeSavings;

        #endregion

        #region Monthly

        [Category("Monthly Cost"), DisplayName("Monthly material cost"), Description("Total cost of materials per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyMaterialCost
        {
            get
            {
                var materials = RoiDocument.MaterialsListCollection;
                return materials.Sum(x => x.CostPerWeightUnit * x.TotalMonthlyPurchasedWeight);
            }
        }

        [Category("Monthly Cost"), DisplayName("Monthly machine cost"), Description("Total cost of all machines per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyMachineCost => DailyMachineCost * WorkDaysInAMonth;

        [Category("Monthly Cost"), DisplayName("Monthly employee cost"), Description("Total cost of all employees per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyEmployeeCost => DailyEmployeeCost * WorkDaysInAMonth;

        [Category("Monthly Cost"), DisplayName("Monthly cost"), Description("Total cost per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyTotalCost => DailyTotalCost * WorkDaysInAMonth;


        [Category("Monthly Savings"), DisplayName("Monthly material savings"), Description("Total savings of all material per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyMaterialSavings => DailyTotalMaterialSavings * WorkDaysInAMonth;

        [Category("Monthly Savings"), DisplayName("Monthly machine savings"), Description("Total savings of all machines per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyMachineSavings => DailyTotalMachineSavings * WorkDaysInAMonth;

        [Category("Monthly Savings"), DisplayName("Monthly employee savings"), Description("Total savings of all employees per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyEmployeeSavings => DailyTotalEmployeeSavings * WorkDaysInAMonth;

        [Category("Monthly Savings"), DisplayName("Monthly total savings"), Description(""), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyTotalSavings => DailyTotalSavings * WorkDaysInAMonth;

        #endregion

        #region Annual
        [Category("Annual Cost"), DisplayName("Annual material cost"), Description("Total cost of all material per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualMaterialCost => DailyMaterialCost * WorkDaysInAYear;

        [Category("Annual Cost"), DisplayName("Annual machine cost"), Description("Total cost of all machines per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualMachineCost => DailyMachineCost * WorkDaysInAYear;

        [Category("Annual Cost"), DisplayName("Annual employee cost"), Description("Total cost of all employees per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualEmployeeCost => DailyEmployeeCost * WorkDaysInAYear;

        [Category("Annual Cost"), DisplayName("Annual cost"), Description("Total cost per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualTotalCost => DailyTotalCost * WorkDaysInAYear;


        [Category("Annual Savings"), DisplayName("Annual material savings"), Description("Total savings of all material per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualMaterialSavings => DailyMaterialCost * WorkDaysInAYear;

        [Category("Annual Savings"), DisplayName("Annual machine savings"), Description("Total savings of all machine per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualMachineSavings => DailyMachineCost * WorkDaysInAYear;

        [Category("Annual Savings"), DisplayName("Annual employee savings"), Description("Total savings of all employees per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualEmployeeSavings => DailyTotalEmployeeSavings * WorkDaysInAYear;

        [Category("Annual Savings"), DisplayName("Annual total savings"), Description(""), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualTotalSavings => DailyTotalSavings * WorkDaysInAYear;


        #endregion
    }
}
