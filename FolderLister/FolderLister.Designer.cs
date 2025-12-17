namespace AndroidFolderLister
{
    partial class FolderLister
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
            this.lblFolder = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lvData = new System.Windows.Forms.ListView();
            this.cmdRead = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtExclude = new System.Windows.Forms.TextBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(16, 91);
            this.lblFolder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(46, 16);
            this.lblFolder.TabIndex = 0;
            this.lblFolder.Text = "Folder";
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(88, 87);
            this.txtPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(824, 22);
            this.txtPath.TabIndex = 1;
            this.txtPath.Text = "d:\\daten\\mp3";
            // 
            // lvData
            // 
            this.lvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvData.HideSelection = false;
            this.lvData.Location = new System.Drawing.Point(16, 174);
            this.lvData.Margin = new System.Windows.Forms.Padding(4);
            this.lvData.Name = "lvData";
            this.lvData.Size = new System.Drawing.Size(1010, 334);
            this.lvData.TabIndex = 2;
            this.lvData.UseCompatibleStateImageBehavior = false;
            // 
            // cmdRead
            // 
            this.cmdRead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRead.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdRead.Location = new System.Drawing.Point(927, 85);
            this.cmdRead.Margin = new System.Windows.Forms.Padding(4);
            this.cmdRead.Name = "cmdRead";
            this.cmdRead.Size = new System.Drawing.Size(100, 28);
            this.cmdRead.TabIndex = 3;
            this.cmdRead.Text = "Read";
            this.cmdRead.UseVisualStyleBackColor = true;
            this.cmdRead.Click += new System.EventHandler(this.cmdRead_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 131);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Exclude (separate by comma \',\')";
            // 
            // txtExclude
            // 
            this.txtExclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExclude.Location = new System.Drawing.Point(232, 127);
            this.txtExclude.Margin = new System.Windows.Forms.Padding(4);
            this.txtExclude.Name = "txtExclude";
            this.txtExclude.Size = new System.Drawing.Size(794, 22);
            this.txtExclude.TabIndex = 5;
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblInfo.Location = new System.Drawing.Point(16, 9);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(1011, 69);
            this.lblInfo.TabIndex = 6;
            this.lblInfo.Text = "lblInfo";
            // 
            // FolderLister
            // 
            this.AcceptButton = this.cmdRead;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1043, 522);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.txtExclude);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdRead);
            this.Controls.Add(this.lvData);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lblFolder);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FolderLister";
            this.Text = "FolderLister";
            this.Load += new System.EventHandler(this.FolderLister_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.ListView lvData;
        private System.Windows.Forms.Button cmdRead;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExclude;
        private System.Windows.Forms.Label lblInfo;
    }
}