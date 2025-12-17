namespace cmaurer
{
    partial class Form1
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
            pbImage = new PictureBox();
            tbUniform = new TrackBar();
            lblValueUniform = new Label();
            cmdGrayscale = new Button();
            lblInfo = new Label();
            lblUniform = new Label();
            radUniform = new RadioButton();
            radExponential = new RadioButton();
            cmdMakeGrayscale = new Button();
            ((System.ComponentModel.ISupportInitialize)pbImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbUniform).BeginInit();
            SuspendLayout();
            // 
            // pbImage
            // 
            pbImage.BorderStyle = BorderStyle.Fixed3D;
            pbImage.Location = new Point(34, 40);
            pbImage.Margin = new Padding(3, 4, 3, 4);
            pbImage.Name = "pbImage";
            pbImage.Size = new Size(267, 316);
            pbImage.SizeMode = PictureBoxSizeMode.StretchImage;
            pbImage.TabIndex = 0;
            pbImage.TabStop = false;
            // 
            // tbUniform
            // 
            tbUniform.Location = new Point(160, 552);
            tbUniform.Margin = new Padding(3, 4, 3, 4);
            tbUniform.Maximum = 100;
            tbUniform.Name = "tbUniform";
            tbUniform.Size = new Size(455, 56);
            tbUniform.TabIndex = 2;
            tbUniform.Value = 1;
            tbUniform.Scroll += tbValue_Scroll;
            // 
            // lblValueUniform
            // 
            lblValueUniform.BorderStyle = BorderStyle.Fixed3D;
            lblValueUniform.Location = new Point(622, 568);
            lblValueUniform.Name = "lblValueUniform";
            lblValueUniform.Size = new Size(93, 31);
            lblValueUniform.TabIndex = 3;
            // 
            // cmdGrayscale
            // 
            cmdGrayscale.Location = new Point(570, 92);
            cmdGrayscale.Margin = new Padding(3, 4, 3, 4);
            cmdGrayscale.Name = "cmdGrayscale";
            cmdGrayscale.Size = new Size(144, 31);
            cmdGrayscale.TabIndex = 4;
            cmdGrayscale.Text = "Toggle grayscale";
            cmdGrayscale.UseVisualStyleBackColor = true;
            cmdGrayscale.Click += cmdGrayscale_Click;
            // 
            // lblInfo
            // 
            lblInfo.BorderStyle = BorderStyle.Fixed3D;
            lblInfo.Location = new Point(34, 387);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(600, 161);
            lblInfo.TabIndex = 5;
            // 
            // lblUniform
            // 
            lblUniform.BorderStyle = BorderStyle.Fixed3D;
            lblUniform.Location = new Point(46, 552);
            lblUniform.Name = "lblUniform";
            lblUniform.Size = new Size(93, 31);
            lblUniform.TabIndex = 6;
            // 
            // radUniform
            // 
            radUniform.AutoSize = true;
            radUniform.Checked = true;
            radUniform.Location = new Point(714, 385);
            radUniform.Margin = new Padding(3, 4, 3, 4);
            radUniform.Name = "radUniform";
            radUniform.Size = new Size(82, 24);
            radUniform.TabIndex = 7;
            radUniform.TabStop = true;
            radUniform.Text = "uniform";
            radUniform.UseVisualStyleBackColor = true;
            // 
            // radExponential
            // 
            radExponential.AutoSize = true;
            radExponential.Location = new Point(714, 419);
            radExponential.Margin = new Padding(3, 4, 3, 4);
            radExponential.Name = "radExponential";
            radExponential.Size = new Size(108, 24);
            radExponential.TabIndex = 8;
            radExponential.Text = "exponential";
            radExponential.UseVisualStyleBackColor = true;
            // 
            // cmdMakeGrayscale
            // 
            cmdMakeGrayscale.Location = new Point(570, 145);
            cmdMakeGrayscale.Margin = new Padding(3, 4, 3, 4);
            cmdMakeGrayscale.Name = "cmdMakeGrayscale";
            cmdMakeGrayscale.Size = new Size(144, 31);
            cmdMakeGrayscale.TabIndex = 9;
            cmdMakeGrayscale.Text = "Make grayscale";
            cmdMakeGrayscale.UseVisualStyleBackColor = true;
            cmdMakeGrayscale.Click += cmdMakeGrayscale_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 661);
            Controls.Add(cmdMakeGrayscale);
            Controls.Add(radExponential);
            Controls.Add(radUniform);
            Controls.Add(lblUniform);
            Controls.Add(lblInfo);
            Controls.Add(cmdGrayscale);
            Controls.Add(lblValueUniform);
            Controls.Add(tbUniform);
            Controls.Add(pbImage);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "BitmapTest";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pbImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbUniform).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pbImage;
        private TrackBar tbUniform;
        private Label lblValueUniform;
        private Button cmdGrayscale;
        private Label lblInfo;
        private Label lblUniform;
        private RadioButton radUniform;
        private RadioButton radExponential;
        private Button cmdMakeGrayscale;
    }
}
