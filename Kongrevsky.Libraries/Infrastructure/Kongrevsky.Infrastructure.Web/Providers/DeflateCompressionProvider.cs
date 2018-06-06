namespace Kongrevsky.Infrastructure.Web.Providers
{
    #region << Using >>

    using System.IO;
    using System.IO.Compression;
    using Microsoft.AspNetCore.ResponseCompression;

    #endregion

    public class DeflateCompressionProvider : ICompressionProvider
    {
        public string EncodingName => "deflate";

        public bool SupportsFlush => true;

        public Stream CreateStream(Stream outputStream)
        {
            return new DeflateStream(outputStream, CompressionLevel.Optimal);
        }
    }
}