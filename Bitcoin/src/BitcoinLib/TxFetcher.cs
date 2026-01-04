using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;

namespace BitcoinLib
{
    /// <summary>
    /// Reads transaction from the internet, caches the data and stores that cache in a file.
    /// For some reason, WebClient.DownloadString(url) stopped working, so I created the cache file manually by copying the cache file from the python code.
    /// Also, you can paste the url into any browser and then copy the resulting serialized tx into the cache file.
    /// </summary>
    public class TxFetcher
    {
        static readonly HttpClient _http = new HttpClient();

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

        //private static readonly HttpClient _client = new HttpClient();

        static TxFetcher()
        {
            Console.WriteLine("Using " + TxFetcher.UrlTestnet);
            Console.WriteLine("      " + TxFetcher.UrlRealnet);
            Console.WriteLine("If these produce an error, download the python source code again and search for the new URLs.");
            Console.WriteLine("");
            Console.WriteLine("");

            //
            // "D:\Daten\Develop\DOTNET\Bitcoin\tests\BitcoinLib.Tests\bin\Debug\net8.0-windows\BitcoinLibTest.exe"
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

        private static bool DataIsValid(string id, string serialized, Tx tx)
        {
            bool success = true;

            string computed;
            if (tx._isSegWit)
            {
                computed = tx.Id();
            }
            else
            {
                byte[] h256 = Tools.Hash256(Tools.HexStringToBytes(serialized));
                Tools.Reverse(h256);
                computed = Tools.BytesToHexString(h256);
            }

            if (!computed.Equals(id))
            {
                Console.WriteLine("TxFetcher.DataIsValid(): ID mismatch");
                Console.WriteLine("id   = " + id);
                Console.WriteLine("computed= " + computed);

                success = false;
            }

            return success;
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
                            string tx_id = arLine[0];
                            string tx_serialized = arLine[1];
                            Tx tx = Tx.Parse(tx_serialized);

                            if (!DataIsValid(tx_id, tx_serialized, tx))
                            {
                                throw new Exception($"TxFetcher.LoadCache(): mismatch");
                            }

                            _cache.Add(tx_id, tx);
                        }
                    }
                }
                Tools.ConsoleOutWriteHeader("...done");
            }
        }


        public static string GetUrlContentNew(string url)
        {
            string data = "";

            try
            {
                data = _http.GetStringAsync(url).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR retrieving " + url);
            }

            return data;
        }
        public static string GetUrlContent(string url)
        {
            string data = "";
            int tries = 5;

            while (tries-- > 0)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {

                        Console.Out.WriteLine("Retrieving URL:" + url);
                        data = client.DownloadString(url);
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("ERROR!!!: " + e.Message);
                }
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

                Tx tx = Tx.Parse(response);
                tx._testnet = testnet;

                //
                // We get the full tx data from the internet so that we can verify all of it.
                // Dont trust anyone!
                //
                if (!DataIsValid(tx_id, response, tx))
                {
                    throw new Exception($"TxFetcher.LoadCache(): mismatch");
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
