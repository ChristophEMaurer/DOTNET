using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace Media
{
	/// <summary>
	/// Summary description for frmFindMediaDlg.
	/// </summary>
	public class frmFindMediaDlg : System.Windows.Forms.Form
	{
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox edId;
        private System.Windows.Forms.TextBox edTitle;
        private System.Windows.Forms.Button cmdFind;
        private System.Windows.Forms.ListView lstMedia;
        private System.Windows.Forms.ColumnHeader colTMED_ID;
        private System.Windows.Forms.ColumnHeader colTMED_TITLE;
        private System.Windows.Forms.ColumnHeader colTMED;

        private SqlConnection   m_sqlConnection;
        private SqlCommand      m_sqlCommand;
        private SqlDataReader   m_sqlDataReader;

        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdCancel;

        private int m_nTMED;


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmFindMediaDlg(SqlConnection p_sqlConnection, SqlCommand p_sqlCommand, SqlDataReader p_sqlDataReader)
		{
            m_sqlConnection = p_sqlConnection;
            m_sqlCommand = p_sqlCommand;
            m_sqlDataReader = p_sqlDataReader;
            m_nTMED = -1;


			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            //this.lstMedia.Columns[0].v
		}

        public int TMED
        {
            get { return m_nTMED; }
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.edTitle = new System.Windows.Forms.TextBox();
            this.edId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdFind = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.lstMedia = new System.Windows.Forms.ListView();
            this.colTMED = new System.Windows.Forms.ColumnHeader();
            this.colTMED_ID = new System.Windows.Forms.ColumnHeader();
            this.colTMED_TITLE = new System.Windows.Forms.ColumnHeader();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.edTitle,
                                                                                    this.edId,
                                                                                    this.label2,
                                                                                    this.label1});
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Find Media";
            // 
            // edTitle
            // 
            this.edTitle.Location = new System.Drawing.Point(48, 24);
            this.edTitle.Name = "edTitle";
            this.edTitle.TabIndex = 1;
            this.edTitle.Text = "";
            // 
            // edId
            // 
            this.edId.Location = new System.Drawing.Point(48, 56);
            this.edId.Name = "edId";
            this.edId.TabIndex = 2;
            this.edId.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Title";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Id ";
            // 
            // cmdFind
            // 
            this.cmdFind.Location = new System.Drawing.Point(224, 16);
            this.cmdFind.Name = "cmdFind";
            this.cmdFind.Size = new System.Drawing.Size(96, 32);
            this.cmdFind.TabIndex = 3;
            this.cmdFind.Text = "&Find";
            this.cmdFind.Click += new System.EventHandler(this.cmdFind_Click);
            // 
            // cmdOk
            // 
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOk.Location = new System.Drawing.Point(16, 456);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(96, 32);
            this.cmdOk.TabIndex = 5;
            this.cmdOk.Text = "&Ok";
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // lstMedia
            // 
            this.lstMedia.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                       this.colTMED,
                                                                                       this.colTMED_ID,
                                                                                       this.colTMED_TITLE});
            this.lstMedia.FullRowSelect = true;
            this.lstMedia.GridLines = true;
            this.lstMedia.Location = new System.Drawing.Point(16, 104);
            this.lstMedia.Name = "lstMedia";
            this.lstMedia.Size = new System.Drawing.Size(536, 344);
            this.lstMedia.TabIndex = 4;
            this.lstMedia.View = System.Windows.Forms.View.Details;
            this.lstMedia.SelectedIndexChanged += new System.EventHandler(this.lstMedia_SelectedIndexChanged);
            // 
            // colTMED
            // 
            this.colTMED.Text = "TMED";
            // 
            // colTMED_ID
            // 
            this.colTMED_ID.Text = "Id";
            // 
            // colTMED_TITLE
            // 
            this.colTMED_TITLE.Text = "Title";
            this.colTMED_TITLE.Width = 400;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(152, 456);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(96, 32);
            this.cmdCancel.TabIndex = 6;
            this.cmdCancel.Text = "&Cancel";
            // 
            // frmFindMediaDlg
            // 
            this.AcceptButton = this.cmdFind;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(558, 501);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.cmdCancel,
                                                                          this.lstMedia,
                                                                          this.cmdOk,
                                                                          this.cmdFind,
                                                                          this.groupBox1});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "frmFindMediaDlg";
            this.Text = "Find Media";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void cmdFind_Click(object sender, System.EventArgs e)
        {
            lstMedia.Items.Clear();

            m_sqlCommand.CommandText = "SELECT tmed, tmed_id, tmed_title"
                + " FROM tmed"
                + " WHERE tmed_id LIKE '%" + edId.Text + "%' AND tmed_title LIKE '%" + edTitle.Text + "%'"
                + " ORDER BY tmed_title";
            m_sqlDataReader = m_sqlCommand.ExecuteReader();

            while (m_sqlDataReader.Read())
            {
                ListViewItem lvi = new ListViewItem(Util.getValueFromSqlDataReader(m_sqlDataReader, 0).ToString());
                lstMedia.Items.Add(lvi);
                for (int i = 1; i < m_sqlDataReader.FieldCount; i++)
                {
                    string s;
                    if (!m_sqlDataReader.IsDBNull(i))
                    {
                        s = Util.getValueFromSqlDataReader(m_sqlDataReader, i).ToString();
                        lvi.SubItems.Add(s);
                    }
                    else
                    {
                        lvi.SubItems.Add("");
                    }

                }

            }
            m_sqlDataReader.Close();
        }

        private void cmdOk_Click(object sender, System.EventArgs e)
        {
            ListView.SelectedListViewItemCollection lic = lstMedia.SelectedItems;
            if (lic.Count == 1)
            {
                ListViewItem lvi = lic[0];
                m_nTMED = Int32.Parse(lvi.Text);
            }
        }

        private void lstMedia_SelectedIndexChanged(object sender, System.EventArgs e)
        {
        
        }
	}
}
