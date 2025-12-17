using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Media
{
	/// <summary>
	/// Summary description for frmImporterDlg.
	/// </summary>
	public class frmImporterDlg : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox edDirectory;
        private System.Windows.Forms.Button cmdRead;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox edTMED_id;
        private System.Windows.Forms.TextBox edTMED_title;
        private System.Windows.Forms.ListView lsvTMED;
        private System.Windows.Forms.Button cmdInsert;
        private System.Windows.Forms.ColumnHeader colTENT_title;
        private System.Windows.Forms.CheckBox chkRecursive;
        private System.Windows.Forms.Button cmdFindMedia;

        private CmDbTabControl  m_mainForm;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox edExtension;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox edTMED;

        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmImporterDlg(CmDbTabControl p_mainForm)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            m_mainForm = p_mainForm;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.cmdRead = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.edDirectory = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.edTMED = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmdFindMedia = new System.Windows.Forms.Button();
            this.lsvTMED = new System.Windows.Forms.ListView();
            this.colTENT_title = new System.Windows.Forms.ColumnHeader();
            this.edTMED_title = new System.Windows.Forms.TextBox();
            this.edTMED_id = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdInsert = new System.Windows.Forms.Button();
            this.chkRecursive = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.edExtension = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdRead
            // 
            this.cmdRead.Location = new System.Drawing.Point(488, 16);
            this.cmdRead.Name = "cmdRead";
            this.cmdRead.Size = new System.Drawing.Size(104, 32);
            this.cmdRead.TabIndex = 3;
            this.cmdRead.Text = "Read!";
            this.cmdRead.Click += new System.EventHandler(this.cmdRead_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Directory ";
            // 
            // edDirectory
            // 
            this.edDirectory.Location = new System.Drawing.Point(88, 16);
            this.edDirectory.Name = "edDirectory";
            this.edDirectory.Size = new System.Drawing.Size(120, 20);
            this.edDirectory.TabIndex = 1;
            this.edDirectory.Text = "e:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.edTMED,
                                                                                    this.label5,
                                                                                    this.cmdFindMedia,
                                                                                    this.lsvTMED,
                                                                                    this.edTMED_title,
                                                                                    this.edTMED_id,
                                                                                    this.label3,
                                                                                    this.label2,
                                                                                    this.cmdInsert});
            this.groupBox1.Location = new System.Drawing.Point(8, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(848, 488);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data";
            // 
            // edTMED
            // 
            this.edTMED.Enabled = false;
            this.edTMED.Location = new System.Drawing.Point(56, 16);
            this.edTMED.Name = "edTMED";
            this.edTMED.Size = new System.Drawing.Size(80, 20);
            this.edTMED.TabIndex = 10;
            this.edTMED.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(16, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "TMED ";
            // 
            // cmdFindMedia
            // 
            this.cmdFindMedia.Location = new System.Drawing.Point(480, 16);
            this.cmdFindMedia.Name = "cmdFindMedia";
            this.cmdFindMedia.Size = new System.Drawing.Size(96, 32);
            this.cmdFindMedia.TabIndex = 5;
            this.cmdFindMedia.Text = "Media...";
            this.cmdFindMedia.Click += new System.EventHandler(this.cmdFindMedia_Click);
            // 
            // lsvTMED
            // 
            this.lsvTMED.CheckBoxes = true;
            this.lsvTMED.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                      this.colTENT_title});
            this.lsvTMED.FullRowSelect = true;
            this.lsvTMED.GridLines = true;
            this.lsvTMED.Location = new System.Drawing.Point(8, 112);
            this.lsvTMED.Name = "lsvTMED";
            this.lsvTMED.Size = new System.Drawing.Size(744, 368);
            this.lsvTMED.TabIndex = 4;
            this.lsvTMED.View = System.Windows.Forms.View.Details;
            // 
            // colTENT_title
            // 
            this.colTENT_title.Text = "Title";
            this.colTENT_title.Width = 600;
            // 
            // edTMED_title
            // 
            this.edTMED_title.Enabled = false;
            this.edTMED_title.Location = new System.Drawing.Point(56, 80);
            this.edTMED_title.Name = "edTMED_title";
            this.edTMED_title.Size = new System.Drawing.Size(384, 20);
            this.edTMED_title.TabIndex = 3;
            this.edTMED_title.Text = "";
            // 
            // edTMED_id
            // 
            this.edTMED_id.Enabled = false;
            this.edTMED_id.Location = new System.Drawing.Point(56, 48);
            this.edTMED_id.Name = "edTMED_id";
            this.edTMED_id.Size = new System.Drawing.Size(144, 20);
            this.edTMED_id.TabIndex = 2;
            this.edTMED_id.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(16, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Title";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(16, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "ID ";
            // 
            // cmdInsert
            // 
            this.cmdInsert.Enabled = false;
            this.cmdInsert.Location = new System.Drawing.Point(760, 112);
            this.cmdInsert.Name = "cmdInsert";
            this.cmdInsert.Size = new System.Drawing.Size(80, 32);
            this.cmdInsert.TabIndex = 6;
            this.cmdInsert.Text = "Insert into database";
            this.cmdInsert.Click += new System.EventHandler(this.cmdInsert_Click);
            // 
            // chkRecursive
            // 
            this.chkRecursive.Checked = true;
            this.chkRecursive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRecursive.Location = new System.Drawing.Point(488, 64);
            this.chkRecursive.Name = "chkRecursive";
            this.chkRecursive.Size = new System.Drawing.Size(128, 16);
            this.chkRecursive.TabIndex = 4;
            this.chkRecursive.Text = "Recursive";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(264, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Extension ";
            // 
            // edExtension
            // 
            this.edExtension.Location = new System.Drawing.Point(336, 16);
            this.edExtension.Name = "edExtension";
            this.edExtension.Size = new System.Drawing.Size(112, 20);
            this.edExtension.TabIndex = 2;
            this.edExtension.Text = "*.*";
            // 
            // frmImporterDlg
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(864, 581);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.edExtension,
                                                                          this.label4,
                                                                          this.label1,
                                                                          this.chkRecursive,
                                                                          this.groupBox1,
                                                                          this.edDirectory,
                                                                          this.cmdRead});
            this.Name = "frmImporterDlg";
            this.Text = "Dateien importieren";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void readDataForDirectory(string p_strDirectory, bool p_fRecursive, ref int p_nNo)
        {
            DirectoryInfo dir = new DirectoryInfo(p_strDirectory);

            if (p_fRecursive)
            {
                DirectoryInfo[] subDirectories = dir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirectories)
                {
                    readDataForDirectory(p_strDirectory + "\\" + subDir.Name, p_fRecursive, ref p_nNo);
                }
            }
                       
            FileInfo[] files = dir.GetFiles(edExtension.Text);
            foreach (FileInfo file in files)
            {
                readDataForFile(ref p_nNo, p_strDirectory + "\\" + file.Name);
                p_nNo++;
            }
        }

        private void readDataForFile(ref int p_nNo, string p_strFilename)
        {
            ListViewItem lvi = new ListViewItem(p_strFilename);
            lsvTMED.Items.Add(lvi);
        }

        private void cmdRead_Click(object sender, System.EventArgs e)
        {
            int i = 1;
            lsvTMED.Items.Clear();
            lsvTMED.BeginUpdate();
            try
            {
                readDataForDirectory(edDirectory.Text, chkRecursive.Checked, ref i);
            }
            catch(Exception)
            {
            }
            lsvTMED.EndUpdate();
        }

        private void cmdFindMedia_Click(object sender, System.EventArgs e)
        {
            frmFindMediaDlg findDlg = new frmFindMediaDlg(m_mainForm.m_sqlConnection, m_mainForm.m_sqlCommand, m_mainForm.m_sqlDataReader);
            DialogResult result = findDlg.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                int nTMED = findDlg.TMED;
                string[] arValues = Db.TMEDGetValuesForPk(nTMED, m_mainForm.m_sqlCommand);
                
                edTMED.Text = nTMED.ToString();
                edTMED_id.Text = arValues[1];
                edTMED_title.Text = arValues[2];

                cmdInsert.Enabled = true;
            }
        }

        private void cmdInsert_Click(object sender, System.EventArgs e)
        {
            cmdInsert.Enabled = false;

            int nTMED = Int32.Parse(edTMED.Text);
            int nTENT_no = Db.GetNextIntValueForIntPk("tent", "tent_no", "tmed", nTMED, m_mainForm.m_sqlCommand);

            SqlTransaction transaction = m_mainForm.m_sqlConnection.BeginTransaction();
            m_mainForm.m_sqlCommand.Transaction = transaction;

            try
            {
                foreach (ListViewItem lvi in lsvTMED.Items)
                {
                    int nTENT = Util.getNextMaxValueForTable(m_mainForm.m_sqlConnection, m_mainForm.m_sqlCommand, "tent");
                    m_mainForm.m_sqlCommand.CommandText = "INSERT INTO tent (tent, tent_no, tent_title, tmed)"
                        + " VALUES (" 
                        + nTENT
                        + ", " + nTENT_no
                        + ", '" + Db.processQuotes(lvi.Text) + "'"
                        + ", " + nTMED + ")";
                    m_mainForm.m_sqlCommand.ExecuteNonQuery();
                    nTENT_no++;
                }
                transaction.Commit();
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show("Error executing expression '" + m_mainForm.m_sqlCommand.CommandText
                    + ex.ToString());
            }
            finally
            {
                cmdInsert.Enabled = true;
            }
        }
	}
}

