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
	public class frmFindArtistDlg : System.Windows.Forms.Form
	{
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdFind;
        private System.Windows.Forms.ListView lstArtist;

        private SqlConnection   m_sqlConnection;
        private SqlCommand      m_sqlCommand;

        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdCancel;

        private int m_nTART;
        private System.Windows.Forms.TextBox edTART_NAME;
        private System.Windows.Forms.ColumnHeader colTART_NAME;
        private System.Windows.Forms.ColumnHeader colTART;


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmFindArtistDlg(SqlConnection p_sqlConnection, SqlCommand p_sqlCommand)
		{
            m_sqlConnection = p_sqlConnection;
            m_sqlCommand = p_sqlCommand;
            m_nTART = -1;


			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

        public int TART
        {
            get { return m_nTART; }
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
            this.edTART_NAME = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdFind = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.lstArtist = new System.Windows.Forms.ListView();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.colTART_NAME = new System.Windows.Forms.ColumnHeader();
            this.colTART = new System.Windows.Forms.ColumnHeader();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.edTART_NAME,
                                                                                    this.label1});
            this.groupBox1.Location = new System.Drawing.Point(8, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 136);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Find Artist";
            // 
            // edTART_NAME
            // 
            this.edTART_NAME.Location = new System.Drawing.Point(80, 32);
            this.edTART_NAME.Name = "edTART_NAME";
            this.edTART_NAME.TabIndex = 2;
            this.edTART_NAME.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fullname ";
            // 
            // cmdFind
            // 
            this.cmdFind.Location = new System.Drawing.Point(224, 24);
            this.cmdFind.Name = "cmdFind";
            this.cmdFind.Size = new System.Drawing.Size(96, 32);
            this.cmdFind.TabIndex = 1;
            this.cmdFind.Text = "Find";
            this.cmdFind.Click += new System.EventHandler(this.cmdFind_Click);
            // 
            // cmdOk
            // 
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOk.Location = new System.Drawing.Point(16, 360);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(96, 32);
            this.cmdOk.TabIndex = 2;
            this.cmdOk.Text = "Ok";
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // lstArtist
            // 
            this.lstArtist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                       this.colTART,
                                                                                       this.colTART_NAME});
            this.lstArtist.FullRowSelect = true;
            this.lstArtist.GridLines = true;
            this.lstArtist.Location = new System.Drawing.Point(8, 160);
            this.lstArtist.Name = "lstArtist";
            this.lstArtist.Size = new System.Drawing.Size(432, 184);
            this.lstArtist.TabIndex = 3;
            this.lstArtist.View = System.Windows.Forms.View.Details;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(120, 360);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(96, 32);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "Cancel";
            // 
            // colTART_NAME
            // 
            this.colTART_NAME.Text = "Fullname";
            // 
            // colTART
            // 
            this.colTART.Text = "tart";
            // 
            // frmFindArtistDlg
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(510, 411);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.cmdCancel,
                                                                          this.lstArtist,
                                                                          this.cmdOk,
                                                                          this.cmdFind,
                                                                          this.groupBox1});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "frmFindArtistDlg";
            this.Text = "Find Artist";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void cmdFind_Click(object sender, System.EventArgs e)
        {
            lstArtist.Items.Clear();

            m_sqlCommand.CommandText = "SELECT tart, tart_name"
                + " FROM tart"
                + " WHERE tart_name LIKE '%" + edTART_NAME.Text + "%'"
                + " ORDER BY tart_name";
            SqlDataReader sqlDataReader = m_sqlCommand.ExecuteReader();

            while (sqlDataReader.Read())
            {
                ListViewItem lvi = new ListViewItem(Util.getValueFromSqlDataReader(sqlDataReader, 0).ToString());
                lstArtist.Items.Add(lvi);
                for (int i = 1; i < sqlDataReader.FieldCount; i++)
                {
                    string s;
                    if (!sqlDataReader.IsDBNull(i))
                    {
                        s = Util.getValueFromSqlDataReader(sqlDataReader, i).ToString();
                        lvi.SubItems.Add(s);
                    }
                    else
                    {
                        lvi.SubItems.Add("");
                    }

                }

            }
            sqlDataReader.Close();
        }

        private void cmdOk_Click(object sender, System.EventArgs e)
        {
            ListView.SelectedListViewItemCollection lic = lstArtist.SelectedItems;
            if (lic.Count == 1)
            {
                ListViewItem lvi = lic[0];
                m_nTART = Int32.Parse(lvi.Text);
            }


        }
	}
}
