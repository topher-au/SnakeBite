using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeBite.QuickMod
{
    public partial class SelectZipPanel : UserControl
    {
        public bool Compatible = false;

        public delegate void CompatibilityChangedDelegate();

        public event CompatibilityChangedDelegate CompatibilityChanged;

        public SelectZipPanel()
        {
            InitializeComponent();
        }

        private void buttonFindZip_Click(object sender, EventArgs e)
        {
            OpenFileDialog zipDialog = new OpenFileDialog();
            zipDialog.Filter = "Zip Archives|*.zip";
            if (zipDialog.ShowDialog() == DialogResult.Cancel) return;
            textZipFile.Text = zipDialog.FileName;

            if(Methods.QuickCheck(zipDialog.FileName))
            {
                picCompat.Image = Properties.Resources.tick7;
                labelCompat.Text = "The mod appears to be compatible";
                Compatible = true;
            } else
            {
                picCompat.Image = Properties.Resources.close7;
                labelCompat.Text = "This mod is not compatible";
                Compatible = false;
            }
            CompatibilityChanged();
        }

        private void WelcomePanel_Load(object sender, EventArgs e)
        {
            CompatibilityChanged();
        }
    }
}
