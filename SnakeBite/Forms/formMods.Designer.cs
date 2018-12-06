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
            this.menuItemOpenLogs = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemOpenMakeBite = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItemBrowseMods = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.menuItemOptions = new System.Windows.Forms.MenuItem();
            this.menuItemSkipLauncher = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItemOpenSettings = new System.Windows.Forms.MenuItem();
            this.menuItemHelp = new System.Windows.Forms.MenuItem();
            this.menuItemHelpInstall = new System.Windows.Forms.MenuItem();
            this.menuItemHelpUninstall = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.menuItemHelpCreate = new System.Windows.Forms.MenuItem();
            this.menuItemHelpConflicts = new System.Windows.Forms.MenuItem();
            this.menuItem18 = new System.Windows.Forms.MenuItem();
            this.menuItemWikiLink = new System.Windows.Forms.MenuItem();
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
            this.menuItemOpenLogs,
            this.menuItem2,
            this.menuItemOpenMakeBite,
            this.menuItem6,
            this.menuItemBrowseMods,
            this.menuItem8,
            this.menuItemExit});
            this.menuItemFile.Text = "File";
            // 
            // menuItemOpenDir
            // 
            this.menuItemOpenDir.Index = 0;
            this.menuItemOpenDir.Text = "Open Game Directory";
            this.menuItemOpenDir.Click += new System.EventHandler(this.menuItemOpenDir_Click);
            // 
            // menuItemOpenLogs
            // 
            this.menuItemOpenLogs.Index = 1;
            this.menuItemOpenLogs.Text = "Open Debug Logs";
            this.menuItemOpenLogs.Click += new System.EventHandler(this.menuItemOpenLogs_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 2;
            this.menuItem2.Text = "-";
            // 
            // menuItemOpenMakeBite
            // 
            this.menuItemOpenMakeBite.Index = 3;
            this.menuItemOpenMakeBite.Text = "Launch MakeBite";
            this.menuItemOpenMakeBite.Click += new System.EventHandler(this.menuItemOpenMakeBite_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 4;
            this.menuItem6.Text = "-";
            // 
            // menuItemBrowseMods
            // 
            this.menuItemBrowseMods.Index = 5;
            this.menuItemBrowseMods.Text = "Browse SnakeBite Mods";
            this.menuItemBrowseMods.Click += new System.EventHandler(this.menuItemBrowseMods_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 6;
            this.menuItem8.Text = "-";
            // 
            // menuItemExit
            // 
            this.menuItemExit.Index = 7;
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
            this.menuItemHelpInstall,
            this.menuItemHelpUninstall,
            this.menuItem15,
            this.menuItemHelpCreate,
            this.menuItemHelpConflicts,
            this.menuItem18,
            this.menuItemWikiLink,
            this.menuItemOpenBugReport});
            this.menuItemHelp.Text = "Help";
            // 
            // menuItemHelpInstall
            // 
            this.menuItemHelpInstall.Index = 0;
            this.menuItemHelpInstall.Text = "Installing Mods...";
            this.menuItemHelpInstall.Click += new System.EventHandler(this.menuItemHelpInstall_Click);
            // 
            // menuItemHelpUninstall
            // 
            this.menuItemHelpUninstall.Index = 1;
            this.menuItemHelpUninstall.Text = "Uninstalling Mods...";
            this.menuItemHelpUninstall.Click += new System.EventHandler(this.menuItemHelpUninstall_Click);
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 2;
            this.menuItem15.Text = "-";
            // 
            // menuItemHelpCreate
            // 
            this.menuItemHelpCreate.Index = 3;
            this.menuItemHelpCreate.Text = "Creating Mods...";
            this.menuItemHelpCreate.Click += new System.EventHandler(this.menuItemHelpCreate_Click);
            // 
            // menuItemHelpConflicts
            // 
            this.menuItemHelpConflicts.Index = 4;
            this.menuItemHelpConflicts.Text = "Mod Conflicts...";
            this.menuItemHelpConflicts.Click += new System.EventHandler(this.menuItemHelpConflicts_Click);
            // 
            // menuItem18
            // 
            this.menuItem18.Index = 5;
            this.menuItem18.Text = "-";
            // 
            // menuItemWikiLink
            // 
            this.menuItemWikiLink.Index = 6;
            this.menuItemWikiLink.Text = "MGSV Modding Wiki";
            this.menuItemWikiLink.Click += new System.EventHandler(this.menuItemWikiLink_Click);
            // 
            // menuItemOpenBugReport
            // 
            this.menuItemOpenBugReport.Index = 7;
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
            this.Menu = this.mainMenuMods;
            this.MinimumSize = new System.Drawing.Size(637, 450);
            this.Name = "formMods";
            this.ShowIcon = false;
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
        private System.Windows.Forms.MenuItem menuItemOpenLogs;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItemBrowseMods;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem menuItemExit;
        private System.Windows.Forms.MenuItem menuItemSkipLauncher;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem menuItemHelp;
        private System.Windows.Forms.MenuItem menuItemOpenSettings;
        private System.Windows.Forms.MenuItem menuItemHelpInstall;
        private System.Windows.Forms.MenuItem menuItemHelpUninstall;
        private System.Windows.Forms.MenuItem menuItem15;
        private System.Windows.Forms.MenuItem menuItemHelpCreate;
        private System.Windows.Forms.MenuItem menuItemHelpConflicts;
        private System.Windows.Forms.MenuItem menuItem18;
        private System.Windows.Forms.MenuItem menuItemOpenBugReport;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItemOpenMakeBite;
        private System.Windows.Forms.MenuItem menuItemWikiLink;
    }
}