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
using System.Web;
using HtmlAgilityPack;
using System.Net;

namespace Statistik
{
    public partial class MainWindows : CMaurer.Common.BaseForm
    {
        private static string _urlAgeMax = "https://www.worldometers.info/demographics/life-expectancy/";
        private static string _urlAgeAvg = "https://www.laenderdaten.de/bevoelkerung/medianalter.aspx";
        private static string _urlAgeAvg2 = "https://www.welt-in-zahlen.de/laendervergleich.phtml?indicator=21";
        
        private static string _urlCorona = "https://www.worldometers.info/coronavirus/";
        private static string _urlPopulation = "https://www.indexmundi.com/map/?v=21&r=xx&l=de";

        /// <summary>
        /// _dataAgeAvg: (country, average age)
        /// </summary>
        public Dictionary<string, float> _dataAgeAvg = new Dictionary<string, float>();

        /// <summary>
        /// _dataAgeMax: (coutnry, max age)
        /// </summary>
        public Dictionary<string, float> _dataAgeMax = new Dictionary<string, float>();

        /// <summary>
        /// _dataPopulation: (country, noOfPopulation)
        /// </summary>
        public Dictionary<string, float> _dataPopulation = new Dictionary<string, float>();

        /// <summary>
        /// _dataCorona: (country, (#infected, #death))
        /// </summary>
        public Dictionary<string, List<float>> _dataCorona = new Dictionary<string, List<float>>();

        public Dictionary<string, int> _dataCountriesForDebugging = new Dictionary<string, int>();

        /// <summary>
        /// _dataAll; (country, (max age, avg age, population, infected, dead))
        /// </summary>
        public Dictionary<string, List<float>> _dataAll = new Dictionary<string, List<float>>();

        /// <summary>
        /// _dataCountryMapping:
        ///     different country names (key) are mapped to one and the same country (value)
        ///         ("United state", "USA)
        ///         ("USA", "USA")
        ///         ("United states of am", "USA")
        /// </summary>
        public Dictionary<string, string> _dataCountryMapping = new Dictionary<string, string>();

        private static string _countryMappingFileName = "country_mapping.txt";

        public MainWindows()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Corona!";

            lblProgress.Text = "Wait!";

            txtInfo.Text =
                          "Average age:     " + _urlAgeAvg
                    + "\r\nLife expectancy: " + _urlAgeMax
                    + "\r\nPopulation:      " + _urlPopulation
                    + "\r\nCorona data:     " + _urlCorona
                    + "\r\n\r\nClick on a column header to sort."
                    ;
            DefaultListViewProperties(lvData);
            lvData.GridLines = true;
            lvData.Columns.Add("Country", 200);
            lvData.Columns.Add("Life expectancy", 100);
            lvData.Columns.Add("Average age", 100);
            lvData.Columns.Add("Population", 100);
            lvData.Columns.Add("Sick", 100);
            lvData.Columns.Add("% sick/pop.", 80);
            lvData.Columns.Add("Dead", 100);
            lvData.Columns.Add("% dead/pop.", 80);
            lvData.Columns.Add("% dead/sick", -2);
        }

        private void ReadCountryFile()
        {
            _dataCountryMapping.Clear();

            //
            //
            // use file from same folder
            string strFileName = _countryMappingFileName;

            if (File.Exists("..\\..\\..\\..\\" + _countryMappingFileName))
            {
                //
                // if running from Visual Studio, go up x from bin/Debug
                //
                strFileName = "..\\..\\..\\..\\" + _countryMappingFileName;
            }

            lblProgress.Text = "Reading file " + strFileName;

            StreamReader reader = new StreamReader(strFileName);
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                DoEvents();

                line = line.Trim();

                string[] arLine = line.Split('|');

                try
                {
                    if (arLine.Length == 1)
                    {
                        //
                        // "usa" -> (usa,usa)
                        //
                        _dataCountryMapping.Add(arLine[0], arLine[0]);
                    }
                    else
                    {
                        //
                        // "united states|usa" -> (united states, usa)
                        //
                        _dataCountryMapping.Add(arLine[0], arLine[1]);
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Duplicate entry: " + line);
                }
            }
            reader.Close();
            reader = null;
        }

        protected bool DownloadFile(string windowTitle, int fileSizeKb, string url, string localFilename)
        {
            bool fileDownloaded = false;

            DownloadFileView dlg = new DownloadFileView(
                windowTitle,
                fileSizeKb,
                url,
                localFilename);

            if (DialogResult.OK == dlg.ShowDialog())
            {
                fileDownloaded = true;
            }

            return fileDownloaded;
        }

        private bool DownloadAgeMax()
        {
            bool success = true;

            try
            {
                lblProgress.Text = "Reading " + _urlAgeMax;

                _dataAgeMax.Clear();

                var url = _urlAgeMax;
                HtmlWeb web = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = web.Load(url);

                HtmlNode node = doc.DocumentNode.SelectSingleNode("//table");

                if (node != null)
                {
                    List<List<string>> table = node
                                .Descendants("tr")
                                .Skip(1)
                                .Where(tr => tr.Elements("td").Count() > 1)
                                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                                .ToList();

                    foreach (List<string> entry in table)
                    {
                        DoEvents();

                        string strCountry = entry[1];
                        string strAgeMax = entry[2].Replace(".", ",");
                        float fAgeMax = 0;

                        if (float.TryParse(strAgeMax, out fAgeMax))
                        {
                            MapAndAdd(_dataAgeMax, strCountry, fAgeMax);
                            AddIfMissing(_dataCountriesForDebugging, strCountry, 0);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                success = false;

                AddErrorMessage("Error processing", _urlAgeMax);
                AddErrorMessage(exc.Message);

                Console.Out.WriteLine(exc.Message);
            }

            return success;
        }

        private bool DownloadAgeAvg()
        {
            bool success = true;
            string url = _urlAgeAvg;
            try
            {
                lblProgress.Text = "Reading " + url;

                _dataAgeAvg.Clear();

                HtmlWeb web = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = web.Load(url);

                List<List<string>> table = doc.DocumentNode.SelectSingleNode("//table")
                            .Descendants("tr")
                            .Skip(1)
                            .Where(tr => tr.Elements("td").Count() > 1)
                            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                            .ToList();

                foreach (List<string> entry in table)
                {
                    DoEvents();

                    string strCountry = entry[1];
                    string strAgeAvg = entry[2].Remove(4);
                    float fAgeAvg = 0;

                    if (float.TryParse(strAgeAvg, out fAgeAvg))
                    {
                        MapAndAdd(_dataAgeAvg, strCountry, fAgeAvg);
                        AddIfMissing(_dataCountriesForDebugging, strCountry, 0);
                    }
                }
            }
            catch (Exception exc)
            {
                success = false;

                AddErrorMessage("Error processing", _urlAgeAvg);
                AddErrorMessage(exc.Message);

                Console.Out.WriteLine(exc.Message);
            }
            return success;
        }

        private bool DownloadPopulation()
        {
            bool success = true;

            try
            {
                lblProgress.Text = "Reading " + _urlPopulation;

                _dataPopulation.Clear();

                var url = _urlPopulation;
                HtmlWeb web = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = web.Load(url);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table");

                foreach (HtmlNode node in nodes)
                {
                    if (node.Id == "gvDataCountry")
                    {
                        List<List<string>> table = node
                                    .Descendants("tr")
                                    .Skip(1)
                                    .Where(tr => tr.Elements("td").Count() > 1)
                                    .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                                    .ToList();

                        foreach (List<string> entry in table)
                        {
                            DoEvents();

                            string strCountry = entry[0];
                            string strValue = entry[1].Replace(",", "");
                            float fValue = 0;

                            if (float.TryParse(strValue, out fValue))
                            {
                                MapAndAdd(_dataPopulation, strCountry, fValue);
                                AddIfMissing(_dataCountriesForDebugging, strCountry, 0);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                success = false;

                AddErrorMessage("Error processing", _urlAgeAvg);
                AddErrorMessage(exc.Message);

                Console.Out.WriteLine(exc.Message);
            }

            return success;
        }

        private bool DownloadCorona()
        {
            bool success = true;

            try
            {
                lblProgress.Text = "Reading " + _urlCorona;

                _dataCorona.Clear();

                var url = _urlCorona;
                HtmlWeb web = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = web.Load(url);

                HtmlNode node = doc.DocumentNode.SelectSingleNode("//table");

                List<List<string>> table = doc.DocumentNode.SelectSingleNode("//table")
                            .Descendants("tr")
                            .Skip(1)
                            .Where(tr => tr.Elements("td").Count() > 1)
                            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                            .ToList();

                foreach (List<string> entry in table)
                {
                    DoEvents();

                    string strCountry = entry[1];
                    string strInfected = entry[2].Replace(",", "");
                    string strDeaths = entry[4].Replace(",", "");
                    float fInfected = 0;
                    float fDeaths = 0;

                    if (float.TryParse(strInfected, out fInfected) && (float.TryParse(strDeaths, out fDeaths)))
                    {
                        List<float> coronaData = new List<float>();
                        coronaData.Add(fInfected);
                        coronaData.Add(fDeaths);

                        MapAndAdd(_dataCorona, strCountry, coronaData);
                        AddIfMissing(_dataCountriesForDebugging, strCountry, 0);
                    }
                }
            }
            catch (Exception exc)
            {
                success = false;

                AddErrorMessage("Error processing", _urlCorona);
                AddErrorMessage(exc.Message);

                Console.Out.WriteLine(exc.Message);
            }

            return success;
        }

        private void AddErrorMessage(string text)
        {
            AddErrorMessage(text, null);
        }

        private void AddErrorMessage(string text1, string text2)
        {
            ListViewItem lvi = new ListViewItem(text1);
            if (!string.IsNullOrEmpty(text2))
            {
                lvi.SubItems.Add(text2);
            }
            lvData.Items.Add(lvi);
        }

        private void MergeData()
        {
            lblProgress.Text = "Merging data...";

            _dataAll.Clear();

            List<float> listValues = null;

            //
            // col 1 life expectancy
            // 
            foreach (KeyValuePair<string, float> data in _dataAgeMax)
            {
                DoEvents();

                listValues = new List<float>();
                listValues.Add(data.Value);

                _dataAll.Add(data.Key, listValues);
            }

            //
            // col 2 avg age
            //
            foreach (KeyValuePair<string, float> data in _dataAgeAvg)
            {
                DoEvents();

                if (_dataAll.ContainsKey(data.Key))
                {
                    listValues = _dataAll[data.Key];
                    while (listValues.Count < 1)
                    {
                        listValues.Add(0);
                    }
                    listValues.Add(data.Value);
                }
                else
                {
                    listValues = new List<float>();
                    listValues.Add(0);
                    listValues.Add(data.Value);
                    _dataAll.Add(data.Key, listValues);
                }
            }

            //
            // col3 population
            //
            foreach (KeyValuePair<string, float> data in _dataPopulation)
            {
                DoEvents();

                if (_dataAll.ContainsKey(data.Key))
                {
                    listValues = _dataAll[data.Key];
                    while (listValues.Count < 2)
                    {
                        listValues.Add(0);
                    }
                    listValues.Add(data.Value);
                }
                else
                {
                    listValues = new List<float>();
                    listValues.Add(0);
                    listValues.Add(0);
                    listValues.Add(data.Value);
                    _dataAll.Add(data.Key, listValues);
                }
            }

            //
            // col 4, col5: infected, deaths
            //
            foreach (KeyValuePair<string, List<float>> data in _dataCorona)
            {
                DoEvents();

                if (_dataAll.ContainsKey(data.Key))
                {
                    listValues = _dataAll[data.Key];
                    while (listValues.Count < 3)
                    {
                        listValues.Add(0);
                    }
                    listValues.Add(data.Value[0]);
                    listValues.Add(data.Value[1]);
                }
                else
                {
                    listValues = new List<float>();
                    listValues.Add(0);
                    listValues.Add(0);
                    listValues.Add(0);

                    listValues.Add(data.Value[0]);
                    listValues.Add(data.Value[1]);
                    _dataAll.Add(data.Key, listValues);
                }
            }

            foreach (string strKey in _dataAll.Keys)
            {
                DoEvents();

                listValues = _dataAll[strKey];
;
                while (listValues.Count < 5)
                {
                    listValues.Add(0);
                }
            }

        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            XableAllButtonsForLongOperation(false);

            int i = 0;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            lvData.Items.Clear();
            ReadCountryFile();

            // DownloadFile("test", 4, _urlPopulation, @"d:\temp\population.html");

            DownloadPopulation();
            DownloadAgeMax();
            DownloadAgeAvg();
            DownloadCorona();

            MergeData();

            List<string> listKeys = _dataAll.Keys.ToList();
            listKeys.Sort();

            foreach (string strKey in listKeys)
            {
                List<float> listValues = _dataAll[strKey];

                //
                // country
                //
                ListViewItem lvi = new ListViewItem(strKey);
                //
                // 0 max
                //
                lvi.SubItems.Add(listValues[0].ToString());
                //
                // 1 average
                //
                lvi.SubItems.Add(listValues[1].ToString());
                string strValue = String.Format("{0:n0}", listValues[2]);
                //
                // 2 population
                //
                lvi.SubItems.Add(strValue);
                //
                // 3 sick
                //
                strValue = listValues[3].ToString();
                lvi.SubItems.Add(strValue);
                strValue = GetPercent(listValues[3], listValues[2]);
                lvi.SubItems.Add(strValue);
                //
                // 5
                //
                strValue = listValues[4].ToString();
                lvi.SubItems.Add(strValue);
                strValue = GetPercent(listValues[4], listValues[2]);
                lvi.SubItems.Add(strValue);

                //
                // 6 % deaths of sick
                //
                strValue = GetPercent(listValues[4], listValues[3]);
                lvi.SubItems.Add(strValue);

                lvData.Items.Add(lvi);
            }

            List<string> listData = _dataCountriesForDebugging.Keys.ToList();
            listData.Sort();

            StreamWriter writer = new StreamWriter("countries.txt", false, Encoding.UTF8);

            foreach (string strKey in listData)
            {
                writer.WriteLine(strKey);
            }
            writer.Close();
            writer = null;
            lblProgress.Text = "Ready. Click '" + cmdUpdate.Text + "' to update the zombie apocalypse data";

            XableAllButtonsForLongOperation(true);

        }

        private string GetPercent(float fValue, float fTotal)
        {
            string strValue = "";

            if ((fValue != 0) && (fTotal != 0))
            {
                string strFormat;
                float fPercentage = fValue / fTotal;

                if (fPercentage < 0.001)
                {
                    strFormat = "P4";
                }
                else if (fPercentage < 0.01)
                {
                    strFormat = "P3";
                }
                else if (fPercentage < 0.01)
                {
                    strFormat = "P3";
                }
                else if (fPercentage < 0.1)
                {
                    strFormat = "P2";
                }
                else
                {
                    strFormat = "P1";
                }

                strValue = fPercentage.ToString(strFormat, System.Globalization.CultureInfo.InvariantCulture);
                strValue = strValue.Replace("%", "");
                strValue = strValue.Replace(".", ",");
            }

            return strValue;
        }

        private void AddIfMissing(Dictionary<string,int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        private void MapAndAdd(Dictionary<string, float> dict, string key, float value)
        {
            string country = key;

            if (country.IndexOf("196") != -1)
            {
                ; // break;
            }

            if (_dataCountryMapping.ContainsKey(key))
            {
                country = _dataCountryMapping[key];
            }

            if (dict.ContainsKey(country))
            {
                //
                // should not happen, but update the value to the latest one if the same country exists more than once
                //
                dict[country] = value;
            }
            else
            {
                //
                // add for the first time
                //
                dict.Add(country, value);
            }
        }

        private void MapAndAdd(Dictionary<string, List<float>> dict, string key, List<float> value)
        {
            string country = key;

            if (country.IndexOf("196") != -1)
            {
                ; // break;
            }

            if (_dataCountryMapping.ContainsKey(key))
            {
                country = _dataCountryMapping[key];
            }

            if (dict.ContainsKey(country))
            {
                //
                // should not happen, but update the value to the latest one if the same country exists more than once
                //
                dict[country] = value;
            }
            else
            {
                //
                // add for the first time
                //
                dict.Add(country, value);
            }
        }

        private void MainWindows_Shown(object sender, EventArgs e)
        {
            cmdUpdate_Click(null, null);
        }
    }
}
