namespace SnakeBite.ModPages
{
    partial class PreinstallDescriptionPage
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
            this.panelInfo = new System.Windows.Forms.Panel();
            this.labelVersionWarning = new System.Windows.Forms.Label();
            this.labelInstallWarning = new System.Windows.Forms.Label();
            this.labelConflictCount = new System.Windows.Forms.Label();
            this.textConflictDescription = new System.Windows.Forms.TextBox();
            this.textModDescription = new System.Windows.Forms.TextBox();
            this.labelModWebsite = new System.Windows.Forms.LinkLabel();
            this.labelModName = new System.Windows.Forms.Label();
            this.labelModAuthor = new System.Windows.Forms.Label();
            this.panelInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelInfo
            // 
            this.panelInfo.BackColor = System.Drawing.Color.DarkGray;
            this.panelInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelInfo.Controls.Add(this.labelVersionWarning);
            this.panelInfo.Controls.Add(this.labelInstallWarning);
            this.panelInfo.Controls.Add(this.labelConflictCount);
            this.panelInfo.Controls.Add(this.textConflictDescription);
            this.panelInfo.Controls.Add(this.textModDescription);
            this.panelInfo.Controls.Add(this.labelModWebsite);
            this.panelInfo.Controls.Add(this.labelModName);
            this.panelInfo.Controls.Add(this.labelModAuthor);
            this.panelInfo.Location = new System.Drawing.Point(0, 0);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(356, 373);
            this.panelInfo.TabIndex = 10;
            // 
            // labelVersionWarning
            // 
            this.labelVersionWarning.BackColor = System.Drawing.Color.LightGray;
            this.labelVersionWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelVersionWarning.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelVersionWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersionWarning.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.labelVersionWarning.Location = new System.Drawing.Point(308, 185);
            this.labelVersionWarning.Name = "labelVersionWarning";
            this.labelVersionWarning.Size = new System.Drawing.Size(42, 26);
            this.labelVersionWarning.TabIndex = 14;
            this.labelVersionWarning.Text = "✔";
            this.labelVersionWarning.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelInstallWarning
            // 
            this.labelInstallWarning.BackColor = System.Drawing.Color.LightGray;
            this.labelInstallWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelInstallWarning.Cursor = System.Windows.Forms.Cursors.Help;
            this.labelInstallWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstallWarning.ForeColor = System.Drawing.Color.Blue;
            this.labelInstallWarning.Location = new System.Drawing.Point(309, 339);
            this.labelInstallWarning.Name = "labelInstallWarning";
            this.labelInstallWarning.Size = new System.Drawing.Size(41, 26);
            this.labelInstallWarning.TabIndex = 10;
            this.labelInstallWarning.Text = "?";
            this.labelInstallWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelConflictCount
            // 
            this.labelConflictCount.BackColor = System.Drawing.Color.Silver;
            this.labelConflictCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelConflictCount.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelConflictCount.Location = new System.Drawing.Point(3, 339);
            this.labelConflictCount.Name = "labelConflictCount";
            this.labelConflictCount.Size = new System.Drawing.Size(347, 26);
            this.labelConflictCount.TabIndex = 11;
            this.labelConflictCount.Text = "0 Conflicts Detected";
            this.labelConflictCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textConflictDescription
            // 
            this.textConflictDescription.BackColor = System.Drawing.Color.Silver;
            this.textConflictDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textConflictDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.textConflictDescription.Location = new System.Drawing.Point(3, 214);
            this.textConflictDescription.Multiline = true;
            this.textConflictDescription.Name = "textConflictDescription";
            this.textConflictDescription.ReadOnly = true;
            this.textConflictDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textConflictDescription.Size = new System.Drawing.Size(346, 126);
            this.textConflictDescription.TabIndex = 13;
            this.textConflictDescription.Text = "Conflict Description";
            this.textConflictDescription.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textConflictDescription.WordWrap = false;
            // 
            // textModDescription
            // 
            this.textModDescription.BackColor = System.Drawing.Color.Silver;
            this.textModDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textModDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.textModDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.textModDescription.Location = new System.Drawing.Point(3, 44);
            this.textModDescription.Multiline = true;
            this.textModDescription.Name = "textModDescription";
            this.textModDescription.ReadOnly = true;
            this.textModDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textModDescription.Size = new System.Drawing.Size(346, 142);
            this.textModDescription.TabIndex = 10;
            this.textModDescription.Text = "Mod Description";
            // 
            // labelModWebsite
            // 
            this.labelModWebsite.BackColor = System.Drawing.Color.Silver;
            this.labelModWebsite.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelModWebsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelModWebsite.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelModWebsite.Location = new System.Drawing.Point(3, 185);
            this.labelModWebsite.Name = "labelModWebsite";
            this.labelModWebsite.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelModWebsite.Size = new System.Drawing.Size(306, 26);
            this.labelModWebsite.TabIndex = 12;
            this.labelModWebsite.TabStop = true;
            this.labelModWebsite.Text = "Mod Version Link To Website";
            this.labelModWebsite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelModName
            // 
            this.labelModName.AutoSize = true;
            this.labelModName.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.labelModName.Location = new System.Drawing.Point(3, 0);
            this.labelModName.Name = "labelModName";
            this.labelModName.Size = new System.Drawing.Size(111, 25);
            this.labelModName.TabIndex = 10;
            this.labelModName.Text = "Mod Name";
            // 
            // labelModAuthor
            // 
            this.labelModAuthor.AutoSize = true;
            this.labelModAuthor.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.labelModAuthor.Location = new System.Drawing.Point(31, 22);
            this.labelModAuthor.Name = "labelModAuthor";
            this.labelModAuthor.Size = new System.Drawing.Size(83, 19);
            this.labelModAuthor.TabIndex = 11;
            this.labelModAuthor.Text = "Mod Author";
            // 
            // PreinstallDescriptionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelInfo);
            this.Name = "PreinstallDescriptionPage";
            this.Size = new System.Drawing.Size(356, 373);
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label labelVersionWarning;
        private System.Windows.Forms.Label labelInstallWarning;
        private System.Windows.Forms.Label labelConflictCount;
        private System.Windows.Forms.TextBox textConflictDescription;
        private System.Windows.Forms.TextBox textModDescription;
        private System.Windows.Forms.LinkLabel labelModWebsite;
        private System.Windows.Forms.Label labelModName;
        private System.Windows.Forms.Label labelModAuthor;
    }
}
