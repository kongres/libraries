namespace Kongrevsky.Infrastructure.FileManager
{
    #region << Using >>

    using System.IO;
    using Kongrevsky.Infrastructure.FileManager.Models;

    #endregion

    public interface IFileManager
    {
        /// <summary>
        /// Returns file
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="compress"></param>
        /// <param name="encryptPassword"></param>
        /// <returns></returns>
        FileObject GetFile(string fileId, bool compress = false, string encryptPassword = null);

        /// <summary>
        /// Tries to save and returns if process was successful
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="stream"></param>
        /// <param name="compress"></param>
        /// <param name="encryptPassword"></param>
        /// <returns></returns>
        bool SaveFile(string fileId, Stream stream, bool compress = false, string encryptPassword = null);

        /// <summary>
        /// Detects if file exists
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        bool CheckFileExists(string fileId);

        /// <summary>
        /// Deletes file
        /// </summary>
        /// <param name="fileId"></param>
        void DeleteFile(string fileId);

        /// <summary>
        /// Tries to delete file and returns if process was successful
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        bool TryDeleteFile(string fileId);
    }
}