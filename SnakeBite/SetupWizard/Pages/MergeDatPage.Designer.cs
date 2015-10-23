namespace SnakeBite.SetupWizard
{
    partial class MergeDatPage
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
            this.labelWelcomeText = new System.Windows.Forms.Label();
            this.labelWelcome = new System.Windows.Forms.Label();
            this.panelProcessing = new System.Windows.Forms.Panel();
            this.labelWorking = new System.Windows.Forms.Label();
            this.pictureWorkingSpiral = new System.Windows.Forms.PictureBox();
            this.panelContent.SuspendLayout();
            this.panelProcessing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureWorkingSpiral)).BeginInit();
            this.SuspendLayout();
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.labelWelcomeText);
            this.panelContent.Controls.Add(this.labelWelcome);
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(440, 340);
            this.panelContent.TabIndex = 3;
            // 
            // labelWelcomeText
            // 
            this.labelWelcomeText.Location = new System.Drawing.Point(5, 51);
            this.labelWelcomeText.Name = "labelWelcomeText";
            this.labelWelcomeText.Size = new System.Drawing.Size(429, 256);
            this.labelWelcomeText.TabIndex = 5;
            this.labelWelcomeText.Text = "For optimum performance, SnakeBite needs to make some changes to your game files." +
    " This may take some time.";
            // 
            // labelWelcome
            // 
            this.labelWelcome.AutoSize = true;
            this.labelWelcome.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWelcome.Location = new System.Drawing.Point(3, 0);
            this.labelWelcome.Name = "labelWelcome";
            this.labelWelcome.Size = new System.Drawing.Size(139, 30);
            this.labelWelcome.TabIndex = 4;
            this.labelWelcome.Text = "Almost done";
            // 
            // panelProcessing
            // 
            this.panelProcessing.Controls.Add(this.labelWorking);
            this.panelProcessing.Controls.Add(this.pictureWorkingSpiral);
            this.panelProcessing.Location = new System.Drawing.Point(96, 160);
            this.panelProcessing.Name = "panelProcessing";
            this.panelProcessing.Size = new System.Drawing.Size(246, 43);
            this.panelProcessing.TabIndex = 8;
            // 
            // labelWorking
            // 
            this.labelWorking.AutoSize = true;
            this.labelWorking.Location = new System.Drawing.Point(43, 14);
            this.labelWorking.Name = "labelWorking";
            this.labelWorking.Size = new System.Drawing.Size(196, 15);
            this.labelWorking.TabIndex = 7;
            this.labelWorking.Text = "Processing game data, please wait...";
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
            // MergeDatPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelProcessing);
            this.Controls.Add(this.panelContent);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MergeDatPage";
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
        public System.Windows.Forms.Panel panelProcessing;
        private System.Windows.Forms.Label labelWorking;
        private System.Windows.Forms.PictureBox pictureWorkingSpiral;
        public System.Windows.Forms.Label labelWelcomeText;
        public System.Windows.Forms.Label labelWelcome;
    }
}
