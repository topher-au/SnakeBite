using System;
using System.ComponentModel;
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
            Left = formInstallOrder.formLocation.X + (formInstallOrder.formSize.Width - Width)/2;
            Top = formInstallOrder.formLocation.Y + (formInstallOrder.formSize.Height - Height)/2;
            ShowDialog();
            return result;
        }
        
        private void buttonInstall_Click(object sender, EventArgs e)
        {
            result = DialogResult.Yes;
            Close();
        }

        private void buttonDontInstall_Click(object sender, EventArgs e)
        {
            result = DialogResult.Cancel;
            Close();
        }

        private void formModConflict_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("Choosing to install this mod will overwrite existing mod or game files. Check the Debug Log to review these file conflicts." +
                "\n\nWarning: overwriting a mod's data may cause problems in-game, which could affect your enjoyment. Install at your own risk." +
                "\n\nIf you are installing multiple mods, cancelling this installation will not affect the other installation processes.", "Installing a mod with conflicts", MessageBoxButtons.OK, MessageBoxIcon.Question);
            e.Cancel = true;
        }

        private void labelCheckDebug_Click(object sender, EventArgs e)
        {
            Debug.OpenLogs(1);
        }
    }
}
