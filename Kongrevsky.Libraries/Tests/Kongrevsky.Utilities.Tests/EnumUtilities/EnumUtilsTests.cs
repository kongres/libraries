namespace Kongrevsky.Utilities.Tests.EnumUtilities
{
    #region << Using >>

    using Kongrevsky.Utilities.Enum;
    using Xunit;

    #endregion

    public class EnumUtilsTests
    {
        #region Nested Classes

        enum TestEnum
        {
            TestTest1 = 1,

            TestTest2 = 2
        }

        #endregion

        [Fact]
        public void GetValue_FirstValue_Test()
        {
            var expected = "testTest1";
            var actual = TestEnum.TestTest1.GetValue();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValue_SecondValue_Test()
        {
            var expected = "testTest2";
            var actual = TestEnum.TestTest2.GetValue();

            Assert.Equal(expected, actual);
        }
    }
}