    using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Media
{
	public class frmMain : CmDbForm
	{
        private CmDbTabControl m_tabControl;
        private CmDbTabPage    m_tabPageMedia;
        private CmDbTabPage    m_tabPageEntries;
        private CmDbTabPage    m_tabPageArtist;
        private CmDbTabPage    m_tabPageDeftables;

        private MenuItem        m_mnFileEdit;
        private bool            m_fInEdit;

        public frmMain()
		{
            m_fInEdit = false;
            populate();
            adjustComponentsAfterPopulate();
        }

        public void DeftablesChanged()
        {
            m_tabPageMedia.DeftablesChanged();
            m_tabPageEntries.DeftablesChanged();
            m_tabPageArtist.DeftablesChanged();
        }

        public void populate()
        {
            m_tabControl.connect();

            m_tabPageMedia.populate();
            m_tabPageEntries.populate();
            m_tabPageArtist.populate();
            m_tabPageDeftables.populate();
        }

        protected override void OnResize(EventArgs e)
        {
            System.Drawing.Size size = Size;

            size.Width -= 30;
            size.Height -= 60;

            m_tabControl.Size = size;
        }

        private void adjustComponentsAfterPopulate()
        {
            m_tabPageMedia.adjustComponentsAfterPopulate();
            m_tabPageEntries.adjustComponentsAfterPopulate();
            m_tabPageArtist.adjustComponentsAfterPopulate();
            m_tabPageDeftables.adjustComponentsAfterPopulate();
        }

        protected override void InitializeComponent()
		{
            int i;

            m_tabControl = new CmDbTabControl(this);

            m_tabPageMedia = new frmMedia(m_tabControl);
            m_tabPageEntries = new frmEntries(m_tabControl);
            m_tabPageArtist = new frmArtist(m_tabControl);
            m_tabPageDeftables = new frmDeftables(m_tabControl);
            
            m_tabPageMedia.InitializeComponent();
            m_tabPageEntries.InitializeComponent();
            m_tabPageArtist.InitializeComponent();
            m_tabPageDeftables.InitializeComponent();

            m_tabControl.SuspendLayout();

            m_tabPageMedia.SuspendLayout();
            m_tabPageEntries.SuspendLayout();
            m_tabPageArtist.SuspendLayout();
            m_tabPageDeftables.SuspendLayout();

            SuspendLayout();

            //
            // Menu
            //
            MainMenu mainMenu = new MainMenu();

            MenuItem mnFile = new MenuItem();
            m_mnFileEdit = new MenuItem();
            MenuItem mnFileRefresh = new MenuItem();
            MenuItem mnFileSave = new MenuItem();
            MenuItem mnFileImport = new MenuItem();
            MenuItem mnFileExit = new MenuItem();

            MenuItem mnHelp = new MenuItem();
            MenuItem mnHelpAbout = new MenuItem();

            mainMenu.MenuItems.AddRange(new MenuItem[] {
                mnFile,
                mnHelp});

            // 
            // mnFile
            // 
            mnFile.Index = 0;
            mnFile.Text = "File";
            mnFile.MenuItems.AddRange(new MenuItem[] 
            {
                m_mnFileEdit,
                mnFileRefresh,
                mnFileSave,
                mnFileImport,
                mnFileExit});

            i = 0;

            // 
            // m_mnFileEdit
            // 
            m_mnFileEdit.Index = i++;
            m_mnFileEdit.Shortcut = Shortcut.CtrlE;
            m_mnFileEdit.Text = "Edit";
            m_mnFileEdit.Click += new System.EventHandler(mnFileEdit_Click);

            // 
            // mnFileRefresh
            // 
            mnFileRefresh.Index = i++;
            mnFileRefresh.Shortcut = Shortcut.CtrlR;
            mnFileRefresh.Text = "Refresh";
            mnFileRefresh.Click += new System.EventHandler(mnFileRefresh_Click);

            // 
            // mnFileSave
            // 
            mnFileSave.Index = i++;
            mnFileSave.Shortcut = Shortcut.CtrlS;
            mnFileSave.Text = "Save";
            mnFileSave.Click += new System.EventHandler(mnFileSave_Click);

            // 
            // mnFileImport
            // 
            mnFileImport.Index = i++;
            mnFileImport.Shortcut = Shortcut.CtrlI;
            mnFileImport.Text = "Import some data...";
            mnFileImport.Click += new System.EventHandler(mnFileImport_Click);

            // 
            // mnFileExit
            // 
            mnFileExit.Index = i++;
            mnFileExit.Shortcut = Shortcut.AltF4;
            mnFileExit.Text = "Exit";
            mnFileExit.Click += new System.EventHandler(mnFileExit_Click);

            // 
            // mnHelp
            // 
            mnHelp.Index = 1;
            mnHelp.MenuItems.AddRange(new MenuItem[] {
                mnHelpAbout});
            mnHelp.Text = "Help";

            // 
            // mnHelpAbout
            // 
            mnHelpAbout.Index = 0;
            mnHelpAbout.Text = "About Media Master";
            mnHelpAbout.Click += new System.EventHandler(mnHelpAbout_Click);

            // 
            // m_tabControl
            // 
            m_tabControl.Controls.AddRange(new Control[] {
                m_tabPageMedia,
                m_tabPageEntries,
                m_tabPageArtist,
                m_tabPageDeftables
            });
            m_tabControl.Dock = DockStyle.Fill;
            m_tabControl.SelectedIndex = 0;
            m_tabControl.TabIndex = 0;

            // 
            // frmMain
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new System.Drawing.Size(820, 680);
            Controls.AddRange(new Control[] {m_tabControl});
            Text = "Media";
            Menu = mainMenu;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;;

            m_tabControl.ResumeLayout(false);
            m_tabPageMedia.ResumeLayout(false);
            m_tabPageEntries.ResumeLayout(false);
            m_tabPageArtist.ResumeLayout(false);
            m_tabPageDeftables.ResumeLayout(false);
            ResumeLayout(false);
        }

        protected override void FormLoad(object sender, System.EventArgs e)
        {
            base.FormLoad(sender, e);

            m_tabPageMedia.FormLoad();
            m_tabPageEntries.FormLoad();
            m_tabPageArtist.FormLoad();
            m_tabPageDeftables.FormLoad();
        }

        private void mnFileExit_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }
        private void mnHelpAbout_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show("Media 1.0 (c) 2003 Christoph Maurer");
        }

        private void mnFileImport_Click(object sender, System.EventArgs e)
        {
            frmImporterDlg findDlg = new frmImporterDlg(m_tabControl);
            DialogResult result = findDlg.ShowDialog(this);
        }

        private void mnFileSave_Click(object sender, System.EventArgs e)
        {
            m_tabPageMedia.save();
            m_tabPageEntries.save();
            m_tabPageArtist.save();
            m_tabPageDeftables.save();
        }

        private void mnFileRefresh_Click(object sender, System.EventArgs e)
        {
            m_tabPageMedia.populate();
            m_tabPageEntries.populate();
            m_tabPageArtist.populate();
            m_tabPageDeftables.populate();
        }

        private void mnFileEdit_Click(object sender, System.EventArgs e)
        {
            m_fInEdit = !m_fInEdit;

            m_tabPageMedia.edit(m_fInEdit);
            m_tabPageMedia.populate();
            m_tabPageEntries.edit(m_fInEdit);
            m_tabPageEntries.populate();
            m_tabPageArtist.edit(m_fInEdit);
            m_tabPageArtist.populate();
            m_tabPageDeftables.edit(m_fInEdit);
            m_tabPageDeftables.populate();
        }

        [STAThread]
		static void Main() 
		{
            Application.Run(new frmMain());
		}
	}
}
