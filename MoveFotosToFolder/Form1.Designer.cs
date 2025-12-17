namespace MoveFotosToFolder
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
            lvData = new ListView();
            SuspendLayout();
            // 
            // lvData
            // 
            lvData.Dock = DockStyle.Fill;
            lvData.FullRowSelect = true;
            lvData.Location = new Point(0, 0);
            lvData.Name = "lvData";
            lvData.Size = new Size(800, 450);
            lvData.TabIndex = 0;
            lvData.UseCompatibleStateImageBehavior = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lvData);
            Name = "Form1";
            Text = "MoveFotosToFolder";
            Load += Form1_Load;
            Shown += Form1_Shown;
            ResumeLayout(false);
        }

        #endregion

        private ListView lvData;
    }
}
