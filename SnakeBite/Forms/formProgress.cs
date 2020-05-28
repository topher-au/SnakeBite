using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SnakeBite
{
    public partial class formProgress : Form
    {
        public formProgress()
        {
            InitializeComponent();
        }

        private void formProgress_VisibleChanged(object sender, EventArgs e)
        {
            this.Refresh();
            this.Width = StatusText.Left + StatusText.Width + 32;
        }

        public void progressWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StatusText.Text = (string)e.UserState;
        }
    }
}