namespace KonDB
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.listView1 = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbShowMultisOnly = new System.Windows.Forms.CheckBox();
            this.cbHideMissing = new System.Windows.Forms.CheckBox();
            this.cbShowMissing = new System.Windows.Forms.CheckBox();
            this.cbShowWorking = new System.Windows.Forms.CheckBox();
            this.cbShowFavoritesOnly = new System.Windows.Forms.CheckBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbShowNonWorking = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnAbout = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AllowColumnReorder = true;
            this.listView1.AllowDrop = true;
            this.listView1.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.ForeColor = System.Drawing.SystemColors.InactiveBorder;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(2, 102);
            this.listView1.Margin = new System.Windows.Forms.Padding(2);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1375, 494);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView1_ItemDrag);
            this.listView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView1_DragDrop);
            this.listView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView1_DragEnter);
            this.listView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDown);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tableLayoutPanel1.Controls.Add(this.listView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82.28783F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1379, 598);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.groupBox1.Controls.Add(this.btnAbout);
            this.groupBox1.Controls.Add(this.cbShowMultisOnly);
            this.groupBox1.Controls.Add(this.cbHideMissing);
            this.groupBox1.Controls.Add(this.cbShowMissing);
            this.groupBox1.Controls.Add(this.cbShowWorking);
            this.groupBox1.Controls.Add(this.cbShowFavoritesOnly);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbShowNonWorking);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(1375, 96);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // cbShowMultisOnly
            // 
            this.cbShowMultisOnly.AutoSize = true;
            this.cbShowMultisOnly.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cbShowMultisOnly.Location = new System.Drawing.Point(1212, 36);
            this.cbShowMultisOnly.Margin = new System.Windows.Forms.Padding(2);
            this.cbShowMultisOnly.Name = "cbShowMultisOnly";
            this.cbShowMultisOnly.Size = new System.Drawing.Size(137, 21);
            this.cbShowMultisOnly.TabIndex = 8;
            this.cbShowMultisOnly.Text = "Show Multis Only";
            this.cbShowMultisOnly.UseVisualStyleBackColor = true;
            this.cbShowMultisOnly.CheckedChanged += new System.EventHandler(this.cbShowMultisOnly_CheckedChanged);
            // 
            // cbHideMissing
            // 
            this.cbHideMissing.AutoSize = true;
            this.cbHideMissing.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cbHideMissing.Location = new System.Drawing.Point(1083, 36);
            this.cbHideMissing.Margin = new System.Windows.Forms.Padding(2);
            this.cbHideMissing.Name = "cbHideMissing";
            this.cbHideMissing.Size = new System.Drawing.Size(110, 21);
            this.cbHideMissing.TabIndex = 7;
            this.cbHideMissing.Text = "Hide Missing";
            this.cbHideMissing.UseVisualStyleBackColor = true;
            this.cbHideMissing.CheckedChanged += new System.EventHandler(this.cbHideMissing_CheckedChanged);
            // 
            // cbShowMissing
            // 
            this.cbShowMissing.AutoSize = true;
            this.cbShowMissing.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cbShowMissing.Location = new System.Drawing.Point(920, 36);
            this.cbShowMissing.Margin = new System.Windows.Forms.Padding(2);
            this.cbShowMissing.Name = "cbShowMissing";
            this.cbShowMissing.Size = new System.Drawing.Size(148, 21);
            this.cbShowMissing.TabIndex = 6;
            this.cbShowMissing.Text = "Show Missing Only";
            this.cbShowMissing.UseVisualStyleBackColor = true;
            this.cbShowMissing.CheckedChanged += new System.EventHandler(this.cbShowMissing_CheckedChanged);
            // 
            // cbShowWorking
            // 
            this.cbShowWorking.AutoSize = true;
            this.cbShowWorking.Checked = true;
            this.cbShowWorking.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowWorking.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cbShowWorking.Location = new System.Drawing.Point(430, 36);
            this.cbShowWorking.Name = "cbShowWorking";
            this.cbShowWorking.Size = new System.Drawing.Size(120, 21);
            this.cbShowWorking.TabIndex = 5;
            this.cbShowWorking.Text = "Show Working";
            this.cbShowWorking.UseVisualStyleBackColor = true;
            this.cbShowWorking.CheckedChanged += new System.EventHandler(this.cbShowWorking_CheckedChanged);
            // 
            // cbShowFavoritesOnly
            // 
            this.cbShowFavoritesOnly.AutoSize = true;
            this.cbShowFavoritesOnly.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cbShowFavoritesOnly.Location = new System.Drawing.Point(751, 36);
            this.cbShowFavoritesOnly.Name = "cbShowFavoritesOnly";
            this.cbShowFavoritesOnly.Size = new System.Drawing.Size(148, 21);
            this.cbShowFavoritesOnly.TabIndex = 4;
            this.cbShowFavoritesOnly.Text = "Show Starred Only";
            this.cbShowFavoritesOnly.UseVisualStyleBackColor = true;
            this.cbShowFavoritesOnly.CheckedChanged += new System.EventHandler(this.cbShowFavoritesOnly_CheckedChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(138, 64);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(20, 17);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(24, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Enter to Search:";
            // 
            // cbShowNonWorking
            // 
            this.cbShowNonWorking.AutoSize = true;
            this.cbShowNonWorking.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.cbShowNonWorking.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cbShowNonWorking.Location = new System.Drawing.Point(574, 36);
            this.cbShowNonWorking.Margin = new System.Windows.Forms.Padding(2);
            this.cbShowNonWorking.Name = "cbShowNonWorking";
            this.cbShowNonWorking.Size = new System.Drawing.Size(151, 21);
            this.cbShowNonWorking.TabIndex = 1;
            this.cbShowNonWorking.Text = "Show Non-Working";
            this.cbShowNonWorking.UseVisualStyleBackColor = false;
            this.cbShowNonWorking.CheckedChanged += new System.EventHandler(this.cbShowNonWorking_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.WindowText;
            this.textBox1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox1.Location = new System.Drawing.Point(138, 34);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(255, 22);
            this.textBox1.TabIndex = 0;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // btnAbout
            // 
            this.btnAbout.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAbout.Location = new System.Drawing.Point(27, 56);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(105, 31);
            this.btnAbout.TabIndex = 9;
            this.btnAbout.Text = "About/Help";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(1379, 598);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormMain";
            this.Text = "KonDB";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox cbShowNonWorking;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox cbShowFavoritesOnly;
        private System.Windows.Forms.CheckBox cbShowWorking;
        private System.Windows.Forms.CheckBox cbShowMissing;
        private System.Windows.Forms.CheckBox cbHideMissing;
        private System.Windows.Forms.CheckBox cbShowMultisOnly;
        private System.Windows.Forms.Button btnAbout;
    }
}

