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
            this.labelCheckDebug = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureExclamation)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonInstall
            // 
            this.buttonInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonInstall.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonInstall.Location = new System.Drawing.Point(81, 136);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(283, 31);
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
            this.buttonDontInstall.Location = new System.Drawing.Point(370, 136);
            this.buttonDontInstall.Name = "buttonDontInstall";
            this.buttonDontInstall.Size = new System.Drawing.Size(149, 31);
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
            this.labelHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.Location = new System.Drawing.Point(80, 12);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(437, 90);
            this.labelHeader.TabIndex = 2;
            this.labelHeader.Text = "[Examplemod1] conflicts with the following mods: ";
            this.labelHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureExclamation
            // 
            this.pictureExclamation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureExclamation.Image = global::SnakeBite.Properties.Resources.mgsexclamation;
            this.pictureExclamation.Location = new System.Drawing.Point(11, 12);
            this.pictureExclamation.Name = "pictureExclamation";
            this.pictureExclamation.Size = new System.Drawing.Size(63, 75);
            this.pictureExclamation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureExclamation.TabIndex = 3;
            this.pictureExclamation.TabStop = false;
            // 
            // labelCheckDebug
            // 
            this.labelCheckDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCheckDebug.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelCheckDebug.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelCheckDebug.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelCheckDebug.Location = new System.Drawing.Point(83, 102);
            this.labelCheckDebug.Name = "labelCheckDebug";
            this.labelCheckDebug.Size = new System.Drawing.Size(434, 23);
            this.labelCheckDebug.TabIndex = 4;
            this.labelCheckDebug.Text = "Open the Debug Log for more information";
            this.labelCheckDebug.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelCheckDebug.Click += new System.EventHandler(this.labelCheckDebug_Click);
            // 
            // formModConflict
            // 
            this.AcceptButton = this.buttonInstall;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.buttonDontInstall;
            this.ClientSize = new System.Drawing.Size(529, 179);
            this.Controls.Add(this.labelCheckDebug);
            this.Controls.Add(this.buttonDontInstall);
            this.Controls.Add(this.buttonInstall);
            this.Controls.Add(this.pictureExclamation);
            this.Controls.Add(this.labelHeader);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formModConflict";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mod Conflict Detected";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.formModConflict_HelpButtonClicked);
            ((System.ComponentModel.ISupportInitialize)(this.pictureExclamation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Button buttonDontInstall;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.PictureBox pictureExclamation;
        private System.Windows.Forms.Label labelCheckDebug;
    }
}