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
    public static class StringDownloader
    {
        private static WebClient _webClient = new WebClient();

        public static string DownloadString(Uri uri)
        {
            string result = null;
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    result = _webClient.DownloadString(uri);
                }
                catch (WebException)
                {
                    DialogResult error = MsgBox.ErrorRetry("Could not connect to download server.\nTry again later.");
                    if (error == DialogResult.Retry)
                        result = DownloadString(uri);
                    else
                    {
                        MsgBox.Error("It was impossible to connect to the server.\nThe application will now close.");
                        return null;
                    }
                }
            }
            return result;
        }
    }
}
