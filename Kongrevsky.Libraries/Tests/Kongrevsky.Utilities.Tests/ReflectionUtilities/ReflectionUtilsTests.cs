namespace Kongrevsky.Utilities.Tests.ReflectionUtilities
{
    #region << Using >>

    using System.Linq;
    using Kongrevsky.Utilities.Reflection;
    using Xunit;

    #endregion

    public class ReflectionUtilsTests
    {
        #region Nested Classes

        class TestClass
        {
            #region Properties

            public string FirstProperty { get; set; }

            public string SecondProperty { get; set; }

            public int ThirdProperty { get; set; }

            #endregion
        }

        #endregion

        [Fact]
        public void GetCurrentFullMethodName_Test()
        {
            var expected = "Kongrevsky.Utilities.Tests.ReflectionUtilities.ReflectionUtilsTests.GetCurrentFullMethodName_Test";
            var actual = ReflectionUtils.GetCurrentFullMethodName();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPropertiesByType_Test()
        {
            var listOfStringProperties = typeof(TestClass).GetPropertiesByType<string>();

            Assert.Equal(2, listOfStringProperties.Count);
            Assert.Equal("FirstProperty", listOfStringProperties[0].Name);
            Assert.Equal("SecondProperty", listOfStringProperties[1].Name);

            var listOfIntProperties = typeof(TestClass).GetPropertiesByType<int>();

            Assert.Single(listOfIntProperties);
            Assert.Equal("ThirdProperty", listOfIntProperties.Single().Name);
        }
    }
}