namespace ZipItemCount
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
            this.txtExtensions = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkIncludeFiles = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtExclude = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvData
            // 
            this.lvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvData.Location = new System.Drawing.Point(3, 16);
            this.lvData.Name = "lvData";
            this.lvData.Size = new System.Drawing.Size(967, 418);
            this.lvData.TabIndex = 0;
            this.lvData.UseCompatibleStateImageBehavior = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lvData);
            this.groupBox1.Location = new System.Drawing.Point(3, 155);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(973, 437);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data";
            // 
            // cmdCount
            // 
            this.cmdCount.Location = new System.Drawing.Point(12, 49);
            this.cmdCount.Name = "cmdCount";
            this.cmdCount.Size = new System.Drawing.Size(75, 23);
            this.cmdCount.TabIndex = 3;
            this.cmdCount.Text = "Count";
            this.cmdCount.UseVisualStyleBackColor = true;
            this.cmdCount.Click += new System.EventHandler(this.cmdCount_Click);
            // 
            // cbFolders
            // 
            this.cbFolders.FormattingEnabled = true;
            this.cbFolders.Location = new System.Drawing.Point(12, 12);
            this.cbFolders.Name = "cbFolders";
            this.cbFolders.Size = new System.Drawing.Size(935, 21);
            this.cbFolders.TabIndex = 4;
            // 
            // cmdOpenInNotepad
            // 
            this.cmdOpenInNotepad.Location = new System.Drawing.Point(505, 49);
            this.cmdOpenInNotepad.Name = "cmdOpenInNotepad";
            this.cmdOpenInNotepad.Size = new System.Drawing.Size(201, 23);
            this.cmdOpenInNotepad.TabIndex = 5;
            this.cmdOpenInNotepad.Text = "Open in Notepad...";
            this.cmdOpenInNotepad.UseVisualStyleBackColor = true;
            this.cmdOpenInNotepad.Click += new System.EventHandler(this.cmdOpenInNotepad_Click);
            // 
            // chkRecurse
            // 
            this.chkRecurse.AutoSize = true;
            this.chkRecurse.Location = new System.Drawing.Point(6, 19);
            this.chkRecurse.Name = "chkRecurse";
            this.chkRecurse.Size = new System.Drawing.Size(137, 17);
            this.chkRecurse.TabIndex = 6;
            this.chkRecurse.Text = "Recurse into subfolders";
            this.chkRecurse.UseVisualStyleBackColor = true;
            // 
            // txtExtensions
            // 
            this.txtExtensions.Location = new System.Drawing.Point(9, 59);
            this.txtExtensions.Name = "txtExtensions";
            this.txtExtensions.Size = new System.Drawing.Size(100, 20);
            this.txtExtensions.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "File extensions:";
            // 
            // chkIncludeFiles
            // 
            this.chkIncludeFiles.AutoSize = true;
            this.chkIncludeFiles.Location = new System.Drawing.Point(6, 19);
            this.chkIncludeFiles.Name = "chkIncludeFiles";
            this.chkIncludeFiles.Size = new System.Drawing.Size(131, 17);
            this.chkIncludeFiles.TabIndex = 9;
            this.chkIncludeFiles.Text = "Include files in archive";
            this.chkIncludeFiles.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Exclude files containing this text:";
            // 
            // txtExclude
            // 
            this.txtExclude.Location = new System.Drawing.Point(37, 65);
            this.txtExclude.Name = "txtExclude";
            this.txtExclude.Size = new System.Drawing.Size(129, 20);
            this.txtExclude.TabIndex = 11;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkIncludeFiles);
            this.groupBox2.Controls.Add(this.txtExclude);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(290, 49);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 100);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Files in archive";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkRecurse);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.txtExtensions);
            this.groupBox3.Location = new System.Drawing.Point(103, 49);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(181, 100);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Archive";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 594);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cmdOpenInNotepad);
            this.Controls.Add(this.cbFolders);
            this.Controls.Add(this.cmdCount);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmMain";
            this.Text = "Zip archive file counter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdCount;
        private System.Windows.Forms.ComboBox cbFolders;
        private System.Windows.Forms.Button cmdOpenInNotepad;
        private System.Windows.Forms.CheckBox chkRecurse;
        private System.Windows.Forms.TextBox txtExtensions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkIncludeFiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtExclude;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

