namespace SnakeBite
{
    partial class formMods
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
            this.components = new System.ComponentModel.Container();
            this.listInstalledMods = new System.Windows.Forms.CheckedListBox();
            this.buttonInstall = new System.Windows.Forms.Button();
            this.buttonUninstall = new System.Windows.Forms.Button();
            this.labelInstalledMods = new System.Windows.Forms.Label();
            this.checkBoxMarkAll = new System.Windows.Forms.CheckBox();
            this.panelModDescription = new System.Windows.Forms.Panel();
            this.labelVersionWarning = new System.Windows.Forms.Label();
            this.labelModWebsite = new System.Windows.Forms.LinkLabel();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.labelModAuthor = new System.Windows.Forms.Label();
            this.labelModName = new System.Windows.Forms.Label();
            this.groupBoxNoModsNotice = new System.Windows.Forms.GroupBox();
            this.linkLabelSnakeBiteModsList = new System.Windows.Forms.LinkLabel();
            this.labelNoMod = new System.Windows.Forms.Label();
            this.labelNoModInstruction = new System.Windows.Forms.Label();
            this.buttonLaunchGame = new System.Windows.Forms.Button();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.panelModList = new System.Windows.Forms.Panel();
            this.panelModDescription.SuspendLayout();
            this.groupBoxNoModsNotice.SuspendLayout();
            this.panelModList.SuspendLayout();
            this.SuspendLayout();
            // 
            // listInstalledMods
            // 
            this.listInstalledMods.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listInstalledMods.BackColor = System.Drawing.Color.Silver;
            this.listInstalledMods.FormattingEnabled = true;
            this.listInstalledMods.IntegralHeight = false;
            this.listInstalledMods.Location = new System.Drawing.Point(0, 23);
            this.listInstalledMods.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listInstalledMods.Name = "listInstalledMods";
            this.listInstalledMods.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listInstalledMods.Size = new System.Drawing.Size(191, 420);
            this.listInstalledMods.TabIndex = 3;
            this.listInstalledMods.Tag = "";
            this.listInstalledMods.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listInstalledMods_ItemCheck);
            this.listInstalledMods.SelectedIndexChanged += new System.EventHandler(this.listInstalledMods_SelectedIndexChanged);
            // 
            // buttonInstall
            // 
            this.buttonInstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInstall.BackColor = System.Drawing.Color.Transparent;
            this.buttonInstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonInstall.Location = new System.Drawing.Point(0, 446);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(191, 23);
            this.buttonInstall.TabIndex = 4;
            this.buttonInstall.Text = "Install .MGSV File(s)";
            this.buttonInstall.UseVisualStyleBackColor = false;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // buttonUninstall
            // 
            this.buttonUninstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUninstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonUninstall.Enabled = false;
            this.buttonUninstall.Location = new System.Drawing.Point(0, 469);
            this.buttonUninstall.Name = "buttonUninstall";
            this.buttonUninstall.Size = new System.Drawing.Size(191, 23);
            this.buttonUninstall.TabIndex = 5;
            this.buttonUninstall.Text = "Uninstall Checked Mod(s)";
            this.buttonUninstall.UseVisualStyleBackColor = true;
            this.buttonUninstall.Click += new System.EventHandler(this.buttonUninstall_Click);
            // 
            // labelInstalledMods
            // 
            this.labelInstalledMods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInstalledMods.BackColor = System.Drawing.Color.Silver;
            this.labelInstalledMods.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelInstalledMods.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelInstalledMods.Location = new System.Drawing.Point(0, 0);
            this.labelInstalledMods.Name = "labelInstalledMods";
            this.labelInstalledMods.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelInstalledMods.Size = new System.Drawing.Size(191, 22);
            this.labelInstalledMods.TabIndex = 6;
            this.labelInstalledMods.Text = "Installed Mods";
            this.labelInstalledMods.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBoxMarkAll
            // 
            this.checkBoxMarkAll.AutoSize = true;
            this.checkBoxMarkAll.Checked = true;
            this.checkBoxMarkAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMarkAll.Location = new System.Drawing.Point(8, 13);
            this.checkBoxMarkAll.Name = "checkBoxMarkAll";
            this.checkBoxMarkAll.Size = new System.Drawing.Size(15, 14);
            this.checkBoxMarkAll.TabIndex = 7;
            this.checkBoxMarkAll.UseVisualStyleBackColor = true;
            this.checkBoxMarkAll.Click += new System.EventHandler(this.checkBoxMarkAll_Click);
            // 
            // panelModDescription
            // 
            this.panelModDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panelModDescription.BackColor = System.Drawing.Color.Silver;
            this.panelModDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelModDescription.Controls.Add(this.labelVersionWarning);
            this.panelModDescription.Controls.Add(this.labelModWebsite);
            this.panelModDescription.Controls.Add(this.textDescription);
            this.panelModDescription.Controls.Add(this.labelModAuthor);
            this.panelModDescription.Controls.Add(this.labelModName);
            this.panelModDescription.Location = new System.Drawing.Point(201, 9);
            this.panelModDescription.Name = "panelModDescription";
            this.panelModDescription.Size = new System.Drawing.Size(379, 442);
            this.panelModDescription.TabIndex = 10;
            // 
            // labelVersionWarning
            // 
            this.labelVersionWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersionWarning.BackColor = System.Drawing.Color.LightGray;
            this.labelVersionWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelVersionWarning.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelVersionWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersionWarning.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.labelVersionWarning.Location = new System.Drawing.Point(331, 410);
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
            this.labelModWebsite.Location = new System.Drawing.Point(4, 410);
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
            this.textDescription.Size = new System.Drawing.Size(369, 365);
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
            // groupBoxNoModsNotice
            // 
            this.groupBoxNoModsNotice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxNoModsNotice.Controls.Add(this.linkLabelSnakeBiteModsList);
            this.groupBoxNoModsNotice.Controls.Add(this.labelNoMod);
            this.groupBoxNoModsNotice.Controls.Add(this.labelNoModInstruction);
            this.groupBoxNoModsNotice.Location = new System.Drawing.Point(201, 1);
            this.groupBoxNoModsNotice.Name = "groupBoxNoModsNotice";
            this.groupBoxNoModsNotice.Size = new System.Drawing.Size(379, 450);
            this.groupBoxNoModsNotice.TabIndex = 1;
            this.groupBoxNoModsNotice.TabStop = false;
            this.groupBoxNoModsNotice.Visible = false;
            // 
            // linkLabelSnakeBiteModsList
            // 
            this.linkLabelSnakeBiteModsList.BackColor = System.Drawing.Color.DarkGray;
            this.linkLabelSnakeBiteModsList.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.linkLabelSnakeBiteModsList.Location = new System.Drawing.Point(5, 93);
            this.linkLabelSnakeBiteModsList.Name = "linkLabelSnakeBiteModsList";
            this.linkLabelSnakeBiteModsList.Size = new System.Drawing.Size(361, 51);
            this.linkLabelSnakeBiteModsList.TabIndex = 2;
            this.linkLabelSnakeBiteModsList.TabStop = true;
            this.linkLabelSnakeBiteModsList.Text = "Browse SnakeBite Compatible Mods";
            this.linkLabelSnakeBiteModsList.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.linkLabelSnakeBiteModsList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSnakeBiteModsList_LinkClicked);
            // 
            // labelNoMod
            // 
            this.labelNoMod.BackColor = System.Drawing.Color.DarkGray;
            this.labelNoMod.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold);
            this.labelNoMod.Location = new System.Drawing.Point(5, 12);
            this.labelNoMod.Name = "labelNoMod";
            this.labelNoMod.Size = new System.Drawing.Size(361, 55);
            this.labelNoMod.TabIndex = 1;
            this.labelNoMod.Text = "No Mods Installed";
            this.labelNoMod.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // labelNoModInstruction
            // 
            this.labelNoModInstruction.BackColor = System.Drawing.Color.DarkGray;
            this.labelNoModInstruction.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelNoModInstruction.Location = new System.Drawing.Point(10, 63);
            this.labelNoModInstruction.Name = "labelNoModInstruction";
            this.labelNoModInstruction.Size = new System.Drawing.Size(361, 30);
            this.labelNoModInstruction.TabIndex = 0;
            this.labelNoModInstruction.Text = "To install mods, click \"Install .MGSV File(s)\"";
            this.labelNoModInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonLaunchGame
            // 
            this.buttonLaunchGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLaunchGame.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonLaunchGame.Location = new System.Drawing.Point(431, 455);
            this.buttonLaunchGame.Name = "buttonLaunchGame";
            this.buttonLaunchGame.Size = new System.Drawing.Size(149, 49);
            this.buttonLaunchGame.TabIndex = 11;
            this.buttonLaunchGame.Text = "Launch Game";
            this.buttonLaunchGame.UseVisualStyleBackColor = true;
            this.buttonLaunchGame.Click += new System.EventHandler(this.buttonLaunchGame_Click);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "Item 1";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.Text = "item 2";
            // 
            // panelModList
            // 
            this.panelModList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelModList.BackColor = System.Drawing.Color.Transparent;
            this.panelModList.Controls.Add(this.listInstalledMods);
            this.panelModList.Controls.Add(this.labelInstalledMods);
            this.panelModList.Controls.Add(this.buttonInstall);
            this.panelModList.Controls.Add(this.buttonUninstall);
            this.panelModList.Location = new System.Drawing.Point(4, 9);
            this.panelModList.MaximumSize = new System.Drawing.Size(300, 9001);
            this.panelModList.Name = "panelModList";
            this.panelModList.Size = new System.Drawing.Size(191, 495);
            this.panelModList.TabIndex = 12;
            // 
            // formMods
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(584, 507);
            this.Controls.Add(this.buttonLaunchGame);
            this.Controls.Add(this.groupBoxNoModsNotice);
            this.Controls.Add(this.checkBoxMarkAll);
            this.Controls.Add(this.panelModDescription);
            this.Controls.Add(this.panelModList);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Menu = this.mainMenu1;
            this.MinimumSize = new System.Drawing.Size(550, 520);
            this.Name = "formMods";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SnakeBite Mod Manager";
            this.Load += new System.EventHandler(this.formMain_Load);
            this.Resize += new System.EventHandler(this.formMods_Resize);
            this.panelModDescription.ResumeLayout(false);
            this.panelModDescription.PerformLayout();
            this.groupBoxNoModsNotice.ResumeLayout(false);
            this.panelModList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckedListBox listInstalledMods;
        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Button buttonUninstall;
        private System.Windows.Forms.Label labelInstalledMods;
        private System.Windows.Forms.CheckBox checkBoxMarkAll;
        private System.Windows.Forms.Panel panelModDescription;
        private System.Windows.Forms.Label labelNoModInstruction;
        private System.Windows.Forms.GroupBox groupBoxNoModsNotice;
        private System.Windows.Forms.LinkLabel linkLabelSnakeBiteModsList;
        private System.Windows.Forms.Label labelNoMod;
        private System.Windows.Forms.Label labelModName;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.Label labelModAuthor;
        private System.Windows.Forms.Button buttonLaunchGame;
        private System.Windows.Forms.LinkLabel labelModWebsite;
        private System.Windows.Forms.Label labelVersionWarning;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.Panel panelModList;
    }
}