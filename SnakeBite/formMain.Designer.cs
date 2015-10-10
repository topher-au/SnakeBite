namespace SnakeBite
{
    partial class formMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formMain));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabInstalledMods = new System.Windows.Forms.TabPage();
            this.buttonInstallMod = new System.Windows.Forms.Button();
            this.listInstalledMods = new System.Windows.Forms.ListBox();
            this.labelNoMods = new System.Windows.Forms.Label();
            this.groupModInfo = new System.Windows.Forms.GroupBox();
            this.panelModDetails = new System.Windows.Forms.Panel();
            this.labelModWebsite = new System.Windows.Forms.LinkLabel();
            this.buttonUninstallMod = new System.Windows.Forms.Button();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.labelModVersion = new System.Windows.Forms.Label();
            this.labelModAuthor = new System.Windows.Forms.Label();
            this.labelModName = new System.Windows.Forms.Label();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.buttonBuildGameDB = new System.Windows.Forms.Button();
            this.checkConflicts = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonFindMGSV = new System.Windows.Forms.Button();
            this.textInstallPath = new System.Windows.Forms.TextBox();
            this.buttonLaunch = new System.Windows.Forms.Button();
            this.groupAbout = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabInstalledMods.SuspendLayout();
            this.groupModInfo.SuspendLayout();
            this.panelModDetails.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabInstalledMods);
            this.tabControl.Controls.Add(this.tabSettings);
            this.tabControl.Location = new System.Drawing.Point(14, 14);
            this.tabControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(585, 438);
            this.tabControl.TabIndex = 0;
            // 
            // tabInstalledMods
            // 
            this.tabInstalledMods.Controls.Add(this.buttonInstallMod);
            this.tabInstalledMods.Controls.Add(this.listInstalledMods);
            this.tabInstalledMods.Controls.Add(this.labelNoMods);
            this.tabInstalledMods.Controls.Add(this.groupModInfo);
            this.tabInstalledMods.Location = new System.Drawing.Point(4, 24);
            this.tabInstalledMods.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabInstalledMods.Name = "tabInstalledMods";
            this.tabInstalledMods.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabInstalledMods.Size = new System.Drawing.Size(577, 410);
            this.tabInstalledMods.TabIndex = 0;
            this.tabInstalledMods.Text = "Installed Mods";
            this.tabInstalledMods.UseVisualStyleBackColor = true;
            // 
            // buttonInstallMod
            // 
            this.buttonInstallMod.Location = new System.Drawing.Point(7, 378);
            this.buttonInstallMod.Name = "buttonInstallMod";
            this.buttonInstallMod.Size = new System.Drawing.Size(187, 23);
            this.buttonInstallMod.TabIndex = 1;
            this.buttonInstallMod.Text = "Install Mod...";
            this.buttonInstallMod.UseVisualStyleBackColor = true;
            this.buttonInstallMod.Click += new System.EventHandler(this.buttonInstallMod_Click);
            // 
            // listInstalledMods
            // 
            this.listInstalledMods.FormattingEnabled = true;
            this.listInstalledMods.ItemHeight = 15;
            this.listInstalledMods.Location = new System.Drawing.Point(7, 7);
            this.listInstalledMods.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listInstalledMods.Name = "listInstalledMods";
            this.listInstalledMods.Size = new System.Drawing.Size(187, 364);
            this.listInstalledMods.TabIndex = 0;
            this.listInstalledMods.SelectedIndexChanged += new System.EventHandler(this.listInstalledMods_SelectedIndexChanged);
            // 
            // labelNoMods
            // 
            this.labelNoMods.AutoSize = true;
            this.labelNoMods.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoMods.Location = new System.Drawing.Point(293, 171);
            this.labelNoMods.Name = "labelNoMods";
            this.labelNoMods.Size = new System.Drawing.Size(192, 30);
            this.labelNoMods.TabIndex = 2;
            this.labelNoMods.Text = "No mods installed";
            // 
            // groupModInfo
            // 
            this.groupModInfo.Controls.Add(this.panelModDetails);
            this.groupModInfo.Location = new System.Drawing.Point(200, 0);
            this.groupModInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupModInfo.Name = "groupModInfo";
            this.groupModInfo.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupModInfo.Size = new System.Drawing.Size(367, 401);
            this.groupModInfo.TabIndex = 1;
            this.groupModInfo.TabStop = false;
            // 
            // panelModDetails
            // 
            this.panelModDetails.Controls.Add(this.labelModWebsite);
            this.panelModDetails.Controls.Add(this.buttonUninstallMod);
            this.panelModDetails.Controls.Add(this.textDescription);
            this.panelModDetails.Controls.Add(this.labelModVersion);
            this.panelModDetails.Controls.Add(this.labelModAuthor);
            this.panelModDetails.Controls.Add(this.labelModName);
            this.panelModDetails.Location = new System.Drawing.Point(6, 13);
            this.panelModDetails.Name = "panelModDetails";
            this.panelModDetails.Size = new System.Drawing.Size(354, 381);
            this.panelModDetails.TabIndex = 6;
            // 
            // labelModWebsite
            // 
            this.labelModWebsite.AutoSize = true;
            this.labelModWebsite.Location = new System.Drawing.Point(5, 48);
            this.labelModWebsite.Name = "labelModWebsite";
            this.labelModWebsite.Size = new System.Drawing.Size(240, 15);
            this.labelModWebsite.TabIndex = 9;
            this.labelModWebsite.TabStop = true;
            this.labelModWebsite.Text = "http://mod.website.com/path/to/mod.html";
            // 
            // buttonUninstallMod
            // 
            this.buttonUninstallMod.Location = new System.Drawing.Point(276, 355);
            this.buttonUninstallMod.Name = "buttonUninstallMod";
            this.buttonUninstallMod.Size = new System.Drawing.Size(75, 23);
            this.buttonUninstallMod.TabIndex = 5;
            this.buttonUninstallMod.Text = "Uninstall";
            this.buttonUninstallMod.UseVisualStyleBackColor = true;
            this.buttonUninstallMod.Click += new System.EventHandler(this.buttonUninstallMod_Click);
            // 
            // textDescription
            // 
            this.textDescription.Location = new System.Drawing.Point(0, 76);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDescription.Size = new System.Drawing.Size(352, 273);
            this.textDescription.TabIndex = 8;
            // 
            // labelModVersion
            // 
            this.labelModVersion.AutoSize = true;
            this.labelModVersion.Location = new System.Drawing.Point(5, 33);
            this.labelModVersion.Name = "labelModVersion";
            this.labelModVersion.Size = new System.Drawing.Size(37, 15);
            this.labelModVersion.TabIndex = 7;
            this.labelModVersion.Text = "v1234";
            // 
            // labelModAuthor
            // 
            this.labelModAuthor.AutoSize = true;
            this.labelModAuthor.Location = new System.Drawing.Point(48, 33);
            this.labelModAuthor.Name = "labelModAuthor";
            this.labelModAuthor.Size = new System.Drawing.Size(60, 15);
            this.labelModAuthor.TabIndex = 6;
            this.labelModAuthor.Text = "by Author";
            this.labelModAuthor.Click += new System.EventHandler(this.labelModAuthor_Click);
            // 
            // labelModName
            // 
            this.labelModName.AutoSize = true;
            this.labelModName.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelModName.Location = new System.Drawing.Point(3, 7);
            this.labelModName.Name = "labelModName";
            this.labelModName.Size = new System.Drawing.Size(145, 25);
            this.labelModName.TabIndex = 5;
            this.labelModName.Text = "Mod Title Here";
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.labelVersion);
            this.tabSettings.Controls.Add(this.groupAbout);
            this.tabSettings.Controls.Add(this.buttonBuildGameDB);
            this.tabSettings.Controls.Add(this.checkConflicts);
            this.tabSettings.Controls.Add(this.groupBox1);
            this.tabSettings.Location = new System.Drawing.Point(4, 24);
            this.tabSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabSettings.Size = new System.Drawing.Size(577, 410);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // buttonBuildGameDB
            // 
            this.buttonBuildGameDB.Location = new System.Drawing.Point(422, 64);
            this.buttonBuildGameDB.Name = "buttonBuildGameDB";
            this.buttonBuildGameDB.Size = new System.Drawing.Size(143, 23);
            this.buttonBuildGameDB.TabIndex = 2;
            this.buttonBuildGameDB.Text = "Rebuild Database";
            this.buttonBuildGameDB.UseVisualStyleBackColor = true;
            this.buttonBuildGameDB.Click += new System.EventHandler(this.buttonBuildGameDB_Click);
            // 
            // checkConflicts
            // 
            this.checkConflicts.AutoSize = true;
            this.checkConflicts.Location = new System.Drawing.Point(12, 67);
            this.checkConflicts.Name = "checkConflicts";
            this.checkConflicts.Size = new System.Drawing.Size(158, 19);
            this.checkConflicts.TabIndex = 1;
            this.checkConflicts.Text = "Disable conflict checking";
            this.checkConflicts.UseVisualStyleBackColor = true;
            this.checkConflicts.CheckedChanged += new System.EventHandler(this.checkConflicts_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonFindMGSV);
            this.groupBox1.Controls.Add(this.textInstallPath);
            this.groupBox1.Location = new System.Drawing.Point(6, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(565, 52);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MGSV Installation";
            // 
            // buttonFindMGSV
            // 
            this.buttonFindMGSV.Location = new System.Drawing.Point(534, 21);
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
            this.textInstallPath.Size = new System.Drawing.Size(522, 23);
            this.textInstallPath.TabIndex = 1;
            // 
            // buttonLaunch
            // 
            this.buttonLaunch.Location = new System.Drawing.Point(476, 8);
            this.buttonLaunch.Name = "buttonLaunch";
            this.buttonLaunch.Size = new System.Drawing.Size(123, 23);
            this.buttonLaunch.TabIndex = 3;
            this.buttonLaunch.Text = "Launch MGSV";
            this.buttonLaunch.UseVisualStyleBackColor = true;
            this.buttonLaunch.Click += new System.EventHandler(this.buttonLaunch_Click);
            // 
            // groupAbout
            // 
            this.groupAbout.Controls.Add(this.linkLabel1);
            this.groupAbout.Controls.Add(this.label1);
            this.groupAbout.Location = new System.Drawing.Point(121, 176);
            this.groupAbout.Name = "groupAbout";
            this.groupAbout.Size = new System.Drawing.Size(339, 133);
            this.groupAbout.TabIndex = 3;
            this.groupAbout.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(325, 105);
            this.label1.TabIndex = 0;
            this.label1.Text = "Thankyou for using SnakeBite!\r\n\r\nLatest version available here:\r\n\r\n\r\nSpecial than" +
    "ks to Atvaark for GzsTool and emoose and many\r\nmore for all their hard work!";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel1.Location = new System.Drawing.Point(24, 70);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(299, 15);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://www.github.com/topher-au/SnakeBite/releases";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(3, 391);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(51, 15);
            this.labelVersion.TabIndex = 2;
            this.labelVersion.Text = "version1";
            this.labelVersion.Click += new System.EventHandler(this.label2_Click);
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 467);
            this.Controls.Add(this.buttonLaunch);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SnakeBite Mod Manager";
            this.Load += new System.EventHandler(this.formMain_Load);
            this.tabControl.ResumeLayout(false);
            this.tabInstalledMods.ResumeLayout(false);
            this.tabInstalledMods.PerformLayout();
            this.groupModInfo.ResumeLayout(false);
            this.panelModDetails.ResumeLayout(false);
            this.panelModDetails.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupAbout.ResumeLayout(false);
            this.groupAbout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabInstalledMods;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.ListBox listInstalledMods;
        private System.Windows.Forms.GroupBox groupModInfo;
        private System.Windows.Forms.Button buttonInstallMod;
        private System.Windows.Forms.Button buttonUninstallMod;
        private System.Windows.Forms.Panel panelModDetails;
        private System.Windows.Forms.LinkLabel labelModWebsite;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.Label labelModVersion;
        private System.Windows.Forms.Label labelModAuthor;
        private System.Windows.Forms.Label labelModName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonFindMGSV;
        private System.Windows.Forms.TextBox textInstallPath;
        private System.Windows.Forms.Button buttonBuildGameDB;
        private System.Windows.Forms.CheckBox checkConflicts;
        private System.Windows.Forms.Label labelNoMods;
        private System.Windows.Forms.Button buttonLaunch;
        private System.Windows.Forms.GroupBox groupAbout;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelVersion;
    }
}

