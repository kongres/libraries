namespace Kongrevsky.Utilities.Tests.EnumerableUtilities
{
    #region << Using >>

    using System;
    using Kongrevsky.Utilities.Enumerable;
    using Xunit;

    #endregion

    public class TupleUtilsTests
    {
        #region Nested Classes

        class TestClass
        {
            #region Properties

            public int TestValue { get; set; }

            #endregion
        }

        class Test1Class
        {
            #region Properties

            public int TestValue1 { get; set; }

            #endregion
        }

        class ChildTestClass : TestClass
        {
            #region Properties

            public bool ChildValue { get; set; }

            #endregion
        }

        class ChildTest1Class : Test1Class
        {
            #region Properties

            public bool ChildValue1 { get; set; }

            #endregion
        }

        #endregion

        [Fact]
        public void IsTypes_ValidSimpleTypes_Test()
        {
            var tuple = new Tuple<object, object>(1, "1");

            Assert.True(tuple.IsTypes(typeof(int), typeof(string)), "Straight");
            Assert.True(tuple.IsTypes(typeof(string), typeof(int)), "Inverse");
        }

        [Fact]
        public void IsTypes_InValidFirstSimpleType_Test()
        {
            var tuple = new Tuple<object, object>("test", 56);

            Assert.False(tuple.IsTypes(typeof(bool), typeof(int)), "Straight");
            Assert.False(tuple.IsTypes(typeof(int), typeof(bool)), "Inverse");
        }

        [Fact]
        public void IsTypes_InValidSecondSimpleType_Test()
        {
            var tuple = new Tuple<object, object>(true, 23);

            Assert.False(tuple.IsTypes(typeof(bool), typeof(string)), "Straight");
            Assert.False(tuple.IsTypes(typeof(string), typeof(bool)), "Inverse");
        }

        [Fact]
        public void IsTypes_InValidSimpleTypes_Test()
        {
            var tuple = new Tuple<object, object>(56m, "tes");

            Assert.False(tuple.IsTypes(typeof(int), typeof(bool)), "Straight");
            Assert.False(tuple.IsTypes(typeof(bool), typeof(int)), "Inverse");
        }

        [Fact]
        public void IsTypes_ValidComplexTypes_Test()
        {
            var tuple = new Tuple<object, object>(new TestClass(), new Test1Class());

            Assert.True(tuple.IsTypes(typeof(TestClass), typeof(Test1Class)), "Straight");
            Assert.True(tuple.IsTypes(typeof(Test1Class), typeof(TestClass)), "Inverse");
        }

        [Fact]
        public void IsTypes_InValidComplexTypes_Test()
        {
            var tuple = new Tuple<object, object>(new TestClass(), new Test1Class());

            Assert.False(tuple.IsTypes(typeof(ChildTestClass), typeof(ChildTest1Class)), "Straight");
            Assert.False(tuple.IsTypes(typeof(ChildTest1Class), typeof(ChildTestClass)), "Inverse");
        }

        [Fact]
        public void IsTypes_FirstInheritedType_Test()
        {
            var tuple = new Tuple<object, object>(new TestClass(), new ChildTest1Class());

            Assert.True(tuple.IsTypes(typeof(TestClass), typeof(Test1Class)), "Straight");
            Assert.True(tuple.IsTypes(typeof(Test1Class), typeof(TestClass)), "Inverse");
        }

        [Fact]
        public void IsTypes_SecondInheritedType_Test()
        {
            var tuple = new Tuple<object, object>(new ChildTestClass(), new Test1Class());

            Assert.True(tuple.IsTypes(typeof(TestClass), typeof(Test1Class)), "Straight");
            Assert.True(tuple.IsTypes(typeof(Test1Class), typeof(TestClass)), "Inverse");
        }

        [Fact]
        public void IsTypes_BothInheritedTypes_Test()
        {
            var tuple = new Tuple<object, object>(new ChildTestClass(), new ChildTest1Class());

            Assert.True(tuple.IsTypes(typeof(TestClass), typeof(Test1Class)), "Straight");
            Assert.True(tuple.IsTypes(typeof(Test1Class), typeof(TestClass)), "Inverse");
        }

        [Fact]
        public void ConvertToTuple_ValidSimpleTypes_Test()
        {
            var tuple = new Tuple<object, object>(1, "1");

            var convertedStraight = tuple.ConvertToTuple<int, string>();

            Assert.Equal(1, convertedStraight.Item1);
            Assert.Equal("1", convertedStraight.Item2);

            var convertedReverse = tuple.ConvertToTuple<string, int>();

            Assert.Equal("1", convertedReverse.Item1);
            Assert.Equal(1, convertedReverse.Item2);
        }

        [Fact]
        public void ConvertToTuple_InValidFirstSimpleType_Test()
        {
            var tuple = new Tuple<object, object>(1, "1");

            var convertedStraight = tuple.ConvertToTuple<int, bool>();

            Assert.Null(convertedStraight);

            var convertedReverse = tuple.ConvertToTuple<bool, int>();

            Assert.Null(convertedReverse);
        }

        [Fact]
        public void ConvertToTuple_InValidSecondSimpleType_Test()
        {
            var tuple = new Tuple<object, object>(1, "1");

            var convertedStraight = tuple.ConvertToTuple<bool, string>();

            Assert.Null(convertedStraight);

            var convertedReverse = tuple.ConvertToTuple<string, bool>();

            Assert.Null(convertedReverse);
        }

        [Fact]
        public void ConvertToTuple_InValidSimpleTypes_Test()
        {
            var tuple = new Tuple<object, object>(1, "1");

            var convertedStraight = tuple.ConvertToTuple<bool, bool>();

            Assert.Null(convertedStraight);

            var convertedReverse = tuple.ConvertToTuple<bool, bool>();

            Assert.Null(convertedReverse);
        }

        [Fact]
        public void ConvertToTuple_ComplexTypes_Test()
        {
            var tuple = new Tuple<object, object>(new TestClass { TestValue = 2 }, new Test1Class { TestValue1 = 3 });

            var convertedStraight = tuple.ConvertToTuple<TestClass, Test1Class>();

            Assert.Equal(2, convertedStraight.Item1.TestValue);
            Assert.Equal(3, convertedStraight.Item2.TestValue1);

            var convertedReverse = tuple.ConvertToTuple<Test1Class, TestClass>();

            Assert.Equal(3, convertedReverse.Item1.TestValue1);
            Assert.Equal(2, convertedReverse.Item2.TestValue);
        }

        [Fact]
        public void ConvertToTuple_InheritedComplexTypes_Test()
        {
            var tuple = new Tuple<object, object>(new ChildTestClass{TestValue = 4}, new ChildTest1Class{TestValue1 = 6});

            var convertedStraight = tuple.ConvertToTuple<TestClass, Test1Class>();

            Assert.Equal(4, convertedStraight.Item1.TestValue);
            Assert.Equal(6, convertedStraight.Item2.TestValue1);

            var convertedReverse = tuple.ConvertToTuple<Test1Class, TestClass>();

            Assert.Equal(6, convertedReverse.Item1.TestValue1);
            Assert.Equal(4, convertedReverse.Item2.TestValue);
        }
    }
}