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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabInstalledMods = new System.Windows.Forms.TabPage();
            this.buttonInstallMod = new System.Windows.Forms.Button();
            this.listInstalledMods = new System.Windows.Forms.ListBox();
            this.groupModInfo = new System.Windows.Forms.GroupBox();
            this.panelModDetails = new System.Windows.Forms.Panel();
            this.labelModWebsite = new System.Windows.Forms.LinkLabel();
            this.buttonUninstallMod = new System.Windows.Forms.Button();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.labelModVersion = new System.Windows.Forms.Label();
            this.labelModAuthor = new System.Windows.Forms.Label();
            this.labelModName = new System.Windows.Forms.Label();
            this.labelNoMods = new System.Windows.Forms.Label();
            this.tabPageDownloadMods = new System.Windows.Forms.TabPage();
            this.listWebMods = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelWebMod = new System.Windows.Forms.Panel();
            this.buttonWebRemove = new System.Windows.Forms.Button();
            this.labelModAd = new System.Windows.Forms.Label();
            this.labelWebWebsite = new System.Windows.Forms.LinkLabel();
            this.buttonWebInstall = new System.Windows.Forms.Button();
            this.textWebDescription = new System.Windows.Forms.TextBox();
            this.labelWebVersion = new System.Windows.Forms.Label();
            this.labelWebAuthor = new System.Windows.Forms.Label();
            this.labelWebName = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabInstalledMods.SuspendLayout();
            this.groupModInfo.SuspendLayout();
            this.panelModDetails.SuspendLayout();
            this.tabPageDownloadMods.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelWebMod.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabInstalledMods);
            this.tabControl.Controls.Add(this.tabPageDownloadMods);
            this.tabControl.Location = new System.Drawing.Point(14, 14);
            this.tabControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(578, 438);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabInstalledMods
            // 
            this.tabInstalledMods.Controls.Add(this.buttonInstallMod);
            this.tabInstalledMods.Controls.Add(this.listInstalledMods);
            this.tabInstalledMods.Controls.Add(this.groupModInfo);
            this.tabInstalledMods.Location = new System.Drawing.Point(4, 24);
            this.tabInstalledMods.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabInstalledMods.Name = "tabInstalledMods";
            this.tabInstalledMods.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabInstalledMods.Size = new System.Drawing.Size(570, 410);
            this.tabInstalledMods.TabIndex = 0;
            this.tabInstalledMods.Text = "Installed Mods";
            this.tabInstalledMods.UseVisualStyleBackColor = true;
            // 
            // buttonInstallMod
            // 
            this.buttonInstallMod.Location = new System.Drawing.Point(3, 382);
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
            this.listInstalledMods.IntegralHeight = false;
            this.listInstalledMods.ItemHeight = 15;
            this.listInstalledMods.Location = new System.Drawing.Point(3, 12);
            this.listInstalledMods.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listInstalledMods.Name = "listInstalledMods";
            this.listInstalledMods.Size = new System.Drawing.Size(187, 368);
            this.listInstalledMods.TabIndex = 0;
            this.listInstalledMods.SelectedIndexChanged += new System.EventHandler(this.listInstalledMods_SelectedIndexChanged);
            // 
            // groupModInfo
            // 
            this.groupModInfo.Controls.Add(this.panelModDetails);
            this.groupModInfo.Controls.Add(this.labelNoMods);
            this.groupModInfo.Location = new System.Drawing.Point(196, 4);
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
            this.labelModWebsite.Location = new System.Drawing.Point(5, 32);
            this.labelModWebsite.Name = "labelModWebsite";
            this.labelModWebsite.Size = new System.Drawing.Size(240, 15);
            this.labelModWebsite.TabIndex = 9;
            this.labelModWebsite.TabStop = true;
            this.labelModWebsite.Text = "http://mod.website.com/path/to/mod.html";
            this.labelModWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelModWebsite_LinkClicked);
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
            this.textDescription.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textDescription.Location = new System.Drawing.Point(8, 61);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.ReadOnly = true;
            this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDescription.Size = new System.Drawing.Size(343, 288);
            this.textDescription.TabIndex = 8;
            // 
            // labelModVersion
            // 
            this.labelModVersion.AutoSize = true;
            this.labelModVersion.Location = new System.Drawing.Point(5, 359);
            this.labelModVersion.Name = "labelModVersion";
            this.labelModVersion.Size = new System.Drawing.Size(37, 15);
            this.labelModVersion.TabIndex = 7;
            this.labelModVersion.Text = "v1234";
            // 
            // labelModAuthor
            // 
            this.labelModAuthor.AutoSize = true;
            this.labelModAuthor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelModAuthor.Location = new System.Drawing.Point(154, 15);
            this.labelModAuthor.Name = "labelModAuthor";
            this.labelModAuthor.Size = new System.Drawing.Size(59, 15);
            this.labelModAuthor.TabIndex = 6;
            this.labelModAuthor.Text = "by Author";
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
            // labelNoMods
            // 
            this.labelNoMods.AutoSize = true;
            this.labelNoMods.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoMods.Location = new System.Drawing.Point(95, 178);
            this.labelNoMods.Name = "labelNoMods";
            this.labelNoMods.Size = new System.Drawing.Size(192, 30);
            this.labelNoMods.TabIndex = 2;
            this.labelNoMods.Text = "No mods installed";
            // 
            // tabPageDownloadMods
            // 
            this.tabPageDownloadMods.Controls.Add(this.listWebMods);
            this.tabPageDownloadMods.Controls.Add(this.groupBox1);
            this.tabPageDownloadMods.Location = new System.Drawing.Point(4, 24);
            this.tabPageDownloadMods.Name = "tabPageDownloadMods";
            this.tabPageDownloadMods.Size = new System.Drawing.Size(570, 410);
            this.tabPageDownloadMods.TabIndex = 2;
            this.tabPageDownloadMods.Text = "Download Mods";
            this.tabPageDownloadMods.UseVisualStyleBackColor = true;
            // 
            // listWebMods
            // 
            this.listWebMods.FormattingEnabled = true;
            this.listWebMods.IntegralHeight = false;
            this.listWebMods.ItemHeight = 15;
            this.listWebMods.Location = new System.Drawing.Point(3, 12);
            this.listWebMods.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listWebMods.Name = "listWebMods";
            this.listWebMods.Size = new System.Drawing.Size(187, 392);
            this.listWebMods.TabIndex = 2;
            this.listWebMods.SelectedIndexChanged += new System.EventHandler(this.listWebMods_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelWebMod);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(196, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(367, 401);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // panelWebMod
            // 
            this.panelWebMod.Controls.Add(this.buttonWebRemove);
            this.panelWebMod.Controls.Add(this.labelModAd);
            this.panelWebMod.Controls.Add(this.labelWebWebsite);
            this.panelWebMod.Controls.Add(this.buttonWebInstall);
            this.panelWebMod.Controls.Add(this.textWebDescription);
            this.panelWebMod.Controls.Add(this.labelWebVersion);
            this.panelWebMod.Controls.Add(this.labelWebAuthor);
            this.panelWebMod.Controls.Add(this.labelWebName);
            this.panelWebMod.Location = new System.Drawing.Point(6, 13);
            this.panelWebMod.Name = "panelWebMod";
            this.panelWebMod.Size = new System.Drawing.Size(354, 381);
            this.panelWebMod.TabIndex = 6;
            // 
            // buttonWebRemove
            // 
            this.buttonWebRemove.Location = new System.Drawing.Point(190, 340);
            this.buttonWebRemove.Name = "buttonWebRemove";
            this.buttonWebRemove.Size = new System.Drawing.Size(62, 23);
            this.buttonWebRemove.TabIndex = 11;
            this.buttonWebRemove.Text = "Delete";
            this.buttonWebRemove.UseVisualStyleBackColor = true;
            this.buttonWebRemove.Click += new System.EventHandler(this.buttonWebRemove_Click);
            // 
            // labelModAd
            // 
            this.labelModAd.Enabled = false;
            this.labelModAd.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelModAd.Location = new System.Drawing.Point(0, 366);
            this.labelModAd.Name = "labelModAd";
            this.labelModAd.Size = new System.Drawing.Size(354, 15);
            this.labelModAd.TabIndex = 10;
            this.labelModAd.Text = "Want your mod listed here? Contact me!";
            this.labelModAd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelWebWebsite
            // 
            this.labelWebWebsite.AutoSize = true;
            this.labelWebWebsite.Location = new System.Drawing.Point(5, 32);
            this.labelWebWebsite.Name = "labelWebWebsite";
            this.labelWebWebsite.Size = new System.Drawing.Size(240, 15);
            this.labelWebWebsite.TabIndex = 9;
            this.labelWebWebsite.TabStop = true;
            this.labelWebWebsite.Text = "http://mod.website.com/path/to/mod.html";
            // 
            // buttonWebInstall
            // 
            this.buttonWebInstall.Location = new System.Drawing.Point(258, 340);
            this.buttonWebInstall.Name = "buttonWebInstall";
            this.buttonWebInstall.Size = new System.Drawing.Size(96, 23);
            this.buttonWebInstall.TabIndex = 5;
            this.buttonWebInstall.Text = "Install";
            this.buttonWebInstall.UseVisualStyleBackColor = true;
            this.buttonWebInstall.Click += new System.EventHandler(this.textWebInstall_Click);
            // 
            // textWebDescription
            // 
            this.textWebDescription.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textWebDescription.Location = new System.Drawing.Point(8, 61);
            this.textWebDescription.Multiline = true;
            this.textWebDescription.Name = "textWebDescription";
            this.textWebDescription.ReadOnly = true;
            this.textWebDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textWebDescription.Size = new System.Drawing.Size(343, 273);
            this.textWebDescription.TabIndex = 8;
            // 
            // labelWebVersion
            // 
            this.labelWebVersion.AutoSize = true;
            this.labelWebVersion.Location = new System.Drawing.Point(5, 344);
            this.labelWebVersion.Name = "labelWebVersion";
            this.labelWebVersion.Size = new System.Drawing.Size(37, 15);
            this.labelWebVersion.TabIndex = 7;
            this.labelWebVersion.Text = "v1234";
            // 
            // labelWebAuthor
            // 
            this.labelWebAuthor.AutoSize = true;
            this.labelWebAuthor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWebAuthor.Location = new System.Drawing.Point(154, 15);
            this.labelWebAuthor.Name = "labelWebAuthor";
            this.labelWebAuthor.Size = new System.Drawing.Size(59, 15);
            this.labelWebAuthor.TabIndex = 6;
            this.labelWebAuthor.Text = "by Author";
            // 
            // labelWebName
            // 
            this.labelWebName.AutoSize = true;
            this.labelWebName.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWebName.Location = new System.Drawing.Point(3, 7);
            this.labelWebName.Name = "labelWebName";
            this.labelWebName.Size = new System.Drawing.Size(145, 25);
            this.labelWebName.TabIndex = 5;
            this.labelWebName.Text = "Mod Title Here";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(95, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(192, 30);
            this.label5.TabIndex = 2;
            this.label5.Text = "No mods installed";
            // 
            // formMods
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 467);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formMods";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mod Manager";
            this.Load += new System.EventHandler(this.formMain_Load);
            this.tabControl.ResumeLayout(false);
            this.tabInstalledMods.ResumeLayout(false);
            this.groupModInfo.ResumeLayout(false);
            this.groupModInfo.PerformLayout();
            this.panelModDetails.ResumeLayout(false);
            this.panelModDetails.PerformLayout();
            this.tabPageDownloadMods.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelWebMod.ResumeLayout(false);
            this.panelWebMod.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabInstalledMods;
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
        private System.Windows.Forms.Label labelNoMods;
        private System.Windows.Forms.TabPage tabPageDownloadMods;
        private System.Windows.Forms.ListBox listWebMods;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panelWebMod;
        private System.Windows.Forms.LinkLabel labelWebWebsite;
        private System.Windows.Forms.Button buttonWebInstall;
        private System.Windows.Forms.TextBox textWebDescription;
        private System.Windows.Forms.Label labelWebVersion;
        private System.Windows.Forms.Label labelWebAuthor;
        private System.Windows.Forms.Label labelWebName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelModAd;
        private System.Windows.Forms.Button buttonWebRemove;
    }
}

