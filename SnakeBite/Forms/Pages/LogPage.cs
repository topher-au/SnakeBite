using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeBite.ModPages
{
    public partial class LogPage : UserControl
    {
        public StringBuilder logStringBuilder = new StringBuilder();
        //DEBUGNOWprivate System.Threading.Timer timer;
        private volatile bool stopTimer = false;

        private Object thisLock = new Object();

        public LogPage()
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

        delegate void WriteTextBox();

        private void UpdateProperty(object state)
        {
            if (!stopTimer)
            {
                if (textLog.InvokeRequired)
                {
                    textLog.Invoke((MethodInvoker)delegate { UpdateProperty(state); });
                }
                else
                {
                    lock (thisLock)
                    {
                        textLog.Text = logStringBuilder.ToString();
                    }
                }
            }
        }

        public void UpdateLog()
        {
            if (textLog.InvokeRequired)
            {
                textLog.Invoke((MethodInvoker)delegate { UpdateLog(); });
            }
            else
            {
                lock (thisLock)
                {
                    textLog.Text = logStringBuilder.ToString();
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

        private void textLog_TextChanged(object sender, EventArgs e)
        {
            textLog.SelectionStart = textLog.Text.Length;
            textLog.ScrollToCaret();
        }
    }
}
