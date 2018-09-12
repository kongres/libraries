namespace Kongrevsky.Utilities.Tests.EnumerableUtilities
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using Kongrevsky.Utilities.Enumerable;
    using Kongrevsky.Utilities.Enumerable.Models;
    using Xunit;

    #endregion

    public class EnumerableUtilsTests
    {
        #region Nested Classes

        class TestClass
        {
            #region Properties

            public int Value { get; set; }

            public int Value1 { get; set; }

            #endregion

            #region Interface Implementations

            public int CompareTo(object obj)
            {
                var other = (TestClass)obj;

                return Value.CompareTo(other.Value);
            }

            #endregion

            public override bool Equals(object obj)
            {
                var other = (TestClass)obj;

                return other != null &&
                       Value == other.Value &&
                       Value1 == other.Value1;
            }
        }

        class Test1Class
        {
            #region Properties

            public double Value { get; set; }

            #endregion
        }

        #endregion

        [Fact]
        public void AddIfTrue_Generic_Test()
        {
            var list = new List<int>();

            int Func(int num)
            {
                return num;
            }

            for (var i = 0; i < 3; i++)
                list.AddIfTrue(i > -1, Func(i));

            Assert.Equal(3, list.Count);
            Assert.Equal(0, list[0]);
            Assert.Equal(1, list[1]);
            Assert.Equal(2, list[2]);
        }

        [Fact]
        public void ChunkBy_Test()
        {
            var list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var chunkList = list.ChunkBy(2);

            Assert.Equal(5, chunkList.Count);

            var j = 0;
            foreach (var item in chunkList)
            {
                Assert.Equal(2, item.Count);

                Assert.Equal(j, item[0]);
                j++;
                Assert.Equal(j, item[1]);
                j++;
            }
        }

        [Fact]
        public void IsPropertyEqual_Equal_Test()
        {
            var list = new List<TestClass>();

            for (var i = 0; i < 5; i++)
                list.Add(new TestClass { Value = 1 });

            Assert.True(list.IsPropertyEqual(r => r.Value == 1));
        }

        [Fact]
        public void IsPropertyEqual_NotEqual_Test()
        {
            var list = new List<TestClass>();

            for (var i = 0; i < 5; i++)
                list.Add(new TestClass { Value = i });

            Assert.False(list.IsPropertyEqual(r => r.Value > 1));
        }

        [Fact]
        public void MinOrDefault_SimpleGeneric_Straight_Test()
        {
            var expected = 10;
            var entity = (new List<TestClass> { new TestClass { Value = 12 }, new TestClass { Value = 10 } }).MinOrDefault(new TestClass { Value = 34 });

            Assert.Equal(expected, entity.Value);
        }

        [Fact]
        public void MinOrDefault_SimpleGeneric_DefaultValue_Test()
        {
            var expected = 12;
            var entity = (new List<TestClass>()).MinOrDefault(new TestClass { Value = 12 });

            Assert.Equal(expected, entity.Value);
        }

        [Fact]
        public void MinOrDefault_SimpleGeneric_SimpleTypes_Test()
        {
            var expected = 1;
            var actual = (new List<int> { 1, 23, 567 }).MinOrDefault(4);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MaxOrDefault_SimpleGeneric_Straight_Test()
        {
            var expected = 100;
            var entity = (new List<TestClass> { new TestClass { Value = 12 }, new TestClass { Value = 100 } }).MaxOrDefault(new TestClass { Value = 34 });

            Assert.Equal(expected, entity.Value);
        }

        [Fact]
        public void MaxOrDefault_SimpleGeneric_DefaultValue_Test()
        {
            var expected = 12;
            var entity = (new List<TestClass>()).MaxOrDefault(new TestClass { Value = 12 });

            Assert.Equal(expected, entity.Value);
        }

        [Fact]
        public void MaxOrDefault_SimpleGeneric_SimpleTypes_Test()
        {
            var expected = 567;
            var actual = (new List<int> { 1, 23, 567 }).MaxOrDefault(4);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinOrDefault_ComplexGeneric_Straight_Test()
        {
            var expected = 12;
            var actual = (new List<TestClass> { new TestClass { Value1 = 12 }, new TestClass { Value1 = 45 }, new TestClass { Value1 = 67 } }).MinOrDefault(r => r.Value1, 56);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinOrDefault_ComplexGeneric_DefaultValue_Test()
        {
            var expected = 56;
            var actual = (new List<TestClass>()).MinOrDefault(r => r.Value1, 56);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinOrDefault_ComplexGeneric_NotComparableType_Test()
        {
            var expected = 56.4;
            var actual = (new List<Test1Class> { new Test1Class { Value = 56.4 }, new Test1Class { Value = 56.7 } }).MinOrDefault(r => r.Value, 57);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MaxOrDefault_ComplexGeneric_Straight_Test()
        {
            var expected = 67;
            var actual = (new List<TestClass> { new TestClass { Value1 = 12 }, new TestClass { Value1 = 45 }, new TestClass { Value1 = 67 } }).MaxOrDefault(r => r.Value1, 56);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MaxOrDefault_ComplexGeneric_DefaultValue_Test()
        {
            var expected = 56;
            var actual = (new List<TestClass>()).MaxOrDefault(r => r.Value1, 56);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MaxOrDefault_ComplexGeneric_NotComparableType_Test()
        {
            var expected = 56.7;
            var actual = (new List<Test1Class> { new Test1Class { Value = 56.4 }, new Test1Class { Value = 56.7 } }).MaxOrDefault(r => r.Value, 57);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToDictionaryIgnoreKeyDuplicates_WithDuplicates_Test()
        {
            var collection = new List<TestClass>
                             {
                                     new TestClass
                                     {
                                             Value = 23,
                                             Value1 = 45
                                     },
                                     new TestClass
                                     {
                                             Value = 23,
                                             Value1 = 5
                                     },
                                     new TestClass
                                     {
                                             Value = 3,
                                             Value1 = 45
                                     },
                                     new TestClass
                                     {
                                             Value = 3,
                                             Value1 = 56
                                     },
                                     new TestClass
                                     {
                                             Value = 26,
                                             Value1 = 45
                                     }
                             };

            var dictionary = collection.ToDictionaryIgnoreKeyDuplicates(r => r.Value);

            Assert.Equal(3, dictionary.Count);

            Assert.Equal(45, dictionary[23].Value1);
            Assert.Equal(45, dictionary[3].Value1);
            Assert.Equal(45, dictionary[26].Value1);
        }

        [Fact]
        public void ToDictionaryIgnoreKeyDuplicates_WithoutDuplicates_Test()
        {
            var collection = new List<TestClass>
                             {
                                     new TestClass
                                     {
                                             Value = 23,
                                             Value1 = 45
                                     },
                                     new TestClass
                                     {
                                             Value = 3,
                                             Value1 = 45
                                     },
                                     new TestClass
                                     {
                                             Value = 26,
                                             Value1 = 45
                                     }
                             };

            var dictionary = collection.ToDictionaryIgnoreKeyDuplicates(r => r.Value);

            Assert.Equal(3, dictionary.Count);

            Assert.Equal(45, dictionary[23].Value1);
            Assert.Equal(45, dictionary[3].Value1);
            Assert.Equal(45, dictionary[26].Value1);
        }

        [Fact]
        public void ScrambledEquals_SimpleTypes_Test()
        {
            Assert.True(EnumerableUtils.ScrambledEquals(new[] { 1, 2, 3, 4, 5, 6 },
                                                        new[] { 1, 2, 3, 4, 5, 6 }),
                        "Equal int[]");

            Assert.True(EnumerableUtils.ScrambledEquals(new[] { "1", "2", "3", "4", "5" },
                                                        new[] { "1", "2", "3", "4", "5" }),
                        "Equal string[]");

            Assert.True(EnumerableUtils.ScrambledEquals(new[] { true, false, true },
                                                        new[] { true, false, true }),
                        "Equal bool[]");

            Assert.False(EnumerableUtils.ScrambledEquals(new[] { 1, 2, 3, 4 },
                                                         new[] { 1, 2, 2 }),
                         "Not equal int[]");

            Assert.False(EnumerableUtils.ScrambledEquals(new[] { "23", "3", "4" },
                                                         new[] { "34", "3", " " }),
                         "Not equal string[]");

            Assert.True(EnumerableUtils.ScrambledEquals(new[] { 1, 2, 3 },
                                                        new List<int> { 1, 2, 3 }),
                        "Equal List and array");
        }

        [Fact]
        public void ScrambledEquals_ComplexTypes_Test()
        {
            var item1 = new TestClass { Value = 1 };
            var item2 = new TestClass { Value = 2 };
            var item3 = new TestClass { Value = 3 };

            Assert.True(EnumerableUtils.ScrambledEquals(new[] { item1, item2, item3 },
                                                        new[] { item1, item2, item3 }),
                        "Equal arrays");

            Assert.False(EnumerableUtils.ScrambledEquals(new[] { item1, item2, item3 },
                                                         new[] { item1, new TestClass { Value = 2 }, item3 }),
                         "Not equal arrays");
        }

        [Fact]
        public void GetPage_ValidPageSettings_Test()
        {
            var expected = new[] { 3, 4, 5 };
            var actual = new[] { 0, 1, 2, 3, 4, 5, 6, 7 }.GetPage(new Page { PageSize = 3, PageNumber = 2 });

            TestUtils.StrictEqual(expected, actual);
        }

        [Fact]
        public void GetPage_InValidPageSettings_Test()
        {
            var expected = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var actual = new[] { 0, 1, 2, 3, 4, 5, 6, 7 }.GetPage(new Page { PageSize = -3, PageNumber = -2 });

            TestUtils.StrictEqual(expected, actual);
        }

        [Fact]
        public void GetPageCount_ValidPageSize_Test()
        {
            var expected = 4;
            var actual = new[] { 1, 2, 3, 4, 5, 6, 7 }.GetPageCount(2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPageCount_InValidPageSize_Test()
        {
            var expected = 1;
            var actual = new[] { 1, 2, 3, 4, 5, 6, 7 }.GetPageCount(-2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OrderBy_ValidName_Asc_Test()
        {
            var expected = new List<TestClass>
                           {
                                   new TestClass { Value = 1 },
                                   new TestClass { Value = 2 },
                                   new TestClass { Value = 3 },
                                   new TestClass { Value = 4 }
                           };

            var actual = new List<TestClass>
                         {
                                 new TestClass { Value = 3 },
                                 new TestClass { Value = 4 },
                                 new TestClass { Value = 2 },
                                 new TestClass { Value = 1 }
                         }.OrderBy("Value", false);

            TestUtils.StrictEqual(expected, actual);
        }

        [Fact]
        public void OrderBy_ValidName_Desc_Test()
        {
            var expected = new List<TestClass>
                           {
                                   new TestClass { Value = 4 },
                                   new TestClass { Value = 3 },
                                   new TestClass { Value = 2 },
                                   new TestClass { Value = 1 }
                           };

            var actual = new List<TestClass>
                         {
                                 new TestClass { Value = 3 },
                                 new TestClass { Value = 4 },
                                 new TestClass { Value = 2 },
                                 new TestClass { Value = 1 }
                         }.OrderBy("Value", true);

            TestUtils.StrictEqual(expected, actual);
        }

        [Fact]
        public void OrderBy_InValidName_Test()
        {
            var expected = new List<TestClass>
                           {
                                   new TestClass { Value = 3 },
                                   new TestClass { Value = 4 },
                                   new TestClass { Value = 2 },
                                   new TestClass { Value = 1 }
                           };

            var actual = new List<TestClass>
                         {
                                 new TestClass { Value = 3 },
                                 new TestClass { Value = 4 },
                                 new TestClass { Value = 2 },
                                 new TestClass { Value = 1 }
                         }.OrderBy("Value4", true);

            TestUtils.StrictEqual(expected, actual);
        }
    }
}