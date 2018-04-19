using System;
using System.Globalization;
using Utilities.DateTime.Models;

namespace Utilities.DateTime
{
    using DateTime = System.DateTime;

    public static class DateTimeUtils
    {

        public static DateTime AddTime(this DateTime dateTime, string time)
        {
            return DateTime.TryParseExact(time, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var addTime) ? dateTime.Add(addTime.TimeOfDay) : dateTime;
        }

        public static DateTime Trim(this DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - date.Ticks % roundTicks, date.Kind);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
                diff += 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek)
        {
            return StartOfWeek(dateTime, startOfWeek).AddDays(6);
        }

        public static ulong GetWeekHash(this DateTime date)
        {
            var startWeek = date.StartOfWeek(DayOfWeek.Monday).Date;
            var kind = (ulong)(int)startWeek.Kind;
            var weekHash = (kind << 62) | (ulong)startWeek.Ticks;
            return weekHash;
        }

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

        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

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

        public static DateTime ToTimezoneTime(this DateTime dateTime, TimeZoneInfo timeZoneInfo)
        {
            var utcTime = dateTime.ToUniversalTime();
            var timezoneTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
            return timezoneTime;
        }

        public static DateTimeInterval GetCurrentQuarter(this DateTime dateTime)
        {
            var quarterNumber = (dateTime.Month - 1) / 3 + 1;

            return new DateTimeInterval
            {
                Start = new DateTime(dateTime.Year, (quarterNumber - 1) * 3 + 1, 1, 0, 0, 0, dateTime.Kind),
                End = new DateTime(dateTime.Year, (quarterNumber - 1) * 3 + 1, 1).AddMonths(3).AddDays(-1).EndOf(PeriodType.Day)
            };
        }

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

        public static DateTime Max(DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1 > dateTime2 ? dateTime1 : dateTime2;
        }
        public static DateTime Min(DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1 < dateTime2 ? dateTime1 : dateTime2;
        }

        public static TimeSpan Difference(this DateTime dateTime1, DateTime dateTime2)
        {
            if (dateTime1 > dateTime2)
                return dateTime1 - dateTime2;
            else
                return dateTime2 - dateTime1;
        }

        public class DateTimeInterval
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
    }

}
