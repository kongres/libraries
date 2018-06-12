namespace Kongrevsky.Utilities.Tests
{
    #region << Using >>

    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    #endregion

    internal static class TestUtils
    {
        public static void StrictEqual<T>(IEnumerable<T> firstEnumerable, IEnumerable<T> secondEnumerable)
        {
            Assert.NotNull(firstEnumerable);
            Assert.NotNull(secondEnumerable);

            var firstList = firstEnumerable.ToList();
            var secondList = secondEnumerable.ToList();

            Assert.Equal(firstList.Count, secondList.Count);

            for (var i = 0; i < firstList.Count; i++)
                Assert.True(firstList[i].Equals(secondList[i]),
                            $"firstList[{i}].Equals(secondList[{i}])");
        }
    }
}