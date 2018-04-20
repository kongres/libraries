namespace Utilities.File.Mime
{
    /// <summary>
    /// Little data structure to hold information about file types. 
    /// Holds information about binary header at the start of the file
    /// </summary>
    public class FileType
    {
        #region Properties

        /*internal byte?[] Header { get; set; }    // most of the times we only need first 8 bytes, but sometimes extend for 16
        internal int HeaderOffset { get; set; }
        internal string Extension { get; set; }
        internal string Mime { get; set; }*/

        public byte?[] Header { get; set; }

        public int HeaderOffset { get; set; }

        public string Extension { get; set; }

        public string Mime { get; set; }

        #endregion

        #region Constructors

        public FileType() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileType"/> class.
        /// Default construction with the header offset being set to zero by default
        /// </summary>
        /// <param name="header">Byte array with header.</param>
        /// <param name="extension">String with extension.</param>
        /// <param name="mime">The description of MIME.</param>
        public FileType(byte?[] header, string extension, string mime)
        {
            Header = header;
            Extension = extension;
            Mime = mime;
            HeaderOffset = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileType"/> struct.
        /// Takes the details of offset for the header
        /// </summary>
        /// <param name="header">Byte array with header.</param>
        /// <param name="offset">The header offset - how far into the file we need to read the header</param>
        /// <param name="extension">String with extension.</param>
        /// <param name="mime">The description of MIME.</param>
        public FileType(byte?[] header, int offset, string extension, string mime)
        {
            Header = null;
            Header = header;
            HeaderOffset = offset;
            Extension = extension;
            Mime = mime;
        }

        #endregion

        public override bool Equals(object other)
        {
            if (!(other is FileType)) return false;

            var otherType = (FileType)other;

            if (Extension == otherType.Extension && Mime == otherType.Mime) return true;

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Extension;
        }
    }
}