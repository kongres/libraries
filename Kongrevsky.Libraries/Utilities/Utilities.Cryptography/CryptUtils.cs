namespace Kongrevsky.Utilities.Cryptography
{
    #region << Using >>

    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    #endregion

    public static class CryptUtils
    {
        #region Hash String

        /// <summary>
        /// Returns Sha256 hash from string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSha256Hash(string input)
        {
            var shaHash = SHA256.Create();
            // Convert the input string to a byte array and compute the hash.
            var data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        #endregion

        #region Authentication

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int KeySize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        /// <summary>
        /// Encrypts string (AES)
        /// </summary>
        /// <param name="plainText">Value</param>
        /// <param name="passPhrase">Encryption key</param>
        /// <returns></returns>
        public static string AES_Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(KeySize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Base36Utils.ByteArrayToBase36String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts encrypted string (AES)
        /// </summary>
        /// <param name="cipherText">Encrypted string</param>
        /// <param name="passPhrase">Decryption key</param>
        /// <returns></returns>
        public static string AES_Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Base36Utils.Base36StringToByteArray(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KeySize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KeySize / 8).Take(KeySize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((KeySize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((KeySize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(KeySize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }

            return randomBytes;
        }

        #endregion

        #region PasswordHash

        private const int NUM_ITERATIONS = 1000;

        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        /// <summary>
        /// Creates a signature for a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>the "salt:hash" for the password.</returns>
        public static string CreatePasswordSalt(string password, int saltSize = 64)
        {
            if (saltSize < 8)
                throw new ArgumentException("Salt is not at least eight bytes", nameof(saltSize));
                
            var buf = new byte[saltSize];
            rng.GetBytes(buf);
            var salt = Convert.ToBase64String(buf);

            var deriver2898 = new Rfc2898DeriveBytes(password.Trim(), buf, NUM_ITERATIONS);
            var hash = Convert.ToBase64String(deriver2898.GetBytes(16));
            return salt + ':' + hash;
        }

        /// <summary>
        /// Validates if a password will generate the passed in salt:hash.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="saltHash">The "salt:hash" this password should generate.</param>
        /// <returns>true if we have a match.</returns>
        public static bool IsPasswordValid(string password, string saltHash)
        {
            var parts = saltHash.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return false;

            var buf = Convert.FromBase64String(parts[0]);
            var deriver2898 = new Rfc2898DeriveBytes(password.Trim(), buf, NUM_ITERATIONS);
            var computedHash = Convert.ToBase64String(deriver2898.GetBytes(16));
            return parts[1].Equals(computedHash);
        }

        #endregion
    }
}