namespace Kongrevsky.Utilities.Tests.DateTimeUtilities
{
    #region << Using >>

    using System;
    using System.Linq;
    using Kongrevsky.Utilities.DateTime;
    using Kongrevsky.Utilities.DateTime.Models;
    using Xunit;

    #endregion

    public class DateTimeUtilsTests
    {
        [Fact]
        public void AddTime_PM_Test()
        {
            var expected = new DateTime(2017, 12, 31, 12, 30, 0);
            var actual = new DateTime(2017, 12, 31, 0, 0, 0).AddTime("12:30 PM");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddTime_AM_Test()
        {
            var expected = new DateTime(2017, 12, 31, 0, 30, 0);
            var actual = new DateTime(2017, 12, 31, 0, 0, 0).AddTime("12:30 AM");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddTime_InvalidString_Test()
        {
            var expected = new DateTime(2017, 12, 31, 0, 0, 0);
            var actual = new DateTime(2017, 12, 31, 0, 0, 0).AddTime("qweqwe");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StartOfWeek_Monday_Test()
        {
            var expected = new DateTime(2018, 5, 28, 0, 0, 0);
            var actual = new DateTime(2018, 6, 1, 0, 0, 0).StartOfWeek(DayOfWeek.Monday);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StartOfWeek_Sunday_Test()
        {
            var expected = new DateTime(2018, 5, 27, 0, 0, 0);
            var actual = new DateTime(2018, 6, 1, 0, 0, 0).StartOfWeek(DayOfWeek.Sunday);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EndOfWeek_Monday_Test()
        {
            var expected = new DateTime(2018, 6, 3, 0, 0, 0);
            var actual = new DateTime(2018, 6, 1, 0, 0, 0).EndOfWeek(DayOfWeek.Monday);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EndOfWeek_Sunday_Test()
        {
            var expected = new DateTime(2018, 6, 2, 0, 0, 0);
            var actual = new DateTime(2018, 6, 1, 0, 0, 0).EndOfWeek(DayOfWeek.Sunday);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StartOf_Year_Test()
        {
            var expected = new DateTime(2018, 1, 1, 0, 0, 0);
            var actual = new DateTime(2018, 6, 1, 0, 0, 0).StartOf(PeriodType.Year);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StartOf_Month_Test()
        {
            var expected = new DateTime(2018, 4, 1, 0, 0, 0);
            var actual = new DateTime(2018, 4, 18, 0, 0, 0).StartOf(PeriodType.Month);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StartOf_Week_Test()
        {
            var expected = new DateTime(2018, 5, 28, 0, 0, 0);
            var actual = new DateTime(2018, 6, 1, 0, 0, 0).StartOf(PeriodType.Week);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StartOf_Day_Test()
        {
            var expected = new DateTime(2018, 6, 1, 0, 0, 0);
            var actual = new DateTime(2018, 6, 1, 23, 56, 0).StartOf(PeriodType.Day);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DaysInMonth_February2018_Test()
        {
            var expected = 28;
            var actual = new DateTime(2018, 2, 25, 0, 0, 0).DaysInMonth();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DaysInMonth_February2016_Test()
        {
            var expected = 29;
            var actual = new DateTime(2016, 2, 25, 0, 0, 0).DaysInMonth();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EndOf_Year_Test()
        {
            var expected = new DateTime(2018, 12, 31, 23, 59, 59, 999);
            var actual = new DateTime(2018, 1, 12).EndOf(PeriodType.Year);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EndOf_Month_Test()
        {
            var expected = new DateTime(2018, 6, 30, 23, 59, 59, 999);
            var actual = new DateTime(2018, 6, 1, 0, 0, 0).EndOf(PeriodType.Month);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EndOf_Week_Test()
        {
            var expected = new DateTime(2018, 6, 3, 23, 59, 59, 999);
            var actual = new DateTime(2018, 6, 1, 12, 55, 0, 999).EndOf(PeriodType.Week);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EndOf_Day_Test()
        {
            var expected = new DateTime(2018, 6, 1, 23, 59, 59, 999);
            var actual = new DateTime(2018, 6, 1, 12, 9, 59, 999).EndOf(PeriodType.Day);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddPeriod_Year_Test()
        {
            var expected = new DateTime(2019, 6, 1, 12, 0, 0);
            var actual = new DateTime(2018, 6, 1, 12, 0, 0).AddPeriod(PeriodType.Year);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddPeriod_Month_Test()
        {
            var expected = new DateTime(2018, 7, 1, 12, 0, 0);
            var actual = new DateTime(2018, 6, 1, 12, 0, 0).AddPeriod(PeriodType.Month);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddPeriod_Week_Test()
        {
            var expected = new DateTime(2018, 6, 8, 12, 0, 0);
            var actual = new DateTime(2018, 6, 1, 12, 0, 0).AddPeriod(PeriodType.Week);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddPeriod_Day_Test()
        {
            var expected = new DateTime(2018, 6, 2, 12, 0, 0);
            var actual = new DateTime(2018, 6, 1, 12, 0, 0).AddPeriod(PeriodType.Day);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToTimeZoneTime_RussianTimeZone_Test()
        {
            var expected = new DateTime(2018, 6, 1, 14, 34, 0);
            var timeZone = TimeZoneInfo.GetSystemTimeZones().Single(r => r.Id == "Russian Standard Time");
            var actual = DateTime.Parse("2018-06-01 07:34 AM -04:00").ToTimezoneTime(timeZone);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToTimeZoneTime_CentralEuropeTimeZone_Test()
        {
            var expected = new DateTime(2018, 6, 1, 13, 34, 0);
            var timeZone = TimeZoneInfo.GetSystemTimeZones().Single(r => r.Id == "Central Europe Standard Time");
            var actual = DateTime.Parse("2018-06-01 07:34 AM -04:00").ToTimezoneTime(timeZone);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetCurrentQuarter_Q1_2018_Test()
        {
            var expectedStart = new DateTime(2018, 1, 1);
            var expectedEnd = new DateTime(2018, 3, 31, 23, 59, 59, 999);

            var quarter = new DateTime(2018, 2, 1, 12, 0, 0).GetCurrentQuarter();

            Assert.Equal(expectedStart, quarter.Start);
            Assert.Equal(expectedEnd, quarter.End);
        }

        [Fact]
        public void GetCurrentQuarter_Q2_2018_Test()
        {
            var expectedStart = new DateTime(2018, 4, 1);
            var expectedEnd = new DateTime(2018, 6, 30, 23, 59, 59, 999);

            var quarter = new DateTime(2018, 6, 1, 12, 0, 0).GetCurrentQuarter();

            Assert.Equal(expectedStart, quarter.Start);
            Assert.Equal(expectedEnd, quarter.End);
        }

        [Fact]
        public void GetCurrentQuarter_Q3_2018_Test()
        {
            var expectedStart = new DateTime(2018, 7, 1);
            var expectedEnd = new DateTime(2018, 9, 30, 23, 59, 59, 999);

            var quarter = new DateTime(2018, 8, 1, 12, 0, 0).GetCurrentQuarter();

            Assert.Equal(expectedStart, quarter.Start);
            Assert.Equal(expectedEnd, quarter.End);
        }

        [Fact]
        public void GetCurrentQuarter_Q4_2018_Test()
        {
            var expectedStart = new DateTime(2018, 10, 1);
            var expectedEnd = new DateTime(2018, 12, 31, 23, 59, 59, 999);

            var quarter = new DateTime(2018, 11, 1, 12, 0, 0).GetCurrentQuarter();

            Assert.Equal(expectedStart, quarter.Start);
            Assert.Equal(expectedEnd, quarter.End);
        }

        [Fact]
        public void GetPreviousQuarter_Q1_2018_Test()
        {
            var expectedStart = new DateTime(2017, 10, 1);
            var expectedEnd = new DateTime(2017, 12, 31, 23, 59, 59, 999);

            var quarter = new DateTime(2018, 2, 1, 12, 0, 0).GetPreviousQuarter();

            Assert.Equal(expectedStart, quarter.Start);
            Assert.Equal(expectedEnd, quarter.End);
        }

        [Fact]
        public void GetPreviousQuarter_Q2_2018_Test()
        {
            var expectedStart = new DateTime(2018, 1, 1);
            var expectedEnd = new DateTime(2018, 3, 31, 23, 59, 59, 999);

            var quarter = new DateTime(2018, 6, 1, 12, 0, 0).GetPreviousQuarter();

            Assert.Equal(expectedStart, quarter.Start);
            Assert.Equal(expectedEnd, quarter.End);
        }

        [Fact]
        public void GetPreviousQuarter_Q3_2018_Test()
        {
            var expectedStart = new DateTime(2018, 4, 1);
            var expectedEnd = new DateTime(2018, 6, 30, 23, 59, 59, 999);

            var quarter = new DateTime(2018, 8, 1, 12, 0, 0).GetPreviousQuarter();

            Assert.Equal(expectedStart, quarter.Start);
            Assert.Equal(expectedEnd, quarter.End);
        }

        [Fact]
        public void GetPreviousQuarter_Q4_2018_Test()
        {
            var expectedStart = new DateTime(2018, 7, 1);
            var expectedEnd = new DateTime(2018, 9, 30, 23, 59, 59, 999);

            var quarter = new DateTime(2018, 11, 1, 12, 0, 0).GetPreviousQuarter();

            Assert.Equal(expectedStart, quarter.Start);
            Assert.Equal(expectedEnd, quarter.End);
        }

        [Fact]
        public void Max_InOneTimeZone_Test()
        {
            var firstDate = new DateTime(2018, 11, 1, 23, 0, 0);
            var secondDate = new DateTime(2018, 12, 5, 1, 0, 0);

            var actual = DateTimeUtils.Max(firstDate, secondDate);

            Assert.Equal(secondDate, actual);
        }

        [Fact]
        public void Max_InDifferentTimeZones_Test()
        {
            var firstDate = DateTime.Parse("2018-11-01 12:56 -04:00");
            var secondDate = DateTime.Parse("2018-11-01 12:44 -06:00");

            var actual = DateTimeUtils.Max(firstDate, secondDate);

            Assert.Equal(secondDate, actual);
        }

        [Fact]
        public void Min_InOneTimeZone_Test()
        {
            var firstDate = new DateTime(2018, 11, 1, 23, 0, 0);
            var secondDate = new DateTime(2018, 12, 5, 1, 0, 0);

            var actual = DateTimeUtils.Min(firstDate, secondDate);

            Assert.Equal(firstDate, actual);
        }

        [Fact]
        public void Min_InDifferentTimeZones_Test()
        {
            var firstDate = DateTime.Parse("2018-11-01 12:56 -04:00");
            var secondDate = DateTime.Parse("2018-11-01 12:44 -06:00");

            var actual = DateTimeUtils.Min(firstDate, secondDate);

            Assert.Equal(firstDate, actual);
        }

        [Fact]
        public void Difference_InOneTimeZone_Test()
        {
            var firstDate = DateTime.Parse("2018-11-01 12:56:58");
            var secondDate = DateTime.Parse("2018-11-01 12:56:50");

            var expected = TimeSpan.Parse("0.00:00:08");
            var actual = firstDate.Difference(secondDate);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Difference_InDifferentTimeZones_Test()
        {
            var firstDate = DateTime.Parse("2018-11-01 12:56:58 -03:00");
            var secondDate = DateTime.Parse("2018-11-01 12:56:50 +04:00");

            var expected = TimeSpan.Parse("0.07:00:08");
            var actual = firstDate.Difference(secondDate);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CutDateTimeForSql_ValidDateTime_Test()
        {
            var expected = DateTime.Parse("2018-11-01 12:56:58 -03:00");
            var actual = DateTime.Parse("2018-11-01 12:56:58 -03:00").CutDateTimeForSql();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CutDateTimeForSql_LessThanValidDateTime_Test()
        {
            var expected = new DateTime(1753, 1, 1);
            var actual = DateTime.Parse("1656-11-01 12:56:58 -03:00").CutDateTimeForSql();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CutDateTimeForSql_GreaterThanValidDateTime_Test()
        {
            var expected = new DateTime(9999, 12, 31, 23, 59, 59, 997);
            var actual = new DateTime(9999, 12, 31, 23, 59, 59, 999).CutDateTimeForSql();

            Assert.Equal(expected, actual);
        }
    }
}