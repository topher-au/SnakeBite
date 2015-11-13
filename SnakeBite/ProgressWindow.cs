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

            var progressWorker = new BackgroundWorker();
            progressWorker.DoWork += (obj, var) =>
            {
                WorkerFunction();
                progressWindow.Invoke((MethodInvoker)delegate { progressWindow.Close(); });
                progressWorker.Dispose();
            };
            
            progressWorker.RunWorkerAsync();
            progressWindow.ShowDialog();
        }
    }
}
