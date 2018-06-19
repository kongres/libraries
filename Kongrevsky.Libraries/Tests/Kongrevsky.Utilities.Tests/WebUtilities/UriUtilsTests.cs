namespace Kongrevsky.Utilities.Tests.WebUtilities
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using Kongrevsky.Utilities.Web;
    using Xunit;

    #endregion

    public class UriUtilsTests
    {
        [Fact]
        public void CheckUrl_Test()
        {
            Assert.True(UriUtils.CheckUrl("https://www.google.com/"), "UriUtils.CheckUrl('https://www.google.com/')");
            Assert.False(UriUtils.CheckUrl("https://www_.google.com/"), "UriUtils.CheckUrl('https://www_.google.com/')");
        }

        [Fact]
        public void AddParameter_OneParameterValue_Test()
        {
            var expected = "https://google.com/?q=3";
            var actual = (new Uri("https://google.com").AddParameter("q", 3.ToString())).ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddParameter_SeveralParameterValues_Test()
        {
            var expected = "https://google.com/?q=3&q=4&q=5&q=6";
            var actual = (new Uri("https://google.com").AddParameter("q", new List<string> { 3.ToString(), 4.ToString(), 5.ToString(), 6.ToString() })).ToString();

            Assert.Equal(expected, actual);
        }
    }
}