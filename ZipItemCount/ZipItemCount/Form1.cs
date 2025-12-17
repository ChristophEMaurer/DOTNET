using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace ZipItemCount
{
    public partial class frmMain : Form
    {
        ZipCounter _program = new ZipCounter();

        public frmMain()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            lvData.View = View.Details;
            lvData.GridLines = true;
            lvData.FullRowSelect = true;

            lvData.Columns.Add("Archive", 150);
            lvData.Columns.Add("Files", 150);
            lvData.Columns.Add("FullName", -2);

            _program = new ZipCounter();
            _program._lvData = lvData;

            cbFolders.Items.Add(@"D:\daten\cmaurer\doc\wandern\USA-CDT-PCT-AT-OCD\USA-PNT-PacificNorthwestTrail\maps");
            cbFolders.Text = (string) cbFolders.Items[0];

            txtExtensions.Text = "*.zip;*.7z";
            txtExclude.Text = "_MACOSX";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdCount_Click(object sender, EventArgs e)
        {
            lvData.Items.Clear();

            DirectoryInfo folder = new DirectoryInfo(cbFolders.Text);

            _program._countFiles = 0;

            _program.countNodes(folder, chkRecurse.Checked, chkIncludeFiles.Checked, txtExtensions.Text, txtExclude.Text);

            ListViewItem lvi = new ListViewItem("Sum");
            lvi.SubItems.Add(_program._countFiles.ToString());
            lvi.SubItems.Add("");
            lvData.Items.Add(lvi);
        }

        private void cmdOpenInNotepad_Click(object sender, EventArgs e)
        {
            string fileName = "d:\\temp\\text.txt";

            try
            {
                StreamWriter writer = new StreamWriter(fileName);

                StringBuilder sb = new StringBuilder();

                foreach (ListViewItem lvi in lvData.Items)
                {
                    sb.Clear();
                    for (int i = 0; i < lvi.SubItems.Count; i++)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(";");
                        }
                        sb.Append(lvi.SubItems[i].Text);
                    }

                    writer.WriteLine(sb.ToString());
                }

                writer.Close();
                writer = null;

                Process.Start("notepad.exe", fileName);

            }
            catch (Exception exception)
            {
                ListViewItem lvi = new ListViewItem(exception.Message);
                lvData.Items.Add(lvi);
            }
        }
    }
}
