namespace Kongrevsky.Utilities.Tests.DateTimeUtilities
{
    #region << Using >>

    using System;
    using System.Linq;
    using Kongrevsky.Utilities.DateTime;
    using Xunit;

    #endregion

    public class TimeZoneInfoUtilsTests
    {
        [Fact]
        public void DetectTimeZoneInfoFromOffset_RussianTimeZoneTest()
        {
            var expected = TimeZoneInfo.GetSystemTimeZones().Single(r => r.Id == "Russian Standard Time");
            var actual = TimeZoneInfoUtils.DetectTimeZoneInfoFromOffset(180);

            Assert.Equal(expected.BaseUtcOffset, actual.BaseUtcOffset);
        }

        [Fact]
        public void DetectTimeZoneInfoFromOffset_CentralEuropeTimeZoneTest()
        {
            var expected = TimeZoneInfo.GetSystemTimeZones().Single(r => r.Id == "Central Europe Standard Time");
            var actual = TimeZoneInfoUtils.DetectTimeZoneInfoFromOffset(60);

            Assert.Equal(expected.BaseUtcOffset, actual.BaseUtcOffset);
        }
    }
}