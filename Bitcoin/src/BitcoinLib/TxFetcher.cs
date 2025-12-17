using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace BitcoinLib
{
    /// <summary>
    /// Reads transaction from the internet, caches the data and stores that cache in a file.
    /// </summary>
    public class TxFetcher
    {
        public static string UrlTestnet = "https://blockstream.info/testnet/api";
        public static string UrlRealnet = "https://blockstream.info/api";
  
        /// <summary>
        /// If we running from Visual Studio, then use the file which is 2 folder up above the bin\Debug so that we do not delete it by accident.
        /// When it is written, it will be written to the location it was first found. If it did not exist, it will be created in the current folder.
        /// </summary>
        private const string CacheFileName = "tx.cache.txt";
        private static string _cacheFileName = CacheFileName;

        /// <summary>
        /// We have to read the full data of the previous transactions from the internet by its ID.
        /// Once the data of a tx has been cached, it must never be refreshed because that data can never change because it is in a block in the blockchain.
        /// </summary>
        private static Dictionary<string, Tx> _cache = new Dictionary<string, Tx>();

        static TxFetcher()
        {
            Console.WriteLine("Using " + TxFetcher.UrlTestnet);
            Console.WriteLine("      " + TxFetcher.UrlRealnet);
            Console.WriteLine("If these produce an error, download the python source code again and search for the new URLs.");
            Console.WriteLine("");
            Console.WriteLine("");

            //
            // "C:\\cmaurer\\cmaurer\\privat\\develop\\DOT.NET\\Bitcoin\\bin\\Debug"
            //
            string folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (File.Exists(folder + "\\" + CacheFileName))
            {
                _cacheFileName = folder + "\\" + CacheFileName;
            }
            else
            {
                folder = folder + "\\..\\..";
                //
                // "C:\\cmaurer\\cmaurer\\privat\\develop\\DOT.NET\\Bitcoin"
                //
                if (File.Exists(folder + "\\" + CacheFileName))
                {
                    _cacheFileName = folder + "\\" + CacheFileName;
                }
            }

            LoadCache();
        }

        public static string GetUrl(bool testnet = false)
        {
            string url;

            if (testnet)
            {
                url = TxFetcher.UrlTestnet;
            }
            else
            {
                url = TxFetcher.UrlRealnet;
            }

            return url;
        }

        /// <summary>
        /// This function must be private. We save the cache to disk everytime it is changed.
        /// </summary>
        private static void SaveCache()
        {
            // save cache into a file
            using (StreamWriter writer = new StreamWriter(_cacheFileName))
            {
                foreach (KeyValuePair<string, Tx> keyValue in _cache)
                {
                    string tx_serialized = Tools.BytesToHexString(keyValue.Value.serialize());
                    string line = keyValue.Key + ":" + tx_serialized;
                    writer.WriteLine(line);
                }
            }
        }

        private static void LoadCache()
        {
            _cache.Clear();

            if (File.Exists(_cacheFileName))
            {
                string text = string.Format("Reading transactions from cache file {0}", _cacheFileName);
                Tools.ConsoleOutWriteHeader(text);

                // populate cache from a file
                using (StreamReader reader = new StreamReader(_cacheFileName))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        String[] arLine = line.Split(new string[] { ":" }, StringSplitOptions.None);
                        if (arLine.Length == 2)
                        {
                            string s1 = arLine[0];
                            string s2 = arLine[1];
                            Tx tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(s2))));
                            _cache.Add(s1, tx);
                        }
                    }
                }
            }
        }

        public static string GetUrlContent(string url)
        {
            string data = "";
            using (WebClient client = new WebClient())
            {
                data = client.DownloadString(url);
            }

            return data;
        }

        public static Tx Fetch(string tx_id, bool testnet = false, bool fresh = false)
        {
            if (fresh || (!_cache.ContainsKey(tx_id)))
            {
                string url = string.Format("{0}/tx/{1}/hex", GetUrl(testnet), tx_id);

                Console.WriteLine("Fetching: " + url);
                string response = GetUrlContent(url);

                // get rid of newlines \n at the end of the response
                response = response.Trim();
                Console.WriteLine("Received: " + response);

                byte[] raw = Tools.HexStringToBytes(response);

                Tx tx = null;

                //
                // index 4 contains the witness marker. Out Tx class can handle that , so we don't have to check for this.
                //
                //if (raw[4] == 0x00)
                //{
                    //
                    //
                    // this cuts out the 2 bytes at index 4 and 5: witness marker and flags
                  //  byte[] raw2 = ArrayHelpers.JoinArrays(raw, 0, 4, raw, 6, -1);
                  //  tx = Tx.Parse(new BinaryReader(new MemoryStream(raw2)), testnet);
                //}
                //else
                //{
                    tx = Tx.Parse(new BinaryReader(new MemoryStream(raw)), testnet);
                //}
                
                //
                // We get the full tx data from the itnernet so that we can verify all of it.
                // Dont trust anyone!
                //
                if (tx.Id() != tx_id)
                {
                    throw new Exception(String.Format("not the same id: {0} vs{1}", tx.Id(), tx_id));
                }

                if (_cache.ContainsKey(tx_id))
                {
                    _cache[tx_id] = tx;
                }
                else
                {
                    _cache.Add(tx_id, tx);
                }
                SaveCache();
            }

            Tx txx = _cache[tx_id];

            return txx;
        }
    }
}
