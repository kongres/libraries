namespace Kongrevsky.Utilities.File.Models
{
    #region << Using >>

    using System;

    #endregion

    public class FileItem
    {
        public string Name { get; set; }

        public long Size { get; set; }

        public DateTime LastWriteTimeUtc { get; set; }

        public string MimeType { get; set; }
    }
}