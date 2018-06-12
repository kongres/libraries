namespace Kongrevsky.Utilities.Tests.ObjectUtilities
{
    #region << Using >>

    using System;
    using Kongrevsky.Utilities.Object;
    using Xunit;

    #endregion

    public class GenericCompareTests
    {
        #region Nested Classes

        class TestClass
        {
            #region Properties

            public bool Value1 { get; set; }

            public DateTime Date1 { get; set; }

            public string Value2 { get; set; }

            #endregion
        }

        #endregion

        [Fact]
        public void GenericCompare_Test()
        {
            var obj1 = new TestClass
                      {
                              Value1 = true,
                              Value2 = "test3",
                              Date1 = new DateTime(2017, 12, 31)
                      };
            var obj2 = new TestClass
                       {
                               Value1 = true,
                               Value2 = "test4",
                               Date1 = new DateTime(2017, 12, 31)
                       };

            var compareValue1 = new GenericCompare<TestClass>(test => test.Value1);
            var compareValue2 = new GenericCompare<TestClass>(test => test.Value2);
            var compareDate1 = new GenericCompare<TestClass>(test => test.Date1);
            
            Assert.True(compareValue1.Equals(obj1, obj2), "compareValue1.Equals(obj1, obj2)");
            Assert.False(compareValue2.Equals(obj1, obj2), "compareValue2.Equals(obj1, obj2)");
            Assert.True(compareDate1.Equals(obj1, obj2), "compareDate1.Equals(obj1, obj2)");
        }
    }
}