

using System.IO;

namespace MoveFotosToFolder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lvData.Clear();

            lvData.View = View.Details;

            lvData.Columns.Add("1", 200);
            lvData.Columns.Add("2", -2);

        }

        private bool isValidDate(int month, int day)
        {
            bool ret = false;

            switch (month)
            {
                case 5:
                    if (day > 1)
                    {
                        ret = true;
                    }
                    break;

                case 6:
                case 7:
                case 8:
                    ret = true;
                    break;

                case 9:
                    if (day < 19)
                    {
                        ret = true;
                    }
                    break;
            }

            return ret;
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            DateTime dtStart = new DateTime(2025, 5, 2);

            DirectoryInfo directoryInfo = new DirectoryInfo(@"d:\temp");

            FileInfo[] arFileInfo = directoryInfo.GetFiles();

            for (int i = 0; i < arFileInfo.Length; i++)
            {
                FileInfo fileInfo = arFileInfo[i];
                // C:\Daten\Neu-ab-2025-02-20\CDT-fertig-Fotos\A54\DCIM\20250429_101559.heic
                string strFullFileName = fileInfo.FullName;

                // 20250429_101559.heic
                string strFileName = fileInfo.Name;

                // 20250429
                string strDate = strFileName.Substring(0, 8);

                int year, month, day;

                year = Int32.Parse(strDate.Substring(0, 4));
                month = Int32.Parse(strDate.Substring(4, 2));
                day = Int32.Parse(strDate.Substring(6, 2));

                if (isValidDate(month, day))
                {
                    DateTime dateTime = new DateTime(year, month, day);

                    int dayDiff = (dateTime - dtStart).Days + 1;

                    string strFolderName = strDate + "_Day_" + dayDiff.ToString("000");
                    string strNewFileName = directoryInfo.FullName + "\\" + strFolderName + "\\" + strFileName;

                    // 20250502_Day_001
                    // 20250503_Day_002
                    // ...
                    // 20250918_Day_140

                    ListViewItem lvi = new ListViewItem(strFullFileName);
                    lvi.SubItems.Add(strFolderName);
                    lvData.Items.Add(lvi);

                    //break;

                    directoryInfo.CreateSubdirectory(strFolderName);
                    fileInfo.MoveTo(strNewFileName);
                }
            }
        }
    }
}
