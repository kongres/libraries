namespace Kongrevsky.Utilities.Common
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public static class LoggerUtils
    {
        private static readonly object _spinLock = new object();

        public static void Log(string str, string fileName = null, string subFolder = null)
        {

            Task.Run(() =>
            {
                lock (_spinLock)
                {
                    try
                    {

                        str = DateTime.UtcNow.ToString("G") + " - Message: " + str;

                        var path = string.IsNullOrEmpty(fileName) ? "log.txt" : $"log.{fileName}.txt";
                        var directory = "Logs";
                        if (!string.IsNullOrEmpty(subFolder))
                            directory = Path.Combine(directory, subFolder);
                        Directory.CreateDirectory(directory);
                        path = Path.Combine(directory, path);

                        var fileInfo = new FileInfo(path);

                        if (!fileInfo.Exists)
                        {
                            using (var fileStream = fileInfo.Create())
                            using (var streamWriter = new StreamWriter(fileStream))
                                streamWriter.WriteLine("Init Log.");
                            fileInfo = new FileInfo(path);
                        }

                        while (fileInfo.Length > 1000000)
                        {
                            var lines = File.ReadAllLines(path);
                            File.WriteAllLines(path, lines.Skip(100).ToArray());
                            fileInfo = new FileInfo(path);
                        }

                        using (var streamWriter = fileInfo.AppendText())
                        {
                            streamWriter.WriteLine(str);
                        }

                    }
                    catch { }
                }
            });

        }

        public static void Log(string message, Exception e, string fileName = null, string subFolder = null)
        {
            var log = message + " Error: " + e.Message + Environment.NewLine;
            log += "__________________________________________________________________________________________________" + Environment.NewLine;
            log += "StackTrace: " + e.StackTrace + Environment.NewLine;

            var innerException = e.InnerException;
            var i = 5;
            while (innerException != null && i > 0)
            {
                log += "Inner Exception Message: " + innerException.Message + Environment.NewLine;
                log += "Inner Exception StackTrace: " + innerException.StackTrace + Environment.NewLine;

                innerException = innerException.InnerException;
                i--;
            }

            if (e is WebException exception)
            {
                using (var responseStream = exception.Response?.GetResponseStream())
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                            log += "Response: " + reader.ReadToEnd() + Environment.NewLine;
                log += "Status: " + exception.Status + Environment.NewLine;
                log += "Mesage: " + exception.Message + Environment.NewLine;

            }
            log += "__________________________________________________________________________________________________" + Environment.NewLine;

            log += Environment.NewLine;
            Log(log, fileName, subFolder);
        }
    }
}