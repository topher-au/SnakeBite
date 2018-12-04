namespace SnakeBite.ModPages
{
    partial class ModDescriptionPage
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
            this.panelModDescription = new System.Windows.Forms.Panel();
            this.labelVersionWarning = new System.Windows.Forms.Label();
            this.labelModWebsite = new System.Windows.Forms.LinkLabel();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.labelModAuthor = new System.Windows.Forms.Label();
            this.labelModName = new System.Windows.Forms.Label();
            this.panelModDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelModDescription
            // 
            this.panelModDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelModDescription.BackColor = System.Drawing.Color.DarkGray;
            this.panelModDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelModDescription.Controls.Add(this.labelVersionWarning);
            this.panelModDescription.Controls.Add(this.labelModWebsite);
            this.panelModDescription.Controls.Add(this.textDescription);
            this.panelModDescription.Controls.Add(this.labelModAuthor);
            this.panelModDescription.Controls.Add(this.labelModName);
            this.panelModDescription.Location = new System.Drawing.Point(0, 0);
            this.panelModDescription.Name = "panelModDescription";
            this.panelModDescription.Size = new System.Drawing.Size(379, 424);
            this.panelModDescription.TabIndex = 11;
            // 
            // labelVersionWarning
            // 
            this.labelVersionWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersionWarning.BackColor = System.Drawing.Color.LightGray;
            this.labelVersionWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelVersionWarning.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelVersionWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersionWarning.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.labelVersionWarning.Location = new System.Drawing.Point(331, 392);
            this.labelVersionWarning.Name = "labelVersionWarning";
            this.labelVersionWarning.Size = new System.Drawing.Size(42, 26);
            this.labelVersionWarning.TabIndex = 15;
            this.labelVersionWarning.Text = "✔";
            this.labelVersionWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelVersionWarning.Click += new System.EventHandler(this.labelVersionWarning_Click);
            // 
            // labelModWebsite
            // 
            this.labelModWebsite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelModWebsite.BackColor = System.Drawing.Color.Silver;
            this.labelModWebsite.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelModWebsite.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelModWebsite.Location = new System.Drawing.Point(4, 392);
            this.labelModWebsite.Name = "labelModWebsite";
            this.labelModWebsite.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelModWebsite.Size = new System.Drawing.Size(328, 26);
            this.labelModWebsite.TabIndex = 4;
            this.labelModWebsite.TabStop = true;
            this.labelModWebsite.Text = "Mod Version Link To Website";
            this.labelModWebsite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelModWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelModWebsite_LinkClicked);
            // 
            // textDescription
            // 
            this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textDescription.BackColor = System.Drawing.Color.Silver;
            this.textDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.textDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.textDescription.Location = new System.Drawing.Point(4, 46);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.ReadOnly = true;
            this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDescription.Size = new System.Drawing.Size(369, 347);
            this.textDescription.TabIndex = 6;
            // 
            // labelModAuthor
            // 
            this.labelModAuthor.AutoSize = true;
            this.labelModAuthor.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.labelModAuthor.Location = new System.Drawing.Point(31, 24);
            this.labelModAuthor.Name = "labelModAuthor";
            this.labelModAuthor.Size = new System.Drawing.Size(83, 19);
            this.labelModAuthor.TabIndex = 3;
            this.labelModAuthor.Text = "Mod Author";
            // 
            // labelModName
            // 
            this.labelModName.AutoSize = true;
            this.labelModName.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.labelModName.Location = new System.Drawing.Point(3, 2);
            this.labelModName.Name = "labelModName";
            this.labelModName.Size = new System.Drawing.Size(111, 25);
            this.labelModName.TabIndex = 2;
            this.labelModName.Text = "Mod Name";
            // 
            // ModDescriptionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelModDescription);
            this.Name = "ModDescriptionPage";
            this.Size = new System.Drawing.Size(379, 424);
            this.panelModDescription.ResumeLayout(false);
            this.panelModDescription.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelModDescription;
        private System.Windows.Forms.Label labelVersionWarning;
        private System.Windows.Forms.LinkLabel labelModWebsite;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.Label labelModAuthor;
        private System.Windows.Forms.Label labelModName;
    }
}
