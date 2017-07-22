namespace SnakeBite
{
    partial class formSettings
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
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.groupSettings = new System.Windows.Forms.GroupBox();
            this.checkEnableSound = new System.Windows.Forms.CheckBox();
            this.groupBackup = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOpenLog = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelSetupWizard = new System.Windows.Forms.Label();
            this.buttonRestoreOriginals = new System.Windows.Forms.Button();
            this.buttonSetupWizard = new System.Windows.Forms.Button();
            this.groupAbout = new System.Windows.Forms.GroupBox();
            this.labelGithub = new System.Windows.Forms.LinkLabel();
            this.labelThanks = new System.Windows.Forms.Label();
            this.groupMGSVDir = new System.Windows.Forms.GroupBox();
            this.buttonFindMGSV = new System.Windows.Forms.Button();
            this.textInstallPath = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabThemes = new System.Windows.Forms.TabPage();
            this.buttonSetTheme = new System.Windows.Forms.Button();
            this.listThemes = new System.Windows.Forms.ListBox();
            this.tabSettings.SuspendLayout();
            this.groupSettings.SuspendLayout();
            this.groupBackup.SuspendLayout();
            this.groupAbout.SuspendLayout();
            this.groupMGSVDir.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabThemes.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.groupSettings);
            this.tabSettings.Controls.Add(this.groupBackup);
            this.tabSettings.Controls.Add(this.groupAbout);
            this.tabSettings.Controls.Add(this.groupMGSVDir);
            this.tabSettings.Location = new System.Drawing.Point(4, 24);
            this.tabSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabSettings.Size = new System.Drawing.Size(363, 426);
            this.tabSettings.TabIndex = 2;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // groupSettings
            // 
            this.groupSettings.Controls.Add(this.checkEnableSound);
            this.groupSettings.Location = new System.Drawing.Point(6, 240);
            this.groupSettings.Name = "groupSettings";
            this.groupSettings.Size = new System.Drawing.Size(349, 50);
            this.groupSettings.TabIndex = 5;
            this.groupSettings.TabStop = false;
            this.groupSettings.Text = "Settings";
            // 
            // checkEnableSound
            // 
            this.checkEnableSound.AutoSize = true;
            this.checkEnableSound.Location = new System.Drawing.Point(6, 22);
            this.checkEnableSound.Name = "checkEnableSound";
            this.checkEnableSound.Size = new System.Drawing.Size(151, 19);
            this.checkEnableSound.TabIndex = 2;
            this.checkEnableSound.Text = "Enable launcher sounds";
            this.checkEnableSound.UseVisualStyleBackColor = true;
            this.checkEnableSound.CheckedChanged += new System.EventHandler(this.checkEnableSound_CheckedChanged);
            // 
            // groupBackup
            // 
            this.groupBackup.Controls.Add(this.label1);
            this.groupBackup.Controls.Add(this.buttonOpenLog);
            this.groupBackup.Controls.Add(this.label2);
            this.groupBackup.Controls.Add(this.labelSetupWizard);
            this.groupBackup.Controls.Add(this.buttonRestoreOriginals);
            this.groupBackup.Controls.Add(this.buttonSetupWizard);
            this.groupBackup.Location = new System.Drawing.Point(6, 65);
            this.groupBackup.Name = "groupBackup";
            this.groupBackup.Size = new System.Drawing.Size(349, 169);
            this.groupBackup.TabIndex = 4;
            this.groupBackup.TabStop = false;
            this.groupBackup.Text = "Tools";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(23, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 44);
            this.label1.TabIndex = 11;
            this.label1.Text = "Opens SnakeBites debug logs";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOpenLog
            // 
            this.buttonOpenLog.Location = new System.Drawing.Point(236, 119);
            this.buttonOpenLog.Name = "buttonOpenLog";
            this.buttonOpenLog.Size = new System.Drawing.Size(107, 44);
            this.buttonOpenLog.TabIndex = 10;
            this.buttonOpenLog.Text = "Open Logs";
            this.buttonOpenLog.UseVisualStyleBackColor = true;
            this.buttonOpenLog.Click += new System.EventHandler(this.buttonOpenLog_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(14, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 44);
            this.label2.TabIndex = 9;
            this.label2.Text = "Permanently removes all mods and SnakeBite settings";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSetupWizard
            // 
            this.labelSetupWizard.Location = new System.Drawing.Point(18, 19);
            this.labelSetupWizard.Name = "labelSetupWizard";
            this.labelSetupWizard.Size = new System.Drawing.Size(203, 44);
            this.labelSetupWizard.TabIndex = 5;
            this.labelSetupWizard.Text = "Run the Setup Wizard again to create backups and fix database errors";
            this.labelSetupWizard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonRestoreOriginals
            // 
            this.buttonRestoreOriginals.Location = new System.Drawing.Point(236, 69);
            this.buttonRestoreOriginals.Name = "buttonRestoreOriginals";
            this.buttonRestoreOriginals.Size = new System.Drawing.Size(107, 44);
            this.buttonRestoreOriginals.TabIndex = 7;
            this.buttonRestoreOriginals.Text = "Restore Original Game Files";
            this.buttonRestoreOriginals.UseVisualStyleBackColor = true;
            this.buttonRestoreOriginals.Click += new System.EventHandler(this.buttonRestoreOriginals_Click);
            // 
            // buttonSetupWizard
            // 
            this.buttonSetupWizard.Location = new System.Drawing.Point(236, 19);
            this.buttonSetupWizard.Name = "buttonSetupWizard";
            this.buttonSetupWizard.Size = new System.Drawing.Size(107, 44);
            this.buttonSetupWizard.TabIndex = 2;
            this.buttonSetupWizard.Text = "Setup Wizard";
            this.buttonSetupWizard.UseVisualStyleBackColor = true;
            this.buttonSetupWizard.Click += new System.EventHandler(this.buttonSetup);
            // 
            // groupAbout
            // 
            this.groupAbout.Controls.Add(this.labelGithub);
            this.groupAbout.Controls.Add(this.labelThanks);
            this.groupAbout.Location = new System.Drawing.Point(6, 296);
            this.groupAbout.Name = "groupAbout";
            this.groupAbout.Size = new System.Drawing.Size(349, 127);
            this.groupAbout.TabIndex = 3;
            this.groupAbout.TabStop = false;
            // 
            // labelGithub
            // 
            this.labelGithub.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelGithub.Location = new System.Drawing.Point(0, 47);
            this.labelGithub.Name = "labelGithub";
            this.labelGithub.Size = new System.Drawing.Size(349, 15);
            this.labelGithub.TabIndex = 1;
            this.labelGithub.TabStop = true;
            this.labelGithub.Text = "www.nexusmods.com/metalgearsolidvtpp/mods/106";
            this.labelGithub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGithub_LinkClicked);
            // 
            // labelThanks
            // 
            this.labelThanks.AutoSize = true;
            this.labelThanks.Location = new System.Drawing.Point(14, 26);
            this.labelThanks.Name = "labelThanks";
            this.labelThanks.Size = new System.Drawing.Size(326, 75);
            this.labelThanks.TabIndex = 0;
            this.labelThanks.Text = "The latest version of SnakeBite is available here:\r\n\r\n\r\nSpecial thanks to Atvaark" +
    " for GzsTool and emoose and many\r\nmore for all their hard work!";
            this.labelThanks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupMGSVDir
            // 
            this.groupMGSVDir.Controls.Add(this.buttonFindMGSV);
            this.groupMGSVDir.Controls.Add(this.textInstallPath);
            this.groupMGSVDir.Location = new System.Drawing.Point(6, 7);
            this.groupMGSVDir.Name = "groupMGSVDir";
            this.groupMGSVDir.Size = new System.Drawing.Size(349, 52);
            this.groupMGSVDir.TabIndex = 0;
            this.groupMGSVDir.TabStop = false;
            this.groupMGSVDir.Text = "MGSV Installation";
            // 
            // buttonFindMGSV
            // 
            this.buttonFindMGSV.Location = new System.Drawing.Point(318, 22);
            this.buttonFindMGSV.Name = "buttonFindMGSV";
            this.buttonFindMGSV.Size = new System.Drawing.Size(25, 25);
            this.buttonFindMGSV.TabIndex = 2;
            this.buttonFindMGSV.Text = "...";
            this.buttonFindMGSV.UseVisualStyleBackColor = true;
            this.buttonFindMGSV.Click += new System.EventHandler(this.buttonFindMGSV_Click);
            // 
            // textInstallPath
            // 
            this.textInstallPath.Location = new System.Drawing.Point(6, 22);
            this.textInstallPath.Name = "textInstallPath";
            this.textInstallPath.ReadOnly = true;
            this.textInstallPath.Size = new System.Drawing.Size(306, 23);
            this.textInstallPath.TabIndex = 1;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSettings);
            this.tabControl.Controls.Add(this.tabThemes);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(371, 454);
            this.tabControl.TabIndex = 0;
            // 
            // tabThemes
            // 
            this.tabThemes.Controls.Add(this.buttonSetTheme);
            this.tabThemes.Controls.Add(this.listThemes);
            this.tabThemes.Location = new System.Drawing.Point(4, 24);
            this.tabThemes.Name = "tabThemes";
            this.tabThemes.Size = new System.Drawing.Size(363, 426);
            this.tabThemes.TabIndex = 3;
            this.tabThemes.Text = "Themes";
            this.tabThemes.UseVisualStyleBackColor = true;
            // 
            // buttonSetTheme
            // 
            this.buttonSetTheme.Location = new System.Drawing.Point(83, 324);
            this.buttonSetTheme.Name = "buttonSetTheme";
            this.buttonSetTheme.Size = new System.Drawing.Size(201, 23);
            this.buttonSetTheme.TabIndex = 1;
            this.buttonSetTheme.Text = "Set Theme";
            this.buttonSetTheme.UseVisualStyleBackColor = true;
            this.buttonSetTheme.Click += new System.EventHandler(this.buttonSetTheme_Click);
            // 
            // listThemes
            // 
            this.listThemes.FormattingEnabled = true;
            this.listThemes.ItemHeight = 15;
            this.listThemes.Items.AddRange(new object[] {
            "Default"});
            this.listThemes.Location = new System.Drawing.Point(83, 29);
            this.listThemes.Name = "listThemes";
            this.listThemes.Size = new System.Drawing.Size(201, 289);
            this.listThemes.TabIndex = 0;
            // 
            // formSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(392, 472);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SnakeBite Settings";
            this.Load += new System.EventHandler(this.formSettings_Load);
            this.tabSettings.ResumeLayout(false);
            this.groupSettings.ResumeLayout(false);
            this.groupSettings.PerformLayout();
            this.groupBackup.ResumeLayout(false);
            this.groupAbout.ResumeLayout(false);
            this.groupAbout.PerformLayout();
            this.groupMGSVDir.ResumeLayout(false);
            this.groupMGSVDir.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabThemes.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.GroupBox groupBackup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelSetupWizard;
        private System.Windows.Forms.Button buttonRestoreOriginals;
        private System.Windows.Forms.Button buttonSetupWizard;
        private System.Windows.Forms.GroupBox groupAbout;
        private System.Windows.Forms.LinkLabel labelGithub;
        private System.Windows.Forms.Label labelThanks;
        private System.Windows.Forms.GroupBox groupMGSVDir;
        private System.Windows.Forms.Button buttonFindMGSV;
        private System.Windows.Forms.TextBox textInstallPath;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.GroupBox groupSettings;
        private System.Windows.Forms.CheckBox checkEnableSound;
        private System.Windows.Forms.TabPage tabThemes;
        private System.Windows.Forms.ListBox listThemes;
        private System.Windows.Forms.Button buttonSetTheme;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOpenLog;
    }
}