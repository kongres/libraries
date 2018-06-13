namespace Kongrevsky.Utilities.Tests.StringUtilities
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using Kongrevsky.Utilities.Enumerable;
    using Kongrevsky.Utilities.String;
    using Xunit;

    #endregion

    public class StringUtilsTests
    {
        [Fact]
        public void IfNullOrEmpty_EmptyString_Test()
        {
            var expected = "default";
            var actual = "".IfNullOrEmpty("default");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IfNullOrEmpty_NullString_Test()
        {
            var expected = "default";
            var actual = ((string)null).IfNullOrEmpty("default");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IfNullOrEmpty_NotEmptyString_Test()
        {
            var expected = "string1";
            var actual = "string1".IfNullOrEmpty("default");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsNullOrEmpty_Test()
        {
            Assert.True("".IsNullOrEmpty(), "''.IsNullOrEmpty()");
            Assert.True(((string)null).IsNullOrEmpty(), "((string)null).IsNullOrEmpty()");
            Assert.False("1".IsNullOrEmpty(), "'1'.IsNullOrEmpty()");
            Assert.False(" ".IsNullOrEmpty(), "' '.IsNullOrEmpty()");
        }

        [Fact]
        public void IsNullOrWhiteSpace_Test()
        {
            Assert.True("".IsNullOrWhiteSpace(), "''.IsNullOrWhiteSpace()");
            Assert.True(((string)null).IsNullOrWhiteSpace(), "((string)null).IsNullOrWhiteSpace()");
            Assert.False("1".IsNullOrWhiteSpace(), "'1'.IsNullOrWhiteSpace()");
            Assert.True(" ".IsNullOrWhiteSpace(), "' '.IsNullOrWhiteSpace()");
        }

        [Fact]
        public void ToCamelCase_NotCamelString_Test()
        {
            var expected = "testTestTest";
            var actual = "TestTestTest".ToCamelCase();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToCamelCase_CamelString_Test()
        {
            var expected = "testTestTest";
            var actual = "testTestTest".ToCamelCase();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SplitBySpaces_OneSpace_test()
        {
            var expected = new[] { "string", "string", "string", "hh" };
            var actual = "string string string hh".SplitBySpaces();

            TestUtils.StrictEqual(expected, actual);
        }

        [Fact]
        public void SplitBySpaces_SeveralSpaces_test()
        {
            var expected = new[] { "string", "string", "string", "hh" };
            var actual = "string  string   string     hh".SplitBySpaces();

            TestUtils.StrictEqual(expected, actual);
        }

        [Fact]
        public void ExtractDigits_StringWithDigits_Test()
        {
            var expected = "4568889112456";
            var actual = "string4_5 string68 8/89 string hh__112````&&&4*56".ExtractDigits();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ExtractDigits_StringWithoutDigits_Test()
        {
            var expected = "";
            var actual = "string string string hh__````&&&".ExtractDigits();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Contains_SecondStringIsSubstring_Test()
        {
            var firstString = "string string string hh__````&&&";
            var secondString = "string";

            Assert.True(firstString.Contains(secondString, StringComparison.CurrentCulture),
                        "firstString.Contains(secondString, StringComparison.CurrentCulture)");

            Assert.False(secondString.Contains(firstString, StringComparison.CurrentCulture),
                         "secondString.Contains(firstString, StringComparison.CurrentCulture)");
        }

        [Fact]
        public void Contains_FirstStringIsSubstring_Test()
        {
            var firstString = "888";
            var secondString = "string 888 test";

            Assert.True(secondString.Contains(firstString, StringComparison.CurrentCulture),
                        "secondString.Contains(firstString, StringComparison.CurrentCulture)");

            Assert.False(firstString.Contains(secondString, StringComparison.CurrentCulture),
                         "firstString.Contains(secondString, StringComparison.CurrentCulture)");
        }

        [Fact]
        public void Contains_SecondStringIsEmpty_Test()
        {
            var firstString = "string string string hh__````&&&";
            var secondString = "";

            Assert.True(firstString.Contains(secondString, StringComparison.CurrentCulture),
                        "firstString.Contains(secondString, StringComparison.CurrentCulture)");

            Assert.False(secondString.Contains(firstString, StringComparison.CurrentCulture),
                         "secondString.Contains(firstString, StringComparison.CurrentCulture)");
        }

        [Fact]
        public void Contains_FirstStringIsEmpty_Test()
        {
            var firstString = "";
            var secondString = "string 888 test";

            Assert.True(secondString.Contains(firstString, StringComparison.CurrentCulture),
                        "secondString.Contains(firstString, StringComparison.CurrentCulture)");

            Assert.False(firstString.Contains(secondString, StringComparison.CurrentCulture),
                         "firstString.Contains(secondString, StringComparison.CurrentCulture)");
        }

        [Fact]
        public void Contains_BothStringsAreDifferent_Test()
        {
            var firstString = "wefwefwefwef";
            var secondString = "string 888 test";

            Assert.False(secondString.Contains(firstString, StringComparison.CurrentCulture),
                         "secondString.Contains(firstString, StringComparison.CurrentCulture)");

            Assert.False(firstString.Contains(secondString, StringComparison.CurrentCulture),
                         "firstString.Contains(secondString, StringComparison.CurrentCulture)");
        }

        [Fact]
        public void Contains_BothStringsAreTheSame_Test()
        {
            var firstString = "string 888 test";
            var secondString = "string 888 test";

            Assert.True(secondString.Contains(firstString, StringComparison.CurrentCulture),
                        "secondString.Contains(firstString, StringComparison.CurrentCulture)");

            Assert.True(firstString.Contains(secondString, StringComparison.CurrentCulture),
                        "firstString.Contains(secondString, StringComparison.CurrentCulture)");
        }

        [Fact]
        public void Contains_SecondStringIsSubstringIgnoreCases_Test()
        {
            var firstString = "string string string hh__````&&&";
            var secondString = "sTRing";

            Assert.True(firstString.Contains(secondString, StringComparison.CurrentCultureIgnoreCase),
                        "firstString.Contains(secondString, StringComparison.CurrentCultureIgnoreCase)");

            Assert.False(secondString.Contains(firstString, StringComparison.CurrentCultureIgnoreCase),
                         "secondString.Contains(firstString, StringComparison.CurrentCultureIgnoreCase)");
        }

        [Fact]
        public void Contains_FirstStringIsSubstringIgnoreCases_Test()
        {
            var firstString = "STring";
            var secondString = "string 888 test";

            Assert.True(secondString.Contains(firstString, StringComparison.CurrentCultureIgnoreCase),
                        "secondString.Contains(firstString, StringComparison.CurrentCultureIgnoreCase)");

            Assert.False(firstString.Contains(secondString, StringComparison.CurrentCultureIgnoreCase),
                         "firstString.Contains(secondString, StringComparison.CurrentCultureIgnoreCase)");
        }

        [Fact]
        public void SplitCamelCase_Standard_Test()
        {
            var expected = "string String String";
            var actual = "stringStringString".SplitCamelCase();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SplitCamelCase_SeparatedString_Test()
        {
            var expected = "string_String String";
            var actual = "string_StringString".SplitCamelCase();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValidJson_Test()
        {
            Assert.True(@"{Value:12, Test:""first"", Array1:[1,2,3,4]}".IsValidJson(), "@'{Value:12, Test:''first'', Array1:[1,2,3,4]}'.IsValidJson()");
            Assert.False(@"{12, Test:""first""}".IsValidJson(), "@'{12, Test:''first''}'.IsValidJson()");
        }

        [Fact]
        public void IsValidJsonArray_Test()
        {
            Assert.True(@"[{Value:""23"", Test:12},{Value:""23"", Test:12},{Value:""23"", Test:12}]".IsValidArrayJson());
            Assert.False("{[1,2,3]}".IsValidArrayJson());
        }

        [Fact]
        public void TrimOrDefault_Test()
        {
            var expected = "string";
            var actual = "string".TrimOrDefault();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TrimOrDefault_NullString_Test()
        {
            var expected = (string)null;
            var actual = ((string)null).TrimOrDefault();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToListInt_Test()
        {
            var expected = new List<int> { 12, 34, 85, 12 };
            var actual = "12*34*85*12".ToListInt('*');

            TestUtils.StrictEqual(expected, actual);
        }

        [Fact]
        public void ContainsOnce_Test()
        {
            var firstString = "string string string hhString_12";
            var secondString = "hh";

            Assert.True(firstString.ContainsOnce(secondString, StringComparison.CurrentCulture),
                        "firstString.ContainsOnce(secondString, StringComparison.CurrentCulture)");

            Assert.False(secondString.ContainsOnce(firstString, StringComparison.CurrentCulture),
                         "secondString.ContainsOnce(firstString, StringComparison.CurrentCulture)");
        }

        [Fact]
        public void ContainsOnce_IgnoreCases_Test()
        {
            var firstString = "sTrING test_rtrttrtr";
            var secondString = "string";

            Assert.True(firstString.ContainsOnce(secondString, StringComparison.CurrentCultureIgnoreCase),
                        "firstString.ContainsOnce(secondString, StringComparison.CurrentCultureIgnoreCase)");

            Assert.False(secondString.ContainsOnce(firstString, StringComparison.CurrentCultureIgnoreCase),
                         "secondString.ContainsOnce(firstString, StringComparison.CurrentCultureIgnoreCase)");
        }

        [Fact]
        public void TrimStart_Test()
        {
            var expected = " String sTring hh__````&&&";
            var actual = "string String sTring hh__````&&&".TrimStart("string", StringComparison.CurrentCulture);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TrimStart_IgnoreCases_Test()
        {
            var expected = "hh__````&&&";
            var actual = "stringStringsTringhh__````&&&".TrimStart("sTriNg", StringComparison.CurrentCultureIgnoreCase);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TrimEnd_Test()
        {
            var expected = "string String sTring hh__````&&&";
            var actual = "string String sTring hh__````&&& _string".TrimEnd(" _string", StringComparison.CurrentCulture);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TrimEnd_IgnoreCases_Test()
        {
            var expected = "string String sTring hh__````&&&";
            var actual = "string String sTring hh__````&&& _string".TrimEnd(" _stRinG", StringComparison.CurrentCultureIgnoreCase);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Trim_Test()
        {
            var expected = " StRiNg String_HHHH string String";
            var actual = " string StRiNg String_HHHH string String string".Trim(" string", StringComparison.CurrentCulture);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Trim_IgnoreCases_Test()
        {
            var expected = "_HHHH";
            var actual = " string StRiNg String_HHHH string String string".Trim(" stRing", StringComparison.CurrentCultureIgnoreCase);

            Assert.Equal(expected, actual);
        }
    }
}