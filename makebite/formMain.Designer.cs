namespace makebite
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
            this.groupModInfo = new System.Windows.Forms.GroupBox();
            this.comboForVersion = new System.Windows.Forms.ComboBox();
            this.labelForVersion = new System.Windows.Forms.Label();
            this.buttonMetaLoad = new System.Windows.Forms.Button();
            this.buttonMetaSave = new System.Windows.Forms.Button();
            this.textModWebsite = new System.Windows.Forms.TextBox();
            this.textModAuthor = new System.Windows.Forms.TextBox();
            this.textModVersion = new System.Windows.Forms.TextBox();
            this.textModName = new System.Windows.Forms.TextBox();
            this.labelModDescription = new System.Windows.Forms.Label();
            this.textModDescription = new System.Windows.Forms.TextBox();
            this.labelModWebsite = new System.Windows.Forms.Label();
            this.labelModVersion = new System.Windows.Forms.Label();
            this.labelModAuthor = new System.Windows.Forms.Label();
            this.labelModName = new System.Windows.Forms.Label();
            this.groupFiles = new System.Windows.Forms.GroupBox();
            this.labelToggleHelp = new System.Windows.Forms.Label();
            this.buttonBuild = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listModFiles = new System.Windows.Forms.ListBox();
            this.buttonSelectPath = new System.Windows.Forms.Button();
            this.textModPath = new System.Windows.Forms.TextBox();
            this.groupModInfo.SuspendLayout();
            this.groupFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupModInfo
            // 
            this.groupModInfo.Controls.Add(this.comboForVersion);
            this.groupModInfo.Controls.Add(this.labelForVersion);
            this.groupModInfo.Controls.Add(this.buttonMetaLoad);
            this.groupModInfo.Controls.Add(this.buttonMetaSave);
            this.groupModInfo.Controls.Add(this.textModWebsite);
            this.groupModInfo.Controls.Add(this.textModAuthor);
            this.groupModInfo.Controls.Add(this.textModVersion);
            this.groupModInfo.Controls.Add(this.textModName);
            this.groupModInfo.Controls.Add(this.labelModDescription);
            this.groupModInfo.Controls.Add(this.textModDescription);
            this.groupModInfo.Controls.Add(this.labelModWebsite);
            this.groupModInfo.Controls.Add(this.labelModVersion);
            this.groupModInfo.Controls.Add(this.labelModAuthor);
            this.groupModInfo.Controls.Add(this.labelModName);
            this.groupModInfo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupModInfo.Location = new System.Drawing.Point(14, 14);
            this.groupModInfo.Name = "groupModInfo";
            this.groupModInfo.Size = new System.Drawing.Size(437, 483);
            this.groupModInfo.TabIndex = 1;
            this.groupModInfo.TabStop = false;
            this.groupModInfo.Text = "Mod Information";
            // 
            // comboForVersion
            // 
            this.comboForVersion.BackColor = System.Drawing.Color.DarkGray;
            this.comboForVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboForVersion.FormattingEnabled = true;
            this.comboForVersion.Items.AddRange(new object[] {
            "",
            Build.MGSVVersionStr});
            this.comboForVersion.Location = new System.Drawing.Point(348, 55);
            this.comboForVersion.Name = "comboForVersion";
            this.comboForVersion.Size = new System.Drawing.Size(83, 23);
            this.comboForVersion.TabIndex = 8;
            this.comboForVersion.SelectedIndexChanged += new System.EventHandler(this.comboForVersion_SelectedIndexChanged);
            // 
            // labelForVersion
            // 
            this.labelForVersion.AutoSize = true;
            this.labelForVersion.Location = new System.Drawing.Point(264, 58);
            this.labelForVersion.Name = "labelForVersion";
            this.labelForVersion.Size = new System.Drawing.Size(81, 15);
            this.labelForVersion.TabIndex = 7;
            this.labelForVersion.Text = "MGSV Version";
            // 
            // buttonMetaLoad
            // 
            this.buttonMetaLoad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonMetaLoad.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonMetaLoad.Location = new System.Drawing.Point(235, 449);
            this.buttonMetaLoad.Name = "buttonMetaLoad";
            this.buttonMetaLoad.Size = new System.Drawing.Size(95, 23);
            this.buttonMetaLoad.TabIndex = 6;
            this.buttonMetaLoad.Text = "Load Mod Info";
            this.buttonMetaLoad.UseVisualStyleBackColor = false;
            this.buttonMetaLoad.Click += new System.EventHandler(this.buttonMetaLoad_Click);
            // 
            // buttonMetaSave
            // 
            this.buttonMetaSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonMetaSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonMetaSave.Location = new System.Drawing.Point(336, 449);
            this.buttonMetaSave.Name = "buttonMetaSave";
            this.buttonMetaSave.Size = new System.Drawing.Size(95, 23);
            this.buttonMetaSave.TabIndex = 5;
            this.buttonMetaSave.Text = "Save Mod Info";
            this.buttonMetaSave.UseVisualStyleBackColor = false;
            this.buttonMetaSave.Click += new System.EventHandler(this.buttonMetaSave_Click);
            // 
            // textModWebsite
            // 
            this.textModWebsite.BackColor = System.Drawing.Color.DarkGray;
            this.textModWebsite.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textModWebsite.Location = new System.Drawing.Point(59, 84);
            this.textModWebsite.Name = "textModWebsite";
            this.textModWebsite.Size = new System.Drawing.Size(372, 23);
            this.textModWebsite.TabIndex = 3;
            // 
            // textModAuthor
            // 
            this.textModAuthor.BackColor = System.Drawing.Color.DarkGray;
            this.textModAuthor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textModAuthor.Location = new System.Drawing.Point(59, 55);
            this.textModAuthor.Name = "textModAuthor";
            this.textModAuthor.Size = new System.Drawing.Size(196, 23);
            this.textModAuthor.TabIndex = 2;
            // 
            // textModVersion
            // 
            this.textModVersion.BackColor = System.Drawing.Color.DarkGray;
            this.textModVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textModVersion.Location = new System.Drawing.Point(381, 26);
            this.textModVersion.Name = "textModVersion";
            this.textModVersion.Size = new System.Drawing.Size(50, 23);
            this.textModVersion.TabIndex = 1;
            // 
            // textModName
            // 
            this.textModName.BackColor = System.Drawing.Color.DarkGray;
            this.textModName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textModName.Location = new System.Drawing.Point(59, 26);
            this.textModName.Name = "textModName";
            this.textModName.Size = new System.Drawing.Size(268, 23);
            this.textModName.TabIndex = 0;
            // 
            // labelModDescription
            // 
            this.labelModDescription.AutoSize = true;
            this.labelModDescription.Location = new System.Drawing.Point(9, 115);
            this.labelModDescription.Name = "labelModDescription";
            this.labelModDescription.Size = new System.Drawing.Size(95, 15);
            this.labelModDescription.TabIndex = 5;
            this.labelModDescription.Text = "Mod Description";
            // 
            // textModDescription
            // 
            this.textModDescription.BackColor = System.Drawing.Color.DarkGray;
            this.textModDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textModDescription.Location = new System.Drawing.Point(12, 133);
            this.textModDescription.Multiline = true;
            this.textModDescription.Name = "textModDescription";
            this.textModDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textModDescription.Size = new System.Drawing.Size(419, 310);
            this.textModDescription.TabIndex = 4;
            // 
            // labelModWebsite
            // 
            this.labelModWebsite.AutoSize = true;
            this.labelModWebsite.Location = new System.Drawing.Point(9, 87);
            this.labelModWebsite.Name = "labelModWebsite";
            this.labelModWebsite.Size = new System.Drawing.Size(49, 15);
            this.labelModWebsite.TabIndex = 3;
            this.labelModWebsite.Text = "Website";
            // 
            // labelModVersion
            // 
            this.labelModVersion.AutoSize = true;
            this.labelModVersion.Location = new System.Drawing.Point(333, 29);
            this.labelModVersion.Name = "labelModVersion";
            this.labelModVersion.Size = new System.Drawing.Size(46, 15);
            this.labelModVersion.TabIndex = 2;
            this.labelModVersion.Text = "Version";
            // 
            // labelModAuthor
            // 
            this.labelModAuthor.AutoSize = true;
            this.labelModAuthor.Location = new System.Drawing.Point(9, 58);
            this.labelModAuthor.Name = "labelModAuthor";
            this.labelModAuthor.Size = new System.Drawing.Size(44, 15);
            this.labelModAuthor.TabIndex = 1;
            this.labelModAuthor.Text = "Author";
            // 
            // labelModName
            // 
            this.labelModName.AutoSize = true;
            this.labelModName.Location = new System.Drawing.Point(9, 29);
            this.labelModName.Name = "labelModName";
            this.labelModName.Size = new System.Drawing.Size(39, 15);
            this.labelModName.TabIndex = 0;
            this.labelModName.Text = "Name";
            // 
            // groupFiles
            // 
            this.groupFiles.Controls.Add(this.labelToggleHelp);
            this.groupFiles.Controls.Add(this.buttonBuild);
            this.groupFiles.Controls.Add(this.label1);
            this.groupFiles.Controls.Add(this.listModFiles);
            this.groupFiles.Controls.Add(this.buttonSelectPath);
            this.groupFiles.Controls.Add(this.textModPath);
            this.groupFiles.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupFiles.Location = new System.Drawing.Point(457, 14);
            this.groupFiles.Name = "groupFiles";
            this.groupFiles.Size = new System.Drawing.Size(621, 483);
            this.groupFiles.TabIndex = 2;
            this.groupFiles.TabStop = false;
            this.groupFiles.Text = "Mod Files";
            // 
            // labelToggleHelp
            // 
            this.labelToggleHelp.Cursor = System.Windows.Forms.Cursors.Help;
            this.labelToggleHelp.ForeColor = System.Drawing.Color.MidnightBlue;
            this.labelToggleHelp.Location = new System.Drawing.Point(12, 453);
            this.labelToggleHelp.Name = "labelToggleHelp";
            this.labelToggleHelp.Size = new System.Drawing.Size(98, 19);
            this.labelToggleHelp.TabIndex = 11;
            this.labelToggleHelp.Text = "Formatting Help";
            this.labelToggleHelp.Click += new System.EventHandler(this.labelToggleHelp_Click);
            // 
            // buttonBuild
            // 
            this.buttonBuild.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonBuild.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonBuild.Location = new System.Drawing.Point(481, 449);
            this.buttonBuild.Name = "buttonBuild";
            this.buttonBuild.Size = new System.Drawing.Size(134, 23);
            this.buttonBuild.TabIndex = 10;
            this.buttonBuild.Text = "Do it (build archive)";
            this.buttonBuild.UseVisualStyleBackColor = false;
            this.buttonBuild.Click += new System.EventHandler(this.buttonBuild_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(17, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(595, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "Note: MakeBite will automatically repack any _fpk and _fpkd folders, overwriting " +
    "the existing .fpk and .fpkd files.";
            // 
            // listModFiles
            // 
            this.listModFiles.BackColor = System.Drawing.Color.DarkGray;
            this.listModFiles.FormattingEnabled = true;
            this.listModFiles.ItemHeight = 15;
            this.listModFiles.Location = new System.Drawing.Point(15, 66);
            this.listModFiles.Name = "listModFiles";
            this.listModFiles.Size = new System.Drawing.Size(600, 379);
            this.listModFiles.TabIndex = 9;
            this.listModFiles.SelectedIndexChanged += new System.EventHandler(this.listModFiles_SelectedIndexChanged);
            // 
            // buttonSelectPath
            // 
            this.buttonSelectPath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonSelectPath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSelectPath.Location = new System.Drawing.Point(582, 20);
            this.buttonSelectPath.Name = "buttonSelectPath";
            this.buttonSelectPath.Size = new System.Drawing.Size(31, 23);
            this.buttonSelectPath.TabIndex = 8;
            this.buttonSelectPath.Text = "...";
            this.buttonSelectPath.UseVisualStyleBackColor = false;
            this.buttonSelectPath.Click += new System.EventHandler(this.buttonSelectPath_Click);
            // 
            // textModPath
            // 
            this.textModPath.BackColor = System.Drawing.Color.DarkGray;
            this.textModPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textModPath.Enabled = false;
            this.textModPath.Location = new System.Drawing.Point(15, 20);
            this.textModPath.Name = "textModPath";
            this.textModPath.Size = new System.Drawing.Size(563, 23);
            this.textModPath.TabIndex = 7;
            this.textModPath.Text = "Please select a directory containing your mod files";
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(1090, 508);
            this.Controls.Add(this.groupFiles);
            this.Controls.Add(this.groupModInfo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MakeBite";
            this.Load += new System.EventHandler(this.formMain_Load);
            this.groupModInfo.ResumeLayout(false);
            this.groupModInfo.PerformLayout();
            this.groupFiles.ResumeLayout(false);
            this.groupFiles.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupModInfo;
        private System.Windows.Forms.TextBox textModWebsite;
        private System.Windows.Forms.TextBox textModAuthor;
        private System.Windows.Forms.TextBox textModVersion;
        private System.Windows.Forms.TextBox textModName;
        private System.Windows.Forms.Label labelModDescription;
        private System.Windows.Forms.TextBox textModDescription;
        private System.Windows.Forms.Label labelModWebsite;
        private System.Windows.Forms.Label labelModVersion;
        private System.Windows.Forms.Label labelModAuthor;
        private System.Windows.Forms.Label labelModName;
        private System.Windows.Forms.GroupBox groupFiles;
        private System.Windows.Forms.ListBox listModFiles;
        private System.Windows.Forms.Button buttonSelectPath;
        private System.Windows.Forms.TextBox textModPath;
        private System.Windows.Forms.Button buttonBuild;
        private System.Windows.Forms.Button buttonMetaLoad;
        private System.Windows.Forms.Button buttonMetaSave;
        private System.Windows.Forms.ComboBox comboForVersion;
        private System.Windows.Forms.Label labelForVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelToggleHelp;
    }
}

