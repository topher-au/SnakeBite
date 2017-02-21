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

        private formLog logForm;
        public LogTextBoxWriter(formLog logForm) {
            this.logForm = logForm;
        }

        public override void Write(char value) {
            logForm.logStringBuilder.Append(value);
        }

        public override void Write(string value) {
            logForm.logStringBuilder.Append(value);
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}
