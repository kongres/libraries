namespace Kongrevsky.Utilities.Tests.CryptographyUtilities
{
    #region << Using >>

    using System;
    using Kongrevsky.Utilities.Cryptography;
    using Xunit;

    #endregion

    public class Base36UtilsTests
    {
        [Fact]
        public void GetBytes_ToBase36FromBase36_Test()
        {
            const int value = 3;

            var array = BitConverter.GetBytes(value);
            var base36String = Base36Utils.ByteArrayToBase36String(array);
            var actual = Base36Utils.Base36StringToByteArray(base36String);

            var expected = BitConverter.GetBytes(value);

            TestUtils.StrictEqual(expected, actual);
        }
    }
}