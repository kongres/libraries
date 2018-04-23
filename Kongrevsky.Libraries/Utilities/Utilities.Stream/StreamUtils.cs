namespace Kongrevsky.Utilities.Stream
{
    #region << Using >>

    using System.IO;

    #endregion

    public static class StreamUtils
    {
        /// <summary>
        /// Returns stream from specified string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Stream ToStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}