using System;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeBite.ModPages
{
    public partial class ModDescriptionPage : UserControl
    {
        public ModDescriptionPage()
        {
            InitializeComponent();
        }

        private SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);

        private Version modMGSVersion = new Version(0, 0, 0, 0);

        private string modWebsite;

        public void ShowModInfo(ModEntry mod)
        {
            modMGSVersion = mod.MGSVersion.AsVersion();

            if (mod.Website.Length > 0)
                modWebsite = mod.Website;
            else
                modWebsite = GamePaths.NexusURLPath;

            labelModAuthor.Text = "By " + mod.Author;
            labelModName.Text = mod.Name;
            labelModWebsite.Text = mod.Version;
            textDescription.Text = mod.Description.Replace("\n", "\r\n");

            if (manager.IsUpToDate(modMGSVersion))
            {
                labelVersionWarning.ForeColor = Color.MediumSeaGreen; labelVersionWarning.BackColor = Color.Gainsboro; labelVersionWarning.Text = "✔";
            }
            else
            {
                labelVersionWarning.ForeColor = Color.Yellow; labelVersionWarning.BackColor = Color.Chocolate; labelVersionWarning.Text = "!";
            }
        }

        private void labelModWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try // in case the mod author screwed up their url
            {
                System.Diagnostics.Process.Start(modWebsite);
            }
            catch
            {
                System.Diagnostics.Process.Start(GamePaths.NexusURLPath);
            }
        }

        private void labelVersionWarning_Click(object sender, EventArgs e)
        {
            {
                var currentMGSVersion = ModManager.GetMGSVersion();
                if (!manager.IsUpToDate(modMGSVersion))
                {
                    if (currentMGSVersion > modMGSVersion && modMGSVersion > new Version(0, 0, 0, 0))
                    {
                        MessageBox.Show(String.Format("This mod appears to be for MGSV Version {0}, but it may be compatible with {1} regardless.\n\nIt is recommended that you check for an updated version before installing.", modMGSVersion, currentMGSVersion), "Game version mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    if (currentMGSVersion < modMGSVersion)
                    {
                        MessageBox.Show(String.Format("This mod is intended for MGSV version {0}, but your MGSV installation is version {1}.\n\nThis mod may not be compatible with your MGSV version. Is your game up to date?", modMGSVersion, currentMGSVersion), "Update recommended", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
                else
                {
                    MessageBox.Show(String.Format("This mod is up to date with MGSV version {0}", currentMGSVersion), "Mod is up to date", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }
    }
}
