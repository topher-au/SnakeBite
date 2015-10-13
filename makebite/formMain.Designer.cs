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
            this.groupModInfo = new System.Windows.Forms.GroupBox();
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
            this.listModFiles = new System.Windows.Forms.ListBox();
            this.buttonSelectPath = new System.Windows.Forms.Button();
            this.textModPath = new System.Windows.Forms.TextBox();
            this.buttonBuild = new System.Windows.Forms.Button();
            this.groupModInfo.SuspendLayout();
            this.groupFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupModInfo
            // 
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
            this.groupModInfo.Location = new System.Drawing.Point(14, 14);
            this.groupModInfo.Name = "groupModInfo";
            this.groupModInfo.Size = new System.Drawing.Size(275, 405);
            this.groupModInfo.TabIndex = 1;
            this.groupModInfo.TabStop = false;
            this.groupModInfo.Text = "Mod Information";
            // 
            // buttonMetaLoad
            // 
            this.buttonMetaLoad.Location = new System.Drawing.Point(23, 367);
            this.buttonMetaLoad.Name = "buttonMetaLoad";
            this.buttonMetaLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonMetaLoad.TabIndex = 6;
            this.buttonMetaLoad.Text = "Load";
            this.buttonMetaLoad.UseVisualStyleBackColor = true;
            this.buttonMetaLoad.Click += new System.EventHandler(this.buttonMetaLoad_Click);
            // 
            // buttonMetaSave
            // 
            this.buttonMetaSave.Location = new System.Drawing.Point(174, 367);
            this.buttonMetaSave.Name = "buttonMetaSave";
            this.buttonMetaSave.Size = new System.Drawing.Size(75, 23);
            this.buttonMetaSave.TabIndex = 5;
            this.buttonMetaSave.Text = "Save";
            this.buttonMetaSave.UseVisualStyleBackColor = true;
            this.buttonMetaSave.Click += new System.EventHandler(this.buttonMetaSave_Click);
            // 
            // textModWebsite
            // 
            this.textModWebsite.Location = new System.Drawing.Point(99, 117);
            this.textModWebsite.Name = "textModWebsite";
            this.textModWebsite.Size = new System.Drawing.Size(150, 23);
            this.textModWebsite.TabIndex = 3;
            // 
            // textModAuthor
            // 
            this.textModAuthor.Location = new System.Drawing.Point(99, 87);
            this.textModAuthor.Name = "textModAuthor";
            this.textModAuthor.Size = new System.Drawing.Size(150, 23);
            this.textModAuthor.TabIndex = 2;
            // 
            // textModVersion
            // 
            this.textModVersion.Location = new System.Drawing.Point(99, 57);
            this.textModVersion.Name = "textModVersion";
            this.textModVersion.Size = new System.Drawing.Size(150, 23);
            this.textModVersion.TabIndex = 1;
            // 
            // textModName
            // 
            this.textModName.Location = new System.Drawing.Point(99, 27);
            this.textModName.Name = "textModName";
            this.textModName.Size = new System.Drawing.Size(150, 23);
            this.textModName.TabIndex = 0;
            // 
            // labelModDescription
            // 
            this.labelModDescription.AutoSize = true;
            this.labelModDescription.Location = new System.Drawing.Point(20, 147);
            this.labelModDescription.Name = "labelModDescription";
            this.labelModDescription.Size = new System.Drawing.Size(67, 15);
            this.labelModDescription.TabIndex = 5;
            this.labelModDescription.Text = "Description";
            // 
            // textModDescription
            // 
            this.textModDescription.Location = new System.Drawing.Point(23, 165);
            this.textModDescription.Multiline = true;
            this.textModDescription.Name = "textModDescription";
            this.textModDescription.Size = new System.Drawing.Size(226, 196);
            this.textModDescription.TabIndex = 4;
            // 
            // labelModWebsite
            // 
            this.labelModWebsite.AutoSize = true;
            this.labelModWebsite.Location = new System.Drawing.Point(20, 120);
            this.labelModWebsite.Name = "labelModWebsite";
            this.labelModWebsite.Size = new System.Drawing.Size(49, 15);
            this.labelModWebsite.TabIndex = 3;
            this.labelModWebsite.Text = "Website";
            // 
            // labelModVersion
            // 
            this.labelModVersion.AutoSize = true;
            this.labelModVersion.Location = new System.Drawing.Point(20, 60);
            this.labelModVersion.Name = "labelModVersion";
            this.labelModVersion.Size = new System.Drawing.Size(45, 15);
            this.labelModVersion.TabIndex = 2;
            this.labelModVersion.Text = "Version";
            // 
            // labelModAuthor
            // 
            this.labelModAuthor.AutoSize = true;
            this.labelModAuthor.Location = new System.Drawing.Point(20, 90);
            this.labelModAuthor.Name = "labelModAuthor";
            this.labelModAuthor.Size = new System.Drawing.Size(44, 15);
            this.labelModAuthor.TabIndex = 1;
            this.labelModAuthor.Text = "Author";
            // 
            // labelModName
            // 
            this.labelModName.AutoSize = true;
            this.labelModName.Location = new System.Drawing.Point(20, 30);
            this.labelModName.Name = "labelModName";
            this.labelModName.Size = new System.Drawing.Size(39, 15);
            this.labelModName.TabIndex = 0;
            this.labelModName.Text = "Name";
            // 
            // groupFiles
            // 
            this.groupFiles.Controls.Add(this.listModFiles);
            this.groupFiles.Controls.Add(this.buttonSelectPath);
            this.groupFiles.Controls.Add(this.textModPath);
            this.groupFiles.Location = new System.Drawing.Point(295, 14);
            this.groupFiles.Name = "groupFiles";
            this.groupFiles.Size = new System.Drawing.Size(440, 376);
            this.groupFiles.TabIndex = 2;
            this.groupFiles.TabStop = false;
            this.groupFiles.Text = "Mod Files";
            // 
            // listModFiles
            // 
            this.listModFiles.FormattingEnabled = true;
            this.listModFiles.ItemHeight = 15;
            this.listModFiles.Location = new System.Drawing.Point(15, 57);
            this.listModFiles.Name = "listModFiles";
            this.listModFiles.Size = new System.Drawing.Size(408, 304);
            this.listModFiles.TabIndex = 9;
            this.listModFiles.SelectedIndexChanged += new System.EventHandler(this.listModFiles_SelectedIndexChanged);
            // 
            // buttonSelectPath
            // 
            this.buttonSelectPath.Location = new System.Drawing.Point(392, 22);
            this.buttonSelectPath.Name = "buttonSelectPath";
            this.buttonSelectPath.Size = new System.Drawing.Size(31, 23);
            this.buttonSelectPath.TabIndex = 8;
            this.buttonSelectPath.Text = "...";
            this.buttonSelectPath.UseVisualStyleBackColor = true;
            this.buttonSelectPath.Click += new System.EventHandler(this.buttonSelectPath_Click);
            // 
            // textModPath
            // 
            this.textModPath.Enabled = false;
            this.textModPath.Location = new System.Drawing.Point(15, 22);
            this.textModPath.Name = "textModPath";
            this.textModPath.Size = new System.Drawing.Size(371, 23);
            this.textModPath.TabIndex = 7;
            this.textModPath.Text = "Please select a directory containing your mod files";
            // 
            // buttonBuild
            // 
            this.buttonBuild.Location = new System.Drawing.Point(601, 396);
            this.buttonBuild.Name = "buttonBuild";
            this.buttonBuild.Size = new System.Drawing.Size(134, 23);
            this.buttonBuild.TabIndex = 10;
            this.buttonBuild.Text = "Do it (build archive)";
            this.buttonBuild.UseVisualStyleBackColor = true;
            this.buttonBuild.Click += new System.EventHandler(this.buttonBuild_Click);
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 428);
            this.Controls.Add(this.buttonBuild);
            this.Controls.Add(this.groupFiles);
            this.Controls.Add(this.groupModInfo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MakeBite";
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
    }
}

