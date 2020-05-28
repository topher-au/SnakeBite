using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeBite {
    public class ControBoxWriter : TextWriter {
        delegate void WriteCallbackChar(char text);
        delegate void WriteCallbackString(string text);

        private TextBox textbox;
        public ControBoxWriter(TextBox textbox) {
            this.textbox = textbox;
        }

        public override void Write(char value) {
            if (this.textbox.InvokeRequired) {
                WriteCallbackChar d = new WriteCallbackChar(Write);
                textbox.Invoke(d, new object[] { value });
            } else {
                textbox.AppendText(value.ToString());
            }
        }

        public override void Write(string value) {
            if (this.textbox.InvokeRequired) {
                WriteCallbackString d = new WriteCallbackString(Write);
                textbox.Invoke(d, new object[] { value });
            } else {
                textbox.AppendText(value);
            }
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}
