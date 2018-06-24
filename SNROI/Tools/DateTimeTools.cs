using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI.Tools
{
    public enum GetDaysReturnType
    {
        AllDays, //Count all days
        BusinessDaysOnly,//No weekends, count bank holidays
        BusinessDaysExcludeHolidays //No weekends, don't count bank holidays

        //ALL OPTIONS: Include start day, exclude end day
    }
    public static class DateTimeTools
    {
        //https://stackoverflow.com/questions/1617049/calculate-the-number-of-business-days-between-two-dates
        //https://stackoverflow.com/a/1646396
        public static int GetDaysBetween(DateTime startDate, DateTime endDate, GetDaysReturnType GetDaysOption = GetDaysReturnType.AllDays)
        {

            var totalDays = (int)(endDate - startDate).TotalDays;

            Debug.WriteLine($"totalDays: {totalDays}");

            if (GetDaysOption == GetDaysReturnType.AllDays)
                return totalDays;

            var calcBusinessDays =
                (int)(1 + ((endDate - startDate).TotalDays * 5 -
                     (startDate.DayOfWeek - endDate.DayOfWeek) * 2) / 7);

            if (endDate.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
            if (startDate.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;


            if (GetDaysOption == GetDaysReturnType.BusinessDaysOnly)
            {
                if (calcBusinessDays > 0 && endDate.DayOfWeek != DayOfWeek.Saturday && endDate.DayOfWeek != DayOfWeek.Sunday)
                    calcBusinessDays--;
                Debug.WriteLine($"calcBusinessDays (BusinessDaysOnly): {calcBusinessDays}");
                return calcBusinessDays;
            }

            var weekendHolidayAdjustedCount = 0;
            var curYear = DateTime.Now.Year;
            if (AdjustForBankHolday(startDate, endDate, new DateTime(curYear, 1, 1), ref calcBusinessDays)) { weekendHolidayAdjustedCount++; }//New Years Day
            if (AdjustForBankHolday(startDate, endDate, new DateTime(curYear, 1, 15), ref calcBusinessDays)) { weekendHolidayAdjustedCount++; } //Martin Luther King, Jr. Day
            if (AdjustForBankHolday(startDate, endDate, new DateTime(curYear, 2, 19), ref calcBusinessDays)) { weekendHolidayAdjustedCount++; } //George Washington’s Birthday
            if (AdjustForBankHolday(startDate, endDate, new DateTime(curYear, 5, 28), ref calcBusinessDays)) { weekendHolidayAdjustedCount++; } //Memorial Day
            if (AdjustForBankHolday(startDate, endDate, new DateTime(curYear, 7, 4), ref calcBusinessDays)) { weekendHolidayAdjustedCount++; } //Independence Day
            if (AdjustForBankHolday(startDate, endDate, new DateTime(curYear, 9, 3), ref calcBusinessDays)) { weekendHolidayAdjustedCount++; } //Labor Day
            if (AdjustForBankHolday(startDate, endDate, new DateTime(curYear, 10, 8), ref calcBusinessDays)) { weekendHolidayAdjustedCount++; }  //Columbus Day
            if (AdjustForBankHolday(startDate, endDate, new DateTime(curYear, 11, 12), ref calcBusinessDays)) { weekendHolidayAdjustedCount++; }  //Veterans Day
            if (AdjustForBankHolday(startDate, endDate, new DateTime(curYear, 11, 22), ref calcBusinessDays)) { weekendHolidayAdjustedCount++; } //Thanksgiving Day
            if (AdjustForBankHolday(startDate, endDate, new DateTime(curYear, 12, 25), ref calcBusinessDays)) { weekendHolidayAdjustedCount++; } //Christmas Day

            Debug.WriteLine($"weekendHolidayAdjustedCount: {weekendHolidayAdjustedCount}");
            if (calcBusinessDays > 0 && endDate.DayOfWeek != DayOfWeek.Saturday && endDate.DayOfWeek != DayOfWeek.Sunday)
                calcBusinessDays--;
            Debug.WriteLine($"calcBusinessDays (BusinessDaysExcludeHolidays): {calcBusinessDays}");
            return calcBusinessDays;
        }

        private static bool AdjustForBankHolday(DateTime startDate, DateTime endDate, DateTime bankHolidayDate, ref int businessDaysCount)
        {
            if ((bankHolidayDate.DayOfWeek == DayOfWeek.Saturday) || (bankHolidayDate.DayOfWeek == DayOfWeek.Sunday))
                return false;

            if (((bankHolidayDate < startDate) || (bankHolidayDate > endDate)) && (bankHolidayDate != startDate) &&
                (bankHolidayDate != endDate))
                return false;

            if (businessDaysCount <= 0)
                return false;

            businessDaysCount--;
            return true;

        }
    }
}
