namespace Kongrevsky.Utilities.Tests.ObjectUtilities
{
    #region << Using >>

    using System;
    using System.ComponentModel.DataAnnotations;
    using Kongrevsky.Utilities.Object;
    using Xunit;

    #endregion

    public class ObjectUtilsTests
    {
        #region Nested Classes

        enum TestEnum
        {
            [Display(Name = "Value 1")]
            Value1 = 1,

            Value2 = 2
        }

        class TestClass
        {
            #region Properties

            public int Value1 { get; set; }

            public string Value2 { get; set; }

            public DateTime Date1 { get; set; }

            public DateTime? Date2 { get; set; }

            public DateTime? Date3 { get; set; }

            #endregion
        }

        #endregion

        [Fact]
        public void GetDisplayName_WithAttribute_Test()
        {
            var expected = "Value 1";
            var actual = TestEnum.Value1.GetDisplayName();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDisplayName_WithoutAttribute_Test()
        {
            var expected = "Value2";
            var actual = TestEnum.Value2.GetDisplayName();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertToType_Test()
        {
            object obj = new TestClass
                         {
                                 Value1 = 1,
                                 Value2 = "test"
                         };

            var convertedObj = obj.ConvertToType<TestClass>();

            Assert.Equal(typeof(TestClass).AssemblyQualifiedName, convertedObj.GetType().AssemblyQualifiedName);
            Assert.Equal(1, convertedObj.Value1);
            Assert.Equal("test", convertedObj.Value2);

            Assert.Equal(default(int), obj.ConvertToType<int>());
        }

        [Fact]
        public void TryConvertToType_Generic_ValidType_Test()
        {
            object obj = new TestClass
                         {
                                 Value1 = 1,
                                 Value2 = "test"
                         };

            var result = obj.TryConvertToType(out TestClass outObj);

            Assert.True(result, "TryConvertToType result");
            Assert.Equal(1, outObj.Value1);
            Assert.Equal("test", outObj.Value2);
        }

        [Fact]
        public void TryConvertToType_Generic_InvalidType_Test()
        {
            object obj = new TestClass
                         {
                                 Value1 = 1,
                                 Value2 = "test"
                         };

            var result = obj.TryConvertToType(out int outObj);

            Assert.False(result, "TryConvertToType result");
            Assert.Equal(default(int), outObj);
        }

        [Fact]
        public void TryConvertToType_ValidType_Test()
        {
            object obj = new TestClass
                         {
                                 Value1 = 1,
                                 Value2 = "test"
                         };

            var result = obj.TryConvertToType(typeof(TestClass), out var outObj);
            var convertedObj = (TestClass)outObj;

            Assert.True(result, "TryConvertToType result");
            Assert.Equal(1, convertedObj.Value1);
            Assert.Equal("test", convertedObj.Value2);
        }

        [Fact]
        public void TryConvertToType_InValidType_Test()
        {
            object obj = new TestClass
                         {
                                 Value1 = 1,
                                 Value2 = "test"
                         };

            var result = obj.TryConvertToType(typeof(int), out var outObj);

            Assert.False(result, "TryConvertToType result");
            Assert.Null(outObj);
        }

        [Fact]
        public void GetPropValue_ValidPropName_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 2,
                              Value2 = "test2"
                      };

            Assert.Equal(2, obj.GetPropValue("Value1"));
            Assert.Equal("test2", obj.GetPropValue("Value2"));
        }

        [Fact]
        public void GetPropValue_InValidPropName_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 2,
                              Value2 = "test2"
                      };

            Assert.Null(obj.GetPropValue("Value"));
        }

        [Fact]
        public void TrySetPropValue_ValidPropName_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 2,
                              Value2 = "2"
                      };

            var result = obj.TrySetPropValue("Value1", 4);

            Assert.True(result, "TrySetPropValue result");
            Assert.Equal(4, obj.Value1);
        }

        [Fact]
        public void TrySetPropValue_InValidPropName_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 2,
                              Value2 = "2"
                      };

            var result = obj.TrySetPropValue("Value", 4);

            Assert.False(result, "TrySetPropValue result");
        }

        [Fact]
        public void TrySetPropValue_InValidPropType_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 2,
                              Value2 = "2"
                      };

            Assert.Throws<ArgumentException>(() => obj.TrySetPropValue("Value1", "test"));
        }

        [Fact]
        public void GetPropValue_Generic_ValidPropName_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 2,
                              Value2 = "test2"
                      };

            Assert.Equal(2, obj.GetPropValue<int>("Value1"));
            Assert.Equal("test2", obj.GetPropValue<string>("Value2"));
        }

        [Fact]
        public void GetPropValue_Generic_InValidPropName_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 2,
                              Value2 = "test2"
                      };

            Assert.Equal(default(string), obj.GetPropValue<string>("Value"));
        }

        [Fact]
        public void GetPropValue_Generic_InValidPropType_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 2,
                              Value2 = "test2"
                      };

            Assert.Throws<InvalidCastException>(() => obj.GetPropValue<string>("Value1"));
        }

        [Fact]
        public void TryGetPropValue_Generic_ValidPropName_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 1,
                              Value2 = "tte"
                      };

            var result = obj.TryGetPropValue("Value1", out int outPropertyValue);

            Assert.True(result);
            Assert.Equal(1, outPropertyValue);
        }

        [Fact]
        public void TryGetPropValue_Generic_InValidPropName_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 1,
                              Value2 = "tte"
                      };

            var result = obj.TryGetPropValue("Value", out int outPropertyValue);

            Assert.False(result);
            Assert.Equal(default(int), outPropertyValue);
        }

        [Fact]
        public void TryGetPropValue_Generic_InValidPropType_Test()
        {
            var obj = new TestClass
                      {
                              Value1 = 1,
                              Value2 = "tte"
                      };

            Assert.Throws<InvalidCastException>(() => obj.TryGetPropValue("Value1", out string outPropertyValue));
        }

        [Fact]
        public void IsNullable_Test()
        {
            Assert.True(typeof(int?).IsNullable(), "typeof(int?).IsNullable()");
            Assert.True(typeof(TestClass).IsNullable(), "typeof(TestClass).IsNullable()");
            Assert.False(typeof(int).IsNullable(), "typeof(int).IsNullable()");
            Assert.False(typeof(bool).IsNullable(), "typeof(bool).IsNullable()");
        }

        [Fact]
        public void IsSimple_Test()
        {
            Assert.True(typeof(decimal).IsSimple(), "typeof(decimal).IsSimple()");
            Assert.True(typeof(int).IsSimple(), "typeof(int).IsSimple()");
            Assert.True(typeof(DateTime).IsSimple(), "typeof(DateTime).IsSimple()");
            Assert.True(typeof(double).IsSimple(), "typeof(double).IsSimple()");
            Assert.True(typeof(string).IsSimple(), "typeof(string).IsSimple()");
            Assert.True(typeof(float).IsSimple(), "typeof(float).IsSimple()");
            Assert.True(typeof(bool).IsSimple(), "typeof(bool).IsSimple()");
            Assert.True(typeof(TestEnum).IsSimple(), "typeof(TestEnum).IsSimple()");
            Assert.False(typeof(TestClass).IsSimple(), "typeof(TestClass).IsSimple()");
        }

        [Fact]
        public void FixDates_Test()
        {
            var obj = new TestClass
                      {
                              Date1 = new DateTime(2017, 12, 31, 12, 34, 34, 34, DateTimeKind.Local),
                              Date2 = new DateTime(2017, 12, 31, 12, 56, 56, 56, DateTimeKind.Local)
                      };

            ObjectUtils.FixDates(obj);

            Assert.Equal(DateTimeKind.Utc, obj.Date1.Kind);
            Assert.True(obj.Date2.HasValue);
            Assert.Equal(DateTimeKind.Utc, obj.Date2.Value.Kind);
            Assert.Null(obj.Date3);
        }
    }
}