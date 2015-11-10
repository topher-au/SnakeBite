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
            this.groupBackup = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSetupWizard = new System.Windows.Forms.Label();
            this.buttonRestoreOriginals = new System.Windows.Forms.Button();
            this.buttonSetupWizard = new System.Windows.Forms.Button();
            this.buttonToggleMods = new System.Windows.Forms.Button();
            this.groupAbout = new System.Windows.Forms.GroupBox();
            this.labelGithub = new System.Windows.Forms.LinkLabel();
            this.labelThanks = new System.Windows.Forms.Label();
            this.checkConflicts = new System.Windows.Forms.CheckBox();
            this.groupMGSVDir = new System.Windows.Forms.GroupBox();
            this.buttonFindMGSV = new System.Windows.Forms.Button();
            this.textInstallPath = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.groupSettings = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabSettings.SuspendLayout();
            this.groupBackup.SuspendLayout();
            this.groupAbout.SuspendLayout();
            this.groupMGSVDir.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.groupSettings.SuspendLayout();
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
            this.tabSettings.Size = new System.Drawing.Size(570, 410);
            this.tabSettings.TabIndex = 2;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // groupBackup
            // 
            this.groupBackup.Controls.Add(this.label2);
            this.groupBackup.Controls.Add(this.label1);
            this.groupBackup.Controls.Add(this.labelSetupWizard);
            this.groupBackup.Controls.Add(this.buttonRestoreOriginals);
            this.groupBackup.Controls.Add(this.buttonSetupWizard);
            this.groupBackup.Controls.Add(this.buttonToggleMods);
            this.groupBackup.Location = new System.Drawing.Point(210, 65);
            this.groupBackup.Name = "groupBackup";
            this.groupBackup.Size = new System.Drawing.Size(349, 172);
            this.groupBackup.TabIndex = 4;
            this.groupBackup.TabStop = false;
            this.groupBackup.Text = "Tools";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 44);
            this.label2.TabIndex = 9;
            this.label2.Text = "Permanently removes all mods and SnakeBite settings";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(15, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(206, 44);
            this.label1.TabIndex = 8;
            this.label1.Text = "Quickly enable or disable all installed mods";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.buttonRestoreOriginals.Location = new System.Drawing.Point(236, 119);
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
            // buttonToggleMods
            // 
            this.buttonToggleMods.Location = new System.Drawing.Point(236, 69);
            this.buttonToggleMods.Name = "buttonToggleMods";
            this.buttonToggleMods.Size = new System.Drawing.Size(107, 44);
            this.buttonToggleMods.TabIndex = 6;
            this.buttonToggleMods.Text = "Toggle Mods";
            this.buttonToggleMods.UseVisualStyleBackColor = true;
            this.buttonToggleMods.Click += new System.EventHandler(this.buttonToggleMods_Click);
            // 
            // groupAbout
            // 
            this.groupAbout.Controls.Add(this.labelGithub);
            this.groupAbout.Controls.Add(this.labelThanks);
            this.groupAbout.Location = new System.Drawing.Point(119, 281);
            this.groupAbout.Name = "groupAbout";
            this.groupAbout.Size = new System.Drawing.Size(349, 97);
            this.groupAbout.TabIndex = 3;
            this.groupAbout.TabStop = false;
            // 
            // labelGithub
            // 
            this.labelGithub.AutoSize = true;
            this.labelGithub.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelGithub.Location = new System.Drawing.Point(20, 32);
            this.labelGithub.Name = "labelGithub";
            this.labelGithub.Size = new System.Drawing.Size(299, 15);
            this.labelGithub.TabIndex = 1;
            this.labelGithub.TabStop = true;
            this.labelGithub.Text = "https://www.github.com/topher-au/SnakeBite/releases";
            this.labelGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGithub_LinkClicked);
            // 
            // labelThanks
            // 
            this.labelThanks.AutoSize = true;
            this.labelThanks.Location = new System.Drawing.Point(6, 15);
            this.labelThanks.Name = "labelThanks";
            this.labelThanks.Size = new System.Drawing.Size(325, 75);
            this.labelThanks.TabIndex = 0;
            this.labelThanks.Text = "The latest version of SnakeBite is available here:\r\n\r\n\r\nSpecial thanks to Atvaark" +
    " for GzsTool and emoose and many\r\nmore for all their hard work!";
            this.labelThanks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkConflicts
            // 
            this.checkConflicts.AutoSize = true;
            this.checkConflicts.Location = new System.Drawing.Point(12, 23);
            this.checkConflicts.Name = "checkConflicts";
            this.checkConflicts.Size = new System.Drawing.Size(171, 19);
            this.checkConflicts.TabIndex = 1;
            this.checkConflicts.Text = "Disable compatibility check";
            this.checkConflicts.UseVisualStyleBackColor = true;
            this.checkConflicts.CheckedChanged += new System.EventHandler(this.checkConflicts_CheckedChanged);
            // 
            // groupMGSVDir
            // 
            this.groupMGSVDir.Controls.Add(this.buttonFindMGSV);
            this.groupMGSVDir.Controls.Add(this.textInstallPath);
            this.groupMGSVDir.Location = new System.Drawing.Point(6, 7);
            this.groupMGSVDir.Name = "groupMGSVDir";
            this.groupMGSVDir.Size = new System.Drawing.Size(558, 52);
            this.groupMGSVDir.TabIndex = 0;
            this.groupMGSVDir.TabStop = false;
            this.groupMGSVDir.Text = "MGSV Installation";
            // 
            // buttonFindMGSV
            // 
            this.buttonFindMGSV.Location = new System.Drawing.Point(528, 21);
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
            this.textInstallPath.Size = new System.Drawing.Size(516, 23);
            this.textInstallPath.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(578, 438);
            this.tabControl1.TabIndex = 0;
            // 
            // groupSettings
            // 
            this.groupSettings.Controls.Add(this.checkBox1);
            this.groupSettings.Controls.Add(this.checkConflicts);
            this.groupSettings.Location = new System.Drawing.Point(6, 65);
            this.groupSettings.Name = "groupSettings";
            this.groupSettings.Size = new System.Drawing.Size(200, 76);
            this.groupSettings.TabIndex = 5;
            this.groupSettings.TabStop = false;
            this.groupSettings.Text = "Settings";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 48);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(151, 19);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Enable launcher sounds";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkEnableSound_CheckedChanged);
            // 
            // formSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(605, 467);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "formSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SnakeBite Settings";
            this.Load += new System.EventHandler(this.formSettings_Load);
            this.tabSettings.ResumeLayout(false);
            this.groupBackup.ResumeLayout(false);
            this.groupAbout.ResumeLayout(false);
            this.groupAbout.PerformLayout();
            this.groupMGSVDir.ResumeLayout(false);
            this.groupMGSVDir.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.groupSettings.ResumeLayout(false);
            this.groupSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.GroupBox groupBackup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSetupWizard;
        private System.Windows.Forms.Button buttonRestoreOriginals;
        private System.Windows.Forms.Button buttonSetupWizard;
        private System.Windows.Forms.Button buttonToggleMods;
        private System.Windows.Forms.GroupBox groupAbout;
        private System.Windows.Forms.LinkLabel labelGithub;
        private System.Windows.Forms.Label labelThanks;
        private System.Windows.Forms.CheckBox checkConflicts;
        private System.Windows.Forms.GroupBox groupMGSVDir;
        private System.Windows.Forms.Button buttonFindMGSV;
        private System.Windows.Forms.TextBox textInstallPath;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.GroupBox groupSettings;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}