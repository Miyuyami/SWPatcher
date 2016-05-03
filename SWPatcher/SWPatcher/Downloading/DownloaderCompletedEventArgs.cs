using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPatcher.Downloading
{
    public class DownloaderDownloadCompletedEventArgs : EventArgs
    {
        public string Date { get; private set; }

        public DownloaderDownloadCompletedEventArgs(string date)
        {
            this.Date = date;
        }
    }
}
