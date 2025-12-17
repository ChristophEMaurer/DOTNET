using CMaurer.Common;

namespace Statistik
{

    partial class MainWindows : BaseForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindows));
            lvData = new SortableListView();
            cmdUpdate = new Button();
            pictureBox1 = new PictureBox();
            txtInfo = new TextBox();
            lblProgress = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // lvData
            // 
            lvData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvData.DoubleClickActivation = false;
            lvData.Location = new Point(16, 283);
            lvData.Margin = new Padding(4, 5, 4, 5);
            lvData.Name = "lvData";
            lvData.Size = new Size(1316, 716);
            lvData.Sortable = true;
            lvData.TabIndex = 0;
            lvData.UseCompatibleStateImageBehavior = false;
            lvData.View = View.Details;
            // 
            // cmdUpdate
            // 
            cmdUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdUpdate.Location = new Point(1181, 220);
            cmdUpdate.Margin = new Padding(4, 5, 4, 5);
            cmdUpdate.Name = "cmdUpdate";
            cmdUpdate.Size = new Size(152, 54);
            cmdUpdate.TabIndex = 1;
            cmdUpdate.Text = "Update";
            cmdUpdate.UseVisualStyleBackColor = true;
            cmdUpdate.Click += cmdUpdate_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(16, 22);
            pictureBox1.Margin = new Padding(4, 5, 4, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(219, 252);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // txtInfo
            // 
            txtInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtInfo.Font = new Font("Courier New", 8.25F);
            txtInfo.Location = new Point(243, 22);
            txtInfo.Margin = new Padding(4, 5, 4, 5);
            txtInfo.Multiline = true;
            txtInfo.Name = "txtInfo";
            txtInfo.ReadOnly = true;
            txtInfo.Size = new Size(1089, 189);
            txtInfo.TabIndex = 4;
            // 
            // lblProgress
            // 
            lblProgress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblProgress.BorderStyle = BorderStyle.Fixed3D;
            lblProgress.ForeColor = SystemColors.Highlight;
            lblProgress.Location = new Point(243, 235);
            lblProgress.Margin = new Padding(4, 0, 4, 0);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(931, 35);
            lblProgress.TabIndex = 5;
            // 
            // MainWindows
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1349, 1008);
            Controls.Add(lblProgress);
            Controls.Add(txtInfo);
            Controls.Add(pictureBox1);
            Controls.Add(cmdUpdate);
            Controls.Add(lvData);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 5, 4, 5);
            Name = "MainWindows";
            SizeGripStyle = SizeGripStyle.Show;
            Text = "Corona!";
            Load += Form1_Load;
            Shown += MainWindows_Shown;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private SortableListView lvData;
        private System.Windows.Forms.Button cmdUpdate;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Label lblProgress;
    }
}

