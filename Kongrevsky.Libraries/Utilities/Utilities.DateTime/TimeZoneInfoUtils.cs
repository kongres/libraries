namespace Kongrevsky.Utilities.DateTime
{
    #region << Using >>

    using System;
    using System.Linq;

    #endregion

    public static class TimeZoneInfoUtils
    {
        /// <summary>
        /// Returns TimeZone from specified in minutes Offset
        /// </summary>
        /// <param name="offsetMinutes"></param>
        /// <returns></returns>
        public static TimeZoneInfo DetectTimeZoneInfoFromOffset(int offsetMinutes)
        {
            var timeSpan = TimeSpan.FromMinutes(offsetMinutes);
            var timeZoneInfos = TimeZoneInfo.GetSystemTimeZones().OrderBy(x => Math.Abs((x.BaseUtcOffset - timeSpan).Ticks));
            var timeZone = timeZoneInfos.First();
            return timeZone;
        }

        public static string DefaultTimeZone { get; set; } = "UTC";

        public static TimeZoneInfo TryFindSystemTimeZoneByIdOrDefault(string timezoneId)
        {
            TimeZoneInfo result;
            try
            {
                result = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            }
            catch (Exception e)
            {
                result = TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZone);
            }

            return result;
        }
    }
}