namespace SnakeBite.ModPages
{
    partial class NoAddedPage
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
            this.groupBoxNoModsNotice = new System.Windows.Forms.GroupBox();
            this.labelNoMod = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxNoModsNotice.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxNoModsNotice
            // 
            this.groupBoxNoModsNotice.BackColor = System.Drawing.Color.Gray;
            this.groupBoxNoModsNotice.Controls.Add(this.labelNoMod);
            this.groupBoxNoModsNotice.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxNoModsNotice.Location = new System.Drawing.Point(0, -6);
            this.groupBoxNoModsNotice.Name = "groupBoxNoModsNotice";
            this.groupBoxNoModsNotice.Size = new System.Drawing.Size(356, 379);
            this.groupBoxNoModsNotice.TabIndex = 11;
            this.groupBoxNoModsNotice.TabStop = false;
            // 
            // labelNoMod
            // 
            this.labelNoMod.BackColor = System.Drawing.Color.Gray;
            this.labelNoMod.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold);
            this.labelNoMod.Location = new System.Drawing.Point(10, 16);
            this.labelNoMod.Name = "labelNoMod";
            this.labelNoMod.Size = new System.Drawing.Size(338, 55);
            this.labelNoMod.TabIndex = 1;
            this.labelNoMod.Text = "No Mods Added";
            this.labelNoMod.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.Controls.Add(this.groupBoxNoModsNotice);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(356, 373);
            this.panel1.TabIndex = 12;
            // 
            // NoAddedPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "NoAddedPage";
            this.Size = new System.Drawing.Size(356, 373);
            this.groupBoxNoModsNotice.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxNoModsNotice;
        private System.Windows.Forms.Label labelNoMod;
        private System.Windows.Forms.Panel panel1;
    }
}
