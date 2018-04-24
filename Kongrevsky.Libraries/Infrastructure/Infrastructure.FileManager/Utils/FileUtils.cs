namespace Kongrevsky.Infrastructure.FileManager.Utils
{
    #region << Using >>

    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Security.Cryptography;

    #endregion

    public static class FileUtils
    {
        #region Constants

        private static readonly string[] sizes = { "B", "KB", "MB", "GB", "TB" };

        #endregion

        /// <summary>
        /// Creates a random salt that will be used to encrypt your file. This method is required on FileEncrypt.
        /// </summary>
        /// <returns></returns>
        private static byte[] GenerateRandomSalt()
        {
            var data = new byte[32];

            using (var rng = new RNGCryptoServiceProvider())
            {
                for (var i = 0; i < 10; i++)
                        // Fille the buffer with the generated data
                    rng.GetBytes(data);
            }

            return data;
        }

        /// <summary>
        /// Encrypts a file from its path and a plain password.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="password"></param>
        public static Stream FileEncrypt(Stream inputFile, string password)
        {
            //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

            //generate random salt
            var salt = GenerateRandomSalt();

            //convert password string to byte arrray
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            //Set Rijndael symmetric encryption algorithm
            var AES = new RijndaelManaged
                      {
                              KeySize = 256,
                              BlockSize = 128,
                              Padding = PaddingMode.PKCS7,
                              Mode = CipherMode.CFB
                      };

            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            var tempFileName = Path.GetTempFileName();

            using (var fsCrypt = new FileStream(tempFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 1048576))
            {
                fsCrypt.Write(salt, 0, salt.Length);
                using (var cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    inputFile.CopyTo(cs, 1048576);
                }
            }

            return new FileStream(tempFileName, FileMode.Open, FileAccess.Read, FileShare.None, 1048576, FileOptions.DeleteOnClose);
        }

        /// <summary>
        /// Decrypts an encrypted file with the FileEncrypt method.
        /// </summary>
        public static Stream FileDecrypt(Stream inputFile, string password)
        {
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            var salt = new byte[32];

            inputFile.Read(salt, 0, salt.Length);

            var AES = new RijndaelManaged
                      {
                              KeySize = 256,
                              BlockSize = 128,
                              Padding = PaddingMode.PKCS7,
                              Mode = CipherMode.CFB
                      };

            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            var tempFileName = Path.GetTempFileName();

            using (var fsOut = new FileStream(tempFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 1048576))
            {
                using (var cs = new CryptoStream(inputFile, AES.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cs.CopyTo(fsOut);
                }
            }

            return new FileStream(tempFileName, FileMode.Open, FileAccess.Read, FileShare.None, 1048576, FileOptions.DeleteOnClose);
        }

        public static Stream CompressFile(Stream inputFile)
        {
            var outputFile = new MemoryStream();
            using (var Compress = new GZipStream(outputFile, CompressionLevel.Optimal, true))
            {
                inputFile.CopyTo(Compress);
            }

            outputFile.Seek(0, SeekOrigin.Begin);
            return outputFile;
        }

        public static Stream DecompressFile(Stream inputFile)
        {
            var outputFile = new MemoryStream();
            using (var compress = new GZipStream(inputFile, CompressionMode.Decompress, true))
            {
                compress.CopyTo(outputFile);
            }

            outputFile.Seek(0, SeekOrigin.Begin);
            return outputFile;
        }

        public static bool SaveFile(string directoryPath, string fileName, Stream fileStream)
        {
            try
            {
                DeleteDirectory(directoryPath);
                CreateDirectoryIfNotExists(directoryPath);

                var filePath = Path.Combine(directoryPath, fileName);

                using (var file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, FileOptions.None))
                {
                    file.Position = 0;
                    fileStream.CopyTo(file);
                }

                fileStream.Position = 0;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsFileExists(string filePath)
        {
            var directory = new FileInfo(filePath);
            return directory.Exists;
        }

        public static bool DeleteFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
                try
                {
                    fileInfo.Delete();
                    return true;
                }
                catch
                {
                    return false;
                }

            return true;
        }

        public static void CreateDirectoryIfNotExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        public static bool DeleteDirectory(string directoryPath)
        {
            var directory = new DirectoryInfo(directoryPath);

            if (directory.Exists)
                try
                {
                    directory.Delete(true);
                    return true;
                }
                catch
                {
                    return false;
                }

            return true;
        }

        public static string GetFormattedFileSize(long length)
        {
            var order = 0;
            while (length >= 1024 && order < sizes.Length - 1)
            {
                order++;
                length = length / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            var result = $"{length:0.##} {sizes[order]}";
            return result;
        }
    }
}