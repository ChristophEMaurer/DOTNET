using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace Media
{
    public class frmEntries : CmDbTabPage
    {
        private GroupBox        grpTENT;
        private Label           lblTENT_TITLE;
        private TextBox         txtTENT_TITLE;
        private Label           lblTENT_SIDE;
        private TextBox         txtTENT_SIDE;
        private Label           lblTENT_NO;
        private TextBox         txtTENT_NO;
        private ListView        lstTENT;
        private ColumnHeader    colTENT;
        private ColumnHeader    colTENT_TMED;
        private ColumnHeader    colTENT_SIDE;
        private ColumnHeader    colTENT_NO;
        private ColumnHeader    colTENT_TITLE;
        private Button          cmdFind;
              
        private GroupBox        grpTMED;
        private ListView        lstTMED;
        private ColumnHeader    colTMED;
        private ColumnHeader    colDTYP_DESCRIPTION;
        private ColumnHeader    colTMED_ID;
        private ColumnHeader    colTART_NAME;
        private ColumnHeader    colTMED_TITLE;
        private ColumnHeader    colTMED_DATE;
                      
        public frmEntries(CmDbTabControl p_parent) : base(p_parent) 
        {
        }

        public override void InitializeComponent()
        {
            //int nWidth = 640;

            grpTENT = new GroupBox();
            lblTENT_TITLE = new CmLabel("Title");
            txtTENT_TITLE = new TextBox();
            lblTENT_SIDE = new CmLabel("Side");
            txtTENT_SIDE = new TextBox();
            lblTENT_NO = new CmLabel("No");
            txtTENT_NO = new TextBox();

            lstTENT = new ListView();
            colTENT = new ColumnHeader();
            colTENT_TMED = new ColumnHeader();
            colTENT_SIDE = new ColumnHeader();
            colTENT_NO = new ColumnHeader();
            colTENT_TITLE = new ColumnHeader();
            cmdFind = new CmButton("Find");

            grpTMED = new GroupBox();
            lstTMED = new ListView();
            colDTYP_DESCRIPTION = new ColumnHeader();
            colTMED = new ColumnHeader();
            colTMED_TITLE = new ColumnHeader();
            colTMED_ID = new ColumnHeader();
            colTMED_DATE = new ColumnHeader();
            colTART_NAME = new ColumnHeader();

            grpTENT.SuspendLayout();
            grpTMED.SuspendLayout();
            SuspendLayout();
           

            Controls.AddRange(new Control[] {
                                                grpTENT, grpTMED});


            // 
            // grpTENT
            // 
            grpTENT.Controls.AddRange(new Control[] {
                                                        txtTENT_NO,
                                                        lblTENT_NO,
                                                        txtTENT_SIDE,
                                                        lblTENT_SIDE,
                                                        cmdFind,
                                                        txtTENT_TITLE,
                                                        lblTENT_TITLE,
                                                        lstTENT});
            grpTENT.Location = new System.Drawing.Point(8, 16);
            grpTENT.Size = new System.Drawing.Size(m_nGrpWidth, 256);
            grpTENT.TabIndex = 0;
            grpTENT.TabStop = false;
            grpTENT.Text = "Entry";

            // 
            // txtTENT_NO
            // 
            txtTENT_NO.Location = new System.Drawing.Point(64, 88);
            txtTENT_NO.Size = new System.Drawing.Size(160, 20);
            txtTENT_NO.TabIndex = 6;

            // 
            // lblTENT_NO
            // 
            lblTENT_NO.Location = new System.Drawing.Point(16, 88);
            lblTENT_NO.TabIndex = 5;

            // 
            // txtTENT_SIDE
            // 
            txtTENT_SIDE.Location = new System.Drawing.Point(64, 56);
            txtTENT_SIDE.Size = new System.Drawing.Size(160, 20);
            txtTENT_SIDE.TabIndex = 4;

            // 
            // lblTENT_SIDE
            // 
            lblTENT_SIDE.Location = new System.Drawing.Point(16, 56);
            lblTENT_SIDE.TabIndex = 3;

            // 
            // cmdFind
            // 
            cmdFind.Location = new System.Drawing.Point(240, 24);
            cmdFind.TabIndex = 2;
            cmdFind.Click += new EventHandler(cmdFind_Click);


            // 
            // txtTENT_TITLE
            // 
            txtTENT_TITLE.Location = new System.Drawing.Point(64, 24);
            txtTENT_TITLE.Size = new System.Drawing.Size(160, 20);
            txtTENT_TITLE.TabIndex = 1;

            // 
            // lblTENT_TITLE
            // 
            lblTENT_TITLE.Location = new System.Drawing.Point(16, 24);
            lblTENT_TITLE.TabIndex = 0;

            // 
            // lstTENT
            // 
            lstTENT.Columns.AddRange(new ColumnHeader[] {
                                                            colTENT,
                                                            colTENT_TMED,
                                                            colTENT_SIDE,
                                                            colTENT_NO,
                                                            colTENT_TITLE});
            lstTENT.Location = new System.Drawing.Point(8, 120);
            lstTENT.MultiSelect = false;
            lstTENT.FullRowSelect = true;
            lstTENT.Size = new System.Drawing.Size(m_nGrpWidth - 16, 128);
            lstTENT.TabIndex = 1;
            lstTENT.View = View.Details;
            lstTENT.SelectedIndexChanged += new System.EventHandler(lstTENT_SelectedIndexChanged);
            

            // 
            // colTENT_SIDE
            // 
            colTENT_SIDE.Text = "Side";

            // 
            // colTENT_NO
            // 
            colTENT_NO.Text = "No";

            colTENT_TMED.Text = "TMED";

            // 
            // colTENT_TITLE
            // 
            colTENT_TITLE.Text = "Title";
            colTENT_TITLE.Width = 358;

            // 
            // lstTMED
            // 
            lstTMED.Columns.AddRange(new ColumnHeader[] {
                                                            colTMED,
                                                            colDTYP_DESCRIPTION,
                                                            colTMED_ID,
                                                            colTMED_TITLE,
                                                            colTMED_DATE,
                                                            colTART_NAME});
            lstTMED.Location = new System.Drawing.Point(8, 16);
            lstTMED.Size = new System.Drawing.Size(m_nGrpWidth - 16, 112);
            lstTMED.TabIndex = 4;
            lstTMED.View = View.Details;
            lstTMED.FullRowSelect = true;
            lstTMED.MultiSelect = false;

            // 
            // colTENT
            // 
            colTENT.Text = "TENT";

            // 
            // colTMED
            // 
            colTMED.Text = "TMED";
            colTMED.Width = 46;

            // 
            // colDTYP_DESCRIPTION
            // 
            colDTYP_DESCRIPTION.Text = "DTYP_description";
            colDTYP_DESCRIPTION.Width = 112;

            // 
            // colTMED_TITLE
            // 
            colTMED_TITLE.Text = "Title";
            colTMED_TITLE.Width = 126;

            // 
            // colTMED_ID
            // 
            colTMED_ID.Text = "Id";
            colTMED_ID.Width = 45;

            // 
            // colTMED_DATE
            // 
            colTMED_DATE.Text = "Date";

            // 
            // colTART_NAME
            // 
            colTART_NAME.Text = "Artist";
            colTART_NAME.Width = 157;

            // 
            // grpTMED
            // 
            grpTMED.Controls.AddRange(new Control[] {
                                                        lstTMED});
            grpTMED.Location = new System.Drawing.Point(8, 280);
            grpTMED.Size = new System.Drawing.Size(m_nGrpWidth, 144);
            grpTMED.TabIndex = 5;
            grpTMED.TabStop = false;
            grpTMED.Text = "Media";

            Text = "Entries";

            grpTENT.ResumeLayout(false);
            grpTMED.ResumeLayout(false);
            ResumeLayout(false);

        }
    
        private void cmdFind_Click(object sender, System.EventArgs e)
        {
            string  strSql;
            string  strWhere = "";
            bool    fHasWhere = false;
            bool    fHasWherePart = false;

            lstTENT.Items.Clear();
            lstTENT.BeginUpdate();

            strSql = "SELECT tent, tmed, tent_side, tent_no, tent_title FROM tent";
            
            if (txtTENT_TITLE.Text.Length > 0)
            {
                if (!fHasWhere)
                {
                    strWhere += " WHERE";
                    fHasWhere = true;
                }
                strWhere += " tent_title LIKE '%" + txtTENT_TITLE.Text + "%'";
                fHasWherePart = true;
            }
            if (txtTENT_SIDE.Text.Length > 0)
            {
                if (!fHasWhere)
                {
                    strWhere += " WHERE";
                    fHasWhere = true;
                }
                if (fHasWherePart)
                {
                    strWhere += " AND";
                }
                strWhere += " tent_side LIKE '%" + txtTENT_SIDE.Text + "%'";
                fHasWherePart = true;
            }
            if (txtTENT_NO.Text.Length > 0)
            {
                if (!fHasWhere)
                {
                    strWhere += " WHERE";
                    fHasWhere = true;
                }
                if (fHasWherePart)
                {
                    strWhere += " AND";
                }
                strWhere += " tent_no LIKE " + txtTENT_NO.Text;
                fHasWherePart = true;
            }

            if (fHasWhere)
            {
                strSql += strWhere;
            }

            m_parent.m_sqlCommand.CommandText = strSql + " ORDER BY tent_title";

            SqlDataReader sqlDataReader = m_parent.m_sqlCommand.ExecuteReader();

            //
            // collect all different TMED values
            // while looping through all TENTs that match the filter settings
            //
            Hashtable htTMED = new Hashtable();

            while (sqlDataReader.Read())
            {
                int nTMED = (int) Util.getValueFromSqlDataReader(sqlDataReader, 1);
                if (!htTMED.ContainsKey(nTMED))
                {
                    htTMED.Add(nTMED, nTMED);
                }

                ListViewItem lvi = new ListViewItem(Util.getValueFromSqlDataReader(sqlDataReader, 0).ToString());
                lstTENT.Items.Add(lvi);
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

            lstTENT.EndUpdate();
            TMEDPopulate(htTMED);
        }

        private void lstTENT_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            TENTRowSelected();
        }

        /*
         * Get the TMED value of the selected row, find the 
         * corresponding entry in the TMED listview and highlight that row
         */ 
        private void TENTRowSelected()
        {
            ListView.SelectedListViewItemCollection lic = lstTENT.SelectedItems;
            if (lic.Count == 1)
            {
                string strTENT_TMED = lic[0].SubItems[1].Text;
                foreach (ListViewItem lvi in lstTMED.Items)
                {
                    string strTMED_TMED = lvi.SubItems[0].Text;
                    if (strTENT_TMED.Equals(strTMED_TMED))
                    {
                        //
                        // set the selection to this item!!!
                        //
//                        OnSelectedIndexChanged(new EventArgs());
                    }
                }
            }
        }

        private void TMEDPopulate(Hashtable p_htTMED)
        {
            lstTMED.Items.Clear();
            lstTMED.BeginUpdate();

            foreach (DictionaryEntry e in p_htTMED)
            {
                string[] arTMEDValues = Db.TMEDGetValuesForPk((int) e.Key, m_parent.m_sqlCommand);

                ListViewItem lvi = new ListViewItem(e.Key.ToString());
                lstTMED.Items.Add(lvi);
                for (int i = 0; i < Db.NO_TMED_VALUES; i++)
                {
                        lvi.SubItems.Add(arTMEDValues[i]);
                }
            }

            lstTMED.EndUpdate();
        }
    }
}
