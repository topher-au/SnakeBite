using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeBite
{
    public partial class formLog : Form
    {
        public StringBuilder logStringBuilder = new StringBuilder();
        //DEBUGNOWprivate System.Threading.Timer timer;
        private volatile bool stopTimer = false;

        private Object thisLock = new Object();

        public formLog()
        {
            InitializeComponent();

            logStringBuilder.Capacity = 200 * 6000;

            Console.SetOut(new MultiTextWriter(new LogTextBoxWriter(this), Console.Out));

            //timer = new System.Threading.Timer(UpdateProperty, null, 400, 400);
            /* //DEBUGNOW
            this.SetStyle(
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.DoubleBuffer, true);
              */
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        delegate void WriteTextBox();
        private void UpdateProperty(object state)
        {
            if (!stopTimer)
            {
                if (textBox1.InvokeRequired)
                {
                    textBox1.Invoke((MethodInvoker)delegate { UpdateProperty(state); });
                } else
                {
                    lock (thisLock)
                    {
                        textBox1.Text = logStringBuilder.ToString();
                    }
                }
            }
        }

        public void UpdateLog()
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke((MethodInvoker)delegate { UpdateLog(); });
            } else
            {
                lock (thisLock)
                {
                    textBox1.Text = logStringBuilder.ToString();
                }
            }
        }

        private void formLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopTimer = true;
          //  timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void formLog_FormClosed(object sender, FormClosedEventArgs e)
        {
          //DEBUGNOW  timer.Dispose();
        }
    }
}
