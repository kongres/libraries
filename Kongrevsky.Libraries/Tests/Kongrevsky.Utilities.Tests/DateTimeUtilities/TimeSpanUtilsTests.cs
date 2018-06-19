namespace Kongrevsky.Utilities.Tests.DateTimeUtilities
{
    #region << Using >>

    using System;
    using Kongrevsky.Utilities.DateTime;
    using Xunit;

    #endregion

    public class TimeSpanUtilsTests
    {
        [Fact]
        public void Parse_EmptyString_Test()
        {
            var expected = TimeSpan.Parse("0.00:00:00.000");
            var actual = TimeSpanUtils.Parse("");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_InvalidString_Test()
        {
            var expected = TimeSpan.Parse("0.00:00:00.000");
            var actual = TimeSpanUtils.Parse("wefwef");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_Ticks_Test()
        {
            var expected = TimeSpan.Parse("0.00:00:00.0000012");
            var actual = TimeSpanUtils.Parse("12");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_HoursMinutesSeconds_Test()
        {
            var expected = TimeSpan.Parse("0.12:23:59");
            var actual = TimeSpanUtils.Parse("12:23:59");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_DaysHoursMinutesSeconds_Test()
        {
            var expected = TimeSpan.Parse("12.23:54:59");
            var actual = TimeSpanUtils.Parse("12:23:54:59");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_DaysHoursMinutesSecondsMilliseconds_Test()
        {
            var expected = TimeSpan.Parse("12.23:54:59.999");
            var actual = TimeSpanUtils.Parse("12:23:54:59:999");

            Assert.Equal(expected, actual);
        }
    }
}