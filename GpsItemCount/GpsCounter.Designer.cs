namespace GpsItemCount
{
    partial class frmMain
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
            this.lvData = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdCount = new System.Windows.Forms.Button();
            this.cbFolders = new System.Windows.Forms.ComboBox();
            this.cmdOpenInNotepad = new System.Windows.Forms.Button();
            this.chkRecurse = new System.Windows.Forms.CheckBox();
            this.txtTrkPts = new System.Windows.Forms.TextBox();
            this.cmdCreateGpx = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblGpxTestInfo = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblNotepadInfo = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtStop = new System.Windows.Forms.TextBox();
            this.chkStop = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvData
            // 
            this.lvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvData.Location = new System.Drawing.Point(3, 16);
            this.lvData.Name = "lvData";
            this.lvData.Size = new System.Drawing.Size(967, 483);
            this.lvData.TabIndex = 0;
            this.lvData.UseCompatibleStateImageBehavior = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lvData);
            this.groupBox1.Location = new System.Drawing.Point(3, 165);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(973, 502);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // cmdCount
            // 
            this.cmdCount.Location = new System.Drawing.Point(6, 42);
            this.cmdCount.Name = "cmdCount";
            this.cmdCount.Size = new System.Drawing.Size(230, 26);
            this.cmdCount.TabIndex = 3;
            this.cmdCount.Text = "Count track points in *.gpx files";
            this.cmdCount.UseVisualStyleBackColor = true;
            this.cmdCount.Click += new System.EventHandler(this.cmdCount_Click);
            // 
            // cbFolders
            // 
            this.cbFolders.FormattingEnabled = true;
            this.cbFolders.Location = new System.Drawing.Point(12, 25);
            this.cbFolders.Name = "cbFolders";
            this.cbFolders.Size = new System.Drawing.Size(953, 21);
            this.cbFolders.TabIndex = 4;
            // 
            // cmdOpenInNotepad
            // 
            this.cmdOpenInNotepad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdOpenInNotepad.Location = new System.Drawing.Point(6, 78);
            this.cmdOpenInNotepad.Name = "cmdOpenInNotepad";
            this.cmdOpenInNotepad.Size = new System.Drawing.Size(166, 23);
            this.cmdOpenInNotepad.TabIndex = 5;
            this.cmdOpenInNotepad.Text = "Open list below in Notepad...";
            this.cmdOpenInNotepad.UseVisualStyleBackColor = true;
            this.cmdOpenInNotepad.Click += new System.EventHandler(this.cmdOpenInNotepad_Click);
            // 
            // chkRecurse
            // 
            this.chkRecurse.AutoSize = true;
            this.chkRecurse.Checked = true;
            this.chkRecurse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRecurse.Location = new System.Drawing.Point(6, 19);
            this.chkRecurse.Name = "chkRecurse";
            this.chkRecurse.Size = new System.Drawing.Size(137, 17);
            this.chkRecurse.TabIndex = 6;
            this.chkRecurse.Text = "Recurse into subfolders";
            this.chkRecurse.UseVisualStyleBackColor = true;
            // 
            // txtTrkPts
            // 
            this.txtTrkPts.Location = new System.Drawing.Point(172, 39);
            this.txtTrkPts.Name = "txtTrkPts";
            this.txtTrkPts.Size = new System.Drawing.Size(100, 20);
            this.txtTrkPts.TabIndex = 7;
            this.txtTrkPts.Text = "50";
            // 
            // cmdCreateGpx
            // 
            this.cmdCreateGpx.Location = new System.Drawing.Point(172, 70);
            this.cmdCreateGpx.Name = "cmdCreateGpx";
            this.cmdCreateGpx.Size = new System.Drawing.Size(172, 23);
            this.cmdCreateGpx.TabIndex = 8;
            this.cmdCreateGpx.Text = "Create dummy .gpx file";
            this.cmdCreateGpx.UseVisualStyleBackColor = true;
            this.cmdCreateGpx.Click += new System.EventHandler(this.cmdCreateGpx_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(169, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Number of track points in file:";
            // 
            // lblGpxTestInfo
            // 
            this.lblGpxTestInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblGpxTestInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblGpxTestInfo.Location = new System.Drawing.Point(8, 23);
            this.lblGpxTestInfo.Name = "lblGpxTestInfo";
            this.lblGpxTestInfo.Size = new System.Drawing.Size(155, 70);
            this.lblGpxTestInfo.TabIndex = 10;
            this.lblGpxTestInfo.Text = "Number of track points in file:";
            this.lblGpxTestInfo.Click += new System.EventHandler(this.label2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.chkStop);
            this.groupBox2.Controls.Add(this.txtStop);
            this.groupBox2.Controls.Add(this.chkRecurse);
            this.groupBox2.Controls.Add(this.cmdCount);
            this.groupBox2.Location = new System.Drawing.Point(12, 52);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(242, 107);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.cmdCreateGpx);
            this.groupBox3.Controls.Add(this.txtTrkPts);
            this.groupBox3.Controls.Add(this.lblGpxTestInfo);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(607, 52);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(366, 107);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = ".gpx test file";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Folder";
            // 
            // lblNotepadInfo
            // 
            this.lblNotepadInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNotepadInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNotepadInfo.Location = new System.Drawing.Point(6, 16);
            this.lblNotepadInfo.Name = "lblNotepadInfo";
            this.lblNotepadInfo.Size = new System.Drawing.Size(167, 59);
            this.lblNotepadInfo.TabIndex = 14;
            this.lblNotepadInfo.Text = "Number of track points in file:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmdOpenInNotepad);
            this.groupBox4.Controls.Add(this.lblNotepadInfo);
            this.groupBox4.Location = new System.Drawing.Point(260, 52);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(179, 107);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            // 
            // txtStop
            // 
            this.txtStop.Location = new System.Drawing.Point(84, 78);
            this.txtStop.Name = "txtStop";
            this.txtStop.Size = new System.Drawing.Size(48, 20);
            this.txtStop.TabIndex = 11;
            this.txtStop.Text = "1";
            // 
            // chkStop
            // 
            this.chkStop.AutoSize = true;
            this.chkStop.Location = new System.Drawing.Point(6, 78);
            this.chkStop.Name = "chkStop";
            this.chkStop.Size = new System.Drawing.Size(72, 17);
            this.chkStop.TabIndex = 12;
            this.chkStop.Text = "Stop after";
            this.chkStop.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(138, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "tracks";
            // 
            // frmMain
            // 
            this.AcceptButton = this.cmdCount;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 669);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cbFolders);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmMain";
            this.Text = "GPS track point counter v1.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdCount;
        private System.Windows.Forms.ComboBox cbFolders;
        private System.Windows.Forms.Button cmdOpenInNotepad;
        private System.Windows.Forms.CheckBox chkRecurse;
        private System.Windows.Forms.TextBox txtTrkPts;
        private System.Windows.Forms.Button cmdCreateGpx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblGpxTestInfo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblNotepadInfo;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkStop;
        private System.Windows.Forms.TextBox txtStop;
    }
}

