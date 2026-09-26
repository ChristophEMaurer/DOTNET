using System.Globalization;
using System.IO;
using System.Reflection;

namespace CaiSiGpxGenerator
{
    public partial class MainForm : Form
    {
        private const string LogFileName = "_logfile.txt";

        public bool _abort = false;
        Dictionary<string, RegionInfo> dictRegions = new Dictionary<string, RegionInfo>();

        public MainForm()
        {
            InitializeComponent();
        }

        public void WriteProgress(string text)
        {
            lblProgress.Text = text;

            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss:fff");
            File.AppendAllText(LogFileName, timestamp + ": " + text + "\n");
            Application.DoEvents();
        }

        private void DefaultListView(ListView lv)
        {
            lv.View = View.Details;
            lv.GridLines = true;
            lv.FullRowSelect = true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            DefaultListView(lvRegions);
            lvRegions.Columns.Add("Region", 150);
            lvRegions.Columns.Add("Tracks", 150);
            lvRegions.Columns.Add("URL", -2);

            DefaultListView(lvStages);
            lvStages.Columns.Add("Region", 150);
            lvStages.Columns.Add("No", 100);
            lvStages.Columns.Add("Stage name", 150);
            lvStages.Columns.Add("URL", -2);

            cmdGenerateHtml.Text = $"3 Generate {CaiSiGpxGenerator.HtmlFileName} to output folder";
            string downloads = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "Downloads");
            txtFolderDownload.Text = downloads;

            lblInfo.Text = $"1: Click '{cmdExtractRegions.Text}' to load the upper box with all " +
                $"regions from the link in the textbox to its left." +
                $"\n\n2: Click '{cmdExtractStageURLsForRegion.Text}' or '{cmdExtractStageUrlsForAllRegions.Text}' " +
                $"to download all the stages into the lower box." +
                $"\nIf you click on a line in the upper box, all corresponding stages " +
                $"that were previously loaded will be displayed in " +
                $"the lower box." +
                $"\n\n3: Click '{cmdGenerateHtml.Text}' to generate file {CaiSiGpxGenerator.HtmlFileName} and a lot of " +
                $".js files in the folder specified " +
                $"in textbox '{lblOutputFolder.Text}'." +
                $"\n\n4: Find file {CaiSiGpxGenerator.HtmlFileName} on your hard drive (NOT in this window!!!), " +
                $"open it in your Internet Browser and click any of the buttons " +
                $"to generate the .gpx files." +
                $"\n\n5: Copy the folder name that contains the .gpx files into textbox '{lblDownloadFolder.Text}', then click " +
                $"'{cmdMoveFiles.Text}': this will move the .gpx files from the download folder '{lblDownloadFolder.Text}'" +
                $"into the correct subfolders in the output folder.'{lblOutputFolder.Text}'." +
                $"\n\nMake sure to operate in a temp folder to not destroy any files!!!" +
                $"\n\nIf something does not work, check the logfile {LogFileName}"
                ;

            File.Delete(LogFileName);
        }

        private void ExtractAndCopyTogpx(string targetFolder)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (string name in assembly.GetManifestResourceNames())
            {
                Console.WriteLine(name);
            }
            string fileName = Path.Combine(targetFolder, "togpx-0.5.4.js");

            if (!File.Exists(fileName))
            {
                using Stream input = assembly.GetManifestResourceStream(
                    "CaiSiGpxGenerator.resources.togpx-0.5.4.js");

                using FileStream outputStream = File.Create(fileName);

                input.CopyTo(outputStream);
            }

        }

        private async void cmdExtractRegions_Click(object sender, EventArgs e)
        {
            dictRegions.Clear();
            await CaiSiGpxGenerator.ExtractRegions(this, txtBaseUrl.Text, dictRegions);
            PopulateRegions();
        }

        public string GetSelectedItemRegion(ListView lv)
        {
            string text = "";

            if (lv.SelectedIndices.Count > 0)
            {
                RegionInfo regionInfo = (RegionInfo)lv.Items[lv.SelectedIndices[0]].Tag;

                if (regionInfo != null)
                {
                    text = regionInfo.region;
                }
            }

            return text;
        }

        private async void cmdExtractStageURLsForRegion_Click(object sender, EventArgs e)
        {
            lvStages.Items.Clear();

            string region = GetSelectedItemRegion(lvRegions);
            if (region.Trim().Length > 0)
            {
                await CaiSiGpxGenerator.GetRegionStageUrls(this, region, dictRegions);
            }

            PopulateRegions();
        }

        private async void cmdExtractStageURLsForAllRegions_Click(object sender, EventArgs e)
        {
            foreach (string region in dictRegions.Keys)
            {
                await CaiSiGpxGenerator.GetRegionStageUrls(this, region, dictRegions);
            }

            PopulateRegions();
        }


        private async void cmdGenerateJsFiles_Click(object sender, EventArgs e)
        {
            _abort = false;

            ExtractAndCopyTogpx(txtOutputFolder.Text);
            await CaiSiGpxGenerator.CreateHtmlFiles(this, txtOutputFolder.Text, dictRegions);
            await CaiSiGpxGenerator.CreateJsFiles(this, txtOutputFolder.Text, dictRegions);
        }

        public void PopulateRegions()
        {
            ListViewItem lvi;

            lvRegions.Items.Clear();
            lvStages.Items.Clear();

            int total = 0;

            foreach (string region in dictRegions.Keys)
            {
                RegionInfo regionInfo = dictRegions[region];
                lvi = new ListViewItem(regionInfo.regionUI + " [" + regionInfo.region + "]");
                lvi.Tag = regionInfo;
                lvi.SubItems.Add(regionInfo.trackInfos.Count.ToString());
                lvi.SubItems.Add(regionInfo.apiUrl);
                lvRegions.Items.Add(lvi);

                total = total + regionInfo.trackInfos.Count;
            }
            lvi = new ListViewItem("Total");
            lvi.SubItems.Add(total.ToString());
            lvRegions.Items.Add(lvi);

        }

        private void AddTrack(string region, string no, string track, string url)
        {
            ListViewItem lvi = new ListViewItem(region);
            lvi.SubItems.Add(no);
            lvi.SubItems.Add(track);
            lvi.SubItems.Add(url);
            lvStages.Items.Add(lvi);
        }

        private void PopulateTracks(string region)
        {
            lvStages.Items.Clear();

            if (!String.IsNullOrEmpty(region))
            {
                RegionInfo regionInfo = dictRegions[region];

                AddTrack(regionInfo.regionUI, "", "apiurl", regionInfo.apiUrl);
                AddTrack(regionInfo.regionUI, "", "count", regionInfo.trackInfos.Count.ToString());

                int count = 1;
                foreach (TrackInfo trackInfo in regionInfo.trackInfos)
                {
                    AddTrack(regionInfo.regionUI, count.ToString(), trackInfo.trackIdUi + " [" + trackInfo.trackId + "]", trackInfo.trackUrl);
                    count++;
                }
            }
        }

        private void lvRegions_SelectedIndexChanged(object sender, EventArgs e)
        {
            string region = GetSelectedItemRegion(lvRegions);

            if (!string.IsNullOrEmpty(region))
            {
                PopulateTracks(region);
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            _abort = true;
        }

        private void cmdMoveFiles_Click(object sender, EventArgs e)
        {

        }

        private void cmdMoveFiles_Click_1(object sender, EventArgs e)
        {
            CaiSiGpxGenerator.MoveFiles(this, txtFolderDownload.Text, txtOutputFolder.Text);
        }

        private void cmdTest_Click(object sender, EventArgs e)
        {
            ExtractAndCopyTogpx(txtOutputFolder.Text);
        }

        private async void cmdGenerateJs_Click(object sender, EventArgs e)
        {
            _abort = false;

        }
    }
}
