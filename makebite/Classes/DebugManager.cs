using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SnakeBite
{
    public static class Debug
    {
        public static readonly string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        public const string LOG_FILE_PREV = "log_prev";
        public const string TXT_EXT = ".txt";
        public const string LOG_FILE_TXT = "log" + TXT_EXT;
        public const int MAX_COUNT = 3;

        public static void Clear()
        {
            RemoveOldFormatLogs();
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            string logFilePath = Path.Combine(logDir, LOG_FILE_TXT);
            if (File.Exists(logFilePath))
            {
                RecursiveCopyLogs(1, MAX_COUNT);

                string logPrevPath = Path.Combine(logDir, $"{LOG_FILE_PREV}.{"1"}{TXT_EXT}");

                File.Copy(logFilePath, logPrevPath, true);
                File.Delete(logFilePath);
            }
        }
        private static void RemoveOldFormatLogs()
        {
            if (File.Exists(LOG_FILE_PREV + TXT_EXT)) // old format in root install directory
                File.Delete(LOG_FILE_PREV + TXT_EXT);

            if (File.Exists(LOG_FILE_TXT))
                File.Delete(LOG_FILE_TXT);
        }
        private static void RecursiveCopyLogs(int i, int max)
        {
            if (i < max)
            {
                string logPrevPath = Path.Combine(logDir, $"{LOG_FILE_PREV}.{i}{TXT_EXT}"); // assumption: logdir already exists
                string logPrevIncrementedPath = Path.Combine(logDir, $"{LOG_FILE_PREV}.{i + 1}{TXT_EXT}");

                RecursiveCopyLogs(i + 1, max);

                if (File.Exists(logPrevPath))
                    File.Copy(logPrevPath, logPrevIncrementedPath, true);
            }

        }

        public static void OpenLogs(int count)
        {
            for (int i = count - 1; i >= 1; i--)
            {
                string logPrevPath = Path.Combine(logDir, $"{LOG_FILE_PREV}.{i}{TXT_EXT}");
                if (File.Exists(logPrevPath))
                    Process.Start(logPrevPath);
            }
            string logFilePath = Path.Combine(logDir, LOG_FILE_TXT);
            if (File.Exists(logFilePath))
                Process.Start(logFilePath);
        }

        public static void OpenLogDirectory()
        {
            string installPath = AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                Process.Start(installPath);
            }
            catch
            {
                Debug.LogLine(String.Format("Failed to open directory: {0}", installPath), Debug.LogLevel.Basic);
            }
        }

        public static void LogLine(string Text, LogLevel LogLevel = LogLevel.All)
        {
            //if (LogLevel == 0) return;
            string logFilePath = Path.Combine(logDir, LOG_FILE_TXT);
            FileMode F = File.Exists(logFilePath) ? FileMode.Append : FileMode.Create;
            using (FileStream s = new FileStream(logFilePath, F))
            {
                string logString = $"{Text}\r\n";
                s.Write(Encoding.UTF8.GetBytes(logString), 0, logString.Length);
                Console.Write(logString);
            }
        }

        public enum LogLevel
        {
            All,
            Debug,
            Basic
        }
    }
}