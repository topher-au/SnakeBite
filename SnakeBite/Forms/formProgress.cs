using System;
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
    }
}