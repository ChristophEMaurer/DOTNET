using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;


namespace ImageResize
{
    public partial class Form1 : Form
    {
        List<ResizeData> _data = new List<ResizeData>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            X.View = View.Details;
            X.GridLines = true;
            X.FullRowSelect = true;

            X.Columns.Add("Device", 200);
            X.Columns.Add("X", 200);
            X.Columns.Add("Y", -2);

            string device = "iPhone-3.5";
            _data.Add(new ResizeData(device, 640, 920));
            _data.Add(new ResizeData(device, 640, 960));
            _data.Add(new ResizeData(device, 960, 600));
            _data.Add(new ResizeData(device, 960, 640));

            device = "iPhone-4";
            _data.Add(new ResizeData(device, 640, 1096));
            _data.Add(new ResizeData(device, 640, 1136));
            _data.Add(new ResizeData(device, 1136, 600));
            _data.Add(new ResizeData(device, 1136, 640));

            device = "iPhone-4.7";
            _data.Add(new ResizeData(device, 750, 1334));
            _data.Add(new ResizeData(device, 1334, 750));

            device = "iPhone-5.5";
            _data.Add(new ResizeData(device, 1242, 2208));
            _data.Add(new ResizeData(device, 2208, 1242));

            device = "iPhone-6";
            _data.Add(new ResizeData(device, 1242, 2208));
            _data.Add(new ResizeData(device, 2208, 1242));

            device = "iPad-9.7";
            _data.Add(new ResizeData(device, 1024, 748));
            _data.Add(new ResizeData(device, 1024, 768));
            _data.Add(new ResizeData(device, 2048, 1496));
            _data.Add(new ResizeData(device, 2048, 1536));
            _data.Add(new ResizeData(device, 768, 1004));
            _data.Add(new ResizeData(device, 768, 1024));
            _data.Add(new ResizeData(device, 1536, 2008));
            _data.Add(new ResizeData(device, 1536, 2048));

            device = "iPad-12.9";
            _data.Add(new ResizeData(device, 2732, 2048));
            _data.Add(new ResizeData(device, 2048, 2732));

            device = "Icon";
            _data.Add(new ResizeData(device, 57, 57, "Icon.png"));
            _data.Add(new ResizeData(device, 114, 114, "Icon@2x.png"));
            _data.Add(new ResizeData(device, 72, 72, "Icon-72.png"));
            _data.Add(new ResizeData(device, 144, 144, "Icon-72@2x.png"));
            _data.Add(new ResizeData(device, 29, 29, "Icon-Small.png"));
            _data.Add(new ResizeData(device, 58, 58, "Icon-Small@2x.png"));
            _data.Add(new ResizeData(device, 50, 50, "Icon-Small-50.png"));
            _data.Add(new ResizeData(device, 100, 100, "IIcon-Small-50@2xcon.png"));

            device = "LaunchImage-iPhone";
            _data.Add(new ResizeData(device, 320, 480, "Default.png"));
            _data.Add(new ResizeData(device, 640, 960, "Default@2x.png"));
            _data.Add(new ResizeData(device, 640, 1136, "Default-568h@2x.png"));

            device = "LaunchImage-iPad";
            _data.Add(new ResizeData(device, 2048, 1496, "Default-Landscape@2x~ipad.png"));
            _data.Add(new ResizeData(device, 1024, 748, "Default-Landscape~ipad.png"));
            _data.Add(new ResizeData(device, 1536, 2008, "Default-Portrait@2x~ipad.png"));
            _data.Add(new ResizeData(device, 768, 1004, "Default-Portrait~ipad"));

            foreach (ResizeData data in _data)
            {
                ListViewItem lvi = new ListViewItem(data._device);
                lvi.SubItems.Add(Convert.ToString(data._x));
                lvi.SubItems.Add(Convert.ToString(data._y));

                X.Items.Add(lvi);
            }
        }

        private void SelectSourceImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "*.*|*.*";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                string fileName = dlg.FileName;

                txtSource.Text = fileName;
            }
        }

        private void SelectSourceFolder()
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            DialogResult result = dlg.ShowDialog();

            if (DialogResult.OK == result)
            {
                string folder = dlg.SelectedPath;

                txtFolder.Text = folder;
            }
        }

        private void cmdSource_Click(object sender, EventArgs e)
        {
            SelectSourceImage();
        }

        public System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxHeight)
        {
            double ratio = (double)maxHeight / image.Height;

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public System.Drawing.Bitmap ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            Bitmap newImage = new Bitmap(maxWidth, maxHeight);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, maxWidth, maxHeight);
            }

            return newImage;
        }

        private void ResizeImage(string fileName, int index)
        {
            ResizeData data = _data[index];

            FileInfo fileInfo = new FileInfo(fileName);
            System.Drawing.Image objImage = Image.FromFile(fileName);
            System.Drawing.Bitmap objBitmap;

            objBitmap = ScaleImage(objImage, data._x, data._y);
            // objImage = ScaleImage(objImage, data._x, data._y);

            DirectoryInfo directoryInfo = new DirectoryInfo(@"d:\temp");
            directoryInfo.CreateSubdirectory(data._device);

            string outFileName;
            
            if (string.IsNullOrEmpty(data._fileName))
            {
                // outFileName = @"d:\temp\" + data._device + @"\" + data._device + "-" + data._x + "-" + data._y + ".png";
                outFileName = @"d:\temp\" + data._device + @"\" + fileInfo.Name + "-" + data._x + "-" + data._y + ".jpg";
            }
            else
            {
                outFileName = @"d:\temp\" + data._device + @"\" + data._fileName;
            }

            Bitmap bitmap2 = ReplaceTransparency(objBitmap, System.Drawing.Color.White);


            // objImage.Save(outFileName);
            // objBitmap.Save(outFileName);
            bitmap2.Save(outFileName);

            fileInfo = null;
            objImage.Dispose();
            objImage = null;
            bitmap2.Dispose();
            bitmap2 = null;
        }

        public static System.Drawing.Bitmap ReplaceTransparency(System.Drawing.Bitmap bitmap, System.Drawing.Color background)
        {
            /* Important: you have to set the PixelFormat to remove the alpha channel.
             * Otherwise you'll still have a transparent image - just without transparent areas */
            var result = new System.Drawing.Bitmap(bitmap.Size.Width, bitmap.Size.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var g = System.Drawing.Graphics.FromImage(result);

            g.Clear(background);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.DrawImage(bitmap, 0, 0);

            return result;
        }

        private void ResizeFolder(string folder, int index)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folder);

            foreach(FileInfo fileInfo in dirInfo.GetFiles("*.*"))
            {
                ResizeImage(fileInfo.FullName, index);
            }

            foreach (DirectoryInfo subfolder in dirInfo.GetDirectories())
            {
                ResizeFolder(subfolder.FullName, index);
            }
        }

        private void cmdResizeSelected_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection selection = X.SelectedIndices;

            for (int i = 0; i < X.Items.Count; i++)
            {
                if (selection.Contains(i))
                {
                    if (chkFolder.Checked)
                    {
                        ResizeFolder(txtFolder.Text, i);
                    }
                    else
                    {
                        ResizeImage(txtSource.Text, i);
                    }
                }
            }
        }

        private void cmdResizeAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < X.Items.Count; i++)
            {
                if (chkFolder.Checked)
                {
                    ResizeFolder(txtFolder.Text, i);
                }
                else
                {
                    ResizeImage(txtSource.Text, i);
                }
            }
        }

        private void cmdFolder_Click(object sender, EventArgs e)
        {
            SelectSourceFolder();
        }
    }
}
