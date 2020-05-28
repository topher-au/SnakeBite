namespace SnakeBite.SetupWizard
{
    partial class CreateBackupPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelContent = new System.Windows.Forms.Panel();
            this.labelNoInstallNote = new System.Windows.Forms.Label();
            this.panelProcessing = new System.Windows.Forms.Panel();
            this.labelWorking = new System.Windows.Forms.Label();
            this.labelSelectDir = new System.Windows.Forms.Label();
            this.labelHeader = new System.Windows.Forms.Label();
            this.panelContent.SuspendLayout();
            this.panelProcessing.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.labelNoInstallNote);
            this.panelContent.Controls.Add(this.panelProcessing);
            this.panelContent.Controls.Add(this.labelSelectDir);
            this.panelContent.Controls.Add(this.labelHeader);
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(440, 340);
            this.panelContent.TabIndex = 4;
            // 
            // labelNoInstallNote
            // 
            this.labelNoInstallNote.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoInstallNote.Location = new System.Drawing.Point(3, 75);
            this.labelNoInstallNote.Name = "labelNoInstallNote";
            this.labelNoInstallNote.Size = new System.Drawing.Size(434, 61);
            this.labelNoInstallNote.TabIndex = 8;
            this.labelNoInstallNote.Text = "If you do not create backup files, you will not be able to use the \"Toggle Mods\" " +
    "feature or restore your game files to their original state.\r\n";
            this.labelNoInstallNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelProcessing
            // 
            this.panelProcessing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelProcessing.Controls.Add(this.labelWorking);
            this.panelProcessing.Location = new System.Drawing.Point(3, 161);
            this.panelProcessing.Name = "panelProcessing";
            this.panelProcessing.Size = new System.Drawing.Size(434, 43);
            this.panelProcessing.TabIndex = 7;
            // 
            // labelWorking
            // 
            this.labelWorking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelWorking.Location = new System.Drawing.Point(0, 0);
            this.labelWorking.Name = "labelWorking";
            this.labelWorking.Size = new System.Drawing.Size(434, 43);
            this.labelWorking.TabIndex = 7;
            this.labelWorking.Text = "Backup in Progress...";
            this.labelWorking.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelSelectDir
            // 
            this.labelSelectDir.Location = new System.Drawing.Point(3, 51);
            this.labelSelectDir.Name = "labelSelectDir";
            this.labelSelectDir.Size = new System.Drawing.Size(434, 24);
            this.labelSelectDir.TabIndex = 5;
            this.labelSelectDir.Text = "Next, some game files need to be backed up in case they are required later.";
            this.labelSelectDir.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.Location = new System.Drawing.Point(3, 0);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(218, 30);
            this.labelHeader.TabIndex = 4;
            this.labelHeader.Text = "Backup existing data";
            // 
            // CreateBackupPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContent);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CreateBackupPage";
            this.Size = new System.Drawing.Size(440, 340);
            this.panelContent.ResumeLayout(false);
            this.panelContent.PerformLayout();
            this.panelProcessing.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label labelSelectDir;
        private System.Windows.Forms.Label labelHeader;
        public System.Windows.Forms.Label labelWorking;
        public System.Windows.Forms.Panel panelProcessing;
        private System.Windows.Forms.Label labelNoInstallNote;
    }
}
