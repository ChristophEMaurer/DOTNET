namespace Sudoku
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdStep = new System.Windows.Forms.Button();
            this.txtPossible = new System.Windows.Forms.TextBox();
            this.cmdRun = new System.Windows.Forms.Button();
            this.lblSolution = new System.Windows.Forms.Label();
            this.lstSolution = new System.Windows.Forms.ListBox();
            this.chkPossible = new System.Windows.Forms.CheckBox();
            this.chkHistory = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(588, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.resetToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // cmdStep
            // 
            this.cmdStep.Location = new System.Drawing.Point(473, 48);
            this.cmdStep.Name = "cmdStep";
            this.cmdStep.Size = new System.Drawing.Size(75, 23);
            this.cmdStep.TabIndex = 1;
            this.cmdStep.Text = "Cheat once";
            this.cmdStep.Click += new System.EventHandler(this.cmdStep_Click);
            // 
            // txtPossible
            // 
            this.txtPossible.Location = new System.Drawing.Point(444, 150);
            this.txtPossible.Name = "txtPossible";
            this.txtPossible.ReadOnly = true;
            this.txtPossible.Size = new System.Drawing.Size(131, 20);
            this.txtPossible.TabIndex = 2;
            // 
            // cmdRun
            // 
            this.cmdRun.Location = new System.Drawing.Point(473, 78);
            this.cmdRun.Name = "cmdRun";
            this.cmdRun.Size = new System.Drawing.Size(75, 23);
            this.cmdRun.TabIndex = 4;
            this.cmdRun.Text = "Solve";
            this.cmdRun.Click += new System.EventHandler(this.cmdRun_Click);
            // 
            // lblSolution
            // 
            this.lblSolution.AutoSize = true;
            this.lblSolution.Location = new System.Drawing.Point(441, 239);
            this.lblSolution.Name = "lblSolution";
            this.lblSolution.Size = new System.Drawing.Size(42, 13);
            this.lblSolution.TabIndex = 5;
            this.lblSolution.Text = "History:";
            // 
            // lstSolution
            // 
            this.lstSolution.FormattingEnabled = true;
            this.lstSolution.Location = new System.Drawing.Point(444, 264);
            this.lstSolution.Name = "lstSolution";
            this.lstSolution.Size = new System.Drawing.Size(132, 212);
            this.lstSolution.TabIndex = 7;
            // 
            // chkPossible
            // 
            this.chkPossible.AutoSize = true;
            this.chkPossible.Checked = true;
            this.chkPossible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPossible.Location = new System.Drawing.Point(444, 127);
            this.chkPossible.Name = "chkPossible";
            this.chkPossible.Size = new System.Drawing.Size(135, 17);
            this.chkPossible.TabIndex = 8;
            this.chkPossible.Text = "Display possible values";
            this.chkPossible.CheckedChanged += new System.EventHandler(this.chkPossible_CheckedChanged);
            // 
            // chkHistory
            // 
            this.chkHistory.AutoSize = true;
            this.chkHistory.Checked = true;
            this.chkHistory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHistory.Location = new System.Drawing.Point(444, 219);
            this.chkHistory.Name = "chkHistory";
            this.chkHistory.Size = new System.Drawing.Size(86, 17);
            this.chkHistory.TabIndex = 9;
            this.chkHistory.Text = "Show history";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 488);
            this.Controls.Add(this.chkHistory);
            this.Controls.Add(this.chkPossible);
            this.Controls.Add(this.lstSolution);
            this.Controls.Add(this.lblSolution);
            this.Controls.Add(this.cmdRun);
            this.Controls.Add(this.txtPossible);
            this.Controls.Add(this.cmdStep);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Christoph Maurer\'s Sudoku";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Click += new System.EventHandler(this.Form1_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Button cmdStep;
        private System.Windows.Forms.TextBox txtPossible;
        private System.Windows.Forms.Button cmdRun;
        private System.Windows.Forms.Label lblSolution;
        private System.Windows.Forms.ListBox lstSolution;
        private System.Windows.Forms.CheckBox chkPossible;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkHistory;
    }
}

