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

        /// <summary>Read a stream into a byte array</summary>
        /// <param name="input">Stream to read</param>
        /// <returns>byte[]</returns>
        public static byte[] ReadAsBytes(this Stream input)
        {
            byte[] buffer = new byte[16384];
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int count;
                while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
                    memoryStream.Write(buffer, 0, count);
                return memoryStream.ToArray();
            }
        }
    }
}