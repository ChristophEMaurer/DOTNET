
namespace Media
{
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;

    public class frmDeftables : CmDbTabPage
    {
        private CmDataGrid  m_dgDEFTABLES;
        private DataTable   m_dtDEFTABLES;

        private CmDataGrid  m_dgDEFTABLE;
        private DataTable   m_dtDEFTABLE;

        private string m_strDEFTABLE;

        TextBox txtDEFTABLE_Id;
        TextBox txtDEFTABLE_Description;
        TextBox txtDEFTABLE_Order;

        CmButton cmdDEFTABLE_Add;
        CmButton cmdDEFTABLE_Ok;
        CmButton cmdDEFTABLE_Delete;
    
        public frmDeftables(CmDbTabControl p_parent) : base(p_parent) {}


        public override void InitializeComponent()
        {
            int nCol1 = 16;
            int nCol2 = 300;
            int nCol3 = 520;
            int nLabelOffset = 40;

            //
            // DEFTABLES
            //
            m_dtDEFTABLES = new CmDbDataTable("DEFTABLES");
            Util.createAndInsertDataColumn(m_dtDEFTABLES, "tablename", typeof(string), true);
            Util.createAndInsertDataColumn(m_dtDEFTABLES, "description", typeof(string), true);

            m_dgDEFTABLES = new CmDataGrid();
            m_dgDEFTABLES.Location = new Point(10, 90);
            m_dgDEFTABLES.Size = new Size(648, 208);
            m_dgDEFTABLES.CaptionText = "All DEFTABLES";
            m_dgDEFTABLES.Click += new System.EventHandler(dgDEFTABLES_Click);
            m_dgDEFTABLES.MouseUp += new MouseEventHandler(dgDEFTABLES_MouseUp);
            Util.setDefaults(m_dgDEFTABLES, m_dtDEFTABLES);

            //
            // DEFTABLE
            //
            m_dtDEFTABLE = new CmDbDataTable("deftable");
            Util.createAndInsertDataColumn(m_dtDEFTABLE, "pk", typeof(int), true);
            Util.createAndInsertDataColumn(m_dtDEFTABLE, "id", typeof(string), Db.COL_LEN_DEF_ID);
            Util.createAndInsertDataColumn(m_dtDEFTABLE, "description", typeof(string), Db.COL_LEN_DEF_DESCRIPTION);
            Util.createAndInsertDataColumn(m_dtDEFTABLE, "order", typeof(string), Db.COL_LEN_DEF_ORDER);

            m_dgDEFTABLE = new CmDataGrid();
            m_dgDEFTABLE.Location = new Point(8, 88);
            m_dgDEFTABLE.Size = new Size(648, 208);
            m_dgDEFTABLE.Click += new System.EventHandler(dgDEFTABLE_Click);
            m_dgDEFTABLE.MouseUp += new MouseEventHandler(dgDEFTABLE_MouseUp);
            m_dgDEFTABLE.CaptionText = "Selected deftable";
            Util.setDefaults(m_dgDEFTABLE, m_dtDEFTABLE);

            GroupBox grpDEFTABLES = new GroupBox();

            GroupBox grpDEFTABLE = new GroupBox();

            CmLabel lblDEFTABLE_Id = new CmLabel("Id");
            txtDEFTABLE_Id = new TextBox();
           
            CmLabel lblDEFTABLE_Description = new CmLabel("Description");
            txtDEFTABLE_Description = new TextBox();

            CmLabel lblDEFTABLE_Order = new CmLabel("Order");
            txtDEFTABLE_Order = new TextBox();

            cmdDEFTABLE_Add = new CmButton("Neu");
            cmdDEFTABLE_Ok = new CmButton("Übernehmen");
            cmdDEFTABLE_Delete = new CmButton("Löschen");

            Controls.AddRange(new Control[] {grpDEFTABLES, grpDEFTABLE});

            Dock = DockStyle.Fill;
            Text = "Deftables";

            grpDEFTABLES.Controls.AddRange(new Control[] 
            {
                m_dgDEFTABLES
            });

            grpDEFTABLE.Controls.AddRange(new Control[] 
            {
                lblDEFTABLE_Id,
                txtDEFTABLE_Id, 
                lblDEFTABLE_Description,
                txtDEFTABLE_Description,
                lblDEFTABLE_Order,
                txtDEFTABLE_Order,
                m_dgDEFTABLE,
                cmdDEFTABLE_Add,
                cmdDEFTABLE_Ok,
                cmdDEFTABLE_Delete});

            grpDEFTABLES.Location = new Point(10, 10);
            grpDEFTABLES.Size = new Size(m_nGrpWidth, 310);
            grpDEFTABLES.TabIndex = 12;
            grpDEFTABLES.TabStop = false;
            grpDEFTABLES.Text = "DEFTABLES";

            grpDEFTABLE.Location = new Point(grpDEFTABLES.Left, grpDEFTABLES.Bottom + 10);
            grpDEFTABLE.Size = new Size(m_nGrpWidth, 310);
            grpDEFTABLE.TabIndex = 12;
            grpDEFTABLE.TabStop = false;
            grpDEFTABLE.Text = "DEFTABLE";
            
            // 
            // DEFTABLE_Id
            // 
            lblDEFTABLE_Id.Location = new Point(nCol1, 16);
            lblDEFTABLE_Id.TabIndex = 6;
            txtDEFTABLE_Id.Location = new Point(lblDEFTABLE_Id.Left + nLabelOffset, 16);
            txtDEFTABLE_Id.Size = new Size(200, 20);
            txtDEFTABLE_Id.TabIndex = 8;

            // 
            // DEFTABLE_ID
            // 
            lblDEFTABLE_Description.Location = new Point(nCol2, 16);
            lblDEFTABLE_Description.TabIndex = 5;
            txtDEFTABLE_Description.Location = new Point(lblDEFTABLE_Description.Left + 2 * nLabelOffset, 16);
            txtDEFTABLE_Description.Size = new Size(120, 20);
            txtDEFTABLE_Description.TabIndex = 7;

            lblDEFTABLE_Order.Location = new Point(nCol1, 48);
            lblDEFTABLE_Order.TabIndex = 5;
            txtDEFTABLE_Order.Location = new Point(lblDEFTABLE_Order.Left + nLabelOffset, 48);
            txtDEFTABLE_Order.Size = new Size(120, 20);
            txtDEFTABLE_Order.TabIndex = 7;

            // 
            // cmdDEFTABLE_Add
            // 
            cmdDEFTABLE_Add.Location = new Point(672, 88);
            cmdDEFTABLE_Add.Click += new EventHandler(cmdDEFTABLE_Add_Click);

            // 
            // cmdDEFTABLE_Ok
            // 
            cmdDEFTABLE_Ok.Location = new Point(672, 120);
            cmdDEFTABLE_Ok.Click += new EventHandler(cmdDEFTABLE_Ok_Click);
            
            // 
            // cmdDEFTABLE_Delete
            // 
            cmdDEFTABLE_Delete.Location = new Point(672, 152);
            cmdDEFTABLE_Delete.Click += new EventHandler(cmdDEFTABLE_Delete_Click);

            DEFTABLEButtonsXable(false);
        }

        public override void populate()
        {
            populateDEFTABLES();
            m_strDEFTABLE = "";
            populateDEFTABLE(m_strDEFTABLE);
        }

        private void populateDEFTABLE(string p_strTableName)
        {
            DataRow         row;
            int             i;

            m_dtDEFTABLE.Clear();

            if (m_strDEFTABLE.Length == 0)
                return;

            this.m_parent.m_sqlCommand.CommandText = "SELECT " + p_strTableName + "," + p_strTableName + "_id, " 
                + p_strTableName + "_description, "
                + p_strTableName + "_order FROM " + p_strTableName + " ORDER BY " + p_strTableName + "_id";
            this.m_parent.m_sqlDataReader = m_parent.m_sqlCommand.ExecuteReader();

            while (this.m_parent.m_sqlDataReader.Read())
            {
                row = m_dtDEFTABLE.NewRow();

                for (i = 0; i < this.m_parent.m_sqlDataReader.FieldCount; i++)
                {
                    row[i] = Util.getValueFromSqlDataReader(this.m_parent.m_sqlDataReader, i);
                }

                m_dtDEFTABLE.Rows.Add(row);
            }
            this.m_parent.m_sqlDataReader.Close();
            m_dtDEFTABLE.AcceptChanges();

            setColumnsDEFTABLE();

            m_dgDEFTABLE.CurrentRowIndex = 0;
        }

        private void DEFTABLESRowSelected()
        {
            int rowIndex = m_dgDEFTABLES.CurrentRowIndex;
            if (rowIndex != -1)
            {
                m_dgDEFTABLES.Select(rowIndex);
                m_strDEFTABLE = (string) m_dgDEFTABLES[rowIndex, 0];
                populateDEFTABLE(m_strDEFTABLE);
            }
        }

        private void dgDEFTABLES_Click(object sender, System.EventArgs e)
        {
            DEFTABLESRowSelected();
        }

        private void AddDefTable(string p_strTableName, string p_strDescription)
        {
            DataRow row = m_dtDEFTABLES.NewRow();

            row["tablename"] = p_strTableName;
            row["description"] = p_strDescription;
            m_dtDEFTABLES.Rows.Add(row);
        }

        private void populateDEFTABLES()
        {
            m_dtDEFTABLES.Clear();

            AddDefTable("dtyp", "Typ");
            AddDefTable("dlan", "Sprache");
 
            m_dtDEFTABLES.AcceptChanges();
//            m_dgDEFTABLES.CurrentRowIndex = 0;

            setColumnsDEFTABLES();
        }

        private void DEFTABLEDetailsXable(bool p_fFlag)
        {
            txtDEFTABLE_Id.Enabled = p_fFlag;
            txtDEFTABLE_Description.Enabled = p_fFlag;
        }    

        private void DEFTABLEButtonsXable(bool p_fFlag)
        {
            cmdDEFTABLE_Add.Enabled = p_fFlag;
            cmdDEFTABLE_Ok.Enabled = p_fFlag;
            cmdDEFTABLE_Delete.Enabled = p_fFlag;
        }    

           
        protected void cmdDEFTABLE_Add_Click(object p_sender, System.EventArgs p_e)
        {
            DataRow row = m_dtDEFTABLE.NewRow();

            int nDEFTABLE = Util.getNextMaxValueForTable(m_parent.m_sqlConnection, m_parent.m_sqlCommand, m_strDEFTABLE);

            row["pk"] = nDEFTABLE.ToString();

            txtDEFTABLE_Id.Text = "";
            row["id"] = txtDEFTABLE_Id.Text;

            txtDEFTABLE_Description.Text = "";
            row["description"] = txtDEFTABLE_Description.Text;

            txtDEFTABLE_Order.Text = "";
            row["order"] = txtDEFTABLE_Order.Text;

            m_dtDEFTABLE.Rows.Add(row);

            m_dgDEFTABLE.selectRowByValue(0, row["pk"].ToString());

            DEFTABLERowSelected();

            txtDEFTABLE_Id.Focus();
        }

        private void DEFTABLERowSelected()
        {
            int rowIndex = m_dgDEFTABLE.CurrentRowIndex;
            if (rowIndex != -1)
            {
                m_dgDEFTABLE.Select(rowIndex);
                DEFTABLESetDetails(rowIndex);
            }
        }

        protected void cmdDEFTABLE_Delete_Click(object p_sender, System.EventArgs p_e)
        {
            int rowIndex = m_dgDEFTABLE.CurrentRowIndex;
            if (rowIndex != -1)
            {
                int nDEFTABLE = (int) m_dgDEFTABLE[rowIndex, 0];

                foreach (DataRow row in m_dtDEFTABLE.Rows)
                {
                    if ((int) row["pk", Util.getDataRowVersion(row)] == nDEFTABLE)
                    {
                        row.Delete();
                        DEFTABLERowSelected();
                        DEFTABLESetDetails(m_dgDEFTABLE.CurrentRowIndex);
                        populateDEFTABLE(m_strDEFTABLE);
                        return;
                    }
                }
            }
        }
        
        private void cmdDEFTABLE_Ok_Click(object p_sender, System.EventArgs p_e)
        {
            DEFTABLEAcceptChanges();
        }

        private void DEFTABLEAcceptChanges()
        {
            int rowIndex = m_dgDEFTABLE.CurrentRowIndex;
            if (rowIndex != -1)
            {
                int nDEFTABLE = (int) m_dgDEFTABLE[rowIndex, 0];

                foreach (DataRow row in m_dtDEFTABLE.Rows)
                {
                    if ((int) row["pk", Util.getDataRowVersion(row)] == nDEFTABLE)
                    {
                        row["id"] = txtDEFTABLE_Id.Text;
                        row["description"] = txtDEFTABLE_Description.Text;
                        row["order"] = txtDEFTABLE_Order.Text;

                        return;
                    }
                }
            }
        }

        private void DEFTABLESetDetails(int p_nRowIndex)
        {
            txtDEFTABLE_Id.Text = Db.createSqlString(m_dgDEFTABLE[p_nRowIndex, 1], true, true, false);
            txtDEFTABLE_Description.Text = Db.createSqlString(m_dgDEFTABLE[p_nRowIndex, 2], true, true, false);
            txtDEFTABLE_Order.Text = Db.createSqlString(m_dgDEFTABLE[p_nRowIndex, 3], true, true, false);
        }


        protected void DEFTABLESave()
        {
            bool fSuccess = true;

            foreach (DataRow row in m_dtDEFTABLE.Rows)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        if (!Db.DEFTABLEInsert(m_parent.m_sqlCommand, m_strDEFTABLE, row))
                        {
                            fSuccess = false;
                        }
                        break;

                    case DataRowState.Modified:
                        if (!Db.DEFTABLEUpdate(m_parent.m_sqlCommand, m_strDEFTABLE, row))
                        {
                            fSuccess = false;
                        }
                        break;

                    case DataRowState.Deleted:
                        if (!Db.DEFTABLEDelete(m_parent.m_sqlCommand, m_strDEFTABLE, (int) row["pk", Util.getDataRowVersion(row)]))
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
                m_dtDEFTABLE.AcceptChanges();
            }
            else
            {
                m_dtDEFTABLE.RejectChanges();
            }
            m_parent.DeftablesChanged();
        }

        private void dgDEFTABLES_MouseUp(object sender, MouseEventArgs e) 
        { 
            System.Drawing.Point pt = new Point(e.X, e.Y); 
            DataGrid.HitTestInfo hti = m_dgDEFTABLES.HitTest(pt); 

            if (hti.Type == DataGrid.HitTestType.Cell) 
            { 
                m_dgDEFTABLES.CurrentCell = new DataGridCell(hti.Row, hti.Column); 
                m_dgDEFTABLES.CurrentRowIndex = hti.Row;
                m_dgDEFTABLES.Select(hti.Row);
                DEFTABLESRowSelected();
            } 
        } 

        private void dgDEFTABLE_MouseUp(object sender, MouseEventArgs e) 
        { 
            System.Drawing.Point pt = new Point(e.X, e.Y); 
            DataGrid.HitTestInfo hti = m_dgDEFTABLE.HitTest(pt); 

            if (hti.Type == DataGrid.HitTestType.Cell) 
            { 
                m_dgDEFTABLE.CurrentCell = new DataGridCell(hti.Row, hti.Column); 
                m_dgDEFTABLE.CurrentRowIndex = hti.Row;
                m_dgDEFTABLE.Select(hti.Row);
                DEFTABLERowSelected();
            } 
        } 

        private void setColumnsDEFTABLES()
        {
            ColumnDescription[] arCols= new ColumnDescription[2];
            int i = 0;

            arCols[i++] = new ColumnDescription("Deftable",     100);
            arCols[i++] = new ColumnDescription("Description",  200);

            Util.setColumns(m_parent.m_sqlCommand, m_dgDEFTABLES, m_dtDEFTABLES, arCols);
        }

        private void setColumnsDEFTABLE()
        {
            ColumnDescription[] arCols= new ColumnDescription[4];
            int i = 0;

            arCols[i++] = new ColumnDescription(true);
            arCols[i++] = new ColumnDescription("Id",           100);
            arCols[i++] = new ColumnDescription("Description",  300);
            arCols[i++] = new ColumnDescription("Order",        100);

            Util.setColumns(m_parent.m_sqlCommand, m_dgDEFTABLE, m_dtDEFTABLE, arCols);
        }

        private void dgDEFTABLE_Click(object sender, System.EventArgs e)
        {
            int rowIndex = m_dgDEFTABLE.CurrentRowIndex;
            if (rowIndex != -1)
            {
                DEFTABLERowSelected();
            }
        }
        private void  DEFTABLEDetailsClear()
        {
            txtDEFTABLE_Id.Text = "";
            txtDEFTABLE_Description.Text = "";
            txtDEFTABLE_Order.Text = "";
        }

        public override void edit(bool p_fIsEdit)
        {
            DEFTABLEDetailsClear();
            DEFTABLEDetailsClear();
            DEFTABLEButtonsXable(p_fIsEdit);
            DEFTABLEDetailsXable(p_fIsEdit);
        }
        public override void save()
        {
            DEFTABLESave();
        }
    }
}

