namespace makebite
{
    partial class formHelp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formHelp));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.labelLooseTextureNote = new System.Windows.Forms.Label();
            this.labelGameDirNote = new System.Windows.Forms.Label();
            this.labelAssetDirNote = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox3);
            this.groupBox1.Controls.Add(this.listBox2);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Controls.Add(this.labelLooseTextureNote);
            this.groupBox1.Controls.Add(this.labelGameDirNote);
            this.groupBox1.Controls.Add(this.labelAssetDirNote);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(499, 460);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // listBox3
            // 
            this.listBox3.BackColor = System.Drawing.Color.Silver;
            this.listBox3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listBox3.FormattingEnabled = true;
            this.listBox3.ItemHeight = 15;
            this.listBox3.Items.AddRange(new object[] {
            "/GameDir/mod/modules/ShiguTppCamera.lua",
            "/GameDir/dxgi.dll",
            "/GameDir/ReShade.fx",
            "/GameDir/Reshade/CustomFX.undef"});
            this.listBox3.Location = new System.Drawing.Point(9, 362);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(481, 79);
            this.listBox3.TabIndex = 5;
            // 
            // listBox2
            // 
            this.listBox2.BackColor = System.Drawing.Color.Silver;
            this.listBox2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 15;
            this.listBox2.Items.AddRange(new object[] {
            "/1d5440a85b9df.1.ftexs",
            "/1d5440a85b9df.2.ftexs",
            "/1d5440a85b9df.3.ftexs",
            "/1d5440a85b9df.ftex",
            "/3c7809fefe1ec.fpk",
            "/30708b7517f09.pftxs"});
            this.listBox2.Location = new System.Drawing.Point(9, 191);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(481, 109);
            this.listBox2.TabIndex = 4;
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.Silver;
            this.listBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Items.AddRange(new object[] {
            "/Assets/tpp/pack/mission2/common/mis_com_dd_soldier_swim_suit.pftxs",
            "/Assets/tpp/pack/player/parts/plparts_ddf_swimwear.pftxs",
            "/Assets/tpp/pack/common_source/chara/cm_head/face/cm_f0_h0_v000_eye0.fpk",
            "/Assets/tpp/pack/common_source/chara/cm_head/face/cm_f0_h0_v000_eye0.fpkd"});
            this.listBox1.Location = new System.Drawing.Point(9, 47);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(481, 79);
            this.listBox1.TabIndex = 3;
            // 
            // labelLooseTextureNote
            // 
            this.labelLooseTextureNote.AutoSize = true;
            this.labelLooseTextureNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLooseTextureNote.ForeColor = System.Drawing.Color.Black;
            this.labelLooseTextureNote.Location = new System.Drawing.Point(6, 320);
            this.labelLooseTextureNote.Name = "labelLooseTextureNote";
            this.labelLooseTextureNote.Size = new System.Drawing.Size(437, 39);
            this.labelLooseTextureNote.TabIndex = 2;
            this.labelLooseTextureNote.Text = resources.GetString("labelLooseTextureNote.Text");
            // 
            // labelGameDirNote
            // 
            this.labelGameDirNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGameDirNote.ForeColor = System.Drawing.Color.Black;
            this.labelGameDirNote.Location = new System.Drawing.Point(6, 149);
            this.labelGameDirNote.Name = "labelGameDirNote";
            this.labelGameDirNote.Size = new System.Drawing.Size(484, 39);
            this.labelGameDirNote.TabIndex = 1;
            this.labelGameDirNote.Text = resources.GetString("labelGameDirNote.Text");
            // 
            // labelAssetDirNote
            // 
            this.labelAssetDirNote.AutoSize = true;
            this.labelAssetDirNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAssetDirNote.ForeColor = System.Drawing.Color.Black;
            this.labelAssetDirNote.Location = new System.Drawing.Point(6, 18);
            this.labelAssetDirNote.Name = "labelAssetDirNote";
            this.labelAssetDirNote.Size = new System.Drawing.Size(484, 26);
            this.labelAssetDirNote.TabIndex = 0;
            this.labelAssetDirNote.Text = "• If your mod contains files within the Assets folder, the Assets folder should b" +
    "e viewable in the file list.\r\n        The list should appear like the following:" +
    "";
            // 
            // formHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(524, 484);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formHelp";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Formatting Help";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelLooseTextureNote;
        private System.Windows.Forms.Label labelGameDirNote;
        private System.Windows.Forms.Label labelAssetDirNote;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ListBox listBox1;
    }
}