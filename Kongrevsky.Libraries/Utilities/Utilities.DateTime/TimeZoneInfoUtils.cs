namespace Utilities.DateTime
{
    using System;
    using System.Linq;

    public static class TimeZoneInfoUtils
    {
        public static TimeZoneInfo DetecTimeZoneInfoFromOffset(int offsetMinutes)
        {
            var timeSpan = TimeSpan.FromMinutes(offsetMinutes);
            var timeZoneInfos = TimeZoneInfo.GetSystemTimeZones().OrderBy(x => Math.Abs((x.BaseUtcOffset - timeSpan).Ticks));
            var timeZone = timeZoneInfos.First();
            return timeZone;
        }
    }
}