using System;
using System.IO;
using System.Text;

namespace SnakeBite
{
    public static class Debug
    {
        public const string LOG_FILE = "log.txt";
        public const string LOG_FILE_PREV = "log_prev.txt";

        public static void Clear()
        {
            if (File.Exists(LOG_FILE)) {
                File.Copy(LOG_FILE, LOG_FILE_PREV, true);//tex make backup
                File.Delete(LOG_FILE);
            }
        }

        public static void LogLine(string Text, LogLevel LogLevel = LogLevel.All)
        {
            //if (LogLevel == 0) return;
            FileMode F = File.Exists(LOG_FILE) ? FileMode.Append : FileMode.Create;
            using (FileStream s = new FileStream(LOG_FILE, F))
            {
                string logString = String.Format("{0}\r\n", Text);
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