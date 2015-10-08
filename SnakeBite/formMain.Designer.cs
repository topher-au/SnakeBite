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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabInstalledMods = new System.Windows.Forms.TabPage();
            this.buttonInstallMod = new System.Windows.Forms.Button();
            this.groupModInfo = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelModWebsite = new System.Windows.Forms.LinkLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.labelModVersion = new System.Windows.Forms.Label();
            this.labelModAuthor = new System.Windows.Forms.Label();
            this.labelModName = new System.Windows.Forms.Label();
            this.buttonUninstallMod = new System.Windows.Forms.Button();
            this.listInstalledMods = new System.Windows.Forms.ListBox();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.buttonBuildGameDB = new System.Windows.Forms.Button();
            this.checkConflicts = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonFindMGSV = new System.Windows.Forms.Button();
            this.textInstallPath = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabInstalledMods.SuspendLayout();
            this.groupModInfo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.tabControl.Size = new System.Drawing.Size(450, 381);
            this.tabControl.TabIndex = 0;
            // 
            // tabInstalledMods
            // 
            this.tabInstalledMods.Controls.Add(this.buttonInstallMod);
            this.tabInstalledMods.Controls.Add(this.groupModInfo);
            this.tabInstalledMods.Controls.Add(this.listInstalledMods);
            this.tabInstalledMods.Location = new System.Drawing.Point(4, 24);
            this.tabInstalledMods.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabInstalledMods.Name = "tabInstalledMods";
            this.tabInstalledMods.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabInstalledMods.Size = new System.Drawing.Size(442, 353);
            this.tabInstalledMods.TabIndex = 0;
            this.tabInstalledMods.Text = "Installed Mods";
            this.tabInstalledMods.UseVisualStyleBackColor = true;
            // 
            // buttonInstallMod
            // 
            this.buttonInstallMod.Location = new System.Drawing.Point(7, 318);
            this.buttonInstallMod.Name = "buttonInstallMod";
            this.buttonInstallMod.Size = new System.Drawing.Size(125, 23);
            this.buttonInstallMod.TabIndex = 1;
            this.buttonInstallMod.Text = "Install Mod...";
            this.buttonInstallMod.UseVisualStyleBackColor = true;
            this.buttonInstallMod.Click += new System.EventHandler(this.buttonInstallMod_Click);
            // 
            // groupModInfo
            // 
            this.groupModInfo.Controls.Add(this.button1);
            this.groupModInfo.Controls.Add(this.panel1);
            this.groupModInfo.Controls.Add(this.buttonUninstallMod);
            this.groupModInfo.Location = new System.Drawing.Point(140, 0);
            this.groupModInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupModInfo.Name = "groupModInfo";
            this.groupModInfo.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupModInfo.Size = new System.Drawing.Size(294, 341);
            this.groupModInfo.TabIndex = 1;
            this.groupModInfo.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 311);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(197, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "THE TEST BUTTON";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelModWebsite);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.labelModVersion);
            this.panel1.Controls.Add(this.labelModAuthor);
            this.panel1.Controls.Add(this.labelModName);
            this.panel1.Location = new System.Drawing.Point(6, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(282, 292);
            this.panel1.TabIndex = 6;
            // 
            // labelModWebsite
            // 
            this.labelModWebsite.AutoSize = true;
            this.labelModWebsite.Location = new System.Drawing.Point(1, 48);
            this.labelModWebsite.Name = "labelModWebsite";
            this.labelModWebsite.Size = new System.Drawing.Size(240, 15);
            this.labelModWebsite.TabIndex = 9;
            this.labelModWebsite.TabStop = true;
            this.labelModWebsite.Text = "http://mod.website.com/path/to/mod.html";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(0, 76);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(282, 213);
            this.textBox1.TabIndex = 8;
            // 
            // labelModVersion
            // 
            this.labelModVersion.AutoSize = true;
            this.labelModVersion.Location = new System.Drawing.Point(1, 33);
            this.labelModVersion.Name = "labelModVersion";
            this.labelModVersion.Size = new System.Drawing.Size(37, 15);
            this.labelModVersion.TabIndex = 7;
            this.labelModVersion.Text = "v1234";
            // 
            // labelModAuthor
            // 
            this.labelModAuthor.AutoSize = true;
            this.labelModAuthor.Location = new System.Drawing.Point(45, 33);
            this.labelModAuthor.Name = "labelModAuthor";
            this.labelModAuthor.Size = new System.Drawing.Size(60, 15);
            this.labelModAuthor.TabIndex = 6;
            this.labelModAuthor.Text = "by Author";
            // 
            // labelModName
            // 
            this.labelModName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelModName.Location = new System.Drawing.Point(0, 4);
            this.labelModName.Name = "labelModName";
            this.labelModName.Size = new System.Drawing.Size(282, 23);
            this.labelModName.TabIndex = 5;
            this.labelModName.Text = "Mod Title Here";
            // 
            // buttonUninstallMod
            // 
            this.buttonUninstallMod.Location = new System.Drawing.Point(213, 311);
            this.buttonUninstallMod.Name = "buttonUninstallMod";
            this.buttonUninstallMod.Size = new System.Drawing.Size(75, 23);
            this.buttonUninstallMod.TabIndex = 5;
            this.buttonUninstallMod.Text = "Uninstall";
            this.buttonUninstallMod.UseVisualStyleBackColor = true;
            this.buttonUninstallMod.Click += new System.EventHandler(this.buttonUninstallMod_Click);
            // 
            // listInstalledMods
            // 
            this.listInstalledMods.FormattingEnabled = true;
            this.listInstalledMods.ItemHeight = 15;
            this.listInstalledMods.Location = new System.Drawing.Point(7, 7);
            this.listInstalledMods.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listInstalledMods.Name = "listInstalledMods";
            this.listInstalledMods.Size = new System.Drawing.Size(125, 304);
            this.listInstalledMods.TabIndex = 0;
            this.listInstalledMods.SelectedIndexChanged += new System.EventHandler(this.listInstalledMods_SelectedIndexChanged);
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.buttonBuildGameDB);
            this.tabSettings.Controls.Add(this.checkConflicts);
            this.tabSettings.Controls.Add(this.groupBox1);
            this.tabSettings.Location = new System.Drawing.Point(4, 24);
            this.tabSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabSettings.Size = new System.Drawing.Size(442, 353);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // buttonBuildGameDB
            // 
            this.buttonBuildGameDB.Location = new System.Drawing.Point(287, 65);
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
            this.checkConflicts.Location = new System.Drawing.Point(12, 68);
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
            this.groupBox1.Size = new System.Drawing.Size(430, 52);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MGSV Installation";
            // 
            // buttonFindMGSV
            // 
            this.buttonFindMGSV.Location = new System.Drawing.Point(399, 22);
            this.buttonFindMGSV.Name = "buttonFindMGSV";
            this.buttonFindMGSV.Size = new System.Drawing.Size(25, 23);
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
            this.textInstallPath.Size = new System.Drawing.Size(387, 23);
            this.textInstallPath.TabIndex = 1;
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 408);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "formMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.formMain_Load);
            this.tabControl.ResumeLayout(false);
            this.tabInstalledMods.ResumeLayout(false);
            this.groupModInfo.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel labelModWebsite;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label labelModVersion;
        private System.Windows.Forms.Label labelModAuthor;
        private System.Windows.Forms.Label labelModName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonFindMGSV;
        private System.Windows.Forms.TextBox textInstallPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonBuildGameDB;
        private System.Windows.Forms.CheckBox checkConflicts;
    }
}

