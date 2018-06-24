using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SNROI.Tools;

namespace SNROI.Models
{
    public class ROIDocumentCalculations : BaseModel
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

        [Category("Employees"), DisplayName("Daily employee cost"), Description("Total cost of all employees per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyEmployeeCost
        {
            get
            {
                var people = RoiDocument.PeopleListCollection;
                return people.Sum(x => x.HourlyWage * WorkHoursInADay);
            }
        }

        [Category("Employees"), DisplayName("Monthly employee cost"), Description("Total cost of all employees per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyEmployeeCost => DailyEmployeeCost * WorkDaysInAMonth;

        [Category("Employees"), DisplayName("Annual employee cost"), Description("Total cost of all employees per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualEmployeeCost => DailyEmployeeCost * WorkDaysInAYear;


        [Category("Employees"), DisplayName("Daily programmer savings"), Description("Total savings of all progammers per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyEmployeeSavings
        {
            get
            {
                var people = RoiDocument.PeopleListCollection;
                var measurements = RoiDocument.ROIDocumentMeasurements;
                double hourlySavings = 0;

                //Todo: Optimize the heck of this
                if (measurements.TimeUnitsEnums == Enums.TimeUnitsEnums.Hour)
                {
                    hourlySavings = measurements.ProgrammingMinutesSavedPerUnit;
                }
                else if (measurements.TimeUnitsEnums == Enums.TimeUnitsEnums.Day)
                {
                    hourlySavings = (double)measurements.ProgrammingMinutesSavedPerUnit / WorkHoursInADay;
                }
                else if (measurements.TimeUnitsEnums == Enums.TimeUnitsEnums.Week)
                {
                    hourlySavings = (double)measurements.ProgrammingMinutesSavedPerUnit / WorkDaysInAWeek / WorkHoursInADay;
                }
                else if (measurements.TimeUnitsEnums == Enums.TimeUnitsEnums.Month)
                {
                    hourlySavings = (double)measurements.ProgrammingMinutesSavedPerUnit / WorkDaysInAMonth / WorkDaysInAWeek / WorkHoursInADay;
                }
                else if (measurements.TimeUnitsEnums == Enums.TimeUnitsEnums.Year)
                {
                    hourlySavings = (double)measurements.ProgrammingMinutesSavedPerUnit / WorkDaysInAYear / WorkDaysInAMonth / WorkDaysInAWeek / WorkHoursInADay;
                }
                return people.Count(x => x.EmployeeKind == EmployeeKind.Programmer) * hourlySavings;
            }
        }

        [Category("Employees"), DisplayName("Monthly programmer savings"), Description("Total savings of all programmers per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyEmployeeSavings => DailyEmployeeSavings * WorkDaysInAMonth;

        [Category("Employees"), DisplayName("Annual programmer savings"), Description("Total savings of all programmers per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualEmployeeSavings=> DailyEmployeeSavings * WorkDaysInAYear;
        #endregion

        #region Summary

        [Category("Summary"), DisplayName("Daily total savings"), Description("Total cost of all employees per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyTotalSavings
        {
            get
            {
                var people = RoiDocument.PeopleListCollection;
                return people.Sum(x => x.HourlyWage * WorkHoursInADay);
            }
        }
        [Category("Summary"), DisplayName("Monthly total savings"), Description(""), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyTotalSavings => DailyEmployeeCost * WorkDaysInAMonth;


        [Category("Summary"), DisplayName("Annual total savings"), Description(""), DisplayFormat(DataFormatString = "0.00")]
        public double TotalAnnualSavings => DailyTotalSavings * WorkDaysInAYear;


        #endregion
    }
}
