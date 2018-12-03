namespace SnakeBite.Forms
{
    partial class formInstallOrder
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
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonContinue = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.labelModCount = new System.Windows.Forms.Label();
            this.labelInstallWarning = new System.Windows.Forms.Label();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.labelVersionWarning = new System.Windows.Forms.Label();
            this.labelConflictCount = new System.Windows.Forms.Label();
            this.textConflictDescription = new System.Windows.Forms.TextBox();
            this.textModDescription = new System.Windows.Forms.TextBox();
            this.labelModWebsite = new System.Windows.Forms.LinkLabel();
            this.labelModName = new System.Windows.Forms.Label();
            this.labelModAuthor = new System.Windows.Forms.Label();
            this.groupBoxNoModsNotice = new System.Windows.Forms.GroupBox();
            this.labelNoMod = new System.Windows.Forms.Label();
            this.labelInstallOrder = new System.Windows.Forms.Label();
            this.listInstallOrder = new System.Windows.Forms.ListView();
            this.columnModNames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelInfo.SuspendLayout();
            this.groupBoxNoModsNotice.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAdd
            // 
            this.buttonAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonAdd.Location = new System.Drawing.Point(12, 390);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(231, 23);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "Add More Mods...";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRemove.Location = new System.Drawing.Point(12, 417);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(231, 23);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "Remove Selected Mod";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonContinue
            // 
            this.buttonContinue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonContinue.Location = new System.Drawing.Point(485, 390);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(173, 50);
            this.buttonContinue.TabIndex = 4;
            this.buttonContinue.Text = "Continue Installation";
            this.buttonContinue.UseVisualStyleBackColor = true;
            this.buttonContinue.Click += new System.EventHandler(this.buttonContinue_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonUp.Location = new System.Drawing.Point(243, 36);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(43, 54);
            this.buttonUp.TabIndex = 6;
            this.buttonUp.Text = "▲";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonDown.Location = new System.Drawing.Point(243, 88);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(43, 54);
            this.buttonDown.TabIndex = 7;
            this.buttonDown.Text = "▼";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // labelModCount
            // 
            this.labelModCount.BackColor = System.Drawing.Color.Silver;
            this.labelModCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelModCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelModCount.Location = new System.Drawing.Point(12, 365);
            this.labelModCount.Name = "labelModCount";
            this.labelModCount.Size = new System.Drawing.Size(231, 20);
            this.labelModCount.TabIndex = 8;
            this.labelModCount.Text = "Total Count: 0";
            this.labelModCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.labelInstallWarning.Click += new System.EventHandler(this.labelInstallWarning_Click);
            // 
            // panelInfo
            // 
            this.panelInfo.BackColor = System.Drawing.Color.Silver;
            this.panelInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelInfo.Controls.Add(this.labelVersionWarning);
            this.panelInfo.Controls.Add(this.labelInstallWarning);
            this.panelInfo.Controls.Add(this.labelConflictCount);
            this.panelInfo.Controls.Add(this.textConflictDescription);
            this.panelInfo.Controls.Add(this.textModDescription);
            this.panelInfo.Controls.Add(this.labelModWebsite);
            this.panelInfo.Controls.Add(this.labelModName);
            this.panelInfo.Controls.Add(this.labelModAuthor);
            this.panelInfo.Location = new System.Drawing.Point(297, 11);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(356, 373);
            this.panelInfo.TabIndex = 9;
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
            this.labelVersionWarning.Click += new System.EventHandler(this.labelVersionWarning_Click);
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
            this.labelModWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelModWebsite_LinkClicked);
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
            // groupBoxNoModsNotice
            // 
            this.groupBoxNoModsNotice.BackColor = System.Drawing.Color.DarkGray;
            this.groupBoxNoModsNotice.Controls.Add(this.labelNoMod);
            this.groupBoxNoModsNotice.Location = new System.Drawing.Point(297, 5);
            this.groupBoxNoModsNotice.Name = "groupBoxNoModsNotice";
            this.groupBoxNoModsNotice.Size = new System.Drawing.Size(361, 380);
            this.groupBoxNoModsNotice.TabIndex = 10;
            this.groupBoxNoModsNotice.TabStop = false;
            this.groupBoxNoModsNotice.Visible = false;
            // 
            // labelNoMod
            // 
            this.labelNoMod.BackColor = System.Drawing.Color.DarkGray;
            this.labelNoMod.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold);
            this.labelNoMod.Location = new System.Drawing.Point(10, 16);
            this.labelNoMod.Name = "labelNoMod";
            this.labelNoMod.Size = new System.Drawing.Size(338, 55);
            this.labelNoMod.TabIndex = 1;
            this.labelNoMod.Text = "No Mods Added";
            this.labelNoMod.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelInstallOrder
            // 
            this.labelInstallOrder.BackColor = System.Drawing.Color.Silver;
            this.labelInstallOrder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelInstallOrder.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstallOrder.Location = new System.Drawing.Point(12, 11);
            this.labelInstallOrder.Name = "labelInstallOrder";
            this.labelInstallOrder.Size = new System.Drawing.Size(231, 26);
            this.labelInstallOrder.TabIndex = 11;
            this.labelInstallOrder.Text = "Installation Order";
            this.labelInstallOrder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listInstallOrder
            // 
            this.listInstallOrder.BackColor = System.Drawing.Color.Silver;
            this.listInstallOrder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listInstallOrder.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnModNames});
            this.listInstallOrder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listInstallOrder.FullRowSelect = true;
            this.listInstallOrder.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listInstallOrder.HideSelection = false;
            this.listInstallOrder.Location = new System.Drawing.Point(12, 36);
            this.listInstallOrder.MultiSelect = false;
            this.listInstallOrder.Name = "listInstallOrder";
            this.listInstallOrder.ShowGroups = false;
            this.listInstallOrder.Size = new System.Drawing.Size(231, 330);
            this.listInstallOrder.TabIndex = 12;
            this.listInstallOrder.UseCompatibleStateImageBehavior = false;
            this.listInstallOrder.View = System.Windows.Forms.View.Details;
            this.listInstallOrder.SelectedIndexChanged += new System.EventHandler(this.listInstallOrder_SelectedIndexChanged);
            // 
            // columnModNames
            // 
            this.columnModNames.Width = 226;
            // 
            // formInstallOrder
            // 
            this.AcceptButton = this.buttonContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(665, 444);
            this.Controls.Add(this.listInstallOrder);
            this.Controls.Add(this.labelInstallOrder);
            this.Controls.Add(this.groupBoxNoModsNotice);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.labelModCount);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonContinue);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formInstallOrder";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SnakeBite Install Manager";
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.groupBoxNoModsNotice.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonContinue;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Label labelModCount;
        private System.Windows.Forms.Label labelInstallWarning;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.TextBox textConflictDescription;
        private System.Windows.Forms.TextBox textModDescription;
        private System.Windows.Forms.LinkLabel labelModWebsite;
        private System.Windows.Forms.Label labelModName;
        private System.Windows.Forms.Label labelModAuthor;
        private System.Windows.Forms.Label labelConflictCount;
        private System.Windows.Forms.GroupBox groupBoxNoModsNotice;
        private System.Windows.Forms.Label labelNoMod;
        private System.Windows.Forms.Label labelInstallOrder;
        private System.Windows.Forms.ListView listInstallOrder;
        private System.Windows.Forms.ColumnHeader columnModNames;
        private System.Windows.Forms.Label labelVersionWarning;
    }
}