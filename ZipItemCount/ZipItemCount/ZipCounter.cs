using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;

namespace ZipItemCount
{
    public class ZipCounter
    {
        public ListView _lvData;

        public Int32 _countFiles;

        private void countNodes(string fileName, bool includeFiles, string exclude)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);

                ZipArchive zipArchive = ZipFile.OpenRead(fileName);

                ListViewItem lvi = new ListViewItem(fileInfo.Name);


                lvi.SubItems.Add(zipArchive.Entries.Count.ToString());
                lvi.SubItems.Add(fileInfo.FullName);
                _lvData.Items.Add(lvi);

                foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                {
                    string name = zipArchiveEntry.FullName;
                    if (string.IsNullOrEmpty(exclude) || name.IndexOf(exclude) == -1)
                    {
                        _countFiles++;

                        if (includeFiles)
                        {
                            lvi = new ListViewItem("");
                            lvi.SubItems.Add("");
                            //lvi.SubItems.Add(zipArchiveEntry.Name);
                            lvi.SubItems.Add(zipArchiveEntry.FullName);
                            _lvData.Items.Add(lvi);
                        }
                    }
                }

                zipArchive.Dispose();
                zipArchive = null;
            }
            catch (Exception exception)
            {
            }
        }

        public void countNodes(DirectoryInfo folder, bool recurse, bool includeFiles, string extensions, string exclude)
        {
            if (Directory.Exists(folder.FullName))
            {
                FileInfo[] files = folder.GetFiles();

                foreach (FileInfo file in files)
                {
                    if (extensions.IndexOf(file.Extension) != -1)
                    {
                        //
                        // Very simple: if the file extension is part of the UI extension textbox, then process it.
                        //
                        countNodes(file.FullName, includeFiles, exclude);
                    }
                }

                if (recurse)
                {
                    DirectoryInfo[] subfolders = folder.GetDirectories();
                    foreach (DirectoryInfo subfolder in subfolders)
                    {
                        countNodes(subfolder, recurse, includeFiles, extensions, exclude);
                    }
                }
            }
            else
            {
                ListViewItem lvi = new ListViewItem("Folder '" + folder.FullName + "' does not exist");
                _lvData.Items.Add(lvi);
            }
        }
    }
}
