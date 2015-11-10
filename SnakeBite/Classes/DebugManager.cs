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
            //if (LogLevel == 0) return;
            FileMode F = File.Exists(LOG_FILE) ? FileMode.Append : FileMode.Create;
            using (FileStream s = new FileStream(LOG_FILE, F))
            { 

                string logString = String.Format("{0}\n", Text);
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
