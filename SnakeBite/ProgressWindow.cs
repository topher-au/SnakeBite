using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeBite
{
    public static class ProgressWindow
    {
        public static void Show(string WindowTitle, string ProgressText, Action WorkerFunction)
        {
            var progressWindow = new formProgress();
            progressWindow.StatusText.Text = ProgressText;

            //var logWindow = new formLog();
            //logWindow.Text = ProgressText;

            var progressWorker = new BackgroundWorker();
            progressWorker.DoWork += (obj, var) =>
            {
                WorkerFunction();
            };
            progressWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                delegate (object sender, RunWorkerCompletedEventArgs e) {
                    if (e.Error != null) {
                        Debug.LogLine(String.Format("[Error] Exception message '{0}'", e.Error.Message));
                        //logWindow.Invoke((MethodInvoker)delegate { logWindow.Text = String.Format("Error during process :'{0}'", ProgressText); });

                        MessageBox.Show(String.Format("Exception :'{0}'\r\nCheck SnakeBites log for more info.", e.Error.Message), String.Format("Error during process :'{0}'", ProgressText), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        progressWindow.Invoke((MethodInvoker)delegate { progressWindow.Close(); });
                    } else if (e.Cancelled) {
                        
                    } else {
                        //logWindow.Invoke((MethodInvoker)delegate { logWindow.Close(); });
                        progressWindow.Invoke((MethodInvoker)delegate { progressWindow.Close(); });
                    }
                    progressWorker.Dispose();
                }
             );

            progressWorker.RunWorkerAsync();
            //logWindow.ShowDialog();
            progressWindow.ShowDialog();
        }
    }
}
