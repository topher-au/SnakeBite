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
            this.Top = this.Owner.Top + (this.Owner.Height / 2 - this.Height / 2);
            this.Left = this.Owner.Left + (this.Owner.Width / 2 - this.Width / 2);
        }
    }
}