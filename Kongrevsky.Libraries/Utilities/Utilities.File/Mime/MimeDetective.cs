namespace Utilities.File.Mime
{
    #region << Using >>

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    #endregion

    public class MimeDetective
    {
        /// <summary>
        /// Returns Mime type
        /// </summary>
        /// <param name="file"></param>
        /// <param name="mimeType"></param>
        /// <param name="headerSize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static FileType LearnMimeType(FileInfo file, string mimeType, int headerSize, int offset = 0)
        {
            var data = new byte?[headerSize];
            using (var stream = file.OpenRead())
            {
                for (var i = 0; i < headerSize; i++)
                {
                    var b = 0;
                    data[i] = (byte)((b = stream.ReadByte()) == -1 ? 0 : b);
                    if (b == -1)
                        break;
                }
            }

            return new FileType(data, offset, file.Extension, mimeType);
        }

        /// <summary>
        /// Returns Mime type
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="mimeType"></param>
        /// <param name="maxHeaderSize"></param>
        /// <param name="minMatches"></param>
        /// <param name="maxNonMatch"></param>
        /// <returns></returns>
        public static FileType LearnMimeType(FileInfo first, FileInfo second, string mimeType, int maxHeaderSize = 12, int minMatches = 2, int maxNonMatch = 3)
        {
            var headerList = new List<byte?>();

            using (Stream firstFile = first.OpenRead())
            {
                using (Stream secondFile = second.OpenRead())
                {
                    var match = false;
                    var mismatchCounter = 0; // mismatches after first match

                    int bFst = 0, bSnd = 0; // current bytes
                    var index = 0;
                    var offset = 0; // index of first match

                    // Read from both files until one of the file streams reaches the end.
                    while ((bFst = firstFile.ReadByte()) != -1 &&
                           (bSnd = secondFile.ReadByte()) != -1)
                    {
                        bFst = firstFile.ReadByte();
                        bSnd = secondFile.ReadByte();

                        if (bFst == bSnd)
                        {
                            if (!match)
                            {
                                match = true; // first match
                                offset = index;
                            }

                            headerList.Add((byte)bFst); // add match to header 
                        }
                        else
                        {
                            if (match)
                                if (mismatchCounter < maxNonMatch)
                                {
                                    headerList.Add(null); // Add a null header, this could be non generic, file size for example
                                    mismatchCounter++;
                                }
                                else
                                {
                                    break; // too much missmatches after the first match 
                                }
                        }

                        if (headerList.Count == maxHeaderSize)
                            break;

                        index++;
                    }

                    FileType type = null;

                    if (headerList.Count((b) => b != null) >= minMatches) // check for enough non null byte? ´s.
                    {
                        var header = headerList.ToArray();
                        type = new FileType(header, offset, first.Extension, mimeType);
                    }

                    return type;
                }
            }
        }
    }
}