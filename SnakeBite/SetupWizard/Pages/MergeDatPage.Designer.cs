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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeDatPage));
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelProcessing = new System.Windows.Forms.Panel();
            this.labelWorking = new System.Windows.Forms.Label();
            this.labelWelcomeText = new System.Windows.Forms.Label();
            this.labelWelcome = new System.Windows.Forms.Label();
            this.panelContent.SuspendLayout();
            this.panelProcessing.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.panelProcessing);
            this.panelContent.Controls.Add(this.labelWelcomeText);
            this.panelContent.Controls.Add(this.labelWelcome);
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(440, 340);
            this.panelContent.TabIndex = 3;
            // 
            // panelProcessing
            // 
            this.panelProcessing.Controls.Add(this.labelWorking);
            this.panelProcessing.Location = new System.Drawing.Point(3, 289);
            this.panelProcessing.Name = "panelProcessing";
            this.panelProcessing.Size = new System.Drawing.Size(434, 48);
            this.panelProcessing.TabIndex = 8;
            // 
            // labelWorking
            // 
            this.labelWorking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelWorking.Location = new System.Drawing.Point(0, 0);
            this.labelWorking.Name = "labelWorking";
            this.labelWorking.Size = new System.Drawing.Size(434, 48);
            this.labelWorking.TabIndex = 7;
            this.labelWorking.Text = "Working on game files, please wait...";
            this.labelWorking.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelWelcomeText
            // 
            this.labelWelcomeText.Location = new System.Drawing.Point(5, 46);
            this.labelWelcomeText.Name = "labelWelcomeText";
            this.labelWelcomeText.Size = new System.Drawing.Size(429, 228);
            this.labelWelcomeText.TabIndex = 5;
            this.labelWelcomeText.Text = resources.GetString("labelWelcomeText.Text");
            // 
            // labelWelcome
            // 
            this.labelWelcome.AutoSize = true;
            this.labelWelcome.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWelcome.Location = new System.Drawing.Point(3, 0);
            this.labelWelcome.Name = "labelWelcome";
            this.labelWelcome.Size = new System.Drawing.Size(157, 30);
            this.labelWelcome.TabIndex = 4;
            this.labelWelcome.Text = "Almost done...";
            // 
            // MergeDatPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContent);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MergeDatPage";
            this.Size = new System.Drawing.Size(440, 340);
            this.panelContent.ResumeLayout(false);
            this.panelContent.PerformLayout();
            this.panelProcessing.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelContent;
        public System.Windows.Forms.Panel panelProcessing;
        public System.Windows.Forms.Label labelWorking;
        public System.Windows.Forms.Label labelWelcomeText;
        public System.Windows.Forms.Label labelWelcome;
    }
}
