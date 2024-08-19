using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SnakeBite
{
    public partial class formSelectModelTarget : Form
    {
        private string formText;
        private List<string> modelNames;

        public formSelectModelTarget(string FormText, List<string> ModelNames, int Width = 300)
        {
            InitializeComponent();
            formText = FormText;
            modelNames = ModelNames;
        }

        private void formSelectModelTarget_Load(object sender, EventArgs e)
        {
            // Set form text
            labelFormText.Text = formText;

            // Create new radio button for each model passed to constructor
            for (int i = 0; i < modelNames.Count; i++)
            {
                groupModelSelect.Controls.Add(new RadioButton() { Text = modelNames[i], Top = i * 20 + 12, Left = 12, Width = groupModelSelect.Width - 24, Tag = i });
                groupModelSelect.Height = i * 20 + 42;
            }

            buttonCancel.Top = groupModelSelect.Top + groupModelSelect.Height + 8;
            buttonConfirm.Top = groupModelSelect.Top + groupModelSelect.Height + 8;

            Height = buttonConfirm.Top + buttonConfirm.Height + 48;

            RadioButton r = groupModelSelect.Controls[0] as RadioButton;
            r.Checked = true;
            buttonConfirm.Focus();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            foreach (RadioButton r in groupModelSelect.Controls)
            {
                if (r.Checked) Tag = r.Tag;
            }
        }
    }
}