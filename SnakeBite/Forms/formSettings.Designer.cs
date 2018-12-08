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
            this.labelNoBackups = new System.Windows.Forms.Label();
            this.groupSettings = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkEnableSound = new System.Windows.Forms.CheckBox();
            this.picModToggle = new System.Windows.Forms.PictureBox();
            this.groupBackup = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOpenLog = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelSetupWizard = new System.Windows.Forms.Label();
            this.buttonRestoreOriginals = new System.Windows.Forms.Button();
            this.buttonSetupWizard = new System.Windows.Forms.Button();
            this.groupMGSVDir = new System.Windows.Forms.GroupBox();
            this.buttonFindMGSV = new System.Windows.Forms.Button();
            this.textInstallPath = new System.Windows.Forms.TextBox();
            this.groupAbout = new System.Windows.Forms.GroupBox();
            this.labelNexusLink = new System.Windows.Forms.LinkLabel();
            this.labelThanks = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picModToggle)).BeginInit();
            this.groupBackup.SuspendLayout();
            this.groupMGSVDir.SuspendLayout();
            this.groupAbout.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelNoBackups
            // 
            this.labelNoBackups.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoBackups.Location = new System.Drawing.Point(3, 58);
            this.labelNoBackups.Name = "labelNoBackups";
            this.labelNoBackups.Size = new System.Drawing.Size(360, 33);
            this.labelNoBackups.TabIndex = 6;
            this.labelNoBackups.Text = "No Backups Detected.\r\nCertain features are unavailable.";
            this.labelNoBackups.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupSettings
            // 
            this.groupSettings.Controls.Add(this.label3);
            this.groupSettings.Controls.Add(this.checkEnableSound);
            this.groupSettings.Controls.Add(this.picModToggle);
            this.groupSettings.Location = new System.Drawing.Point(3, 269);
            this.groupSettings.Name = "groupSettings";
            this.groupSettings.Size = new System.Drawing.Size(360, 82);
            this.groupSettings.TabIndex = 5;
            this.groupSettings.TabStop = false;
            this.groupSettings.Text = "Settings";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(102, 55);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label3.Size = new System.Drawing.Size(118, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "Toggle Mods On/Off";
            // 
            // checkEnableSound
            // 
            this.checkEnableSound.Location = new System.Drawing.Point(84, 21);
            this.checkEnableSound.Name = "checkEnableSound";
            this.checkEnableSound.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkEnableSound.Size = new System.Drawing.Size(215, 19);
            this.checkEnableSound.TabIndex = 2;
            this.checkEnableSound.Text = "Enable Launcher Sounds";
            this.checkEnableSound.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkEnableSound.UseVisualStyleBackColor = true;
            this.checkEnableSound.CheckedChanged += new System.EventHandler(this.checkEnableSound_CheckedChanged);
            // 
            // picModToggle
            // 
            this.picModToggle.BackColor = System.Drawing.Color.Transparent;
            this.picModToggle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picModToggle.Image = global::SnakeBite.Properties.Resources.toggledisabled;
            this.picModToggle.Location = new System.Drawing.Point(246, 43);
            this.picModToggle.Name = "picModToggle";
            this.picModToggle.Size = new System.Drawing.Size(92, 39);
            this.picModToggle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picModToggle.TabIndex = 9;
            this.picModToggle.TabStop = false;
            this.picModToggle.Click += new System.EventHandler(this.picModToggle_Click);
            // 
            // groupBackup
            // 
            this.groupBackup.Controls.Add(this.label1);
            this.groupBackup.Controls.Add(this.buttonOpenLog);
            this.groupBackup.Controls.Add(this.label2);
            this.groupBackup.Controls.Add(this.labelSetupWizard);
            this.groupBackup.Controls.Add(this.buttonRestoreOriginals);
            this.groupBackup.Controls.Add(this.buttonSetupWizard);
            this.groupBackup.Location = new System.Drawing.Point(3, 94);
            this.groupBackup.Name = "groupBackup";
            this.groupBackup.Size = new System.Drawing.Size(360, 169);
            this.groupBackup.TabIndex = 4;
            this.groupBackup.TabStop = false;
            this.groupBackup.Text = "Tools";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(14, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 44);
            this.label1.TabIndex = 11;
            this.label1.Text = "Open SnakeBite\'s Debug Logs";
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
            this.label2.Location = new System.Drawing.Point(6, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(215, 44);
            this.label2.TabIndex = 9;
            this.label2.Text = "Permanently remove all mods and SnakeBite settings";
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
            this.buttonRestoreOriginals.Text = "Restore Backup Game Files";
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
            // groupMGSVDir
            // 
            this.groupMGSVDir.Controls.Add(this.buttonFindMGSV);
            this.groupMGSVDir.Controls.Add(this.textInstallPath);
            this.groupMGSVDir.Location = new System.Drawing.Point(3, 3);
            this.groupMGSVDir.Name = "groupMGSVDir";
            this.groupMGSVDir.Size = new System.Drawing.Size(360, 52);
            this.groupMGSVDir.TabIndex = 0;
            this.groupMGSVDir.TabStop = false;
            this.groupMGSVDir.Text = "MGSV Installation";
            // 
            // buttonFindMGSV
            // 
            this.buttonFindMGSV.Location = new System.Drawing.Point(322, 20);
            this.buttonFindMGSV.Name = "buttonFindMGSV";
            this.buttonFindMGSV.Size = new System.Drawing.Size(32, 25);
            this.buttonFindMGSV.TabIndex = 2;
            this.buttonFindMGSV.Text = "...";
            this.buttonFindMGSV.UseVisualStyleBackColor = true;
            this.buttonFindMGSV.Click += new System.EventHandler(this.buttonFindMGSV_Click);
            // 
            // textInstallPath
            // 
            this.textInstallPath.Location = new System.Drawing.Point(6, 20);
            this.textInstallPath.Name = "textInstallPath";
            this.textInstallPath.ReadOnly = true;
            this.textInstallPath.Size = new System.Drawing.Size(313, 23);
            this.textInstallPath.TabIndex = 1;
            // 
            // groupAbout
            // 
            this.groupAbout.Controls.Add(this.labelNexusLink);
            this.groupAbout.Controls.Add(this.labelThanks);
            this.groupAbout.Location = new System.Drawing.Point(3, 357);
            this.groupAbout.Name = "groupAbout";
            this.groupAbout.Size = new System.Drawing.Size(360, 90);
            this.groupAbout.TabIndex = 3;
            this.groupAbout.TabStop = false;
            // 
            // labelNexusLink
            // 
            this.labelNexusLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelNexusLink.Location = new System.Drawing.Point(6, 38);
            this.labelNexusLink.Name = "labelNexusLink";
            this.labelNexusLink.Size = new System.Drawing.Size(351, 16);
            this.labelNexusLink.TabIndex = 1;
            this.labelNexusLink.TabStop = true;
            this.labelNexusLink.Text = "www.nexusmods.com/metalgearsolidvtpp/mods/106";
            this.labelNexusLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelNexusLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkNexusLink_LinkClicked);
            // 
            // labelThanks
            // 
            this.labelThanks.Location = new System.Drawing.Point(6, 19);
            this.labelThanks.Name = "labelThanks";
            this.labelThanks.Size = new System.Drawing.Size(351, 70);
            this.labelThanks.TabIndex = 0;
            this.labelThanks.Text = "The latest version of SnakeBite is available here:\r\n\r\nSpecial thanks to Atvaark f" +
    "or GzsTool and emoose and many more for all their hard work!";
            this.labelThanks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.groupAbout);
            this.panel1.Controls.Add(this.groupMGSVDir);
            this.panel1.Controls.Add(this.labelNoBackups);
            this.panel1.Controls.Add(this.groupBackup);
            this.panel1.Controls.Add(this.groupSettings);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(368, 452);
            this.panel1.TabIndex = 1;
            // 
            // formSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(392, 476);
            this.Controls.Add(this.panel1);
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
            this.groupSettings.ResumeLayout(false);
            this.groupSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picModToggle)).EndInit();
            this.groupBackup.ResumeLayout(false);
            this.groupMGSVDir.ResumeLayout(false);
            this.groupMGSVDir.PerformLayout();
            this.groupAbout.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelNoBackups;
        private System.Windows.Forms.GroupBox groupSettings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkEnableSound;
        private System.Windows.Forms.PictureBox picModToggle;
        private System.Windows.Forms.GroupBox groupBackup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOpenLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelSetupWizard;
        private System.Windows.Forms.Button buttonRestoreOriginals;
        private System.Windows.Forms.Button buttonSetupWizard;
        private System.Windows.Forms.GroupBox groupMGSVDir;
        private System.Windows.Forms.Button buttonFindMGSV;
        private System.Windows.Forms.TextBox textInstallPath;
        private System.Windows.Forms.GroupBox groupAbout;
        private System.Windows.Forms.LinkLabel labelNexusLink;
        private System.Windows.Forms.Label labelThanks;
        private System.Windows.Forms.Panel panel1;
    }
}