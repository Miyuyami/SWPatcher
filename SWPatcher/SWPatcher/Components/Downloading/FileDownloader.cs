using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;
using SWPatcher.Helpers;

namespace SWPatcher.Components.Downloading
{
    public class FileDownloader : Component
    {
        private readonly WebClient _webClient;
        private readonly BackgroundWorker _worker;

        public FileDownloader()
        {
            _webClient = new WebClient();
        }

        public string AsString(string absoluteUri)
        {
            String stringData = null;
            try
            {

                stringData = _webClient.DownloadString(absoluteUri);
            }
            catch (WebException)
            {
                DialogResult error = MsgBox.ErrorRetry("Could not connect to download server.\nTry again later.");
                if (error == DialogResult.Retry)
                    stringData = AsString(absoluteUri);
                else
                {

                }
            }
            return stringData;
        }
    }
}
