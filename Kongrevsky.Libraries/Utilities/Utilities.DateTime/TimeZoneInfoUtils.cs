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

        public static string DefaultTimeZoneId { get; set; } = "UTC";

        public static TimeZoneInfo TryFromSerializedStringOrDefault(string timezoneId)
        {
            TimeZoneInfo result;
            try
            {
                result = TimeZoneInfo.FromSerializedString(timezoneId);
            }
            catch (Exception e)
            {
                result = TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZoneId);
            }

            return result;
        }
        public static TimeZoneInfo TryFindSystemTimeZoneByIdOrDefault(string timezoneId)
        {
            TimeZoneInfo result;
            try
            {
                result = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            }
            catch (Exception e)
            {
                result = TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZoneId);
            }

            return result;
        }

        public static string GetTimezoneDisplayNameIgnoreDaylight(this TimeZoneInfo timezone)
        {
            if (!timezone.SupportsDaylightSavingTime || !timezone.IsDaylightSavingTime(DateTime.UtcNow))
                return timezone.DisplayName;

            var adjustmentRule = timezone.GetAdjustmentRules().FirstOrDefault(r => r.DateStart <= DateTime.UtcNow && r.DateEnd >= DateTime.UtcNow);
            if (adjustmentRule == null)
                return timezone.DisplayName;

            var actualOffset = timezone.BaseUtcOffset + adjustmentRule.DaylightDelta;
            var offsetSign = actualOffset.Hours > 0 ? "+" : "";
            return actualOffset == TimeSpan.Zero
                    ? $"(UTC+00:00) {timezone.StandardName}"
                    : $"(UTC{offsetSign}{actualOffset.Hours:00}:{actualOffset.Duration().Minutes:00}) {timezone.StandardName}";
        }

        public static double GetTimezoneOffsetIgnoreDaylight(this TimeZoneInfo timezone)
        {
            if (!timezone.SupportsDaylightSavingTime || !timezone.IsDaylightSavingTime(DateTime.UtcNow))
                return timezone.BaseUtcOffset.TotalMinutes;

            var adjustmentRule = timezone.GetAdjustmentRules().FirstOrDefault(r => r.DateStart <= DateTime.UtcNow && r.DateEnd >= DateTime.UtcNow);
            return adjustmentRule == null ? timezone.BaseUtcOffset.TotalMinutes : (timezone.BaseUtcOffset + adjustmentRule.DaylightDelta).TotalMinutes;
        }
    }
}