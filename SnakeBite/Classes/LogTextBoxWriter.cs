using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeBite {
    public class LogTextBoxWriter : TextWriter {
        delegate void WriteCallbackChar(char text);
        delegate void WriteCallbackString(string text);

        private DateTime lastUpdate = new DateTime();
        private int displayRate = 100;//ms

        private formLog logForm;
        public LogTextBoxWriter(formLog logForm) {
            this.logForm = logForm;
        }

        public override void Write(char value) {                
            logForm.logStringBuilder.Append(value);

            var current = DateTime.Now;
            var delta = (current - lastUpdate).TotalMilliseconds;
            if (delta > displayRate)
            {
                lastUpdate = current;
                logForm.UpdateLog();
            }
        }

        public override void Write(string value) {
            logForm.logStringBuilder.Append(value);

            var current = DateTime.Now;
            var delta = (current - lastUpdate).TotalMilliseconds;
            if (delta > displayRate)
            {
                lastUpdate = current;
                logForm.UpdateLog();
            }
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}
