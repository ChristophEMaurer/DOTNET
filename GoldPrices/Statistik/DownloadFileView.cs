using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime;
using CMaurer.Common;


namespace Statistik
{

    public partial class DownloadFileView : BaseForm
    {
        private int _fileSizeBytes;
        private string _url;
        private string _localFileName;


        public DownloadFileView(string windowTitle, int fileSizeKB, string url, string localFileName)
            : base()
        {
            _fileSizeBytes = fileSizeKB << 10;
            _url = url;
            _localFileName = localFileName;

            InitializeComponent();

            // must be visible for design mode or else we see nothing.
            lblStatus.BorderStyle = BorderStyle.None;

        }

        private void Progress(int percent, string text)
        {
            if (this.progressBar.InvokeRequired)
            {
                ProgressCallbackPercentTextDelegate d = new ProgressCallbackPercentTextDelegate(Progress);
                this.Invoke(d, percent, text);
            }
            else
            {
                if (percent > 100)
                {
                    percent = 100;
                }
                if (percent < 0)
                {
                    percent = 0;
                }
                progressBar.Value = percent;
            }
            if (text.Length > 0)
            {
                lblStatus.Text = text;
            }
            Application.DoEvents();
        }

        public void SetWebProxy(System.Net.HttpWebRequest webRequest)
        {
            switch ("ie")
            {
                case "ie":
                default:
                    IWebProxy iwebProxy = (IWebProxy)WebRequest.DefaultWebProxy;
                    iwebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                    webRequest.Proxy = iwebProxy;
                    break;

                    /*
                case "user":
                    string address = GetUserSettingsString(GlobalConstants.SectionProxy, GlobalConstants.KeyProxyUserAddress);
                    string user = ConfigProxyUser;
                    string password = ConfigProxyPassword;
                    string domain = ConfigProxyDomain;

                    WebProxy webProxy = new WebProxy(address);
                    webProxy.Credentials = new System.Net.NetworkCredential(user, password, domain);
                    webRequest.Proxy = webProxy;
                    break;
*/

                case "none":
                    webRequest.Proxy = null;
                    break;
            }
        }

        private bool DownloadFile()
        {
            bool success = false;
            string tempFilename = null;
            System.IO.FileStream stream = null;

            try
            {
                tempFilename = System.IO.Path.GetTempFileName();

                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(_url);

                SetWebProxy(webRequest);

                System.Net.HttpWebResponse webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse();

                System.IO.BinaryReader streamReader = new System.IO.BinaryReader(webResponse.GetResponseStream());
                const int BUFFER_SIZE = 4096;

                byte[] buf = new byte[BUFFER_SIZE];
                stream = new System.IO.FileStream(tempFilename, System.IO.FileMode.Create);
                int bytesRead = 0;
                int n = streamReader.Read(buf, 0, BUFFER_SIZE);
                while (n > 0)
                {
                    bytesRead += n;

                    string message = string.Format("downloadStatus{0}{1}", 
                        bytesRead >> 10, 
                        _fileSizeBytes >> 10);

                    Progress((int)((double)bytesRead / (double)_fileSizeBytes * 100.0), message);

                    stream.Write(buf, 0, n);
                    n = streamReader.Read(buf, 0, BUFFER_SIZE);
                    Application.DoEvents();

                    if (Abort)
                    {
                        throw new Exception("abortedByUser");
                    }
                }

                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }

                Tools.CopyFile(tempFilename, _localFileName, false);
                Tools.DeleteFile(tempFilename);

                success = true;

            }
            catch (Exception e)
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
                if (!string.IsNullOrEmpty(tempFilename))
                {
                    CMaurer.Common.Tools.DeleteFile(tempFilename);
                }
                if (!string.IsNullOrEmpty(_localFileName))
                {
                    Tools.DeleteFile(_localFileName);
                }

                MessageBox(string.Format("errorDownloading{0}{1}", 
                    _url, e.Message));
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
            }

            return success;
        }

        private void DownloadFileView_Shown(object sender, EventArgs e)
        {
            if (DownloadFile())
            {
                Application.DoEvents();

                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Abort = true;
        }

        private void DownloadFileView_Load(object sender, EventArgs e)
        {
        }
    }
}

