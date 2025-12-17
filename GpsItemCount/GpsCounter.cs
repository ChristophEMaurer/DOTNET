/*
 * GPS track point counter - count the number of track points in a .gps file
 * This is useful if your device has limits.
 * 
 * If you need help, contact me!
 * 
 * Christoph Maurer
 * Elisabethenstr. 38
 * 61184 Karben
 * Germany
 * 
 * ch.maurer@gmx.de
 */

using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace GpsItemCount
{
    public partial class frmMain : Form
    {
        string TempFolderName = "d:\\temp";

        GpxCounter _program = new GpxCounter();

        public frmMain()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            lblGpxTestInfo.Text = "Create a dummy .gpx file in folder '" + TempFolderName + "' so that you can test how much your device can handle.";
            lblNotepadInfo.Text = "Open the data below in notepad. You can save and import this data into Excel.";

            lvData.View = View.Details;
            lvData.GridLines = true;
            lvData.FullRowSelect = true;

            lvData.Columns.Add("FileName", 150);
            lvData.Columns.Add("trk", 250);
            lvData.Columns.Add("trkpt", 100);
            lvData.Columns.Add("wpt", -2);

            _program = new GpxCounter();
            _program._lvData = lvData;

            cbFolders.Items.Add(@"D:\daten\cmaurer\doc\wandern\PCT\GPS\ca_state_gps");
            cbFolders.Items.Add(@"D:\daten\cmaurer\doc\wandern\GR10-GR11-HRP\GR11-GPS");

            cbFolders.Text = (string) cbFolders.Items[0];
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdCount_Click(object sender, EventArgs e)
        {
            lvData.Items.Clear();

            int stopParsingCount;
            Int32.TryParse(txtStop.Text, out stopParsingCount);

            DirectoryInfo folder = new DirectoryInfo(cbFolders.Text);

            _program._stopParsing = chkStop.Checked;
            _program._stopParsingCount = stopParsingCount;
            _program._countTrackpoints = 0;
            _program._countWaypoints = 0;

            _program.countNodes(folder, chkRecurse.Checked);

            ListViewItem lvi = new ListViewItem("Sum");
            lvi.SubItems.Add("");
            lvi.SubItems.Add(_program._countTrackpoints.ToString());
            lvi.SubItems.Add(_program._countWaypoints.ToString());
            lvData.Items.Add(lvi);
        }

        private void cmdOpenInNotepad_Click(object sender, EventArgs e)
        {
            string fileName = "d:\\temp\\text.txt";

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

        private void cmdCreateGpx_Click(object sender, EventArgs e)
        {

            try
            {
                Directory.CreateDirectory(TempFolderName);
            }
            catch (Exception exception)
            {
                ListViewItem lvi = new ListViewItem("Could not create folder '" + TempFolderName + "'");
            }

            string fileName = "d:\\temp\\test-" + txtTrkPts.Text + ".gpx";

            StreamWriter writer = new StreamWriter(fileName);

            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");

            writer.WriteLine(@"
<gpx xmlns=""http://www.topografix.com/GPX/1/1""
     creator=""Christoph Maurer"" version=""1.1""
     xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
     xsi:schemaLocation=""http://www.topografix.com/GPX/1/1
     http://www.topografix.com/GPX/1/1/gpx.xsd"">");

     writer.WriteLine("<trk>");
            writer.WriteLine("<name>Test-" + txtTrkPts.Text + "</name>");
            writer.WriteLine("<trkseg>");

            int noTrackPoints = Convert.ToInt32(txtTrkPts.Text);

            int offset = 0;
            for (int i = 0; i < noTrackPoints; i++)
            {
                offset += 50;
                if (offset > 999999)
                {
                    break;
                }
                string coord = String.Format("{0:000000}", offset);

                //
                // <trkpt lat="50.168716" lon="8.623258" />
                // <trkpt lat="32.595078" lon="-116.466729"></trkpt>
                // 
                string line = "<trkpt lat=\"32." + coord + "\" lon=\"8." + coord + "\"></trkpt>";
                writer.WriteLine(line);
            }

            writer.WriteLine("</trkseg>");
            writer.WriteLine("</trk>");
            writer.WriteLine("</gpx>");

            writer.Close();
            writer = null;
             
            Process.Start("notepad.exe", fileName);


        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
