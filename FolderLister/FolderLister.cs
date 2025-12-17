using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AndroidFolderLister
{
    public partial class FolderLister : Form
    {
        List<string> _excludeList = new List<string>();

        public FolderLister()
        {
            InitializeComponent();
        }

        private void cmdRead_Click(object sender, EventArgs e)
        {
            lvData.Items.Clear();

            _excludeList.Clear();

            string[] arExclude = txtExclude.Text.Split(',');
            foreach (string word in arExclude)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    _excludeList.Add(word.ToLower());
                }
            }

            DirectoryInfo rootPath = new DirectoryInfo(txtPath.Text);

            ReadNodeFolderRecursive(rootPath);
        }

        private void ReadNodeFolderRecursive(DirectoryInfo rootPath)
        {
            DirectoryInfo[] folders = rootPath.GetDirectories();
            if (folders.Length == 0)
            {
                AddFolderToGui(rootPath);
            }
            else
            {
                foreach (DirectoryInfo subfolder in folders)
                {
                    ReadNodeFolderRecursive(subfolder);
                }
            }
        }

        private void AddFolderToGui(DirectoryInfo folder)
        {
            bool exclude = false;

            string folderLowerCase = folder.FullName.ToLower();

            foreach (string word in _excludeList)
            {
                if (folderLowerCase.IndexOf(word) != -1)
                {
                    exclude = true;
                    break;
                }
            }

            if (!exclude)
            {
                ListViewItem lvi = new ListViewItem(folder.Name);
                lvi.SubItems.Add(folder.FullName);
                lvData.Items.Add(lvi);
            }
        }

        private void FolderLister_Load(object sender, EventArgs e)
        {
            lvData.View = View.Details;
            lvData.GridLines = true;
            lvData.FullRowSelect = true;
            lvData.Sorting = SortOrder.Ascending;

            lvData.Columns.Add("Folder", 300);
            lvData.Columns.Add("FullName", -2);

            lblInfo.Text = "Read all files from the specified folder but exclude all files which match the exclude list. Files are displayed only, they are not touched in any way";
        }
    }
}
