namespace SnakeBite.QuickMod
{
    partial class SelectZipPanel
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
            this.labelSelectZip = new System.Windows.Forms.Label();
            this.textZipFile = new System.Windows.Forms.TextBox();
            this.buttonFindZip = new System.Windows.Forms.Button();
            this.labelCompat = new System.Windows.Forms.Label();
            this.panelCompat = new System.Windows.Forms.Panel();
            this.picCompat = new System.Windows.Forms.PictureBox();
            this.panelCompat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCompat)).BeginInit();
            this.SuspendLayout();
            // 
            // labelSelectZip
            // 
            this.labelSelectZip.Location = new System.Drawing.Point(3, 0);
            this.labelSelectZip.Name = "labelSelectZip";
            this.labelSelectZip.Size = new System.Drawing.Size(354, 78);
            this.labelSelectZip.TabIndex = 0;
            this.labelSelectZip.Text = "SnakeBite will attempt to detect and install the mod files in the archive. This f" +
    "eature is experimental and may or may not work correctly.\r\n\r\nPlease select the Z" +
    "ip archive you wish to install";
            // 
            // textZipFile
            // 
            this.textZipFile.Location = new System.Drawing.Point(6, 81);
            this.textZipFile.Name = "textZipFile";
            this.textZipFile.ReadOnly = true;
            this.textZipFile.Size = new System.Drawing.Size(351, 23);
            this.textZipFile.TabIndex = 1;
            // 
            // buttonFindZip
            // 
            this.buttonFindZip.Location = new System.Drawing.Point(282, 110);
            this.buttonFindZip.Name = "buttonFindZip";
            this.buttonFindZip.Size = new System.Drawing.Size(75, 23);
            this.buttonFindZip.TabIndex = 2;
            this.buttonFindZip.Text = "Browse...";
            this.buttonFindZip.UseVisualStyleBackColor = true;
            this.buttonFindZip.Click += new System.EventHandler(this.buttonFindZip_Click);
            // 
            // labelCompat
            // 
            this.labelCompat.AutoSize = true;
            this.labelCompat.Location = new System.Drawing.Point(41, 13);
            this.labelCompat.Name = "labelCompat";
            this.labelCompat.Size = new System.Drawing.Size(0, 15);
            this.labelCompat.TabIndex = 3;
            // 
            // panelCompat
            // 
            this.panelCompat.Controls.Add(this.picCompat);
            this.panelCompat.Controls.Add(this.labelCompat);
            this.panelCompat.Location = new System.Drawing.Point(59, 139);
            this.panelCompat.Name = "panelCompat";
            this.panelCompat.Size = new System.Drawing.Size(242, 38);
            this.panelCompat.TabIndex = 5;
            // 
            // picCompat
            // 
            this.picCompat.Location = new System.Drawing.Point(3, 3);
            this.picCompat.Name = "picCompat";
            this.picCompat.Size = new System.Drawing.Size(32, 32);
            this.picCompat.TabIndex = 4;
            this.picCompat.TabStop = false;
            // 
            // SelectZipPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelCompat);
            this.Controls.Add(this.buttonFindZip);
            this.Controls.Add(this.textZipFile);
            this.Controls.Add(this.labelSelectZip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SelectZipPanel";
            this.Size = new System.Drawing.Size(360, 180);
            this.Load += new System.EventHandler(this.WelcomePanel_Load);
            this.panelCompat.ResumeLayout(false);
            this.panelCompat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCompat)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSelectZip;
        private System.Windows.Forms.Button buttonFindZip;
        private System.Windows.Forms.Label labelCompat;
        private System.Windows.Forms.PictureBox picCompat;
        private System.Windows.Forms.Panel panelCompat;
        public System.Windows.Forms.TextBox textZipFile;
    }
}
