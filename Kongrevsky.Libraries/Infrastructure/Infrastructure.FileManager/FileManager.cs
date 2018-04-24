﻿namespace Kongrevsky.Infrastructure.FileManager
{
    #region << Using >>

    using System;
    using System.IO;
    using Kongrevsky.Infrastructure.FileManager.Models;
    using Kongrevsky.Infrastructure.FileManager.Utils;
    using Kongrevsky.Utilities.Common;

    #endregion

    public class FileManager : IFileManager
    {
        #region Properties

        private FileStorageConfig _fileStorageConfig { get; set; } = new FileStorageConfig();

        #endregion

        #region Interface Implementations

        public void SetConfigs(FileStorageConfig fileStorageConfig)
        {
            _fileStorageConfig = fileStorageConfig ?? throw new ArgumentNullException(nameof(fileStorageConfig));
            if (string.IsNullOrEmpty(_fileStorageConfig.RootPath) || !Directory.Exists(_fileStorageConfig.RootPath))
                throw new ArgumentNullException(nameof(_fileStorageConfig.RootPath));
        }

        public void SetConfigs(Action<FileStorageConfig> configuration)
        {
            configuration(_fileStorageConfig);
        }

        public FileObject GetFile(string fileId, bool compress = false, string encryptPassword = null)
        {
            var filePath = _getFilePath(fileId);

            if (!FileUtils.IsFileExists(filePath))
                return new FileObject(Stream.Null, fileId);

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.Read, 4096, true);

            try
            {
                if (string.IsNullOrEmpty(encryptPassword))
                    if (compress)
                        using (fileStream)
                        {
                            return new FileObject(FileUtils.DecompressFile(fileStream), fileId);
                        }
                    else
                        return new FileObject(fileStream, fileId);
                else
                    using (fileStream)
                    {
                        var decryptFile = FileUtils.FileDecrypt(fileStream, encryptPassword);

                        if (compress)
                            using (decryptFile)
                            {
                                return new FileObject(FileUtils.DecompressFile(decryptFile), fileId);
                            }
                        else
                            return new FileObject(decryptFile, fileId);
                    }
            }
            catch (Exception e)
            {
                return new FileObject(Stream.Null, fileId);
            }
        }

        public bool SaveFile(string fileId, Stream stream, bool compress = false, string encryptPassword = null)
        {
            try
            {
                var directoryPath = _getDirectoryPath(fileId);

                if (compress)
                {
                    using (var compressedStream = FileUtils.CompressFile(stream))
                    {
                        if (string.IsNullOrEmpty(encryptPassword))
                            FileUtils.SaveFile(directoryPath, fileId, compressedStream);
                        else
                            using (var enryptStream = FileUtils.FileEncrypt(compressedStream, encryptPassword))
                            {
                                FileUtils.SaveFile(directoryPath, fileId, enryptStream);
                            }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(encryptPassword))
                        FileUtils.SaveFile(directoryPath, fileId, stream);
                    else
                        using (var enryptStream = FileUtils.FileEncrypt(stream, encryptPassword))
                        {
                            FileUtils.SaveFile(directoryPath, fileId, enryptStream);
                        }
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool CheckFileExists(string fileId)
        {
            var filePath = _getFilePath(fileId);
            return File.Exists(filePath);
        }

        public void DeleteFile(string fileId)
        {
            RetryUtils.Do(() => FileUtils.DeleteDirectory(_getDirectoryPath(fileId)), TimeSpan.FromSeconds(1));
        }

        public bool TryDeleteFile(string fileId)
        {
            try
            {
                DeleteFile(fileId);
                return true;
            }
            catch (Exception e)
            {
                // log
                return false;
            }
        }

        #endregion

        private string _getDirectoryPath(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                throw new ArgumentException("FileId can't be null or empty", nameof(fileId));

            var directoryPath = string.IsNullOrEmpty(_fileStorageConfig.SubFolder) ? Path.Combine(_fileStorageConfig.RootPath, fileId) : Path.Combine(_fileStorageConfig.RootPath, _fileStorageConfig.SubFolder, fileId);
            return directoryPath;
        }

        private string _getFilePath(string fileId)
        {
            var directoryPath = _getDirectoryPath(fileId);
            return Path.Combine(directoryPath, fileId);
        }
    }
}