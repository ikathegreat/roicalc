using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SNROI.Tools;

namespace SNROI.Test
{
    [TestClass]
    public class DateTimeToolsUnitTest
    {
        //https://www.timeanddate.com/date/workdays.html

        [TestMethod]
        public void GetDaysBetween_FullYear()
        {
            var curYear = DateTime.Now.Year;

            var startDate = new DateTime(curYear, 1, 1);
            var endDate = new DateTime(curYear, 12, 31);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate, GetDaysReturnType.BusinessDaysExcludeHolidays), 250);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate), 364);
        }

        [TestMethod]
        public void GetDaysBetween_SameDateBankHoliday()
        {
            var curYear = DateTime.Now.Year;

            var startDate = new DateTime(curYear, 1, 1);
            var endDate = new DateTime(curYear, 1, 1);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate, GetDaysReturnType.BusinessDaysExcludeHolidays), 0);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate), 0);
        }

        [TestMethod]
        public void GetDaysBetween_SameDateNonHoliday()
        {
            var curYear = DateTime.Now.Year;

            var startDate = new DateTime(curYear, 1, 4);
            var endDate = new DateTime(curYear, 1, 4);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate, GetDaysReturnType.BusinessDaysExcludeHolidays), 0);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate), 0);
        }

        [TestMethod]
        public void GetDaysBetween_OneDayWithBankHoliday()
        {
            var curYear = DateTime.Now.Year;
            //New Year's Day to next day
            var startDate = new DateTime(curYear, 1, 1);
            var endDate = new DateTime(curYear, 1, 2);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate, GetDaysReturnType.BusinessDaysExcludeHolidays), 0);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate), 1);
        }

        [TestMethod]
        public void GetDaysBetween_OneDayWithoutBankHoliday()
        {
            var curYear = DateTime.Now.Year;

            var startDate = new DateTime(curYear, 1, 2);
            var endDate = new DateTime(curYear, 1, 3);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate, GetDaysReturnType.BusinessDaysExcludeHolidays), 1);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate), 1);
        }

        [TestMethod]
        public void GetDaysBetween_HalfYearEarlyHalf()
        {

            var curYear = DateTime.Now.Year;

            var startDate = new DateTime(curYear, 1, 1);
            var endDate = new DateTime(curYear, 7, 1);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate, GetDaysReturnType.BusinessDaysExcludeHolidays), 126);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate), 181);
        }

        [TestMethod]
        public void GetDaysBetween_HalfYearLateHalf()
        {
            var curYear = DateTime.Now.Year;

            var startDate = new DateTime(curYear, 6, 1);
            var endDate = new DateTime(curYear, 12, 31);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate, GetDaysReturnType.BusinessDaysExcludeHolidays), 145);
            Assert.AreEqual(DateTimeTools.GetDaysBetween(startDate,
                endDate), 213);
        }
    }
}
