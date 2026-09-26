namespace CaiSiGpxGenerator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lvRegions = new ListView();
            cmdExtractRegions = new Button();
            txtBaseUrl = new TextBox();
            cmdExtractStageURLsForRegion = new Button();
            lvStages = new ListView();
            splitContainer1 = new SplitContainer();
            cmdExtractStageUrlsForAllRegions = new Button();
            cmdMoveFiles = new Button();
            txtFolderDownload = new TextBox();
            lblDownloadFolder = new Label();
            cmdCancel = new Button();
            lblProgress = new Label();
            lblOutputFolder = new Label();
            txtOutputFolder = new TextBox();
            cmdGenerateHtml = new Button();
            lblInfo = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // lvRegions
            // 
            lvRegions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvRegions.FullRowSelect = true;
            lvRegions.GridLines = true;
            lvRegions.Location = new Point(3, 43);
            lvRegions.Name = "lvRegions";
            lvRegions.Size = new Size(1129, 289);
            lvRegions.TabIndex = 1;
            lvRegions.UseCompatibleStateImageBehavior = false;
            lvRegions.SelectedIndexChanged += lvRegions_SelectedIndexChanged;
            // 
            // cmdExtractRegions
            // 
            cmdExtractRegions.Location = new Point(450, 9);
            cmdExtractRegions.Name = "cmdExtractRegions";
            cmdExtractRegions.Size = new Size(357, 29);
            cmdExtractRegions.TabIndex = 2;
            cmdExtractRegions.Text = "1 Extract regions from the url on the left";
            cmdExtractRegions.UseVisualStyleBackColor = true;
            cmdExtractRegions.Click += cmdExtractRegions_Click;
            // 
            // txtBaseUrl
            // 
            txtBaseUrl.Location = new Point(10, 9);
            txtBaseUrl.Name = "txtBaseUrl";
            txtBaseUrl.Size = new Size(434, 27);
            txtBaseUrl.TabIndex = 3;
            txtBaseUrl.Text = "https://sentieroitalia.cai.it/de/der-weg/etappenblaetter/";
            // 
            // cmdExtractStageURLsForRegion
            // 
            cmdExtractStageURLsForRegion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmdExtractStageURLsForRegion.Location = new Point(3, 337);
            cmdExtractStageURLsForRegion.Name = "cmdExtractStageURLsForRegion";
            cmdExtractStageURLsForRegion.Size = new Size(309, 29);
            cmdExtractStageURLsForRegion.TabIndex = 4;
            cmdExtractStageURLsForRegion.Text = "2 Extract stage URLs for selected region";
            cmdExtractStageURLsForRegion.UseVisualStyleBackColor = true;
            cmdExtractStageURLsForRegion.Click += cmdExtractStageURLsForRegion_Click;
            // 
            // lvStages
            // 
            lvStages.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvStages.Location = new Point(3, 3);
            lvStages.Name = "lvStages";
            lvStages.Size = new Size(1129, 274);
            lvStages.TabIndex = 5;
            lvStages.UseCompatibleStateImageBehavior = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(txtBaseUrl);
            splitContainer1.Panel1.Controls.Add(cmdExtractRegions);
            splitContainer1.Panel1.Controls.Add(lvRegions);
            splitContainer1.Panel1.Controls.Add(cmdExtractStageURLsForRegion);
            splitContainer1.Panel1.Controls.Add(cmdExtractStageUrlsForAllRegions);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(cmdMoveFiles);
            splitContainer1.Panel2.Controls.Add(txtFolderDownload);
            splitContainer1.Panel2.Controls.Add(lblDownloadFolder);
            splitContainer1.Panel2.Controls.Add(cmdCancel);
            splitContainer1.Panel2.Controls.Add(lblProgress);
            splitContainer1.Panel2.Controls.Add(lblOutputFolder);
            splitContainer1.Panel2.Controls.Add(txtOutputFolder);
            splitContainer1.Panel2.Controls.Add(cmdGenerateHtml);
            splitContainer1.Panel2.Controls.Add(lvStages);
            splitContainer1.Panel2.Paint += splitContainer1_Panel2_Paint;
            splitContainer1.Size = new Size(1138, 827);
            splitContainer1.SplitterDistance = 373;
            splitContainer1.SplitterWidth = 10;
            splitContainer1.TabIndex = 6;
            // 
            // cmdExtractStageUrlsForAllRegions
            // 
            cmdExtractStageUrlsForAllRegions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmdExtractStageUrlsForAllRegions.Location = new Point(318, 337);
            cmdExtractStageUrlsForAllRegions.Name = "cmdExtractStageUrlsForAllRegions";
            cmdExtractStageUrlsForAllRegions.Size = new Size(263, 29);
            cmdExtractStageUrlsForAllRegions.TabIndex = 6;
            cmdExtractStageUrlsForAllRegions.Text = "2 Extract stage URLs for all regions";
            cmdExtractStageUrlsForAllRegions.UseVisualStyleBackColor = true;
            cmdExtractStageUrlsForAllRegions.Click += cmdExtractStageURLsForAllRegions_Click;
            // 
            // cmdMoveFiles
            // 
            cmdMoveFiles.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdMoveFiles.Location = new Point(720, 318);
            cmdMoveFiles.Name = "cmdMoveFiles";
            cmdMoveFiles.Size = new Size(315, 39);
            cmdMoveFiles.TabIndex = 15;
            cmdMoveFiles.Text = "5 Move files from download to output folder";
            cmdMoveFiles.UseVisualStyleBackColor = true;
            cmdMoveFiles.Click += cmdMoveFiles_Click_1;
            // 
            // txtFolderDownload
            // 
            txtFolderDownload.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtFolderDownload.Location = new Point(178, 324);
            txtFolderDownload.Name = "txtFolderDownload";
            txtFolderDownload.Size = new Size(537, 27);
            txtFolderDownload.TabIndex = 14;
            txtFolderDownload.Text = "C:\\Users\\chmau\\Downloads";
            // 
            // lblDownloadFolder
            // 
            lblDownloadFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblDownloadFolder.Location = new Point(15, 326);
            lblDownloadFolder.Name = "lblDownloadFolder";
            lblDownloadFolder.Size = new Size(162, 25);
            lblDownloadFolder.TabIndex = 13;
            lblDownloadFolder.Text = "Folder with .gpx files:";
            // 
            // cmdCancel
            // 
            cmdCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdCancel.Location = new Point(1041, 303);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(90, 39);
            cmdCancel.TabIndex = 12;
            cmdCancel.Text = "Stop";
            cmdCancel.UseVisualStyleBackColor = true;
            cmdCancel.Click += cmdCancel_Click;
            // 
            // lblProgress
            // 
            lblProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblProgress.BackColor = SystemColors.Control;
            lblProgress.BorderStyle = BorderStyle.Fixed3D;
            lblProgress.ForeColor = SystemColors.Highlight;
            lblProgress.Location = new Point(15, 360);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(1021, 43);
            lblProgress.TabIndex = 11;
            lblProgress.Text = "Progress";
            // 
            // lblOutputFolder
            // 
            lblOutputFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblOutputFolder.Location = new Point(15, 288);
            lblOutputFolder.Name = "lblOutputFolder";
            lblOutputFolder.Size = new Size(125, 28);
            lblOutputFolder.TabIndex = 9;
            lblOutputFolder.Text = "Output folder:";
            // 
            // txtOutputFolder
            // 
            txtOutputFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtOutputFolder.Location = new Point(178, 286);
            txtOutputFolder.Name = "txtOutputFolder";
            txtOutputFolder.Size = new Size(537, 27);
            txtOutputFolder.TabIndex = 8;
            txtOutputFolder.Text = "c:\\temp";
            // 
            // cmdGenerateHtml
            // 
            cmdGenerateHtml.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdGenerateHtml.Location = new Point(720, 279);
            cmdGenerateHtml.Name = "cmdGenerateHtml";
            cmdGenerateHtml.Size = new Size(315, 39);
            cmdGenerateHtml.TabIndex = 7;
            cmdGenerateHtml.Text = "3 Generate si.html/.js files";
            cmdGenerateHtml.UseVisualStyleBackColor = true;
            cmdGenerateHtml.Click += cmdGenerateJsFiles_Click;
            // 
            // lblInfo
            // 
            lblInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblInfo.BackColor = SystemColors.Control;
            lblInfo.BorderStyle = BorderStyle.Fixed3D;
            lblInfo.ForeColor = SystemColors.Highlight;
            lblInfo.Location = new Point(1144, 11);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(283, 817);
            lblInfo.TabIndex = 12;
            lblInfo.Text = "Info";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1440, 839);
            Controls.Add(lblInfo);
            Controls.Add(splitContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            SizeGripStyle = SizeGripStyle.Show;
            Text = "CAI-Sentiero Italia - GPX download tool - Christoph Maurer";
            Load += Form1_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ListView lvRegions;
        private Button cmdExtractRegions;
        private TextBox txtBaseUrl;
        private Button cmdExtractStageURLsForRegion;
        private ListView lvStages;
        private SplitContainer splitContainer1;
        private Button cmdExtractStageUrlsForAllRegions;
        private Button cmdGenerateHtml;
        private TextBox txtOutputFolder;
        private Label lblOutputFolder;
        private Label lblProgress;
        private Button cmdCancel;
        private Button cmdMoveFiles;
        private TextBox txtFolderDownload;
        private Label lblDownloadFolder;
        private Label lblInfo;
    }
}
