namespace SnakeBite
{
    partial class formProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formProgress));
            this.StatusText = new System.Windows.Forms.Label();
            this.pictureSpiral = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSpiral)).BeginInit();
            this.SuspendLayout();
            // 
            // StatusText
            // 
            this.StatusText.AutoSize = true;
            this.StatusText.Location = new System.Drawing.Point(60, 21);
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(188, 15);
            this.StatusText.TabIndex = 0;
            this.StatusText.Text = "SnakeBite is working, please wait...\r\n";
            this.StatusText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureSpiral
            // 
            this.pictureSpiral.BackColor = System.Drawing.Color.Transparent;
            this.pictureSpiral.Image = ((System.Drawing.Image)(resources.GetObject("pictureSpiral.Image")));
            this.pictureSpiral.Location = new System.Drawing.Point(12, 12);
            this.pictureSpiral.Name = "pictureSpiral";
            this.pictureSpiral.Size = new System.Drawing.Size(32, 32);
            this.pictureSpiral.TabIndex = 1;
            this.pictureSpiral.TabStop = false;
            // 
            // formProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(298, 105);
            this.ControlBox = false;
            this.Controls.Add(this.pictureSpiral);
            this.Controls.Add(this.StatusText);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Processing";
            this.VisibleChanged += new System.EventHandler(this.formProgress_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureSpiral)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureSpiral;
        public System.Windows.Forms.Label StatusText;
    }
}