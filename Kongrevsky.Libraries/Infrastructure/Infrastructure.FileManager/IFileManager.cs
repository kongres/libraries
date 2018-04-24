namespace Kongrevsky.Infrastructure.FileManager
{
    #region << Using >>

    using System;
    using System.IO;
    using Kongrevsky.Infrastructure.FileManager.Models;

    #endregion

    public interface IFileManager
    {
        void SetConfigs(FileStorageConfig fileStorageConfig);

        void SetConfigs(Action<FileStorageConfig> configuration);

        FileObject GetFile(string fileId, bool compress = false, string encryptPassword = null);

        bool SaveFile(string fileId, Stream stream, bool compress = false, string encryptPassword = null);

        bool CheckFileExists(string fileId);

        void DeleteFile(string fileId);

        bool TryDeleteFile(string fileId);
    }
}