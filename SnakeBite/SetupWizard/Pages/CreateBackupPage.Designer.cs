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
            this.panelProcessing = new System.Windows.Forms.Panel();
            this.labelWorking = new System.Windows.Forms.Label();
            this.pictureWorkingSpiral = new System.Windows.Forms.PictureBox();
            this.labelSelectDir = new System.Windows.Forms.Label();
            this.labelHeader = new System.Windows.Forms.Label();
            this.labelNoInstallNote = new System.Windows.Forms.Label();
            this.panelContent.SuspendLayout();
            this.panelProcessing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureWorkingSpiral)).BeginInit();
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
            // panelProcessing
            // 
            this.panelProcessing.Controls.Add(this.labelWorking);
            this.panelProcessing.Controls.Add(this.pictureWorkingSpiral);
            this.panelProcessing.Location = new System.Drawing.Point(114, 160);
            this.panelProcessing.Name = "panelProcessing";
            this.panelProcessing.Size = new System.Drawing.Size(221, 43);
            this.panelProcessing.TabIndex = 7;
            // 
            // labelWorking
            // 
            this.labelWorking.AutoSize = true;
            this.labelWorking.Location = new System.Drawing.Point(43, 14);
            this.labelWorking.Name = "labelWorking";
            this.labelWorking.Size = new System.Drawing.Size(169, 15);
            this.labelWorking.TabIndex = 7;
            this.labelWorking.Text = "Your backup is being created...";
            // 
            // pictureWorkingSpiral
            // 
            this.pictureWorkingSpiral.Image = global::SnakeBite.Properties.Resources.loading_spiral;
            this.pictureWorkingSpiral.Location = new System.Drawing.Point(3, 5);
            this.pictureWorkingSpiral.Name = "pictureWorkingSpiral";
            this.pictureWorkingSpiral.Size = new System.Drawing.Size(32, 32);
            this.pictureWorkingSpiral.TabIndex = 6;
            this.pictureWorkingSpiral.TabStop = false;
            // 
            // labelSelectDir
            // 
            this.labelSelectDir.Location = new System.Drawing.Point(5, 51);
            this.labelSelectDir.Name = "labelSelectDir";
            this.labelSelectDir.Size = new System.Drawing.Size(432, 24);
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
            // labelNoInstallNote
            // 
            this.labelNoInstallNote.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoInstallNote.Location = new System.Drawing.Point(38, 75);
            this.labelNoInstallNote.Name = "labelNoInstallNote";
            this.labelNoInstallNote.Size = new System.Drawing.Size(364, 68);
            this.labelNoInstallNote.TabIndex = 8;
            this.labelNoInstallNote.Text = "Note:\r\nIf you do not create a backup file, you will not be able to use the Toggle" +
    " feature or restore your game files to their default state.\r\n";
            this.labelNoInstallNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.panelProcessing.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureWorkingSpiral)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label labelSelectDir;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.Label labelWorking;
        private System.Windows.Forms.PictureBox pictureWorkingSpiral;
        public System.Windows.Forms.Panel panelProcessing;
        private System.Windows.Forms.Label labelNoInstallNote;
    }
}
