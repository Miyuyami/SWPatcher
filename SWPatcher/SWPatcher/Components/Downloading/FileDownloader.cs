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
    public class FileDownloader
    {
        private readonly WebClient _webClient;
        private readonly BackgroundWorker _worker;

        public FileDownloader()
        {
            _webClient = new WebClient();
        }
    }
}
