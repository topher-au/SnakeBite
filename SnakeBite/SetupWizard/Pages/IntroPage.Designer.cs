﻿namespace SnakeBite.SetupWizard
{
    partial class IntroPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IntroPage));
            this.labelWelcome = new System.Windows.Forms.Label();
            this.labelWelcomeText = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.labelPiracyWarning = new System.Windows.Forms.Label();
            this.panelContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelWelcome
            // 
            this.labelWelcome.AutoSize = true;
            this.labelWelcome.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWelcome.Location = new System.Drawing.Point(3, 0);
            this.labelWelcome.Name = "labelWelcome";
            this.labelWelcome.Size = new System.Drawing.Size(234, 30);
            this.labelWelcome.TabIndex = 4;
            this.labelWelcome.Text = "Welcome to SnakeBite";
            // 
            // labelWelcomeText
            // 
            this.labelWelcomeText.Location = new System.Drawing.Point(5, 51);
            this.labelWelcomeText.Name = "labelWelcomeText";
            this.labelWelcomeText.Size = new System.Drawing.Size(429, 132);
            this.labelWelcomeText.TabIndex = 5;
            this.labelWelcomeText.Text = resources.GetString("labelWelcomeText.Text");
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.labelPiracyWarning);
            this.panelContent.Controls.Add(this.labelWelcomeText);
            this.panelContent.Controls.Add(this.labelWelcome);
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(440, 340);
            this.panelContent.TabIndex = 2;
            // 
            // labelPiracyWarning
            // 
            this.labelPiracyWarning.ForeColor = System.Drawing.Color.Red;
            this.labelPiracyWarning.Location = new System.Drawing.Point(5, 198);
            this.labelPiracyWarning.Name = "labelPiracyWarning";
            this.labelPiracyWarning.Size = new System.Drawing.Size(429, 95);
            this.labelPiracyWarning.TabIndex = 6;
            this.labelPiracyWarning.Text = "Notice:\r\n\r\nSnakeBite is not compatible with pirated copies of MGSV:TPP. \r\nIf you " +
    "do not own a genuine copy of the game, please consider purchasing it through Ste" +
    "am before using SnakeBite.";
            this.labelPiracyWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // IntroPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContent);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "IntroPage";
            this.Size = new System.Drawing.Size(440, 340);
            this.panelContent.ResumeLayout(false);
            this.panelContent.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelWelcome;
        private System.Windows.Forms.Label labelWelcomeText;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label labelPiracyWarning;
    }
}
