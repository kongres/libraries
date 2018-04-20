namespace Utilities.DateTime
{
    #region << Using >>

    using System;
    using System.Globalization;
    using Utilities.DateTime.Models;

    #endregion

    public static class DateTimeUtils
    {
        #region Nested Classes

        public class DateTimeInterval
        {
            #region Properties

            public DateTime Start { get; set; }

            public DateTime End { get; set; }

            #endregion
        }

        #endregion

        /// <summary>
        /// Returns DateTime with parsed from string time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime AddTime(this DateTime dateTime, string time)
        {
            return DateTime.TryParseExact(time, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var addTime) ? dateTime.Add(addTime.TimeOfDay) : dateTime;
        }

        public static DateTime Trim(this DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - date.Ticks % roundTicks, date.Kind);
        }

        /// <summary>
        /// Returns Date of the start of a week
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
                diff += 7;

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Returns Date of the end of a week
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek)
        {
            return StartOfWeek(dateTime, startOfWeek).AddDays(6);
        }

        /// <summary>
        /// Returns hash of week by specified DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static ulong GetWeekHash(this DateTime date)
        {
            var startWeek = date.StartOfWeek(DayOfWeek.Monday).Date;
            var kind = (ulong)(int)startWeek.Kind;
            var weekHash = (kind << 62) | (ulong)startWeek.Ticks;
            return weekHash;
        }

        /// <summary>
        /// Returns start Date of specified period
        /// </summary>
        /// <param name="value"></param>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public static DateTime StartOf(this DateTime value, PeriodType periodType)
        {
            switch (periodType)
            {
                case PeriodType.Year:
                    return new DateTime(value.Year, 1, 1, 0, 0, 0, value.Kind);
                default:
                case PeriodType.Month:
                    return new DateTime(value.Year, value.Month, 1, 0, 0, 0, value.Kind);
                case PeriodType.Week:
                    return value.StartOfWeek(DayOfWeek.Monday);
                case PeriodType.Day:
                    return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, value.Kind);
            }
        }

        /// <summary>
        /// Returns number of days in the month of specified DatTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        /// <summary>
        /// Returns end Date of specified period
        /// </summary>
        /// <param name="value"></param>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public static DateTime EndOf(this DateTime value, PeriodType periodType)
        {
            switch (periodType)
            {
                case PeriodType.Year:
                    return new DateTime(value.Year, 12, 31, 23, 59, 59, 999, value.Kind);
                default:
                case PeriodType.Month:
                    return new DateTime(value.Year, value.Month, value.DaysInMonth(), 23, 59, 59, 999, value.Kind);
                case PeriodType.Week:
                    return value.EndOfWeek(DayOfWeek.Monday).EndOf(PeriodType.Day);
                case PeriodType.Day:
                    return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59, 999, value.Kind);
            }
        }

        /// <summary>
        /// Returns DateTime with added period
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public static DateTime AddPeriod(this DateTime dateTime, PeriodType periodType)
        {
            switch (periodType)
            {
                case PeriodType.Year:
                    return dateTime.AddYears(1);
                default:
                case PeriodType.Month:
                    return dateTime.AddMonths(1);
                case PeriodType.Week:
                    return dateTime.AddDays(7);
                case PeriodType.Day:
                    return dateTime.AddDays(1);
            }
        }

        /// <summary>
        /// Converts to DateTime in specified TimeZone
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeZoneInfo"></param>
        /// <returns></returns>
        public static DateTime ToTimezoneTime(this DateTime dateTime, TimeZoneInfo timeZoneInfo)
        {
            var utcTime = dateTime.ToUniversalTime();
            var timezoneTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
            return timezoneTime;
        }

        /// <summary>
        /// Returns DateTimeInterval of current Quarter
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTimeInterval GetCurrentQuarter(this DateTime dateTime)
        {
            var quarterNumber = (dateTime.Month - 1) / 3 + 1;

            return new DateTimeInterval
                   {
                           Start = new DateTime(dateTime.Year, (quarterNumber - 1) * 3 + 1, 1, 0, 0, 0, dateTime.Kind),
                           End = new DateTime(dateTime.Year, (quarterNumber - 1) * 3 + 1, 1).AddMonths(3).AddDays(-1).EndOf(PeriodType.Day)
                   };
        }

        /// <summary>
        /// Returns DateTimeInterval of previous Quarter
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTimeInterval GetPreviousQuarter(this DateTime dateTime)
        {
            var date = dateTime.AddMonths(-3);

            var quarterNumber = (date.Month - 1) / 3 + 1;

            return new DateTimeInterval
                   {
                           Start = new DateTime(date.Year, (quarterNumber - 1) * 3 + 1, 1, 0, 0, 0, dateTime.Kind),
                           End = new DateTime(date.Year, (quarterNumber - 1) * 3 + 1, 1).AddMonths(3).AddDays(-1).EndOf(PeriodType.Day)
                   };
        }

        /// <summary>
        /// Returns Max DateTime of two specified DateTimes
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public static DateTime Max(DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1 > dateTime2 ? dateTime1 : dateTime2;
        }

        /// <summary>
        /// Returns Min DateTime of two specified DateTimes
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public static DateTime Min(DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1 < dateTime2 ? dateTime1 : dateTime2;
        }

        /// <summary>
        /// Returns difference between two DateTimes
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public static TimeSpan Difference(this DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1 > dateTime2 ?
                           dateTime1 - dateTime2 :
                           dateTime2 - dateTime1;
        }
    }
}