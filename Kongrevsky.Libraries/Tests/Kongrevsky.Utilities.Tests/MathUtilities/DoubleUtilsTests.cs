namespace Kongrevsky.Utilities.Tests.MathUtilities
{
    #region << Using >>

    using Kongrevsky.Utilities.Math;
    using Xunit;

    #endregion

    public class DoubleUtilsTests
    {
        [Fact]
        public void IsCloseToInt_Test()
        {
            Assert.True(4.0000009.IsCloseToInt(), "4.0000009.IsCloseToInt()");
            Assert.True(4.0000002.IsCloseToInt(), "4.0000002.IsCloseToInt()");
            Assert.False(4.000001.IsCloseToInt(), "4.000001.IsCloseToInt()");
            Assert.False(4.0000009.IsCloseToInt(0.0000001), "4.0000009.IsCloseToInt(0.0000001)");
        }
    }
}