using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
            CultureInfo fr = new CultureInfo(roiDocument.Language);

            var symbol = new CultureInfo(roiDocument.Language).NumberFormat.CurrencySymbol;
        }

        #region Information
        [Category("Materials"), DisplayName("Number of materials"), Description("Number of materials defined")]
        public int NumberOfMaterials => RoiDocument.MaterialsListCollection.Count;

        [Category("Machines"), DisplayName("Number of machines"), Description("Number of machines available")]
        public int NumberOfMachines => RoiDocument.MachinesListCollection.Count;

        [Category("Employees"), DisplayName("Number of employees"), Description("Number of employees in manufacturing and fabrication operations")]
        public int NumberOfEmployees => RoiDocument.PeopleListCollection.Count;


        #endregion

        #region Daily
        [Category("Daily"), DisplayName("Daily employee cost"), Description("Total cost of all employees per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyEmployeeCost
        {
            get
            {
                var people = RoiDocument.PeopleListCollection;
                return people.Sum(x => x.HourlyWage * WorkHoursInADay);
            }
        }

        [Category("Daily"), DisplayName("Daily programmer savings"), Description("Total savings of all progammers per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyEmployeeSavings
        {
            get
            {
                var people = RoiDocument.PeopleListCollection;
                var measurements = RoiDocument.ROIDocumentMeasurements;
                double hourlySavings = 0;

                //Todo: Optimize the heck of this
                if (measurements.ProgrammingTimeUnits == Enums.TimeUnitsEnums.Hour)
                {
                    hourlySavings = measurements.ProgrammingMinutesSavedPerUnit;
                }
                else if (measurements.ProgrammingTimeUnits == Enums.TimeUnitsEnums.Day)
                {
                    hourlySavings = (double)measurements.ProgrammingMinutesSavedPerUnit / WorkHoursInADay;
                }
                else if (measurements.ProgrammingTimeUnits == Enums.TimeUnitsEnums.Week)
                {
                    hourlySavings = (double)measurements.ProgrammingMinutesSavedPerUnit / WorkDaysInAWeek / WorkHoursInADay;
                }
                else if (measurements.ProgrammingTimeUnits == Enums.TimeUnitsEnums.Month)
                {
                    hourlySavings = (double)measurements.ProgrammingMinutesSavedPerUnit / WorkDaysInAMonth / WorkDaysInAWeek / WorkHoursInADay;
                }
                else if (measurements.ProgrammingTimeUnits == Enums.TimeUnitsEnums.Year)
                {
                    hourlySavings = (double)measurements.ProgrammingMinutesSavedPerUnit / WorkDaysInAYear / WorkDaysInAMonth / WorkDaysInAWeek / WorkHoursInADay;
                }
                return people.Count(x => x.EmployeeKind == EmployeeKind.Programmer) * hourlySavings;
            }
        }

        [Category("Daily"), DisplayName("Daily total savings"), Description("Total cost of all employees per day"), DisplayFormat(DataFormatString = "0.00")]
        public double DailyTotalSavings
        {
            get
            {
                //Todo: Add other savings here
                return DailyEmployeeSavings;
            }
        }

        #endregion

        #region Monthly

        [Category("Monthly"), DisplayName("Monthly employee cost"), Description("Total cost of all employees per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyEmployeeCost => DailyEmployeeCost * WorkDaysInAMonth;

        [Category("Monthly"), DisplayName("Monthly programmer savings"), Description("Total savings of all programmers per month"), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyEmployeeSavings => DailyEmployeeSavings * WorkDaysInAMonth;

        [Category("Monthly"), DisplayName("Monthly total savings"), Description(""), DisplayFormat(DataFormatString = "0.00")]
        public double MonthlyTotalSavings => DailyEmployeeSavings * WorkDaysInAMonth;

        #endregion

        #region Annual

        [Category("Annual"), DisplayName("Annual employee cost"), Description("Total cost of all employees per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualEmployeeCost => DailyEmployeeCost * WorkDaysInAYear;


        [Category("Annual"), DisplayName("Annual programmer savings"), Description("Total savings of all programmers per year"), DisplayFormat(DataFormatString = "0.00")]
        public double AnnualEmployeeSavings => DailyEmployeeSavings * WorkDaysInAYear;

        [Category("Annual"), DisplayName("Annual total savings"), Description(""), DisplayFormat(DataFormatString = "0.00")]
        public double TotalAnnualSavings => DailyEmployeeSavings * WorkDaysInAYear;


        #endregion
    }
}
