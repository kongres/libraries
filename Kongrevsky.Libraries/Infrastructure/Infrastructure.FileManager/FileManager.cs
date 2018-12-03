namespace Kongrevsky.Infrastructure.FileManager
{
    #region << Using >>

    using System;
    using System.IO;
    using Kongrevsky.Infrastructure.FileManager.Models;
    using Kongrevsky.Infrastructure.FileManager.Utils;
    using Kongrevsky.Infrastructure.Impersonator;
    using Kongrevsky.Utilities.Common;
    using Microsoft.Extensions.Options;

    #endregion

    public class FileManager : IFileManager
    {
        private string _getDirectoryPath(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                throw new ArgumentException("FileId can't be null or empty", nameof(fileId));

            var directoryPath = Path.Combine(_fileManagerOptions.RootPath, fileId);
            return directoryPath;
        }

        private string _getFilePath(string fileId)
        {
            var directoryPath = _getDirectoryPath(fileId);
            return Path.Combine(directoryPath, fileId);
        }

        #region Properties

        private FileManagerOptions _fileManagerOptions { get; }

        private UserCredentials _userCredentialsImpersonation { get; }

        #endregion

        #region Interface Implementations

        public FileManager(IOptions<FileManagerOptions> fileManagerOptions)
        {
            _fileManagerOptions = fileManagerOptions.Value ?? throw new ArgumentNullException(nameof(fileManagerOptions));
            if (string.IsNullOrEmpty(_fileManagerOptions.RootPath))
                throw new ArgumentNullException(nameof(_fileManagerOptions.RootPath));

            if ((_fileManagerOptions.Impersonation ?? (_fileManagerOptions.Impersonation = new ImpersonationOptions())).IsImpersonationEnabled)
            {
                _userCredentialsImpersonation = new UserCredentials(_fileManagerOptions.Impersonation.ImpersonationDomain, _fileManagerOptions.Impersonation.ImpersonationUsername, _fileManagerOptions.Impersonation.ImpersonationPassword);
                Impersonation.RunAsUser(_userCredentialsImpersonation,
                                        () => { Directory.CreateDirectory(_fileManagerOptions.RootPath); },
                                        LogonType.Interactive,
                                        LogonProvider.Default);
            }
            else
            {
                Directory.CreateDirectory(_fileManagerOptions.RootPath);
            }
        }

        public FileObject GetFile(string fileId, bool compress = false, string encryptPassword = null)
        {
            FileObject getFile()
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

            return _fileManagerOptions.Impersonation.IsImpersonationEnabled ? Impersonation.RunAsUser(_userCredentialsImpersonation, getFile, LogonType.Interactive, LogonProvider.Default) : getFile();
        }

        public bool SaveFile(string fileId, Stream stream, bool compress = false, string encryptPassword = null)
        {
            bool saveFile()
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
                                using (var encryptStream = FileUtils.FileEncrypt(compressedStream, encryptPassword))
                                {
                                    FileUtils.SaveFile(directoryPath, fileId, encryptStream);
                                }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(encryptPassword))
                            FileUtils.SaveFile(directoryPath, fileId, stream);
                        else
                            using (var encryptStream = FileUtils.FileEncrypt(stream, encryptPassword))
                            {
                                FileUtils.SaveFile(directoryPath, fileId, encryptStream);
                            }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return _fileManagerOptions.Impersonation.IsImpersonationEnabled ? Impersonation.RunAsUser(_userCredentialsImpersonation, saveFile, LogonType.Interactive, LogonProvider.Default) : saveFile();
        }

        public bool CheckFileExists(string fileId)
        {
            bool checkFileExists()
            {
                var filePath = _getFilePath(fileId);
                return File.Exists(filePath);
            }

            return _fileManagerOptions.Impersonation.IsImpersonationEnabled ? Impersonation.RunAsUser(_userCredentialsImpersonation, checkFileExists, LogonType.Interactive, LogonProvider.Default) : checkFileExists();
        }

        public void DeleteFile(string fileId)
        {
            void deleteFile()
            {
                RetryUtils.Do(() => FileUtils.DeleteDirectory(_getDirectoryPath(fileId)), TimeSpan.FromSeconds(1));
            }

            if (_fileManagerOptions.Impersonation.IsImpersonationEnabled)
                Impersonation.RunAsUser(_userCredentialsImpersonation, deleteFile);
            else
                deleteFile();
        }

        public bool TryDeleteFile(string fileId)
        {
            bool tryDeleteFile()
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

            return _fileManagerOptions.Impersonation.IsImpersonationEnabled ? Impersonation.RunAsUser(_userCredentialsImpersonation, tryDeleteFile, LogonType.Interactive, LogonProvider.Default) : tryDeleteFile();
        }

        #endregion
    }
}