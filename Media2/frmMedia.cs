using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Media
{
    public class frmMedia : CmDbTabPage
    {
        private CmDataGrid  m_dgTMED;
        private DataTable   m_dtTMED;

        private CmDataGrid  m_dgTENT;
        private DataTable   m_dtTENT;

        private int         m_nTMED;

        private bool        m_fDeftablesPopulated;

        GroupBox    grpTMED;
        CmLabel     lblTMED_TITLE;
        TextBox     txtTMED_TITLE;
        CmLabel     lblTMED_DATE;
        TextBox     txtTMED_DATE;
        CmLabel     lblTMED_ID;
        TextBox     txtTMED_ID;
        CmLabel     lblDTYP;
        ComboBox    cbDTYP;
        CmLabel     lblDLAN;
        ComboBox    cbDLAN;
        GroupBox    grpTART;
        CmLabel     lblTART_NAME;
        TextBox     txtTART_NAME;
        Button      cmdTMED_Add;
        Button      cmdTMED_Ok;
        Button      cmdTMED_Delete;
        Button      cmdTMED_Artist;
        CheckBox    chkTMED_Filter;

        CmLabel     lblTENT_NO;
        TextBox     txtTENT_NO;
        CmLabel     lblTENT_SIDE;
        TextBox     txtTENT_SIDE;
        CmLabel     lblTENT_TITLE;
        TextBox     txtTENT_TITLE;
        GroupBox    grpTART2;
        CmLabel     lblTART2_FIRSTNAME;
        TextBox     txtTART2_FIRSTNAME;
        CmLabel     lblTART2_LASTNAME;
        TextBox     txtTART2_LASTNAME;
        Button      cmdTENT_Add;
        Button      cmdTENT_Ok;
        Button      cmdTENT_Delete;
        Button      cmdTENT_Artist;


        public frmMedia(CmDbTabControl p_parent) : base(p_parent) 
        {
            m_fDeftablesPopulated = false;
        }
        
        public override void InitializeComponent()
        {
            int nCol1 = 10;
            int nCol2 = 340;
            int nCol3 = 500;
            int nLabelOffset = 40;

            //
            // TMED
            //
            m_dtTMED = new CmDbDataTable("tmed");
            Util.createAndInsertDataColumn(m_dtTMED, "tmed", typeof(int), true);
            Util.createAndInsertDataColumn(m_dtTMED, "dtyp", typeof(int));
            Util.createAndInsertDataColumn(m_dtTMED, "dtyp_id", typeof(string));
            Util.createAndInsertDataColumn(m_dtTMED, "dtyp_description", typeof(string));
            Util.createAndInsertDataColumn(m_dtTMED, "tmed_id", typeof(string));
            Util.createAndInsertDataColumn(m_dtTMED, "tmed_title", typeof(string)).AllowDBNull = true;
            Util.createAndInsertDataColumn(m_dtTMED, "tmed_date", typeof(string)).AllowDBNull = true;
            Util.createAndInsertDataColumn(m_dtTMED, "tart", typeof(int)).AllowDBNull = true;
            Util.createAndInsertDataColumn(m_dtTMED, "tart_name", typeof(string)).AllowDBNull = true;
            Util.createAndInsertDataColumn(m_dtTMED, "dlan", typeof(int));
            Util.createAndInsertDataColumn(m_dtTMED, "dlan_id", typeof(string));
            Util.createAndInsertDataColumn(m_dtTMED, "dlan_description", typeof(string));

            m_dgTMED = new CmDataGrid();
            m_dgTMED.CaptionText = "Media";
            m_dgTMED.RowHeadersVisible = true;
            m_dgTMED.Click += new System.EventHandler(dgTMED_Click);
            m_dgTMED.MouseUp += new MouseEventHandler(dgTMED_MouseUp);
            Util.setDefaults(m_dgTMED, m_dtTMED);

            grpTMED = new GroupBox();
            lblTMED_TITLE = new CmLabel("Title");
            txtTMED_TITLE = new TextBox();
           
            lblTMED_DATE = new CmLabel("Date");
            txtTMED_DATE = new TextBox();

            lblTMED_ID = new CmLabel("Id");
            txtTMED_ID = new TextBox();

            lblDTYP = new CmLabel("Typ");
            cbDTYP = new ComboBox();

            lblDLAN = new CmLabel("Sprache");
            cbDLAN = new ComboBox();

            grpTART = new GroupBox();
            lblTART_NAME = new CmLabel("Name");
            txtTART_NAME = new TextBox();

            cmdTMED_Add = new CmButton("Neu");
            cmdTMED_Ok = new CmButton("Übernehmen");
            cmdTMED_Delete = new CmButton("Löschen");
            cmdTMED_Artist = new CmButton("Artist...");
            chkTMED_Filter = new CheckBox();

            //
            // TENT
            //
            GroupBox    grpTENT = new GroupBox();

            m_dtTENT = new CmDbDataTable("tent");
            Util.createAndInsertDataColumn(m_dtTENT, "tent", typeof(int), true);
            Util.createAndInsertDataColumn(m_dtTENT, "tent_side", typeof(string)).AllowDBNull = true;
            Util.createAndInsertDataColumn(m_dtTENT, "tent_no", typeof(int));
            Util.createAndInsertDataColumn(m_dtTENT, "tent_title", typeof(string));
            Util.createAndInsertDataColumn(m_dtTENT, "tmed", typeof(int), true);
            Util.createAndInsertDataColumn(m_dtTENT, "tart", typeof(int)).AllowDBNull = true;
            Util.createAndInsertDataColumn(m_dtTENT, "tart_name", typeof(string)).AllowDBNull = true;

            m_dgTENT = new CmDataGrid();
            m_dgTENT.CaptionText = "Entries";
            m_dgTENT.Click += new System.EventHandler(dgTENT_Click);
            m_dgTENT.MouseUp += new MouseEventHandler(dgTENT_MouseUp);
            Util.setDefaults(m_dgTENT, m_dtTENT);

            lblTENT_NO = new CmLabel("No");
            txtTENT_NO = new TextBox();

            lblTENT_SIDE = new CmLabel("Side");
            txtTENT_SIDE = new TextBox();

            lblTENT_TITLE = new CmLabel("Title");
            txtTENT_TITLE = new TextBox();

            grpTART2 = new GroupBox();
            lblTART2_FIRSTNAME = new CmLabel("First name");
            txtTART2_FIRSTNAME = new TextBox();
            lblTART2_LASTNAME = new CmLabel("Last name");
            txtTART2_LASTNAME = new TextBox();

            cmdTENT_Add = new CmButton("Neu");
            cmdTENT_Ok = new CmButton("Übernehmen");
            cmdTENT_Delete = new CmButton("Löschen");
            cmdTENT_Artist = new CmButton("Artist...");

            // 
            // frmMedia
            // 
            Controls.AddRange(new Control[] {grpTMED, grpTENT});

            Dock = DockStyle.Fill;
            Text = "Media";

            // 
            // grpTMED
            // 
            grpTMED.Controls.AddRange(new Control[] 
            {
                lblDTYP,
                cbDTYP,
                lblDLAN,
                cbDLAN,
                lblTMED_TITLE,
                txtTMED_TITLE,
                lblTMED_ID,
                txtTMED_ID,
                lblTMED_DATE,
                txtTMED_DATE,
                grpTART,
                m_dgTMED,
                cmdTMED_Add,
                cmdTMED_Ok,
                cmdTMED_Delete,
                cmdTMED_Artist,
                chkTMED_Filter});

            grpTMED.Location = new Point(10, 10);
            grpTMED.Size = new Size(m_nGrpWidth, 310);
            grpTMED.TabIndex = 12;
            grpTMED.TabStop = false;
            grpTMED.Text = "Media";

            // 
            // DTYP
            // 
            lblDTYP.Location = new Point(nCol1, 16);
            lblDTYP.TabIndex = 0;
            cbDTYP.DropDownStyle = ComboBoxStyle.DropDownList;
            cbDTYP.Location = new Point(lblDTYP.Left + nLabelOffset, 16);
            cbDTYP.Size = new Size(121, 21);
            cbDTYP.TabIndex = 4;

            // 
            // DLAN
            // 
            lblDLAN.Location = new Point(cbDTYP.Right + 10, 16);
            lblDLAN.TabIndex = 0;
            cbDLAN.DropDownStyle = ComboBoxStyle.DropDownList;
            cbDLAN.Location = new Point(lblDLAN.Right + 10, 16);
            cbDLAN.Size = new Size(121, 21);
            cbDLAN.TabIndex = 4;

            // 
            // TMED_TITLE
            // 
            lblTMED_TITLE.Location = new Point(nCol1, 48);
            lblTMED_TITLE.TabIndex = 6;
            txtTMED_TITLE.Location = new Point(lblTMED_TITLE.Left + nLabelOffset, 48);
            txtTMED_TITLE.Size = new Size(200, 20);
            txtTMED_TITLE.TabIndex = 8;

            // 
            // TMED_ID
            // 
            lblTMED_ID.Location = new Point(cbDLAN.Right + 10, 16);
            lblTMED_ID.TabIndex = 5;
            txtTMED_ID.Location = new Point(lblTMED_ID.Left + nLabelOffset, 16);
            txtTMED_ID.Size = new Size(120, 20);
            txtTMED_ID.TabIndex = 7;

            // 
            // TMED_DATE
            // 
            lblTMED_DATE.Location = new Point(nCol2, 48);
            lblTMED_DATE.TabIndex = 9;
            txtTMED_DATE.Location = new Point(lblTMED_DATE.Left + nLabelOffset, 48);
            txtTMED_DATE.Size = new Size(120, 20);
            txtTMED_DATE.TabIndex = 10;

            // 
            // grpTART
            // 
            grpTART.Controls.AddRange(new Control[] 
                {   txtTART_NAME,
                    lblTART_NAME
                });

            grpTART.Location = new Point(txtTMED_DATE.Right + 10, 10);
            grpTART.Size = new Size(232, 72);
            grpTART.TabIndex = 11;
            grpTART.TabStop = false;
            grpTART.Text = "Artist";

            // 
            // txtTART_NAME
            // 
            lblTART_NAME.Location = new Point(24, 16);
            lblTART_NAME.TabIndex = 11;
            txtTART_NAME.Enabled = false;
            txtTART_NAME.Location = new Point(lblTART_NAME.Left + nLabelOffset, 16);
            txtTART_NAME.Size = new Size(120, 20);
            txtTART_NAME.TabIndex = 12;

            //
            // m_dgTENT
            //
            m_dgTMED.Location = new Point(10, 90);
            m_dgTMED.Size = new Size(648, 208);
            m_dgTMED.TabIndex = 1;

            // 
            // cmdTMED_Add
            // 
            cmdTMED_Add.Location = new Point(672, 96);
            cmdTMED_Add.TabIndex = 2;
            cmdTMED_Add.Click += new EventHandler(cmdTMED_Add_Click);

            // 
            // cmdTMED_Ok
            // 
            cmdTMED_Ok.Location = new Point(672, 128);
            cmdTMED_Ok.Click += new EventHandler(cmdTMED_Ok_Click);

            // 
            // cmdTMED_Delete
            // 
            cmdTMED_Delete.Location = new Point(672, 160);
            cmdTMED_Delete.Click += new EventHandler(cmdTMED_Delete_Click);

            // 
            // cmdTMED_Artist
            // 
            cmdTMED_Artist.Location = new Point(672, 192);
            cmdTMED_Artist.Click += new EventHandler(cmdTMED_Artist_Click);
            
            // 
            // chkTMED_Filter
            // 
            chkTMED_Filter.Location = new Point(672, 222);
            chkTMED_Filter.Size = new Size(64, 24);
            chkTMED_Filter.Text = "Filter";

            // 
            // grpTENT
            // 
            grpTENT.Controls.AddRange(new Control[] 
            {
                lblTENT_SIDE,
                txtTENT_SIDE,
                lblTENT_NO,
                txtTENT_NO,
                lblTENT_TITLE,
                txtTENT_TITLE,
                grpTART2,
                m_dgTENT,
                cmdTENT_Add,
                cmdTENT_Ok,
                cmdTENT_Delete,
                cmdTENT_Artist
            });

            grpTENT.Location = new Point(grpTMED.Left, grpTMED.Bottom + 10);
            grpTENT.Size = grpTMED.Size;
            grpTENT.TabIndex = 13;
            grpTENT.TabStop = false;
            grpTENT.Text = "Entries";

            // 
            // TENT_SIDE
            // 
            lblTENT_SIDE.Location = new Point(nCol1, 16);
            lblTENT_SIDE.TabIndex = 0;
            txtTENT_SIDE.Location = new Point(lblTENT_SIDE.Left + nLabelOffset, 16);
            txtTENT_SIDE.Size = new Size(120, 20);
            txtTENT_SIDE.TabIndex = 12;

            // 
            // TENT_NO
            // 
            lblTENT_NO.Location = new Point(nCol1, 48);
            lblTENT_NO.TabIndex = 6;
            txtTENT_NO.Location = new Point(lblTENT_NO.Left + nLabelOffset, 48);
            txtTENT_NO.Size = new Size(120, 20);
            txtTENT_NO.TabIndex = 7;

            // 
            // TENT_TITLE
            // 
            lblTENT_TITLE.Location = new Point(nCol2, 16);
            lblTENT_TITLE.TabIndex = 5;
            txtTENT_TITLE.Location = new Point(lblTENT_TITLE.Left + nLabelOffset, 16);
            txtTENT_TITLE.Size = new Size(120, 20);
            txtTENT_TITLE.TabIndex = 8;

            // 
            // grpTART2
            // 
            grpTART2.Controls.AddRange(new Control[] 
            {
                lblTART2_LASTNAME,
                txtTART2_LASTNAME,
                lblTART2_FIRSTNAME,
                txtTART2_FIRSTNAME
            });

            grpTART2.Location = grpTART.Location;//new Point(424, 8);
            grpTART2.Size = grpTART.Size;//new Size(232, 72);
            grpTART2.TabIndex = 11;
            grpTART2.TabStop = false;
            grpTART2.Text = "Artist";

            // 
            // txtTART2_FIRSTNAME
            // 
            lblTART2_FIRSTNAME.Location = new Point(8, 40);
            lblTART2_FIRSTNAME.TabIndex = 13;
            txtTART2_FIRSTNAME.Enabled = false;
            txtTART2_FIRSTNAME.Location = new Point(72, 40);
            txtTART2_FIRSTNAME.Size = new Size(152, 20);
            txtTART2_FIRSTNAME.TabIndex = 14;

            // 
            // txtTART2_LASTNAME
            // 
            lblTART2_LASTNAME.Location = new Point(8, 16);
            lblTART2_LASTNAME.TabIndex = 11;
            txtTART2_LASTNAME.Enabled = false;
            txtTART2_LASTNAME.Location = new Point(72, 16);
            txtTART2_LASTNAME.Size = new Size(152, 20);
            txtTART2_LASTNAME.TabIndex = 12;

            // 
            // cmdTENT_Add
            // 
            cmdTENT_Add.Location = new Point(672, 88);
            cmdTENT_Add.Click += new EventHandler(cmdTENT_Add_Click);
            
            // 
            // cmdTENT_Ok
            // 
            cmdTENT_Ok.Location = new Point(672, 120);
            cmdTENT_Ok.Click += new EventHandler(cmdTENT_Ok_Click);
            
            // 
            // cmdTENT_Delete
            // 
            cmdTENT_Delete.Location = new Point(672, 152);
            cmdTENT_Delete.Click += new EventHandler(cmdTENT_Delete_Click);

            // 
            // cmdTENT_Artist
            // 
            cmdTENT_Artist.Location = new Point(672, 184);
            cmdTENT_Artist.Click += new EventHandler(cmdTENT_Artist_Click);

            // 
            // m_dgTENT
            // 
            m_dgTENT.Location = new Point(8, 88);
            m_dgTENT.Size = new Size(648, 208);
            m_dgTENT.TabIndex = 1;

            TMEDButtonsXable(false);
            TENTDetailsXable(false);
            TENTButtonsXable(false);
        }

        private void dgTMED_Click(object sender, System.EventArgs e)
        {
            TMEDRowSelected();
        }

        private void TMEDRowSelected()
        {
            int rowIndex = m_dgTMED.CurrentRowIndex;
            if (rowIndex != -1)
            {
                m_dgTMED.Select(rowIndex);
                m_nTMED = (int) m_dgTMED[rowIndex, 0];
                TMEDSetDetails(rowIndex);
                populateTENT(m_nTMED);
            }
        }

        private void TENTRowSelected()
        {
            int rowIndex = m_dgTENT.CurrentRowIndex;
            if (rowIndex != -1)
            {
                m_dgTENT.Select(rowIndex);
                TENTSetDetails(rowIndex);
            }
        }

        private void dgTENT_Click(object sender, System.EventArgs e)
        {
            int rowIndex = m_dgTENT.CurrentRowIndex;
            if (rowIndex != -1)
            {
                TENTRowSelected();
            }
        }

        private void dgTMED_MouseUp(object sender, MouseEventArgs e) 
        { 
            Point pt = new Point(e.X, e.Y); 
            DataGrid.HitTestInfo hti = m_dgTMED.HitTest(pt); 

            if (hti.Type == DataGrid.HitTestType.Cell) 
            { 
                m_dgTMED.CurrentCell = new DataGridCell(hti.Row, hti.Column); 
                m_dgTMED.CurrentRowIndex = hti.Row;
                m_dgTMED.Select(hti.Row);
                TMEDRowSelected();
            } 
        } 

        private void dgTENT_MouseUp(object sender, MouseEventArgs e) 
        { 
            Point pt = new Point(e.X, e.Y); 
            DataGrid.HitTestInfo hti = m_dgTENT.HitTest(pt); 

            if (hti.Type == DataGrid.HitTestType.Cell) 
            { 
                m_dgTENT.CurrentCell = new DataGridCell(hti.Row, hti.Column); 
                m_dgTENT.CurrentRowIndex = hti.Row;
                m_dgTENT.Select(hti.Row);
                TENTRowSelected();
            } 
        } 

        public override void populate()
        {
            populateDeftables(false);
            populateTMED();
            m_nTMED = -1;
            populateTENT(m_nTMED);
        }

        private void populateTMED()
        {
            DataRow     row;
            int         i;
            string      strSql;
            string      strWhere = "";
            bool        fHasWhere = false;
            bool        fHasWherePart = false;

            m_dtTMED.Clear();

            strSql = "SELECT tmed, dtyp.dtyp, dtyp.dtyp_id, dtyp.dtyp_description"
                + ", tmed_id, tmed_title, tmed_date, tart.tart, tart.tart_name"
                + ", dlan.dlan, dlan.dlan_id, dlan.dlan_description"
                + " FROM tmed"
                + " INNER JOIN dtyp ON tmed.dtyp=dtyp.dtyp"
                + " INNER JOIN dlan ON tmed.dlan=dlan.dlan"
                + " LEFT OUTER JOIN tart ON tmed.tart=tart.tart";

            if (chkTMED_Filter.Checked)
            {
                if (0 != cbDTYP.SelectedValue.ToString().CompareTo(Db.s_strDEFTABLE_NONE))
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
                    int nDTYP = Db.getDefTablePk(m_parent.m_sqlCommand, "dtyp", (string) cbDTYP.SelectedValue);
                    strWhere += " tmed.dtyp=" + nDTYP;
                    fHasWherePart = true;
                }
                if (0 != cbDLAN.SelectedValue.ToString().CompareTo(Db.s_strDEFTABLE_NONE))
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
                    int nDLAN = Db.getDefTablePk(m_parent.m_sqlCommand, "dlan", (string) cbDLAN.SelectedValue);
                    strWhere += " tmed.dlan=" + nDLAN;
                    fHasWherePart = true;
                }
                if (txtTMED_TITLE.Text.Length > 0)
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
                    strWhere += " tmed_title LIKE '%" + txtTMED_TITLE.Text + "%'";
                    fHasWherePart = true;
                }
                if (txtTMED_ID.Text.Length > 0)
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
                    strWhere += " tmed_id LIKE '%" + txtTMED_ID.Text + "%'";
                    fHasWherePart = true;
                }
            }

            if (fHasWhere)
            {
                strSql += strWhere;
            }
            m_parent.m_sqlCommand.CommandText = strSql + " ORDER BY dtyp_order, tmed_id";
            m_parent.m_sqlDataReader = m_parent.m_sqlCommand.ExecuteReader();

            while (m_parent.m_sqlDataReader.Read())
            {
                row = m_dtTMED.NewRow();

                for (i = 0; i < m_parent.m_sqlDataReader.FieldCount; i++)
                {
                    row[i] = Util.getValueFromSqlDataReader(m_parent.m_sqlDataReader, i, true);
                }

                m_dtTMED.Rows.Add(row);
            }
            m_parent.m_sqlDataReader.Close();
            m_dtTMED.AcceptChanges();
            m_dgTMED.CurrentRowIndex = 0;

            setColumnsTMED();
        }

        private void populateTENT(int m_nTMED)
        {
            DataRow         row;
            int             i;

            m_dtTENT.Clear();

            m_parent.m_sqlCommand.CommandText = "SELECT tent, tent_side, tent_no, tent_title"
                + ", tmed, tart.tart, tart_name"
                + " FROM tent"
                + " LEFT OUTER JOIN tart ON tent.tart=tart.tart"
                + " WHERE tmed=" + m_nTMED
                + " ORDER BY tmed";
            m_parent.m_sqlDataReader = m_parent.m_sqlCommand.ExecuteReader();

            while (m_parent.m_sqlDataReader.Read())
            {
                row = m_dtTENT.NewRow();

                for (i = 0; i < m_parent.m_sqlDataReader.FieldCount; i++)
                {
                    if (!m_parent.m_sqlDataReader.IsDBNull(i))
                    {
                        row[i] = Util.getValueFromSqlDataReader(m_parent.m_sqlDataReader, i);
                    }
                }

                m_dtTENT.Rows.Add(row);
            }
            m_parent.m_sqlDataReader.Close();
            m_dtTENT.AcceptChanges();
            m_dgTENT.CurrentRowIndex = 0;

            setColumnsTENT();
        }

        private void setColumnsTMED()
        {
            ColumnDescription[] arCols= new ColumnDescription[12];
            int i = 0;

            arCols[i++] = new ColumnDescription(true);              // tmed
            arCols[i++] = new ColumnDescription(true);              // dtyp
            arCols[i++] = new ColumnDescription(true);              // dtyp_id
            arCols[i++] = new ColumnDescription("Art", 120);        // dtyp_description
            arCols[i++] = new ColumnDescription("Id", 50);          // tmed_id
            arCols[i++] = new ColumnDescription("Name", 200);       // tmed_title
            arCols[i++] = new ColumnDescription("Datum", 50);       // tmed_date
            arCols[i++] = new ColumnDescription(true);              // tart
            arCols[i++] = new ColumnDescription("Artist", 200);     // tart_name
            arCols[i++] = new ColumnDescription(true);              // dlan
            arCols[i++] = new ColumnDescription(true);              // dlan_id
            arCols[i++] = new ColumnDescription("Sprache", 120);    // dlan_description

            Util.setColumns(m_parent.m_sqlCommand, m_dgTMED, m_dtTMED, arCols);
        }

        private void setColumnsTENT()
        {
            ColumnDescription[] arCols= new ColumnDescription[7];
            int i = 0;

            arCols[i++] = new ColumnDescription(true);      // tent
            arCols[i++] = new ColumnDescription("Seite", 50);        // tent_side
            arCols[i++] = new ColumnDescription("No", 50);        // tent_no
            arCols[i++] = new ColumnDescription("Name", 200);       // tent_title
            arCols[i++] = new ColumnDescription(true);      // tmed
            arCols[i++] = new ColumnDescription(true);      // tart
            arCols[i++] = new ColumnDescription("Artist", 150);       // tart_name

            Util.setColumns(m_parent.m_sqlCommand, m_dgTENT, m_dtTENT, arCols);
        }

        private void  TMEDDetailsClear()
        {
            cbDTYP.SelectedValue = Db.s_strDEFTABLE_DTYP_DEFAULT;
            cbDLAN.SelectedValue = Db.s_strDEFTABLE_DLAN_DEFAULT;
            txtTMED_TITLE.Text = "";
            txtTMED_ID.Text = "";
            txtTMED_DATE.Text = "";
            txtTART_NAME.Text = "";
        }

        private void TENTDetailsClear()
        {
            txtTENT_SIDE.Text = "";
            txtTENT_NO.Text = "";
            txtTENT_TITLE.Text = "";
            txtTART2_LASTNAME.Text = "";
            txtTART2_FIRSTNAME.Text = "";
        }

        private void TMEDDetailsXable(bool p_fFlag)
        {
            cbDTYP.Enabled = p_fFlag;
            cbDLAN.Enabled = p_fFlag;
            txtTMED_TITLE.Enabled = p_fFlag;
            txtTMED_ID.Enabled = p_fFlag;
            txtTMED_DATE.Enabled = p_fFlag;
        }

        private void TENTDetailsXable(bool p_fFlag)
        {
            txtTENT_SIDE.Enabled = p_fFlag;
            txtTENT_NO.Enabled = p_fFlag;
            txtTENT_TITLE.Enabled = p_fFlag;
        }    

        private void TMEDButtonsXable(bool p_fFlag)
        {
            cmdTMED_Add.Enabled = p_fFlag;
            cmdTMED_Ok.Enabled = p_fFlag;
            cmdTMED_Delete.Enabled = p_fFlag;
            cmdTMED_Artist.Enabled = p_fFlag;
        }    

        private void TENTButtonsXable(bool p_fFlag)
        {
            cmdTENT_Add.Enabled = p_fFlag;
            cmdTENT_Delete.Enabled = p_fFlag;
            cmdTENT_Ok.Enabled = p_fFlag;
            cmdTENT_Artist.Enabled = p_fFlag;
        }    

        private void cmdTENT_Add_Click(object p_sender, System.EventArgs p_e)
        {
            DataRow row = m_dtTENT.NewRow();

            int nTENT = Util.getNextMaxValueForTable(
                m_parent.m_sqlConnection, m_parent.m_sqlCommand, "tent");

            row["tent"] = nTENT.ToString();
            txtTENT_NO.Text = "1";
            row["tent_no"] = Int32.Parse(txtTENT_NO.Text);
            txtTENT_TITLE.Text = "";
            row["tent_title"] = txtTENT_TITLE.Text;
            row["tmed"] = m_nTMED;

            m_dtTENT.Rows.Add(row);

            m_dgTENT.selectRowByValue(0, row["tent"].ToString());
        }

        protected void cmdTENT_Ok_Click(object p_sender, System.EventArgs p_e)
        {
            TENTAcceptChanges();
        }

        private void cmdTMED_Add_Click(object p_sender, System.EventArgs p_e)
        {
            DataRow row = m_dtTMED.NewRow();

            m_nTMED = Util.getNextMaxValueForTable(
                m_parent.m_sqlConnection, m_parent.m_sqlCommand, "tmed");

            row["tmed"] = m_nTMED.ToString();

            txtTMED_TITLE.Text = "";
            row["tmed_title"] = txtTMED_TITLE.Text;

            txtTMED_ID.Text = "";
            row["tmed_id"] = txtTMED_ID.Text;

            txtTMED_DATE.Text = "";
            row["tmed_date"] = txtTMED_DATE.Text;

            cbDTYP.SelectedValue = Db.s_strDEFTABLE_DTYP_DEFAULT;
            row["dtyp"] = Db.getDefTablePk(m_parent.m_sqlCommand, "dtyp", (string) cbDTYP.SelectedValue);
            row["dtyp_id"] = cbDTYP.SelectedValue;
            row["dtyp_description"] = cbDTYP.Text;

            cbDLAN.SelectedValue = Db.s_strDEFTABLE_DLAN_DEFAULT;
            row["dlan"] = Db.getDefTablePk(m_parent.m_sqlCommand, "dlan", (string) cbDLAN.SelectedValue);
            row["dlan_id"] = cbDLAN.SelectedValue;
            row["dlan_description"] = cbDLAN.Text;

            txtTART_NAME.Text = "";
            row["tart_name"] = "";

            m_dtTMED.Rows.Add(row);

            m_dgTMED.selectRowByValue(0, row["tmed"].ToString());

            TMEDRowSelected();

            txtTMED_TITLE.Focus();
        }

        private void TMEDAcceptChanges()
        {
            int rowIndex = m_dgTMED.CurrentRowIndex;
            if (rowIndex != -1)
            {
                int nTMED = (int) m_dgTMED[rowIndex, 0];

                foreach (DataRow row in m_dtTMED.Rows)
                {
                    if ((int) row["tmed", Util.getDataRowVersion(row)] == nTMED)
                    {
                        row["tmed_title"] = txtTMED_TITLE.Text;
                        row["tmed_id"] = txtTMED_ID.Text;
                        row["tmed_date"] = txtTMED_DATE.Text;
                        row["dtyp"] = Db.getDefTablePk(m_parent.m_sqlCommand, "dtyp", (string) cbDTYP.SelectedValue);
                        row["dtyp_id"] = cbDTYP.SelectedValue;
                        row["dtyp_description"] = cbDTYP.Text;
                        row["dlan"] = Db.getDefTablePk(m_parent.m_sqlCommand, "dlan", (string) cbDLAN.SelectedValue);
                        row["dlan_id"] = cbDLAN.SelectedValue;
                        row["dlan_description"] = cbDLAN.Text;

                        return;
                    }
                }
            }
        }

        private void TENTAcceptChanges()
        {
            int rowIndex = m_dgTENT.CurrentRowIndex;
            if (rowIndex != -1)
            {
                int nTENT = (int) m_dgTENT[rowIndex, 0];

                foreach (DataRow row in m_dtTENT.Rows)
                {
                    if ((int) row["tent", Util.getDataRowVersion(row)] == nTENT)
                    {
                        row["tent_side"] = txtTENT_SIDE.Text;
                        row["tent_no"] = txtTENT_NO.Text;
                        row["tent_title"] = txtTENT_TITLE.Text;
                        return;
                    }
                }
            }
        }

        private void cmdTMED_Ok_Click(object p_sender, System.EventArgs p_e)
        {
            TMEDAcceptChanges();
        }

        private void populateDeftables(bool p_fForce)
        {
            if (!m_fDeftablesPopulated || p_fForce)
            {
                Util.fillComboBoxFromDeftable(cbDTYP, "dtyp", m_parent.m_sqlCommand);
                Util.fillComboBoxFromDeftable(cbDLAN, "dlan", m_parent.m_sqlCommand);
                m_fDeftablesPopulated = true;
            }
        }

        private void TMEDSetDetails(int p_nRowIndex)
        {
            cbDTYP.SelectedValue = (string) m_dgTMED[p_nRowIndex, 2];
            cbDLAN.SelectedValue = (string) m_dgTMED[p_nRowIndex, 10];

            txtTMED_ID.Text = (string) m_dgTMED[p_nRowIndex, 4];
            txtTMED_TITLE.Text = Db.createSqlString(m_dgTMED[p_nRowIndex, 5], true, true, false);
            txtTMED_DATE.Text = Db.createSqlString(m_dgTMED[p_nRowIndex, 6], true, true, false);
            txtTART_NAME.Text = Db.createSqlString(m_dgTMED[p_nRowIndex, 8], true, true, false);
        }

        private void TENTSetDetails(int p_nRowIndex)
        {
            txtTENT_SIDE.Text = Db.createSqlString(m_dgTENT[p_nRowIndex, 1], true, true, false);
            txtTENT_NO.Text = m_dgTENT[p_nRowIndex, 2].ToString();
            txtTENT_TITLE.Text = (string) m_dgTENT[p_nRowIndex, 3];
        }

        public override void save()
        {
            TENTSave();
            TMEDSave();
        }

            
        protected void cmdTMED_Artist_Click(object p_sender, System.EventArgs p_e)
        {
            frmFindArtistDlg findDlg = new frmFindArtistDlg(m_parent.m_sqlConnection, m_parent.m_sqlCommand);
            DialogResult result = findDlg.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                int nTART = findDlg.TART;
                string[] arValues = Db.TARTGetValuesForPk(nTART, m_parent.m_sqlCommand);
                
                int rowIndex = m_dgTMED.CurrentRowIndex;
                if (rowIndex != -1)
                {
                    int nTMED = (int) m_dgTMED[rowIndex, 0];

                    foreach (DataRow row in m_dtTMED.Rows)
                    {
                        if ((int) row["tmed", Util.getDataRowVersion(row)] == nTMED)
                        {
                            if (nTART == -1)
                            {
                                row["tart"] = DBNull.Value;
                                row["tart_name"] = "";
                            }
                            else
                            {
                                row["tart"] = nTART;
                                row["tart_name"] = arValues[0];
                            }
                            return;
                        }
                    }
                }
            }
            
        }

        protected void cmdTENT_Artist_Click(object p_sender, System.EventArgs p_e)
        {
            frmFindArtistDlg findDlg = new frmFindArtistDlg(m_parent.m_sqlConnection, m_parent.m_sqlCommand);
            DialogResult result = findDlg.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                int nTART = findDlg.TART;
                string[] arValues = Db.TARTGetValuesForPk(nTART, m_parent.m_sqlCommand);
                
                int rowIndex = m_dgTENT.CurrentRowIndex;
                if (rowIndex != -1)
                {
                    int nTENT = (int) m_dgTENT[rowIndex, 0];

                    foreach (DataRow row in m_dtTENT.Rows)
                    {
                        if ((int) row["tent", Util.getDataRowVersion(row)] == nTENT)
                        {
                            if (nTART == -1)
                            {
                                row["tart"] = DBNull.Value;
                                row["tart_name"] = "";
                            }
                            else
                            {
                                row["tart"] = nTART;
                                row["tart_name"] = arValues[0];
                            }
                            return;
                        }
                    }
                }
            }
        }

        protected void cmdTMED_Delete_Click(object p_sender, System.EventArgs p_e)
        {
            int rowIndex = m_dgTMED.CurrentRowIndex;
            if (rowIndex != -1)
            {
                int nTMED = (int) m_dgTMED[rowIndex, 0];

                foreach (DataRow row in m_dtTMED.Rows)
                {
                    if ((int) row["tmed", Util.getDataRowVersion(row)] == nTMED)
                    {
                        row.Delete();
                        TMEDRowSelected();
                        TMEDSetDetails(m_dgTMED.CurrentRowIndex);
                        populateTENT(nTMED);
                        return;
                    }
                }
            }
        }

        protected void cmdTENT_Delete_Click(object p_sender, System.EventArgs p_e)
        {
            ArrayList selectedIndices = new ArrayList();

            int numRows = m_dgTENT.BindingContext[m_dgTENT.DataSource, m_dgTENT.DataMember].Count;             
            
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                if (m_dgTENT.IsSelected(rowIndex))
                {
                    selectedIndices.Add((int) m_dgTENT[rowIndex, 0]);
                }
            }

            foreach (int index in selectedIndices)
            {
                foreach (DataRow row in m_dtTENT.Rows)
                {
                    int x = (int) row["tent", Util.getDataRowVersion(row)];
                    if (x == index)
                    {
                        row.Delete();
                    }
                }
            }
            TENTRowSelected();
        }

        protected void cmdTMEDSave_Click(object p_sender, System.EventArgs p_e)
        {
            TMEDSave();
        }

        private void TMEDSave()
        {
            bool fSuccess = true;

            foreach (DataRow row in m_dtTMED.Rows)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        if (!Db.TMEDInsert(m_parent.m_sqlCommand, row))
                        {
                            fSuccess = false;
                            break;
                        }
                        break;

                    case DataRowState.Modified:
                        if (!Db.TMEDUpdate(m_parent.m_sqlCommand, row))
                        {
                            fSuccess = false;
                            break;
                        }
                        break;

                    case DataRowState.Deleted:
                        if (DialogResult.Yes != MessageBox.Show("Media '" 
                            + row["tmed_title", Util.getDataRowVersion(row)] + "' löschen?" ,
                            "Bestätigen", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            fSuccess = false;
                            break;
                        }
                        if (!Db.TMEDDelete(m_parent.m_sqlConnection, m_parent.m_sqlCommand, row))
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
                m_dtTMED.AcceptChanges();
            }
            else
            {
                m_dtTMED.RejectChanges();
            }
        }

        protected void cmdTENTSave_Click(object p_sender, System.EventArgs p_e)
        {
            TENTSave();
        }

        private void TENTSave()
        {
            bool fSuccess = true;

            foreach (DataRow row in m_dtTENT.Rows)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        if (!Db.TENTInsert(m_parent.m_sqlCommand, row))
                        {
                            fSuccess = false;
                            break;
                        }
                        break;

                    case DataRowState.Modified:
                        if (!Db.TENTUpdate(m_parent.m_sqlCommand, row))
                        {
                            fSuccess = false;
                            break;
                        }
                        break;

                    case DataRowState.Deleted:
                        if (!Db.TENTDelete(m_parent.m_sqlCommand, row))
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
                m_dtTENT.AcceptChanges();
            }
            else
            {
                m_dtTENT.RejectChanges();
            }
        }

        public override void edit(bool p_fIsEdit)
        {
            TMEDDetailsClear();
            TENTDetailsClear();
            TMEDButtonsXable(p_fIsEdit);
            TENTDetailsXable(p_fIsEdit);
            TENTButtonsXable(p_fIsEdit);
        }

        public override void DeftablesChanged()
        {
            populateDeftables(true);
        }
    }
}

