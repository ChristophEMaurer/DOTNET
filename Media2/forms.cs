namespace Media
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Data;
    using System.Data.SqlClient;

    public class CmButton : Button
    {
        public CmButton() : this("") {}

        public CmButton(string p_strText) : base()
        {
            Width = 80;
            Height = 24;
            Text = p_strText;
        }
    }

    public class CmLabel : Label
    {
        public CmLabel(string p_strText) : base()
        {
            AutoSize = true;
            Text = p_strText + " ";
        }
    }

    public class CmDbTabControl : TabControl
    {
        public SqlConnection   m_sqlConnection;
        public SqlCommand      m_sqlCommand;
        public SqlDataReader   m_sqlDataReader;

        private bool    m_bInitialized;
        private frmMain m_parent;

        protected const string s_strUsage = "Available options are: [-DEBUG] [-USER]";

        ~CmDbTabControl()
        {
            if (m_sqlConnection != null && m_sqlConnection.State == ConnectionState.Open)
            {
                m_sqlConnection.Close();
            }
        }
        

        public CmDbTabControl(frmMain parent) : this(null, parent)
        {
        }

        public CmDbTabControl(string[] p_args, frmMain parent) : base()
        {
            Db.s_bDebug = false;

            m_parent = parent;
            string strConnectionString = Db.s_strConnectionTrusted;

            m_bInitialized = true;
            if (p_args != null)
            {
                for (int i = 0; i < p_args.Length; i++)
                {
                    if (p_args[i].ToUpper().Equals("-DEBUG"))
                    {
                        Db.s_bDebug = true;
                    }
                    else if (p_args[i].ToUpper().Equals("-USER"))
                    {
                        strConnectionString = Db.s_strConnectionUser;
                    }
                    else
                    {
                        m_bInitialized = false;
                        break;
                    }
                }
            }
            if (m_bInitialized)
            {
                Db.ConnectionString = strConnectionString;
            }
        }

        public void DeftablesChanged()
        {
            m_parent.DeftablesChanged();
        }

        protected bool initialized
        {
            get { return m_bInitialized; }
        }

        public SqlConnection getNewSqlConnection()
        {
            return Db.getNewSqlConnection();
        }

        public void connect()
        {
            m_sqlConnection = getNewSqlConnection();
            m_sqlCommand = m_sqlConnection.CreateCommand();
        }

        public Button createButtonCommon(string p_strText, int p_nLeft, int p_nTop, int p_nWidth, int p_nHeight, EventHandler p_e)
        {
            Button b = new Button();

            b.Location = new Point(p_nLeft, p_nTop);
            b.Size = new Size(p_nWidth, p_nHeight);
            b.Text = p_strText;
            b.Click += p_e;

            return b;
        }

        public Button createButton(string p_strText, int p_nLeft, int p_nTop, int p_nWidth, int p_nHeight, EventHandler p_e)
        {
            Button b = createButtonCommon(p_strText, p_nLeft, p_nTop, p_nWidth, p_nHeight, p_e);
            this.Controls.Add(b);
            return b;
        }

        public Button createButton(string p_strText, int p_nLeft, int p_nTop, int p_nWidth, int p_nHeight, EventHandler p_e, Panel p_panel)
        {
            Button b = createButtonCommon(p_strText, p_nLeft, p_nTop, p_nWidth, p_nHeight, p_e);
            p_panel.Controls.Add(b);
            return b;
        }

        public Button createButton(string p_strText, int p_nLeft, int p_nTop, int p_nWidth, int p_nHeight, EventHandler p_e, 
            Form p_form)
        {
            Button b = createButtonCommon(p_strText, p_nLeft, p_nTop, p_nWidth, p_nHeight, p_e);
            p_form.Controls.Add(b);
            return b;
        }
    }

    public abstract class CmDbTabPage : TabPage
    {
        protected CmDbTabControl m_parent;
        protected int m_nGrpWidth = 760;

        public CmDbTabPage(CmDbTabControl p_parent) : base()
        {
            m_parent = p_parent;
        }

        public abstract void InitializeComponent();
        public virtual void populate() {}
        public virtual void adjustComponentsAfterPopulate() {}
        public virtual void save() {}
        public virtual void edit(bool p_fInEdit) {}
        public virtual void FormLoad() {}
        public virtual void DeftablesChanged(){}
    }

    public class CmDbSplitter : Splitter
    {
        public CmDbSplitter(DockStyle p_dockStyle): base()
        {
            this.BackColor = Color.Blue;
            this.Width = 7;
            this.Height = 7;
            this.TabStop = false;
            this.Dock = p_dockStyle;
            this.BorderStyle = BorderStyle.Fixed3D;
        }
    }

    public class CmDbPanel : Panel
    {
        public CmDbPanel(DockStyle p_dockStyle) : base()
        {
            this.Dock = p_dockStyle;
            this.TabStop = false;
        }

        public CmDbPanel() : this(DockStyle.None) {}
    }

    public class CmDbButtonPanel : CmDbPanel
    {
        public CmDbButtonPanel(DockStyle p_dockStyle) : base(p_dockStyle)
        {
            Height = 40;
        }
    }

    public class CmDataGrid : DataGrid 
    {
        private Point m_pointInCell00;

        public CmDataGrid(DockStyle p_dockStyle) : base()
        {
            Dock = p_dockStyle;
            RowHeadersVisible = true;
            ReadOnly = true;
        }
        public CmDataGrid() : this(DockStyle.None) 
        {
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            DataGrid.HitTestInfo hti = this.HitTest(new Point(e.X, e.Y));
            if(hti.Type == DataGrid.HitTestType.RowResize) 
            {
                return; //no baseclass call
            }
            base.OnMouseMove(e);
        }

        public void ResizeLastColumn()
        {
            TableStyles[0].RowHeaderWidth = 50;
            RowHeaderWidth = 200; //use if no tablestyle
			
            //set topleft corner point so we can locate the toprow
            m_pointInCell00 = new Point(GetCellBounds(0,0).X + 4, GetCellBounds(0,0).Y + 4);

            int numCols = ((DataView)(DataSource)).Table.Columns.Count; 
 
            //the fudge -4 is for the grid borders 
            int targetWidth = ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4; 
            int runningWidthUsed = this.TableStyles[0].RowHeaderWidth; 
            for (int i = 0; i < numCols - 1; ++i)
            {
                runningWidthUsed += this.TableStyles[0].GridColumnStyles[i].Width; 
            }

            if (runningWidthUsed < targetWidth)
            {
                this.TableStyles[0].GridColumnStyles[numCols - 1].Width = targetWidth - runningWidthUsed; 
            }
        }

        public int TopRow()
        {
            DataGrid.HitTestInfo hti = HitTest(m_pointInCell00);
            return hti.Row;
        }

        public int NumRows
        {
            get { return BindingContext[DataSource, DataMember].Count; }
        }

        public int selectRowByValue(int p_nColumn, string p_strValue)
        {
            for (int rowIndex = 0; rowIndex < NumRows; rowIndex++)
            {
                string strValue =  this[rowIndex, p_nColumn].ToString();
                if (0 == strValue.CompareTo(p_strValue))
                {
                    this.CurrentRowIndex = rowIndex;
                    this.Select(rowIndex);
                    return rowIndex;
                }
            }

            return -1;
        }
    }

    public class CmDbDataTable : DataTable
    {
        public CmDbDataTable(string p_strTableName) : base(p_strTableName){}
    }

    public abstract class CmDbForm : Form
    {
        protected SqlConnection   m_sqlConnection;
        protected SqlCommand      m_sqlCommand;
        protected SqlDataReader   m_sqlDataReader;

        private bool m_bInitialized;

        protected const string s_strUsage = "Available options are: [-DEBUG] [-USER]";

        protected virtual void InitializeComponent() {}

        ~CmDbForm()
        {
            if (m_sqlConnection != null && m_sqlConnection.State == ConnectionState.Open)
            {
                m_sqlConnection.Close();
            }
        }

        public CmDbForm() : this(null)        
        {
        }

        public CmDbForm(string[] p_args) : base()        
        {
            Db.s_bDebug = false;

            string strConnectionString = Db.s_strConnectionTrusted;

            m_bInitialized = true;
            if (p_args != null)
            {
                for (int i = 0; i < p_args.Length; i++)
                {
                    if (p_args[i].ToUpper().Equals("-DEBUG"))
                    {
                        Db.s_bDebug = true;
                    }
                    else if (p_args[i].ToUpper().Equals("-USER"))
                    {
                        strConnectionString = Db.s_strConnectionUser;
                    }
                    else
                    {
                        m_bInitialized = false;
                        break;
                    }
                }
            }

            if (m_bInitialized)
            {
                Db.ConnectionString = strConnectionString;

                InitializeComponent();

                Load += new System.EventHandler(FormLoad);

                adjustFormSize();

                FormBorderStyle = FormBorderStyle.Sizable;
            }
            else
            {
                Console.WriteLine(s_strUsage);
                MessageBox.Show(s_strUsage, "speeddating database form");
            }
        }

        protected virtual void FormLoad(object sender, System.EventArgs e)
        {
            connect();
        }

        public virtual void adjustFormSize()
        {
            int nBottom = 0;
            int nRight = 0;

            foreach (Control control in Controls)
            {
                if (control.Bottom > nBottom)
                {
                    nBottom = control.Bottom;
                }
                if (control.Right > nRight)
                {
                    nRight = control.Right;
                }
            }

            ClientSize = new Size(nRight + 10, nBottom + 10);
        }

        public virtual ProgressBar createProgressBar()
        {
            int nBottom = 0;
            int nRight = 0;

            foreach (Control control in Controls)
            {
                if (control.Bottom > nBottom)
                {
                    nBottom = control.Bottom;
                }
                if (control.Right > nRight)
                {
                    nRight = control.Right;
                }
            }

            ProgressBar progress = new ProgressBar();
            progress.Left = 10;
            progress.Top = nBottom + 10;
            progress.Width = nRight - progress.Left;
            
            Controls.Add(progress);

            return progress;
        }

        protected bool initialized
        {
            get { return m_bInitialized; }
        }

        public SqlConnection getNewSqlConnection()
        {
            return Db.getNewSqlConnection();
        }

        protected void connect()
        {
            m_sqlConnection = getNewSqlConnection();
            m_sqlCommand = m_sqlConnection.CreateCommand();
        }

        protected Button createButtonCommon(string p_strText, int p_nLeft, int p_nTop, int p_nWidth, int p_nHeight, EventHandler p_e)
        {
            Button b = new Button();

            b.Location = new Point(p_nLeft, p_nTop);
            b.Size = new Size(p_nWidth, p_nHeight);
            b.Text = p_strText;
            b.Click += p_e;

            return b;
        }

        protected Button createButton(string p_strText, int p_nLeft, int p_nTop, int p_nWidth, int p_nHeight, EventHandler p_e)
        {
            Button b = createButtonCommon(p_strText, p_nLeft, p_nTop, p_nWidth, p_nHeight, p_e);
            this.Controls.Add(b);
            return b;
        }

        protected Button createButton(string p_strText, int p_nLeft, int p_nTop, int p_nWidth, int p_nHeight, EventHandler p_e, Panel p_panel)
        {
            Button b = createButtonCommon(p_strText, p_nLeft, p_nTop, p_nWidth, p_nHeight, p_e);
            p_panel.Controls.Add(b);
            return b;
        }

        protected Button createButton(string p_strText, int p_nLeft, int p_nTop, int p_nWidth, int p_nHeight, EventHandler p_e, 
            Form p_form)
        {
            Button b = createButtonCommon(p_strText, p_nLeft, p_nTop, p_nWidth, p_nHeight, p_e);
            p_form.Controls.Add(b);
            return b;
        }
    }

    public abstract class CmDbFormWithSplitters : CmDbForm
    {
        protected Panel       m_pnlStatusBar;

        public CmDbFormWithSplitters(string[] p_args) : base(p_args) {}
        public CmDbFormWithSplitters() : base() {}

        public override ProgressBar createProgressBar()
        {
            ProgressBar progress = new ProgressBar();
            progress.Dock = DockStyle.Fill;
            m_pnlStatusBar.Controls.Add(progress);

            return progress;
        }

        public override void adjustFormSize() {}

        protected void setSplitterWindowSize()
        {
            Width = 1200;
            Height = 800;
            Left = 50;
            Top = 50;
        }

        protected void createStatusBarPanel()
        {
            m_pnlStatusBar = new CmDbPanel(DockStyle.Bottom);
            m_pnlStatusBar.Height = 50;
            m_pnlStatusBar.BackColor = SystemColors.ScrollBar;
            m_pnlStatusBar.DockPadding.All = 5;
        }
    }
}
