namespace Kongrevsky.Utilities.File.Models
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    public class DirectoryItem
    {
        public string Name { get; set; }

        public long Size => (Folders?.Sum(x => (long?)x.Size) ?? 0) + (Files?.Sum(x => (long?)x.Size) ?? 0);

        public DateTime LastWriteTimeUtc { get; set; }

        public IEnumerable<DirectoryItem> Folders { get; set; }

        public IEnumerable<FileItem> Files { get; set; }
    }
}