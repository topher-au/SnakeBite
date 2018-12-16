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
            this.panelContent = new System.Windows.Forms.Panel();
            this.labelInstallOrder = new System.Windows.Forms.Label();
            this.listInstallOrder = new System.Windows.Forms.ListView();
            this.columnModNames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelConflictCount = new System.Windows.Forms.Label();
            this.labelExplainConflict = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAdd
            // 
            this.buttonAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonAdd.Location = new System.Drawing.Point(12, 438);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(231, 24);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "Add More Mods...";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRemove.Location = new System.Drawing.Point(12, 465);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(231, 24);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "Remove Selected Mod";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonContinue
            // 
            this.buttonContinue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonContinue.Location = new System.Drawing.Point(522, 438);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(173, 51);
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
            this.labelModCount.Location = new System.Drawing.Point(12, 388);
            this.labelModCount.Name = "labelModCount";
            this.labelModCount.Size = new System.Drawing.Size(231, 24);
            this.labelModCount.TabIndex = 8;
            this.labelModCount.Text = "Total Count: 0";
            this.labelModCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.Transparent;
            this.panelContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelContent.Location = new System.Drawing.Point(293, 9);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(400, 424);
            this.panelContent.TabIndex = 9;
            // 
            // labelInstallOrder
            // 
            this.labelInstallOrder.BackColor = System.Drawing.Color.Silver;
            this.labelInstallOrder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelInstallOrder.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstallOrder.Location = new System.Drawing.Point(12, 9);
            this.labelInstallOrder.Name = "labelInstallOrder";
            this.labelInstallOrder.Size = new System.Drawing.Size(231, 24);
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
            this.listInstallOrder.Size = new System.Drawing.Size(231, 349);
            this.listInstallOrder.TabIndex = 12;
            this.listInstallOrder.UseCompatibleStateImageBehavior = false;
            this.listInstallOrder.View = System.Windows.Forms.View.Details;
            this.listInstallOrder.SelectedIndexChanged += new System.EventHandler(this.listInstallOrder_SelectedIndexChanged);
            // 
            // columnModNames
            // 
            this.columnModNames.Width = 226;
            // 
            // labelConflictCount
            // 
            this.labelConflictCount.BackColor = System.Drawing.Color.Silver;
            this.labelConflictCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelConflictCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelConflictCount.Location = new System.Drawing.Point(12, 411);
            this.labelConflictCount.Name = "labelConflictCount";
            this.labelConflictCount.Size = new System.Drawing.Size(231, 24);
            this.labelConflictCount.TabIndex = 13;
            this.labelConflictCount.Text = "Conflicts Detected: 0";
            this.labelConflictCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelExplainConflict
            // 
            this.labelExplainConflict.BackColor = System.Drawing.Color.LightGray;
            this.labelExplainConflict.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelExplainConflict.Cursor = System.Windows.Forms.Cursors.Help;
            this.labelExplainConflict.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExplainConflict.ForeColor = System.Drawing.Color.Blue;
            this.labelExplainConflict.Location = new System.Drawing.Point(199, 411);
            this.labelExplainConflict.Name = "labelExplainConflict";
            this.labelExplainConflict.Size = new System.Drawing.Size(44, 24);
            this.labelExplainConflict.TabIndex = 14;
            this.labelExplainConflict.Text = "?";
            this.labelExplainConflict.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelExplainConflict.Click += new System.EventHandler(this.labelExplainConflict_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(381, 470);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // formInstallOrder
            // 
            this.AcceptButton = this.buttonContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(705, 499);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelExplainConflict);
            this.Controls.Add(this.labelConflictCount);
            this.Controls.Add(this.listInstallOrder);
            this.Controls.Add(this.labelInstallOrder);
            this.Controls.Add(this.panelContent);
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
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.formInstallOrder_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonContinue;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Label labelModCount;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label labelInstallOrder;
        private System.Windows.Forms.ListView listInstallOrder;
        private System.Windows.Forms.ColumnHeader columnModNames;
        private System.Windows.Forms.Label labelConflictCount;
        private System.Windows.Forms.Label labelExplainConflict;
        private System.Windows.Forms.Button button1;
    }
}