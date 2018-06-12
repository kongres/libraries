namespace Kongrevsky.Utilities.Tests.ReflectionUtilities
{
    #region << Using >>

    using Kongrevsky.Utilities.Reflection;
    using Xunit;

    #endregion

    public class TypeUtilsTests
    {
        #region Nested Classes

        class TestClass
        {
            #region Properties

            public string Test1 { get; set; }

            public int Test2 { get; set; }

            public bool Test3 { get; set; }

            #endregion
        }

        #endregion

        [Fact]
        public void GetPropertyByName_StringProp_Test()
        {
            var test1Prop = typeof(TestClass).GetPropertyByName("Test1", false);

            Assert.Equal(nameof(TestClass.Test1), test1Prop.Name);
            Assert.Equal(typeof(string).AssemblyQualifiedName, test1Prop.PropertyType.AssemblyQualifiedName);
        }

        [Fact]
        public void GetPropertyByName_IntProp_Test()
        {
            var test2Prop = typeof(TestClass).GetPropertyByName("test2", true);

            Assert.Equal(nameof(TestClass.Test2), test2Prop.Name);
            Assert.Equal(typeof(int).AssemblyQualifiedName, test2Prop.PropertyType.AssemblyQualifiedName);
        }

        [Fact]
        public void GetPropertyByName_BoolProp_Test()
        {
            var test3Prop = typeof(TestClass).GetPropertyByName("Test3");

            Assert.Equal(nameof(TestClass.Test3), test3Prop.Name);
            Assert.Equal(typeof(bool).AssemblyQualifiedName, test3Prop.PropertyType.AssemblyQualifiedName);
        }
    }
}