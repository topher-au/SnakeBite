namespace SnakeBite.Forms
{
    partial class formModConflict
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
            this.buttonInstall = new System.Windows.Forms.Button();
            this.buttonDontInstall = new System.Windows.Forms.Button();
            this.labelHeader = new System.Windows.Forms.Label();
            this.pictureExclamation = new System.Windows.Forms.PictureBox();
            this.labelInstallWarning = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureExclamation)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonInstall
            // 
            this.buttonInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonInstall.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonInstall.Location = new System.Drawing.Point(132, 176);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(283, 30);
            this.buttonInstall.TabIndex = 0;
            this.buttonInstall.Text = "Install Anyway (Overwrite Existing Data)";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // buttonDontInstall
            // 
            this.buttonDontInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDontInstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonDontInstall.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonDontInstall.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonDontInstall.Location = new System.Drawing.Point(421, 175);
            this.buttonDontInstall.Name = "buttonDontInstall";
            this.buttonDontInstall.Size = new System.Drawing.Size(154, 31);
            this.buttonDontInstall.TabIndex = 1;
            this.buttonDontInstall.Text = "Cancel Installation";
            this.buttonDontInstall.UseVisualStyleBackColor = true;
            this.buttonDontInstall.Click += new System.EventHandler(this.buttonDontInstall_Click);
            // 
            // labelHeader
            // 
            this.labelHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.Location = new System.Drawing.Point(111, 9);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(431, 163);
            this.labelHeader.TabIndex = 2;
            this.labelHeader.Text = "[Examplemod1] conflicts with the following mods: ";
            this.labelHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureExclamation
            // 
            this.pictureExclamation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureExclamation.Image = global::SnakeBite.Properties.Resources.mgsexclamation;
            this.pictureExclamation.Location = new System.Drawing.Point(9, 17);
            this.pictureExclamation.Name = "pictureExclamation";
            this.pictureExclamation.Size = new System.Drawing.Size(91, 131);
            this.pictureExclamation.TabIndex = 3;
            this.pictureExclamation.TabStop = false;
            // 
            // labelInstallWarning
            // 
            this.labelInstallWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInstallWarning.BackColor = System.Drawing.Color.Gainsboro;
            this.labelInstallWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelInstallWarning.Cursor = System.Windows.Forms.Cursors.Help;
            this.labelInstallWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstallWarning.ForeColor = System.Drawing.Color.Blue;
            this.labelInstallWarning.Location = new System.Drawing.Point(548, 9);
            this.labelInstallWarning.Name = "labelInstallWarning";
            this.labelInstallWarning.Size = new System.Drawing.Size(27, 29);
            this.labelInstallWarning.TabIndex = 5;
            this.labelInstallWarning.Text = "?";
            this.labelInstallWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelInstallWarning.Click += new System.EventHandler(this.labelInstallWarning_Click);
            // 
            // formModConflict
            // 
            this.AcceptButton = this.buttonInstall;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.CancelButton = this.buttonDontInstall;
            this.ClientSize = new System.Drawing.Size(587, 218);
            this.ControlBox = false;
            this.Controls.Add(this.labelInstallWarning);
            this.Controls.Add(this.buttonDontInstall);
            this.Controls.Add(this.buttonInstall);
            this.Controls.Add(this.pictureExclamation);
            this.Controls.Add(this.labelHeader);
            this.Name = "formModConflict";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mod Conflict Detected";
            ((System.ComponentModel.ISupportInitialize)(this.pictureExclamation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Button buttonDontInstall;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.PictureBox pictureExclamation;
        private System.Windows.Forms.Label labelInstallWarning;
    }
}