namespace Kongrevsky.Utilities.File.Mime
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml.Serialization;

    #endregion

    public static class MimeTypes
    {
        #region Constructors

        static MimeTypes()
        {
            types = new List<FileType>
                    {
                            PDF,
                            WORD,
                            EXCEL,
                            JPEG,
                            ZIP,
                            RAR,
                            HTML,
                            RTF,
                            PNG,
                            PPT,
                            GIF,
                            DLL_EXE,
                            MSDOC,
                            BMP,
                            DLL_EXE,
                            ZIP_7z,
                            ZIP_7z_2,
                            GZ_TGZ,
                            TAR_ZH,
                            TAR_ZV,
                            OGG,
                            ICO,
                            XML,
                            MIDI,
                            FLV,
                            WAVE,
                            DWG,
                            LIB_COFF,
                            PST,
                            PSD,
                            AES,
                            SKR,
                            SKR_2,
                            PKR,
                            EML_FROM,
                            ELF,
                            TXT_UTF8,
                            TXT_UTF16_BE,
                            TXT_UTF16_LE,
                            TXT_UTF32_BE,
                            TXT_UTF32_LE
                    };
        }

        #endregion

        #region Constants

        // number of bytes we read from a file
        public const int MaxHeaderSize = 560; // some file formats have headers offset to 512 bytes

        // all the file types to be put into one list
        public static List<FileType> types;

        public static readonly FileType LIB_COFF = new FileType(new byte?[] { 0x21, 0x3C, 0x61, 0x72, 0x63, 0x68, 0x3E, 0x0A }, "lib", "application/octet-stream");

        /*
         * 46 72 6F 6D 20 20 20 or	 	From
        46 72 6F 6D 20 3F 3F 3F or	 	From ???
        46 72 6F 6D 3A 20	 	From:
        EML	 	A commmon file extension for e-mail files. Signatures shown here
        are for Netscape, Eudora, and a generic signature, respectively.
        EML is also used by Outlook Express and QuickMail.
         */
        public static readonly FileType EML_FROM = new FileType(new byte?[] { 0x46, 0x72, 0x6F, 0x6D }, "eml", "message/rfc822");

        //EVTX	 	Windows Vista event log file
        public static readonly FileType ELF = new FileType(new byte?[] { 0x45, 0x6C, 0x66, 0x46, 0x69, 0x6C, 0x65, 0x00 }, "elf", "text/plain");

        #endregion

        // file headers are taken from here:
        //http://www.garykessler.net/library/file_sigs.html
        //mime types are taken from here:
        //http://www.webmaster-toolkit.com/mime-types.shtml

        #region office, excel, ppt and documents, xml, pdf, rtf, msdoc

        // office and documents
        public static readonly FileType WORD = new FileType(new byte?[] { 0xEC, 0xA5, 0xC1, 0x00 }, 512, "doc", "application/msword");

        public static readonly FileType EXCEL = new FileType(new byte?[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 }, 512, "xls", "application/excel");

        public static readonly FileType PPT = new FileType(new byte?[] { 0xFD, 0xFF, 0xFF, 0xFF, null, 0x00, 0x00, 0x00 }, 512, "ppt", "application/mspowerpoint");

        //ms office and openoffice docs (they're zip files: rename and enjoy!)
        //don't add them to the list, as they will be 'subtypes' of the ZIP type
        public static readonly FileType WORDX = new FileType(new byte?[0], 512, "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

        public static readonly FileType EXCELX = new FileType(new byte?[0], 512, "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        public static readonly FileType ODT = new FileType(new byte?[0], 512, "odt", "application/vnd.oasis.opendocument.text");

        public static readonly FileType ODS = new FileType(new byte?[0], 512, "ods", "application/vnd.oasis.opendocument.spreadsheet");

        // common documents
        public static readonly FileType RTF = new FileType(new byte?[] { 0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31 }, "rtf", "application/rtf");

        public static readonly FileType PDF = new FileType(new byte?[] { 0x25, 0x50, 0x44, 0x46 }, "pdf", "application/pdf");

        public static readonly FileType MSDOC = new FileType(new byte?[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }, "", "application/octet-stream");

        //application/xml text/xml
        public static readonly FileType XML = new FileType(new byte?[] { 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22 },
                                                           "xml,xul",
                                                           "text/xml");

        //text files
        public static readonly FileType HTML = new FileType(new byte?[] { 0x3C, 0x21, 0x44, 0x4F, 0x43, 0x54, 0x59, 0x50 }, "html", "text/html");

        public static readonly FileType TXT = new FileType(new byte?[0], "txt", "text/plain");

        public static readonly FileType TXT_UTF8 = new FileType(new byte?[] { 0xEF, 0xBB, 0xBF }, "txt", "text/plain");

        public static readonly FileType TXT_UTF16_BE = new FileType(new byte?[] { 0xFE, 0xFF }, "txt", "text/plain");

        public static readonly FileType TXT_UTF16_LE = new FileType(new byte?[] { 0xFF, 0xFE }, "txt", "text/plain");

        public static readonly FileType TXT_UTF32_BE = new FileType(new byte?[] { 0x00, 0x00, 0xFE, 0xFF }, "txt", "text/plain");

        public static readonly FileType TXT_UTF32_LE = new FileType(new byte?[] { 0xFF, 0xFE, 0x00, 0x00 }, "txt", "text/plain");

        #endregion

        // graphics

        #region Graphics jpeg, png, gif, bmp, ico

        public static readonly FileType JPEG = new FileType(new byte?[] { 0xFF, 0xD8, 0xFF }, "jpg", "image/jpeg");

        public static readonly FileType PNG = new FileType(new byte?[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "png", "image/png");

        public static readonly FileType GIF = new FileType(new byte?[] { 0x47, 0x49, 0x46, 0x38, null, 0x61 }, "gif", "image/gif");

        public static readonly FileType BMP = new FileType(new byte?[] { 66, 77 }, "bmp", "image/gif");

        public static readonly FileType ICO = new FileType(new byte?[] { 0, 0, 1, 0 }, "ico", "image/x-icon");

        #endregion

        //bmp, tiff

        #region Zip, 7zip, rar, dll_exe, tar, bz2, gz_tgz

        public static readonly FileType GZ_TGZ = new FileType(new byte?[] { 0x1F, 0x8B, 0x08 }, "gz, tgz", "application/x-gz");

        public static readonly FileType ZIP_7z = new FileType(new byte?[] { 66, 77 }, "7z", "application/x-compressed");

        public static readonly FileType ZIP_7z_2 = new FileType(new byte?[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C }, "7z", "application/x-compressed");

        public static readonly FileType ZIP = new FileType(new byte?[] { 0x50, 0x4B, 0x03, 0x04 }, "zip", "application/x-compressed");

        public static readonly FileType RAR = new FileType(new byte?[] { 0x52, 0x61, 0x72, 0x21 }, "rar", "application/x-compressed");

        public static readonly FileType DLL_EXE = new FileType(new byte?[] { 0x4D, 0x5A }, "dll, exe", "application/octet-stream");

        //Compressed tape archive file using standard (Lempel-Ziv-Welch) compression
        public static readonly FileType TAR_ZV = new FileType(new byte?[] { 0x1F, 0x9D }, "tar.z", "application/x-tar");

        //Compressed tape archive file using LZH (Lempel-Ziv-Huffman) compression
        public static readonly FileType TAR_ZH = new FileType(new byte?[] { 0x1F, 0xA0 }, "tar.z", "application/x-tar");

        //bzip2 compressed archive
        public static readonly FileType BZ2 = new FileType(new byte?[] { 0x42, 0x5A, 0x68 }, "bz2,tar,bz2,tbz2,tb2", "application/x-bzip2");

        #endregion

        #region Media ogg, midi, flv, dwg, pst, psd

        // media 
        public static readonly FileType OGG = new FileType(new byte?[] { 103, 103, 83, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 }, "oga,ogg,ogv,ogx", "application/ogg");

        //MID, MIDI	 	Musical Instrument Digital Interface (MIDI) sound file
        public static readonly FileType MIDI = new FileType(new byte?[] { 0x4D, 0x54, 0x68, 0x64 }, "midi,mid", "audio/midi");

        //FLV	 	Flash video file
        public static readonly FileType FLV = new FileType(new byte?[] { 0x46, 0x4C, 0x56, 0x01 }, "flv", "application/unknown");

        //WAV	 	Resource Interchange File Format -- Audio for Windows file, where xx xx xx xx is the file size (little endian), audio/wav audio/x-wav

        public static readonly FileType WAVE = new FileType(new byte?[]
                                                            {
                                                                    0x52, 0x49, 0x46, 0x46, null, null, null, null,
                                                                    0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20
                                                            },
                                                            "wav",
                                                            "audio/wav");

        public static readonly FileType PST = new FileType(new byte?[] { 0x21, 0x42, 0x44, 0x4E }, "pst", "application/octet-stream");

        //eneric AutoCAD drawing image/vnd.dwg  image/x-dwg application/acad
        public static readonly FileType DWG = new FileType(new byte?[] { 0x41, 0x43, 0x31, 0x30 }, "dwg", "application/acad");

        //Photoshop image file
        public static readonly FileType PSD = new FileType(new byte?[] { 0x38, 0x42, 0x50, 0x53 }, "psd", "application/octet-stream");

        #endregion

        #region Crypto aes, skr, skr_2, pkr

        //AES Crypt file format. (The fourth byte is the version number.)
        public static readonly FileType AES = new FileType(new byte?[] { 0x41, 0x45, 0x53 }, "aes", "application/octet-stream");

        //SKR	 	PGP secret keyring file
        public static readonly FileType SKR = new FileType(new byte?[] { 0x95, 0x00 }, "skr", "application/octet-stream");

        //SKR	 	PGP secret keyring file
        public static readonly FileType SKR_2 = new FileType(new byte?[] { 0x95, 0x01 }, "skr", "application/octet-stream");

        //PKR	 	PGP public keyring file
        public static readonly FileType PKR = new FileType(new byte?[] { 0x99, 0x01 }, "pkr", "application/octet-stream");

        #endregion

        #region Main Methods

        public static void SaveToXmlFile(string path)
        {
            using (var file = File.OpenWrite(path))
            {
                var serializer = new XmlSerializer(types.GetType());
                serializer.Serialize(file, types);
            }
        }

        public static void LoadFromXmlFile(string path)
        {
            using (var file = File.OpenRead(path))
            {
                var serializer = new XmlSerializer(types.GetType());
                var tmpTypes = (List<FileType>)serializer.Deserialize(file);
                foreach (var type in tmpTypes)
                    types.Add(type);
            }
        }

        /// <summary>
        ///     Read header of bytes and depending on the information in the header
        ///     return object FileType.
        /// </summary>
        /// <param name="bytes">Bytes array of file.</param>
        /// <returns>FileType or null not identified</returns>
        public static FileType GetFileType(this byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                var header = ReadFileHeader(memoryStream, MaxHeaderSize);
                return GetFileType(() => header, memoryStream);
            }
        }

        /// <summary>
        ///     Read header of a stream and depending on the information in the header
        ///     return object FileType.
        /// </summary>
        /// <param name="stream">The Stream object.</param>
        /// <returns>FileType or null not identified</returns>
        public static FileType GetFileType(this Stream stream)
        {
            return GetFileType(() => ReadFileHeader(stream, MaxHeaderSize), stream);
        }

        /// <summary>
        ///     Read header of a file and depending on the information in the header
        ///     return object FileType.
        /// </summary>
        /// <param name="file">The FileInfo object.</param>
        /// <returns>FileType or null not identified</returns>
        public static FileType GetFileType(this FileInfo file)
        {
            using (var fsSource = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                var header = ReadFileHeader(fsSource, MaxHeaderSize);
                return GetFileType(() => header, fsSource);
            }
        }

        /// <summary>
        ///     Read header of a file and depending on the information in the header
        ///     return object FileType.
        /// </summary>
        /// <param name="fileHeaderReadFunc">A function which returns the bytes found</param>
        /// <param name="stream">If given and file typ is a zip file, a check for docx and xlsx is done</param>
        /// <returns>FileType or null not identified</returns>
        public static FileType GetFileType(Func<byte[]> fileHeaderReadFunc, Stream stream = null)
        {
            // if none of the types match, return null
            FileType fileType = null;

            // read first n-bytes from the file
            var fileHeader = fileHeaderReadFunc();

            // compare the file header to the stored file headers
            foreach (var type in types)
            {
                var matchingCount = SearchBytes(fileHeader, type.Header.Select(x => x.GetValueOrDefault()).ToArray());
                if (matchingCount != -1)
                {
                    // check for docx and xlsx only if a file name is given
                    if (type.Equals(ZIP) && stream != null)
                        fileType = CheckForDocxAndXlsx(stream) ?? type;
                    else
                        fileType = type; // if all the bytes match, return the type

                    break;
                }
            }

            return fileType ?? TXT;
        }

        /// <summary>
        ///     Determines whether provided file belongs to one of the provided list of files
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="requiredTypes">The required types.</param>
        /// <returns>
        ///     <c>true</c> if file of the one of the provided types; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFileOfTypes(this FileInfo file, params FileType[] requiredTypes)
        {
            var currentType = file.GetFileType();
            return currentType != null && requiredTypes.Contains(currentType);
        }

        /// <summary>
        ///     Determines whether provided file belongs to one of the provided list of files,
        ///     where list of files provided by string with Comma-Separated-Values of extensions
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="extensions">The extensions of required types.</param>
        /// <returns>
        ///     <c>true</c> if file of the one of the provided types; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFileOfTypes(this FileInfo file, params String[] extensions)
        {
            var providedTypes = GetFileTypesByExtensions(extensions);

            return file.IsFileOfTypes(providedTypes.ToArray());
        }

        private static List<FileType> GetFileTypesByExtensions(string[] extensions)
        {
            var result = new List<FileType>();

            foreach (var type in types)
                if (extensions.Contains(type.Extension, StringComparer.InvariantCultureIgnoreCase))
                    result.Add(type);

            return result;
        }

        private static FileType CheckForDocxAndXlsx(Stream stream)
        {
            FileType result = null;

            var position = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            //check for docx and xlsx
            using (var zipFile = new ZipArchive(stream, ZipArchiveMode.Read, true))
            {
                if (zipFile.Entries.Any(e => e.FullName.StartsWith("word/")))
                    result = WORDX;
                else if (zipFile.Entries.Any(e => e.FullName.StartsWith("xl/")))
                    result = EXCELX;
                else
                {
                    var ooMimeType = zipFile.Entries.FirstOrDefault(e => e.FullName == "mimetype");
                    if (ooMimeType == null)
                        return null;
                    using (var textReader = new StreamReader(ooMimeType.Open()))
                    {
                        var mimeType = textReader.ReadToEnd();
                        textReader.Close();

                        if (mimeType == ODT.Mime)
                            result = ODT;
                        else if (mimeType == ODS.Mime)
                            result = ODS;
                    }
                }
            }

            stream.Seek(position, SeekOrigin.Begin);

            return result;
        }

        private static int SearchBytes(byte[] haystack, byte[] needle)
        {
            var len = needle.Length;
            var limit = haystack.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                    if (needle[k] != haystack[i + k])
                        break;

                if (k == len) return i;
            }

            return -1;
        }

        /// <summary>
        ///     Reads the file header - first (16) bytes from the file
        /// </summary>
        /// <param name="file">The file to work with</param>
        /// <param name="headerSize">Number of bytes to read from file</param>
        /// <returns>Array of bytes</returns>
        private static byte[] ReadFileHeader(Stream file, int headerSize)
        {
            var header = new byte[headerSize];
            var position = file.Position;
            file.Seek(0, SeekOrigin.Begin);
            file.Read(header, 0, headerSize);
            file.Seek(position, SeekOrigin.Begin);

            return header;
        }

        #endregion

        #region isType functions

        /// <summary>
        ///     Determines whether the specified file is of provided type
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="type">The FileType</param>
        /// <returns>
        ///     <c>true</c> if the specified file is type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsType(this FileInfo file, FileType type)
        {
            var actualType = GetFileType(file);
            return null != actualType && actualType.Equals(type);
        }

        public static bool IsType(this Stream file, FileType type)
        {
            var actualType = GetFileType(file);
            return null != actualType && actualType.Equals(type);
        }

        /// <summary>
        ///     Determines whether the specified file is MS Excel spreadsheet
        /// </summary>
        /// <param name="fileInfo">The FileInfo</param>
        /// <returns>
        ///     <c>true</c> if the specified file info is excel; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExcel(this FileInfo fileInfo)
        {
            return fileInfo.IsType(EXCEL);
        }

        /// <summary>
        ///     Determines whether the specified file is Microsoft PowerPoint Presentation
        /// </summary>
        /// <param name="fileInfo">The FileInfo object.</param>
        /// <returns>
        ///     <c>true</c> if the specified file info is PPT; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPpt(this FileInfo fileInfo)
        {
            return fileInfo.IsType(PPT);
        }

        /// <summary>
        ///     Checks if the file is executable
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool IsExe(this FileInfo fileInfo)
        {
            return fileInfo.IsType(DLL_EXE);
        }

        /// <summary>
        ///     Check if the file is Microsoft Installer.
        ///     Beware, many Microsoft file types are starting with the same header.
        ///     So use this one with caution. If you think the file is MSI, just need to confirm, use this method.
        ///     But it could be MSWord or MSExcel, or Powerpoint...
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool IsMsi(this FileInfo fileInfo)
        {
            // MSI has a generic DOCFILE header. Also it matches PPT files
            return fileInfo.IsType(PPT) || fileInfo.IsType(MSDOC);
        }

        #endregion
    }
}