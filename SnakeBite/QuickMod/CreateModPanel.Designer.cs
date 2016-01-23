namespace SnakeBite.QuickMod
{
    partial class CreateModPanel
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
            this.labelModName = new System.Windows.Forms.Label();
            this.textModName = new System.Windows.Forms.TextBox();
            this.checkExport = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelModName
            // 
            this.labelModName.AutoSize = true;
            this.labelModName.Location = new System.Drawing.Point(3, 6);
            this.labelModName.Name = "labelModName";
            this.labelModName.Size = new System.Drawing.Size(70, 15);
            this.labelModName.TabIndex = 0;
            this.labelModName.Text = "Mod Name:";
            // 
            // textModName
            // 
            this.textModName.Location = new System.Drawing.Point(79, 3);
            this.textModName.Name = "textModName";
            this.textModName.Size = new System.Drawing.Size(278, 23);
            this.textModName.TabIndex = 1;
            // 
            // checkExport
            // 
            this.checkExport.AutoSize = true;
            this.checkExport.Location = new System.Drawing.Point(79, 32);
            this.checkExport.Name = "checkExport";
            this.checkExport.Size = new System.Drawing.Size(92, 19);
            this.checkExport.TabIndex = 2;
            this.checkExport.Text = "Export to file";
            this.checkExport.UseVisualStyleBackColor = true;
            // 
            // CreateModPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.checkExport);
            this.Controls.Add(this.textModName);
            this.Controls.Add(this.labelModName);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CreateModPanel";
            this.Size = new System.Drawing.Size(360, 180);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelModName;
        public System.Windows.Forms.TextBox textModName;
        public System.Windows.Forms.CheckBox checkExport;
    }
}
