using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace GpsItemCount
{
    public class GpxCounter
    {
        public ListView _lvData;

        public Int32 _countTrackpoints;
        public Int32 _countWaypoints;

        public bool _stopParsing = false;
        public int _stopParsingCount = 1;

        private void countNodes(string fileName)
        {
            int trackCount = 0;

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);

                XmlDocument xmldoc = new XmlDocument();

                xmldoc.Load(fileName);

                //
                // Das ist eine Überschrift, falls es viele Einträge gibt in einer Datei.
                //
                 ListViewItem lvi = new ListViewItem(fileInfo.Name);
                _lvData.Items.Add(lvi);

                XmlNamespaceManager xmlnsm = new XmlNamespaceManager(xmldoc.NameTable);
                xmlnsm.AddNamespace("x", "http://www.topografix.com/GPX/1/1");

                XmlNodeList tracks = xmldoc.SelectNodes("//x:gpx/x:trk", xmlnsm);
                foreach (XmlNode track in tracks)
                {
                    trackCount++;
                    XmlNode nodeName = track.SelectSingleNode("./x:name", xmlnsm);
                    string name = nodeName.InnerText;
                    XmlNodeList trkPoints = track.SelectNodes("./x:trkseg/x:trkpt", xmlnsm);
                    int count = trkPoints.Count;
                    _countTrackpoints += count;

                    lvi = new ListViewItem(fileInfo.Name);
                    lvi.SubItems.Add(name);
                    lvi.SubItems.Add("" + count);
                    _lvData.Items.Add(lvi);

                    if (_stopParsing && trackCount >= _stopParsingCount)
                    {
                        break;
                    }
                }

                XmlNodeList nodes = xmldoc.SelectNodes("//x:gpx/x:wpt", xmlnsm);
                int countWpt = nodes.Count;

                if (countWpt > 0)
                {
                    _countWaypoints += countWpt;

                    lvi = new ListViewItem(fileInfo.Name);
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("" + countWpt);
                    _lvData.Items.Add(lvi);
                }

            }
            catch (Exception exception)
            {
            }
        }

        public void countNodes(DirectoryInfo folder, bool recurse)
        {
            if (Directory.Exists(folder.FullName))
            {
                FileInfo[] files = folder.GetFiles();

                foreach (FileInfo file in files)
                {
                    countNodes(file.FullName);
                }

                if (recurse)
                {
                    DirectoryInfo[] subfolders = folder.GetDirectories();
                    foreach (DirectoryInfo subfolder in subfolders)
                    {
                        countNodes(subfolder, recurse);
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
