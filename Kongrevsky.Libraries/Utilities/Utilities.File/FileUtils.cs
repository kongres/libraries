namespace Kongrevsky.Utilities.File
{
    #region << Using >>

    using System.IO;
    using System.Linq;
    using Kongrevsky.Utilities.File.Models;
    using Microsoft.Win32;

    #endregion

    public static class FileUtils
    {
        /// <summary>
        /// Get Tree structure of Directory
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns></returns>
        public static DirectoryItem GetTreeInfo(this DirectoryInfo directoryInfo)
        {
            var directories = directoryInfo.EnumerateDirectories();
            var files = directoryInfo.EnumerateFiles();

            var folder = new DirectoryItem()
                         {
                                 Name = directoryInfo.Name,
                                 LastWriteTimeUtc = directoryInfo.LastWriteTimeUtc,
                                 Folders = directories.Select(GetTreeInfo).ToList(),
                                 Files = files.Select(f => new FileItem()
                                                           {
                                                                   Name = f.Name,
                                                                   Size = f.Length,
                                                                   LastWriteTimeUtc = f.LastWriteTimeUtc,
                                                                   MimeType = GetMimeType(f)
                                                           }).ToList()
                         };
            return folder;
        }

        /// <summary>
        /// Get MimeType of File
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static string GetMimeType(this FileInfo fileInfo)
        {
            var ext = fileInfo.Extension.ToLower();
            var regKey = Registry.ClassesRoot.OpenSubKey(ext);
            const string contentType = "Content Type";
            return regKey?.GetValue(contentType) != null ? regKey.GetValue(contentType).ToString() : "application/octet-stream";
        }
    }
}