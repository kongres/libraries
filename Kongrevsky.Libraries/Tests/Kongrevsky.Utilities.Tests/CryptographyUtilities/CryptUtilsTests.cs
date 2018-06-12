namespace Kongrevsky.Utilities.Tests.CryptographyUtilities
{
    #region << Using >>

    using Kongrevsky.Utilities.Cryptography;
    using Xunit;

    #endregion

    public class CryptUtilsTests
    {
        [Fact]
        public void AES_Encrypt_Decrypt_Test()
        {
            var expected = "etwfwefewfwefwefwwefwe";
            var password = "HstrPghT";

            var encrypted = CryptUtils.AES_Encrypt(expected, password);
            var actual = CryptUtils.AES_Decrypt(encrypted, password);

            Assert.Equal(expected, actual);
        }
    }
}