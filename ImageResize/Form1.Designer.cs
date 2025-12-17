namespace ImageResize
{
    partial class Form1
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
            this.X = new System.Windows.Forms.ListView();
            this.lblSource = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.cmdSource = new System.Windows.Forms.Button();
            this.cmdResizeSelected = new System.Windows.Forms.Button();
            this.cmdResizeAll = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.cmdFolder = new System.Windows.Forms.Button();
            this.chkFolder = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // X
            // 
            this.X.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.X.HideSelection = false;
            this.X.Location = new System.Drawing.Point(16, 126);
            this.X.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.X.Name = "X";
            this.X.Size = new System.Drawing.Size(923, 459);
            this.X.TabIndex = 0;
            this.X.UseCompatibleStateImageBehavior = false;
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(28, 10);
            this.lblSource.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(200, 16);
            this.lblSource.TabIndex = 1;
            this.lblSource.Text = "Input file name (output in d:\\temp)";
            // 
            // txtSource
            // 
            this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.Location = new System.Drawing.Point(31, 37);
            this.txtSource.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(800, 22);
            this.txtSource.TabIndex = 2;
            this.txtSource.Text = "D:\\temp\\main-640 x 960.png";
            // 
            // cmdSource
            // 
            this.cmdSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSource.Location = new System.Drawing.Point(840, 37);
            this.cmdSource.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdSource.Name = "cmdSource";
            this.cmdSource.Size = new System.Drawing.Size(100, 28);
            this.cmdSource.TabIndex = 3;
            this.cmdSource.Text = "...";
            this.cmdSource.UseVisualStyleBackColor = true;
            this.cmdSource.Click += new System.EventHandler(this.cmdSource_Click);
            // 
            // cmdResizeSelected
            // 
            this.cmdResizeSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdResizeSelected.Location = new System.Drawing.Point(16, 592);
            this.cmdResizeSelected.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdResizeSelected.Name = "cmdResizeSelected";
            this.cmdResizeSelected.Size = new System.Drawing.Size(359, 28);
            this.cmdResizeSelected.TabIndex = 4;
            this.cmdResizeSelected.Text = "Resize selected rows";
            this.cmdResizeSelected.UseVisualStyleBackColor = true;
            this.cmdResizeSelected.Click += new System.EventHandler(this.cmdResizeSelected_Click);
            // 
            // cmdResizeAll
            // 
            this.cmdResizeAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdResizeAll.Location = new System.Drawing.Point(582, 592);
            this.cmdResizeAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdResizeAll.Name = "cmdResizeAll";
            this.cmdResizeAll.Size = new System.Drawing.Size(359, 28);
            this.cmdResizeAll.TabIndex = 5;
            this.cmdResizeAll.Text = "Resize all rows";
            this.cmdResizeAll.UseVisualStyleBackColor = true;
            this.cmdResizeAll.Click += new System.EventHandler(this.cmdResizeAll_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolder.Location = new System.Drawing.Point(32, 94);
            this.txtFolder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(800, 22);
            this.txtFolder.TabIndex = 6;
            this.txtFolder.Text = "D:\\temp";
            // 
            // cmdFolder
            // 
            this.cmdFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdFolder.Location = new System.Drawing.Point(842, 90);
            this.cmdFolder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdFolder.Name = "cmdFolder";
            this.cmdFolder.Size = new System.Drawing.Size(100, 28);
            this.cmdFolder.TabIndex = 7;
            this.cmdFolder.Text = "...";
            this.cmdFolder.UseVisualStyleBackColor = true;
            this.cmdFolder.Click += new System.EventHandler(this.cmdFolder_Click);
            // 
            // chkFolder
            // 
            this.chkFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkFolder.AutoSize = true;
            this.chkFolder.Location = new System.Drawing.Point(400, 597);
            this.chkFolder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkFolder.Name = "chkFolder";
            this.chkFolder.Size = new System.Drawing.Size(157, 20);
            this.chkFolder.TabIndex = 8;
            this.chkFolder.Text = "Use folder (recursive)";
            this.chkFolder.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 74);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "Input folder (output in d:\\temp)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(956, 636);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkFolder);
            this.Controls.Add(this.cmdFolder);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.cmdResizeAll);
            this.Controls.Add(this.cmdResizeSelected);
            this.Controls.Add(this.cmdSource);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.X);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Resize for iTunes";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView X;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Button cmdSource;
        private System.Windows.Forms.Button cmdResizeSelected;
        private System.Windows.Forms.Button cmdResizeAll;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button cmdFolder;
        private System.Windows.Forms.CheckBox chkFolder;
        private System.Windows.Forms.Label label1;
    }
}

