using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeBite
{
    public static class Debug
    {
        private const string LOG_FILE = "log.txt";

        public static void Clear()
        {
            if (File.Exists(LOG_FILE)) File.Delete(LOG_FILE);
        }

        public static void LogLine(string Text, LogLevel LogLevel = LogLevel.All)
        {
            if (LogLevel == 0) return;
            using (FileStream s = new FileStream(LOG_FILE, FileMode.Append))
            {
                Text = Text.Insert(0, LogLevel.ToString() + "> ");

                string logString = String.Format("{0}\n", Text);
                s.Write(Encoding.UTF8.GetBytes(logString), 0, logString.Length);
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
