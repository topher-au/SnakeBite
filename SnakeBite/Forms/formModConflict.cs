using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeBite.Forms
{
    public partial class formModConflict : Form
    {
        DialogResult result;
        public formModConflict()
        {
            InitializeComponent();
        }
        public DialogResult ShowDialog(string conflictMessage)
        {
            result = DialogResult.Cancel;
            labelHeader.Text = conflictMessage;
            this.ShowDialog();
            return result;
        }

        private void labelInstallWarning_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Choosing to install this mod will overwrite existing mod or game data.\n" +
       "Warning: overwriting a mod's data may cause significant problems in-game, which could affect your enjoyment. Install at your own risk.\n\n" +
       "If you are installing multiple mods, cancelling this installation will not affect the other installation processes.", "Installing a mod with conflicts", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            result = DialogResult.Yes;
            this.Close();
        }

        private void buttonDontInstall_Click(object sender, EventArgs e)
        {
            result = DialogResult.Cancel;
            this.Close();
        }
    }
}
