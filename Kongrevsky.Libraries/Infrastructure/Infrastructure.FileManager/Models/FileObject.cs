namespace Kongrevsky.Infrastructure.FileManager.Models
{
    #region << Using >>

    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    #endregion

    public class FileObject : IDisposable
    {
        #region Properties

        public string FileName { get; set; }

        public Stream Stream { get; }

        #endregion

        #region Constructors

        public FileObject(Stream stream, string fileName)
        {
            Stream = stream;
            FileName = fileName;
        }

        #endregion

        #region Interface Implementations

        public void Dispose()
        {
            Stream?.Dispose();
        }

        #endregion

        public bool IsFileExists()
        {
            return Stream != null && Stream != Stream.Null;
        }

        public void CopyTo(Stream target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Stream.CopyTo(target, 81920);
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            await Stream.CopyToAsync(target, 81920, cancellationToken);
        }
    }
}