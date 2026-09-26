namespace fsd
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Data;
    using System.Data.SqlClient;

    public class FsdDbTabControl : TabControl
    {
        public SqlConnection   m_sqlConnection;
        public SqlCommand      m_sqlCommand;
        public SqlDataReader   m_sqlDataReader;

        private bool m_bInitialized;

        protected const string s_strUsage = "Available options are: [-DEBUG] [-USER]";

        ~FsdDbTabControl()
        {
            if (m_sqlConnection != null && m_sqlConnection.State == ConnectionState.Open)
            {
                m_sqlConnection.Close();
            }
        }
        
        public FsdDbTabControl() : this(null)        
        {
        }

        public FsdDbTabControl(string[] p_args) : base()        
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
            }
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

    public abstract class FsdDbTabPage : TabPage
    {
        protected FsdDbTabControl m_parent;

        public FsdDbTabPage(FsdDbTabControl p_parent) : base()
        {
            m_parent = p_parent;
        }

        public abstract void createComponents();
        public abstract void populate();
        public virtual void adjustComponentsAfterPopulate() {}
    }

    public class SxsDbSplitter : Splitter
    {
        public SxsDbSplitter(DockStyle p_dockStyle): base()
        {
            this.BackColor = Color.Blue;
            this.Width = 7;
            this.Height = 7;
            this.TabStop = false;
            this.Dock = p_dockStyle;
            this.BorderStyle = BorderStyle.Fixed3D;
        }
    }

    public class SxsDbPanel : Panel
    {
        public SxsDbPanel(DockStyle p_dockStyle) : base()
        {
            this.Dock = p_dockStyle;
            this.TabStop = false;
        }

        public SxsDbPanel() : this(DockStyle.None) {}
    }

    public class SxsDbButtonPanel : SxsDbPanel
    {
        public SxsDbButtonPanel(DockStyle p_dockStyle) : base(p_dockStyle)
        {
            Height = 40;
        }
    }

    public class SxsDbDataGrid : DataGrid 
    {
        public SxsDbDataGrid(DockStyle p_dockStyle) : base()
        {
            Dock = p_dockStyle;
            RowHeadersVisible = true;
        }
        public SxsDbDataGrid() : base() 
        {
            RowHeadersVisible = true;
        }
    }

    public class SxsDbDataTable : DataTable
    {
        public SxsDbDataTable(string p_strTableName) : base(p_strTableName){}
    }

    public abstract class SxsDbForm : Form
    {
        protected SqlConnection   m_sqlConnection;
        protected SqlCommand      m_sqlCommand;
        protected SqlDataReader   m_sqlDataReader;

        private bool m_bInitialized;

        protected const string s_strUsage = "Available options are: [-DEBUG] [-USER]";

        ~SxsDbForm()
        {
            if (m_sqlConnection != null && m_sqlConnection.State == ConnectionState.Open)
            {
                m_sqlConnection.Close();
            }
        }

        public SxsDbForm() : this(null)        
        {
        }

        public SxsDbForm(string[] p_args) : base()        
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

        protected abstract void InitializeComponent();

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

    public abstract class SxsDbFormWithSplitters : SxsDbForm
    {
        protected Panel       m_pnlStatusBar;

        public SxsDbFormWithSplitters(string[] p_args) : base(p_args) {}
        public SxsDbFormWithSplitters() : base() {}

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
            m_pnlStatusBar = new SxsDbPanel(DockStyle.Bottom);
            m_pnlStatusBar.Height = 50;
            m_pnlStatusBar.BackColor = SystemColors.ScrollBar;
            m_pnlStatusBar.DockPadding.All = 5;
        }

    }
}
