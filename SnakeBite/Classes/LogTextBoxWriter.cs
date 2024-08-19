using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnakeBite.ModPages;

namespace SnakeBite {
    public class LogTextBoxWriter : TextWriter {
        delegate void WriteCallbackChar(char text);
        delegate void WriteCallbackString(string text);

        //private DateTime lastUpdate = new DateTime();
        //private int displayRate = 100;//ms

        private LogPage logPage;

        public LogTextBoxWriter(LogPage logPage) {
            this.logPage = logPage;
        }

        public override void Write(char value) {                
            logPage.logStringBuilder.Append(value);
            logPage.UpdateLog();

            /*
            var current = DateTime.Now;
            var delta = (current - lastUpdate).TotalMilliseconds;
            if (delta > displayRate)
            {
                lastUpdate = current;
                logPage.UpdateLog();
            }
            */
        }

        public override void Write(string value) {
            logPage.logStringBuilder.Append(value);
            logPage.UpdateLog();

            /*
            var current = DateTime.Now;
            var delta = (current - lastUpdate).TotalMilliseconds;
            if (delta > displayRate)
            {
                lastUpdate = current;
                logPage.UpdateLog();
            }
            */
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}
