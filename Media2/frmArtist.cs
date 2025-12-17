using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Media
{
    public class frmArtist : CmDbTabPage
    {
        private CmDataGrid  m_dgTART;
        private DataTable   m_dtTART;

        private int         m_nTART;

        GroupBox    grpTART;
        CmLabel     lblTART_NAME;
        TextBox     txtTART_NAME;

        Button      cmdTART_Add;
        Button      cmdTART_Ok;
        Button      cmdTART_Delete;
        CheckBox    chkTART_Filter;

        public frmArtist(CmDbTabControl p_parent) : base(p_parent) 
        {
        }
        
        public override void InitializeComponent()
        {
            int nCol1 = 16;
            int nLabelOffset = 40;

            //
            // TART
            //
            m_dtTART = new CmDbDataTable("tart");
            Util.createAndInsertDataColumn(m_dtTART, "tart", typeof(int), true);
            Util.createAndInsertDataColumn(m_dtTART, "tart_name", typeof(string));

            m_dgTART = new CmDataGrid();
            m_dgTART.CaptionText = "Artist";
            m_dgTART.RowHeadersVisible = true;
            m_dgTART.Click += new System.EventHandler(dgTART_Click);
            m_dgTART.MouseUp += new MouseEventHandler(dgTART_MouseUp);
            Util.setDefaults(m_dgTART, m_dtTART);

            grpTART = new GroupBox();
            lblTART_NAME = new CmLabel("Name");
            txtTART_NAME = new TextBox();
           
            cmdTART_Add = new CmButton("Neu");
            cmdTART_Ok = new CmButton("Übernehmen");
            cmdTART_Delete = new CmButton("Löschen");
            chkTART_Filter = new CheckBox();

            // 
            // frmArtist
            // 
            Controls.AddRange(new Control[] {grpTART});

            Dock = DockStyle.Fill;
            Text = "Artist";

            // 
            // grpTMED
            // 
            grpTART.Controls.AddRange(new Control[] 
            {
                lblTART_NAME,
                txtTART_NAME,
                m_dgTART, 
                cmdTART_Add,
                cmdTART_Ok,
                cmdTART_Delete,
                chkTART_Filter});

            grpTART.Location = new Point(10, 10);
            grpTART.Size = new Size(m_nGrpWidth, 310);
            grpTART.TabIndex = 12;
            grpTART.TabStop = false;
            grpTART.Text = "Artist";

            //
            // m_dgTENT
            //
            m_dgTART.Location = new Point(10, 90);
            m_dgTART.Size = new Size(648, 208);
            m_dgTART.TabIndex = 1;

            // 
            // TMED_TITLE
            // 
            lblTART_NAME.Location = new Point(nCol1, 48);
            lblTART_NAME.TabIndex = 6;
            txtTART_NAME.Location = new Point(lblTART_NAME.Left + nLabelOffset, 48);
            txtTART_NAME.Size = new Size(200, 20);
            txtTART_NAME.TabIndex = 8;

            // 
            // cmdTART_Add
            // 
            cmdTART_Add.Location = new Point(672, 88);
            cmdTART_Add.Click += new EventHandler(cmdTART_Add_Click);
            
            // 
            // cmdTART_Ok
            // 
            cmdTART_Ok.Location = new Point(672, 120);
            cmdTART_Ok.Click += new EventHandler(cmdTART_Ok_Click);
            
            // 
            // cmdTART_Delete
            // 
            cmdTART_Delete.Location = new Point(672, 152);
            cmdTART_Delete.Click += new EventHandler(cmdTART_Delete_Click);

            // 
            // chkTMED_Filter
            // 
            chkTART_Filter.Location = new Point(672, 222);
            chkTART_Filter.Size = new Size(64, 24);
            chkTART_Filter.Text = "Filter";

            // 
            // frmArtist
            // 
            Text = "Artist";

            TARTButtonsXable(false);
        }

        private void dgTART_Click(object sender, System.EventArgs e)
        {
            TARTRowSelected();
        }

        private void TARTRowSelected()
        {
            int rowIndex = m_dgTART.CurrentRowIndex;
            if (rowIndex != -1)
            {
                m_dgTART.Select(rowIndex);
                m_nTART = (int) m_dgTART[rowIndex, 0];
                TARTSetDetails(rowIndex);
            }
        }

        public override void populate()
        {
            populateTART();
        }

        private void populateTART()
        {
            DataRow     row;
            int         i;
            string      strSql;
            string      strWhere = "";
            bool        fHasWhere = false;
            bool        fHasWherePart = false;

            m_dtTART.Clear();

            strSql = "SELECT tart, tart_name FROM tart";

            if (chkTART_Filter.Checked)
            {
                if (txtTART_NAME.Text.Length > 0)
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
                    strWhere += " tart_name LIKE '%" + txtTART_NAME.Text + "%'";
                    fHasWherePart = true;
                }
            }

            if (fHasWhere)
            {
                strSql += strWhere;
            }
            m_parent.m_sqlCommand.CommandText = strSql + " ORDER BY tart_name";
            m_parent.m_sqlDataReader = m_parent.m_sqlCommand.ExecuteReader();

            while (m_parent.m_sqlDataReader.Read())
            {
                row = m_dtTART.NewRow();

                for (i = 0; i < m_parent.m_sqlDataReader.FieldCount; i++)
                {
                    row[i] = Util.getValueFromSqlDataReader(m_parent.m_sqlDataReader, i, true);
                }

                m_dtTART.Rows.Add(row);
            }
            m_parent.m_sqlDataReader.Close();
            m_dtTART.AcceptChanges();
            m_dgTART.CurrentRowIndex = 0;

            setColumnsTART();
        }

        private void setColumnsTART()
        {
            ColumnDescription[] arCols= new ColumnDescription[2];
            int i = 0;

            arCols[i++] = new ColumnDescription(true);              // tart
            arCols[i++] = new ColumnDescription("Name", 200);       // tart_name

            Util.setColumns(m_parent.m_sqlCommand, m_dgTART, m_dtTART, arCols);
        }

        private void  TARTDetailsClear()
        {
            txtTART_NAME.Text = "";
        }

        private void TARTDetailsXable(bool p_fFlag)
        {
            txtTART_NAME.Enabled = p_fFlag;
        }

        private void TARTButtonsXable(bool p_fFlag)
        {
            cmdTART_Add.Enabled = p_fFlag;
            cmdTART_Ok.Enabled = p_fFlag;
            cmdTART_Delete.Enabled = p_fFlag;
        }    

        protected void cmdTART_Ok_Click(object p_sender, System.EventArgs p_e)
        {
            TARTAcceptChanges();
        }

        private void cmdTART_Add_Click(object p_sender, System.EventArgs p_e)
        {
            DataRow row = m_dtTART.NewRow();

            int m_nTART = Util.getNextMaxValueForTable(
                m_parent.m_sqlConnection, m_parent.m_sqlCommand, "tart");

            row["tart"] = m_nTART.ToString();
            txtTART_NAME.Text = "";
            row["tart_name"] = txtTART_NAME.Text;

            m_dtTART.Rows.Add(row);

            m_dgTART.selectRowByValue(0, row["tart"].ToString());

            TARTRowSelected();

            txtTART_NAME.Focus();
        }

        private void TARTAcceptChanges()
        {
            int rowIndex = m_dgTART.CurrentRowIndex;
            if (rowIndex != -1)
            {
                int nTART = (int) m_dgTART[rowIndex, 0];

                foreach (DataRow row in m_dtTART.Rows)
                {
                    if ((int) row["tart", Util.getDataRowVersion(row)] == nTART)
                    {
                        row["tart_name"] = txtTART_NAME.Text;
                        return;
                    }
                }
            }
        }

        private void TARTSetDetails(int p_nRowIndex)
        {
            txtTART_NAME.Text = Db.createSqlString(m_dgTART[p_nRowIndex, 1], true, true, false);
        }

        public override void save()
        {
            TARTSave();
        }
            
        protected void cmdTART_Delete_Click(object p_sender, System.EventArgs p_e)
        {
            int rowIndex = m_dgTART.CurrentRowIndex;
            if (rowIndex != -1)
            {
                int nTART = (int) m_dgTART[rowIndex, 0];

                foreach (DataRow row in m_dtTART.Rows)
                {
                    if ((int) row["tart", Util.getDataRowVersion(row)] == nTART)
                    {
                        row.Delete();
                        TARTRowSelected();
                        TARTSetDetails(m_dgTART.CurrentRowIndex);
                        return;
                    }
                }
            }
        }

        protected void cmdTARTSave_Click(object p_sender, System.EventArgs p_e)
        {
            TARTSave();
        }

        private void TARTSave()
        {
            bool fSuccess = true;

            foreach (DataRow row in m_dtTART.Rows)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        if (!Db.TARTInsert(m_parent.m_sqlCommand, row))
                        {
                            fSuccess = false;
                            break;
                        }
                        break;

                    case DataRowState.Modified:
                        if (!Db.TARTUpdate(m_parent.m_sqlCommand, row))
                        {
                            fSuccess = false;
                            break;
                        }
                        break;

                    case DataRowState.Deleted:
                        if (DialogResult.Yes != MessageBox.Show("Artist '" 
                            + row["tart_name", Util.getDataRowVersion(row)] + "' löschen?" ,
                            "Bestätigen", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            fSuccess = false;
                            break;
                        }
                        if (!Db.TARTDelete(m_parent.m_sqlCommand, row))
                        {
                            fSuccess = false;
                        }
                        break;
                }
                if (!fSuccess)
                {
                    break;
                }
            }
            if (fSuccess)
            {
                m_dtTART.AcceptChanges();
            }
            else
            {
                m_dtTART.RejectChanges();
            }
        }

        public override void edit(bool p_fIsEdit)
        {
            TARTDetailsClear();
            TARTButtonsXable(p_fIsEdit);
        }

        private void dgTART_MouseUp(object sender, MouseEventArgs e) 
        { 
            Point pt = new Point(e.X, e.Y); 
            DataGrid.HitTestInfo hti = m_dgTART.HitTest(pt); 

            if (hti.Type == DataGrid.HitTestType.Cell) 
            { 
                m_dgTART.CurrentCell = new DataGridCell(hti.Row, hti.Column); 
                m_dgTART.CurrentRowIndex = hti.Row;
                m_dgTART.Select(hti.Row);
                TARTRowSelected();
            } 
        }     
    }
}

