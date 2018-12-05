using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SnakeBite.ModPages
{
    public partial class NoInstalledPage : UserControl
    {
        public NoInstalledPage()
        {
            InitializeComponent();
        }

        private void linkLabelSnakeBiteModsList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // opens the [SBWM] search filter on nexus mods, randomly sorted.
        {
            Process.Start(ModManager.SBWMSearchURL);
        }
    }
}
