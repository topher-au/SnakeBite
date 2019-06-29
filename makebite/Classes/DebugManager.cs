using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SnakeBite
{
    public static class Debug
    {
        public const string LOG_FILE_PREV = "log_prev";
        public const string TXT_EXT = ".txt";
        public const string LOG_FILE_TXT = "log" + TXT_EXT;
        public const int MAX_COUNT = 3;

        public static void Clear()
        {
            if (File.Exists("log_prev.txt")) // old format
                File.Delete("log_prev.txt");

            if (File.Exists(LOG_FILE_TXT))
            {
                RecursiveCopyLogs(1, MAX_COUNT);
                File.Copy(LOG_FILE_TXT, $"{LOG_FILE_PREV}.{"1"}{TXT_EXT}", true);//tex make backup
                File.Delete(LOG_FILE_TXT);
            }
        }

        private static void RecursiveCopyLogs(int i, int max)
        {
            if (i < max)
            {
                RecursiveCopyLogs(i + 1, max);
                if (File.Exists($"{LOG_FILE_PREV}.{i}{TXT_EXT}"))
                    File.Copy($"{LOG_FILE_PREV}.{i}{TXT_EXT}", $"{LOG_FILE_PREV}.{i + 1}{TXT_EXT}", true);
            }

        }

        public static void OpenLogs(int count)
        {
            for (int i = count - 1; i >= 1; i--)
            {
                if (File.Exists($"{LOG_FILE_PREV}.{i}{TXT_EXT}"))
                    Process.Start($"{LOG_FILE_PREV}.{i}{TXT_EXT}");
            }
            if (File.Exists(LOG_FILE_TXT))
                Process.Start(LOG_FILE_TXT);
        }

        public static void LogLine(string Text, LogLevel LogLevel = LogLevel.All)
        {
            //if (LogLevel == 0) return;
            FileMode F = File.Exists(LOG_FILE_TXT) ? FileMode.Append : FileMode.Create;
            using (FileStream s = new FileStream(LOG_FILE_TXT, F))
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