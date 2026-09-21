using Microsoft.Extensions.Logging;
using System.IO;
using System;

namespace SV22T1080045.Shop.Admin.Extensions.FileLogger
{
    public class FileLogger : ILogger
    {
        private string filePath;
        private static object _lock = new object();
        public FileLogger(string filePath)
        {
            this.filePath = filePath;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
        string GetPathLog(string path, string logLevel)
        {
            var folderPath = Path.Combine(path, "wwwroot", "Logs", logLevel, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
            var folder = new DirectoryInfo(folderPath);
            if (!folder.Exists)
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, DateTime.Now.ToString("dd") + ".txt");
            if (!File.Exists(filePath))
            {
                using (var FS = File.Create(filePath))
                {
                    FS.Close();
                }
            }
            return filePath;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    string fullFilePath = GetPathLog(Directory.GetCurrentDirectory(), logLevel.ToString());
                    var n = Environment.NewLine;
                    string exc = "";
                    if (exception != null)
                    {
                        exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
                    }
                    File.AppendAllText(fullFilePath, logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter(state, exception) + n + exc);
                }
            }
        }
    }
}