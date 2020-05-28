namespace SnakeBite
{
    partial class formMods
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formMods));
            this.listInstalledMods = new System.Windows.Forms.CheckedListBox();
            this.buttonInstall = new System.Windows.Forms.Button();
            this.buttonUninstall = new System.Windows.Forms.Button();
            this.labelInstalledMods = new System.Windows.Forms.Label();
            this.checkBoxMarkAll = new System.Windows.Forms.CheckBox();
            this.panelContent = new System.Windows.Forms.Panel();
            this.buttonLaunchGame = new System.Windows.Forms.Button();
            this.mainMenuMods = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemFile = new System.Windows.Forms.MenuItem();
            this.menuItemOpenDir = new System.Windows.Forms.MenuItem();
            this.menuItemOpenMakeBite = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItemBrowseMods = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItemSavePreset = new System.Windows.Forms.MenuItem();
            this.menuItemLoadPreset = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.menuItemOptions = new System.Windows.Forms.MenuItem();
            this.menuItemSkipLauncher = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItemOpenSettings = new System.Windows.Forms.MenuItem();
            this.menuItemHelp = new System.Windows.Forms.MenuItem();
            this.menuItemLearn = new System.Windows.Forms.MenuItem();
            this.menuItemInstalling = new System.Windows.Forms.MenuItem();
            this.menuItemUninstalling = new System.Windows.Forms.MenuItem();
            this.menuItemConflicts = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemCreating = new System.Windows.Forms.MenuItem();
            this.menuItemHelpPresets = new System.Windows.Forms.MenuItem();
            this.menuItemWikiLink = new System.Windows.Forms.MenuItem();
            this.menuItem18 = new System.Windows.Forms.MenuItem();
            this.menuItemOpenLogs = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemOpenBugReport = new System.Windows.Forms.MenuItem();
            this.panelModList = new System.Windows.Forms.Panel();
            this.panelModList.SuspendLayout();
            this.SuspendLayout();
            // 
            // listInstalledMods
            // 
            this.listInstalledMods.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listInstalledMods.BackColor = System.Drawing.Color.Silver;
            this.listInstalledMods.FormattingEnabled = true;
            this.listInstalledMods.IntegralHeight = false;
            this.listInstalledMods.Location = new System.Drawing.Point(0, 23);
            this.listInstalledMods.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listInstalledMods.Name = "listInstalledMods";
            this.listInstalledMods.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listInstalledMods.Size = new System.Drawing.Size(191, 380);
            this.listInstalledMods.TabIndex = 3;
            this.listInstalledMods.Tag = "";
            this.listInstalledMods.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listInstalledMods_ItemCheck);
            this.listInstalledMods.SelectedIndexChanged += new System.EventHandler(this.listInstalledMods_SelectedIndexChanged);
            // 
            // buttonInstall
            // 
            this.buttonInstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInstall.BackColor = System.Drawing.Color.Transparent;
            this.buttonInstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonInstall.Location = new System.Drawing.Point(0, 406);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(191, 23);
            this.buttonInstall.TabIndex = 4;
            this.buttonInstall.Text = "Install .MGSV File(s)";
            this.buttonInstall.UseVisualStyleBackColor = false;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // buttonUninstall
            // 
            this.buttonUninstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUninstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonUninstall.Enabled = false;
            this.buttonUninstall.Location = new System.Drawing.Point(0, 432);
            this.buttonUninstall.Name = "buttonUninstall";
            this.buttonUninstall.Size = new System.Drawing.Size(191, 23);
            this.buttonUninstall.TabIndex = 5;
            this.buttonUninstall.Text = "Uninstall Checked Mod(s)";
            this.buttonUninstall.UseVisualStyleBackColor = true;
            this.buttonUninstall.Click += new System.EventHandler(this.buttonUninstall_Click);
            // 
            // labelInstalledMods
            // 
            this.labelInstalledMods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInstalledMods.BackColor = System.Drawing.Color.Silver;
            this.labelInstalledMods.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelInstalledMods.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelInstalledMods.Location = new System.Drawing.Point(0, 0);
            this.labelInstalledMods.Name = "labelInstalledMods";
            this.labelInstalledMods.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelInstalledMods.Size = new System.Drawing.Size(191, 22);
            this.labelInstalledMods.TabIndex = 6;
            this.labelInstalledMods.Text = "Installed Mods";
            this.labelInstalledMods.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBoxMarkAll
            // 
            this.checkBoxMarkAll.AutoSize = true;
            this.checkBoxMarkAll.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxMarkAll.Checked = true;
            this.checkBoxMarkAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMarkAll.Location = new System.Drawing.Point(7, 13);
            this.checkBoxMarkAll.Name = "checkBoxMarkAll";
            this.checkBoxMarkAll.Size = new System.Drawing.Size(15, 14);
            this.checkBoxMarkAll.TabIndex = 7;
            this.checkBoxMarkAll.UseVisualStyleBackColor = false;
            this.checkBoxMarkAll.Click += new System.EventHandler(this.checkBoxMarkAll_Click);
            // 
            // panelContent
            // 
            this.panelContent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panelContent.BackColor = System.Drawing.Color.Transparent;
            this.panelContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelContent.Location = new System.Drawing.Point(197, 9);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(420, 403);
            this.panelContent.TabIndex = 10;
            // 
            // buttonLaunchGame
            // 
            this.buttonLaunchGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLaunchGame.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonLaunchGame.Location = new System.Drawing.Point(432, 415);
            this.buttonLaunchGame.Name = "buttonLaunchGame";
            this.buttonLaunchGame.Size = new System.Drawing.Size(185, 49);
            this.buttonLaunchGame.TabIndex = 11;
            this.buttonLaunchGame.Text = "Launch Game";
            this.buttonLaunchGame.UseVisualStyleBackColor = true;
            this.buttonLaunchGame.Click += new System.EventHandler(this.buttonLaunchGame_Click);
            // 
            // mainMenuMods
            // 
            this.mainMenuMods.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFile,
            this.menuItemOptions,
            this.menuItemHelp});
            // 
            // menuItemFile
            // 
            this.menuItemFile.Index = 0;
            this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemOpenDir,
            this.menuItemOpenMakeBite,
            this.menuItem6,
            this.menuItemBrowseMods,
            this.menuItem8,
            this.menuItemSavePreset,
            this.menuItemLoadPreset,
            this.menuItem4,
            this.menuItemExit});
            this.menuItemFile.Text = "File";
            // 
            // menuItemOpenDir
            // 
            this.menuItemOpenDir.Index = 0;
            this.menuItemOpenDir.Text = "Open Game Directory";
            this.menuItemOpenDir.Click += new System.EventHandler(this.menuItemOpenDir_Click);
            // 
            // menuItemOpenMakeBite
            // 
            this.menuItemOpenMakeBite.Index = 1;
            this.menuItemOpenMakeBite.Text = "Launch MakeBite";
            this.menuItemOpenMakeBite.Click += new System.EventHandler(this.menuItemOpenMakeBite_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 2;
            this.menuItem6.Text = "-";
            // 
            // menuItemBrowseMods
            // 
            this.menuItemBrowseMods.Index = 3;
            this.menuItemBrowseMods.Text = "Browse SnakeBite Mods";
            this.menuItemBrowseMods.Click += new System.EventHandler(this.menuItemBrowseMods_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 4;
            this.menuItem8.Text = "-";
            // 
            // menuItemSavePreset
            // 
            this.menuItemSavePreset.Index = 5;
            this.menuItemSavePreset.Text = "Save .MGSVPreset File...";
            this.menuItemSavePreset.Click += new System.EventHandler(this.menuItemSavePreset_Click);
            // 
            // menuItemLoadPreset
            // 
            this.menuItemLoadPreset.Index = 6;
            this.menuItemLoadPreset.Text = "Load .MGSVPreset File...";
            this.menuItemLoadPreset.Click += new System.EventHandler(this.menuItemLoadPreset_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 7;
            this.menuItem4.Text = "-";
            // 
            // menuItemExit
            // 
            this.menuItemExit.Index = 8;
            this.menuItemExit.Text = "Exit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuItemOptions
            // 
            this.menuItemOptions.Index = 1;
            this.menuItemOptions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSkipLauncher,
            this.menuItem11,
            this.menuItemOpenSettings});
            this.menuItemOptions.Text = "Options";
            // 
            // menuItemSkipLauncher
            // 
            this.menuItemSkipLauncher.Index = 0;
            this.menuItemSkipLauncher.Text = "Skip Startup Launcher";
            this.menuItemSkipLauncher.Click += new System.EventHandler(this.menuItemSkipLauncher_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 1;
            this.menuItem11.Text = "-";
            // 
            // menuItemOpenSettings
            // 
            this.menuItemOpenSettings.Index = 2;
            this.menuItemOpenSettings.Text = "Settings...";
            this.menuItemOpenSettings.Click += new System.EventHandler(this.menuItemOpenSettings_Click);
            // 
            // menuItemHelp
            // 
            this.menuItemHelp.Index = 2;
            this.menuItemHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemLearn,
            this.menuItemWikiLink,
            this.menuItem18,
            this.menuItemOpenLogs,
            this.menuItem1,
            this.menuItemOpenBugReport});
            this.menuItemHelp.Text = "Help";
            // 
            // menuItemLearn
            // 
            this.menuItemLearn.Index = 0;
            this.menuItemLearn.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemInstalling,
            this.menuItemUninstalling,
            this.menuItemConflicts,
            this.menuItem3,
            this.menuItemCreating,
            this.menuItemHelpPresets});
            this.menuItemLearn.Text = "About SnakeBite...";
            // 
            // menuItemInstalling
            // 
            this.menuItemInstalling.Index = 0;
            this.menuItemInstalling.Text = "Installing Mods";
            this.menuItemInstalling.Click += new System.EventHandler(this.menuItemHelpInstall_Click);
            // 
            // menuItemUninstalling
            // 
            this.menuItemUninstalling.Index = 1;
            this.menuItemUninstalling.Text = "Uninstalling Mods";
            this.menuItemUninstalling.Click += new System.EventHandler(this.menuItemHelpUninstall_Click);
            // 
            // menuItemConflicts
            // 
            this.menuItemConflicts.Index = 2;
            this.menuItemConflicts.Text = "Mod Conflicts";
            this.menuItemConflicts.Click += new System.EventHandler(this.menuItemHelpConflicts_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 3;
            this.menuItem3.Text = "-";
            // 
            // menuItemCreating
            // 
            this.menuItemCreating.Index = 4;
            this.menuItemCreating.Text = "Creating Mods";
            this.menuItemCreating.Click += new System.EventHandler(this.menuItemHelpCreate_Click);
            // 
            // menuItemHelpPresets
            // 
            this.menuItemHelpPresets.Index = 5;
            this.menuItemHelpPresets.Text = "Saving/Loading Presets";
            this.menuItemHelpPresets.Click += new System.EventHandler(this.menuItemHelpPresets_Click);
            // 
            // menuItemWikiLink
            // 
            this.menuItemWikiLink.Index = 1;
            this.menuItemWikiLink.Text = "Visit Modding Wiki";
            this.menuItemWikiLink.Click += new System.EventHandler(this.menuItemWikiLink_Click);
            // 
            // menuItem18
            // 
            this.menuItem18.Index = 2;
            this.menuItem18.Text = "-";
            // 
            // menuItemOpenLogs
            // 
            this.menuItemOpenLogs.Index = 3;
            this.menuItemOpenLogs.Text = "Open Debug Logs";
            this.menuItemOpenLogs.Click += new System.EventHandler(this.menuItemOpenLogs_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 4;
            this.menuItem1.Text = "-";
            // 
            // menuItemOpenBugReport
            // 
            this.menuItemOpenBugReport.Index = 5;
            this.menuItemOpenBugReport.Text = "Report a Bug";
            this.menuItemOpenBugReport.Click += new System.EventHandler(this.menuItemOpenBugReport_Click);
            // 
            // panelModList
            // 
            this.panelModList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelModList.BackColor = System.Drawing.Color.Transparent;
            this.panelModList.Controls.Add(this.listInstalledMods);
            this.panelModList.Controls.Add(this.labelInstalledMods);
            this.panelModList.Controls.Add(this.buttonInstall);
            this.panelModList.Controls.Add(this.buttonUninstall);
            this.panelModList.Location = new System.Drawing.Point(4, 9);
            this.panelModList.MaximumSize = new System.Drawing.Size(300, 9001);
            this.panelModList.Name = "panelModList";
            this.panelModList.Size = new System.Drawing.Size(191, 455);
            this.panelModList.TabIndex = 12;
            // 
            // formMods
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(621, 476);
            this.Controls.Add(this.buttonLaunchGame);
            this.Controls.Add(this.checkBoxMarkAll);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelModList);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenuMods;
            this.MinimumSize = new System.Drawing.Size(637, 450);
            this.Name = "formMods";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SnakeBite Mod Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formMods_FormClosing);
            this.Load += new System.EventHandler(this.formMain_Load);
            this.Resize += new System.EventHandler(this.formMods_Resize);
            this.panelModList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckedListBox listInstalledMods;
        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Button buttonUninstall;
        private System.Windows.Forms.Label labelInstalledMods;
        private System.Windows.Forms.CheckBox checkBoxMarkAll;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Button buttonLaunchGame;
        private System.Windows.Forms.MainMenu mainMenuMods;
        private System.Windows.Forms.MenuItem menuItemFile;
        private System.Windows.Forms.MenuItem menuItemOptions;
        private System.Windows.Forms.Panel panelModList;
        private System.Windows.Forms.MenuItem menuItemOpenDir;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItemBrowseMods;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem menuItemExit;
        private System.Windows.Forms.MenuItem menuItemSkipLauncher;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem menuItemHelp;
        private System.Windows.Forms.MenuItem menuItemOpenSettings;
        private System.Windows.Forms.MenuItem menuItem18;
        private System.Windows.Forms.MenuItem menuItemOpenBugReport;
        private System.Windows.Forms.MenuItem menuItemOpenMakeBite;
        private System.Windows.Forms.MenuItem menuItemWikiLink;
        private System.Windows.Forms.MenuItem menuItemLearn;
        private System.Windows.Forms.MenuItem menuItemInstalling;
        private System.Windows.Forms.MenuItem menuItemUninstalling;
        private System.Windows.Forms.MenuItem menuItemCreating;
        private System.Windows.Forms.MenuItem menuItemConflicts;
        private System.Windows.Forms.MenuItem menuItemOpenLogs;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItemSavePreset;
        private System.Windows.Forms.MenuItem menuItemLoadPreset;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItemHelpPresets;
    }
}