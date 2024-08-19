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
            this.StatusText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StatusText
            // 
            this.StatusText.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusText.Location = new System.Drawing.Point(12, 9);
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(335, 38);
            this.StatusText.TabIndex = 0;
            this.StatusText.Text = "SnakeBite is working, please wait...";
            this.StatusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.StatusText.UseWaitCursor = true;
            // 
            // formProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(359, 56);
            this.ControlBox = false;
            this.Controls.Add(this.StatusText);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formProgress";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Processing...";
            this.UseWaitCursor = true;
            this.VisibleChanged += new System.EventHandler(this.formProgress_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Label StatusText;
    }
}