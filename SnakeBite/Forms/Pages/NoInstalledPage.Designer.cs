namespace SnakeBite.ModPages
{
    partial class NoInstalledPage
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxNoModsNotice = new System.Windows.Forms.GroupBox();
            this.linkLabelSnakeBiteModsList = new System.Windows.Forms.LinkLabel();
            this.labelNoMod = new System.Windows.Forms.Label();
            this.labelNoModInstruction = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBoxNoModsNotice.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.Controls.Add(this.groupBoxNoModsNotice);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(379, 430);
            this.panel1.TabIndex = 0;
            // 
            // groupBoxNoModsNotice
            // 
            this.groupBoxNoModsNotice.BackColor = System.Drawing.Color.Gray;
            this.groupBoxNoModsNotice.Controls.Add(this.linkLabelSnakeBiteModsList);
            this.groupBoxNoModsNotice.Controls.Add(this.labelNoMod);
            this.groupBoxNoModsNotice.Controls.Add(this.labelNoModInstruction);
            this.groupBoxNoModsNotice.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxNoModsNotice.Location = new System.Drawing.Point(0, -6);
            this.groupBoxNoModsNotice.Name = "groupBoxNoModsNotice";
            this.groupBoxNoModsNotice.Size = new System.Drawing.Size(379, 436);
            this.groupBoxNoModsNotice.TabIndex = 2;
            this.groupBoxNoModsNotice.TabStop = false;
            this.groupBoxNoModsNotice.Visible = false;
            // 
            // linkLabelSnakeBiteModsList
            // 
            this.linkLabelSnakeBiteModsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelSnakeBiteModsList.BackColor = System.Drawing.Color.Gray;
            this.linkLabelSnakeBiteModsList.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.linkLabelSnakeBiteModsList.Location = new System.Drawing.Point(9, 98);
            this.linkLabelSnakeBiteModsList.Name = "linkLabelSnakeBiteModsList";
            this.linkLabelSnakeBiteModsList.Size = new System.Drawing.Size(361, 51);
            this.linkLabelSnakeBiteModsList.TabIndex = 2;
            this.linkLabelSnakeBiteModsList.TabStop = true;
            this.linkLabelSnakeBiteModsList.Text = "Browse SnakeBite Compatible Mods";
            this.linkLabelSnakeBiteModsList.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelNoMod
            // 
            this.labelNoMod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNoMod.BackColor = System.Drawing.Color.Gray;
            this.labelNoMod.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold);
            this.labelNoMod.Location = new System.Drawing.Point(9, 17);
            this.labelNoMod.Name = "labelNoMod";
            this.labelNoMod.Size = new System.Drawing.Size(361, 55);
            this.labelNoMod.TabIndex = 1;
            this.labelNoMod.Text = "No Mods Installed";
            this.labelNoMod.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // labelNoModInstruction
            // 
            this.labelNoModInstruction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNoModInstruction.BackColor = System.Drawing.Color.Gray;
            this.labelNoModInstruction.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelNoModInstruction.Location = new System.Drawing.Point(9, 68);
            this.labelNoModInstruction.Name = "labelNoModInstruction";
            this.labelNoModInstruction.Size = new System.Drawing.Size(361, 30);
            this.labelNoModInstruction.TabIndex = 0;
            this.labelNoModInstruction.Text = "To install mods, click \"Install .MGSV File(s)\"";
            this.labelNoModInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NoInstalledPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "NoInstalledPage";
            this.Size = new System.Drawing.Size(379, 430);
            this.panel1.ResumeLayout(false);
            this.groupBoxNoModsNotice.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBoxNoModsNotice;
        private System.Windows.Forms.LinkLabel linkLabelSnakeBiteModsList;
        private System.Windows.Forms.Label labelNoMod;
        private System.Windows.Forms.Label labelNoModInstruction;
    }
}
