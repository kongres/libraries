namespace Kongrevsky.Utilities.Tests.MathUtilities
{
    #region << Using >>

    using Kongrevsky.Utilities.Math;
    using Xunit;

    #endregion

    public class DecimalUtilsTests
    {
        [Fact]
        public void LessThan_Test()
        {
            Assert.True(5m.LessThan(6m), "5m.LessThan(6m)");
            Assert.True(5.03m.LessThan(5.04m), "5.03m.LessThan(5.04m)");
            Assert.True(5.034m.LessThan(5.035m, 3), "5.034m.LessThan(5.035m, 3)");
            Assert.False(5.034m.LessThan(5.035m, 2), "5.034m.LessThan(5.035m, 2)");
            Assert.False(5.01m.LessThan(5.01m), "5.01m.LessThan(5.01m)");
        }

        [Fact]
        public void LessThanOrEqualTo_Test()
        {
            Assert.True(5m.LessThanOrEqualTo(6m), "5m.LessThanOrEqualTo(6m)");
            Assert.True(5.03m.LessThanOrEqualTo(5.04m), "5.03m.LessThanOrEqualTo(5.04m)");
            Assert.True(5.034m.LessThanOrEqualTo(5.035m, 3), "5.034m.LessThanOrEqualTo(5.035m, 3)");
            Assert.True(5.034m.LessThanOrEqualTo(5.035m, 2), "5.034m.LessThanOrEqualTo(5.035m, 2)");
            Assert.True(5.01m.LessThanOrEqualTo(5.01m), "5.01m.LessThanOrEqualTo(5.01m)");
            Assert.False(5.02m.LessThanOrEqualTo(5.01m), "5.02m.LessThanOrEqualTo(5.01m)");
        }

        [Fact]
        public void GreaterThan_Test()
        {
            Assert.True(5m.GreaterThan(4m), "5m.GreaterThan(4m)");
            Assert.True(5.01m.GreaterThan(5m), "5.01m.GreaterThan(5m)");
            Assert.True(5.034m.GreaterThan(5.033m, 3), "5.034m.GreaterThan(5.033m, 3)");
            Assert.False(5.034m.GreaterThan(5.033m), "5.01m.GreaterThan(5.011m)");
            Assert.False(5.01m.GreaterThan(5.01m), "5.01m.GreaterThan(5.01m)");
        }

        [Fact]
        public void GreaterThanOrEqualTo_Test()
        {
            Assert.True(5m.GreaterThanOrEqualTo(4m), "5m.GreaterThanOrEqualTo(4m)");
            Assert.True(5.01m.GreaterThanOrEqualTo(5m), "5.01m.GreaterThanOrEqualTo(5m)");
            Assert.True(5.034m.GreaterThanOrEqualTo(5.033m, 3), "5.034m.GreaterThanOrEqualTo(5.033m, 3)");
            Assert.True(5.034m.GreaterThanOrEqualTo(5.033m), "5.034m.GreaterThanOrEqualTo(5.033m)");
            Assert.True(5.01m.GreaterThanOrEqualTo(5.01m), "5.01m.GreaterThanOrEqualTo(5.01m)");
            Assert.False(5m.GreaterThanOrEqualTo(5.01m), "5m.GreaterThanOrEqualTo(5.01m)");
        }

        [Fact]
        public void AlmostEquals_Test()
        {
            Assert.True(5m.AlmostEquals(5m), "5m.AlmostEquals(5m)");
            Assert.True(5.01m.AlmostEquals(5.01m), "5.01m.AlmostEquals(5.01m)");
            Assert.True(5.011m.AlmostEquals(5.01m, 2), "5.011m.AlmostEquals(5.01m, 2)");
            Assert.False(5.011m.AlmostEquals(5.01m, 3), "5.011m.AlmostEquals(5.01m,3)");
            Assert.False(4m.AlmostEquals(5m), "4m.AlmostEquals(5m)");
        }
    }
}