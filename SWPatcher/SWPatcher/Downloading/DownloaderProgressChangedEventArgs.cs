using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;

namespace SWPatcher.Downloading
{
    public class DownloaderProgressChangedEventArgs : EventArgs
    {
        public int FileNumber { get; private set; }
        public int TotalFileCount { get; private set; }
        public string FileName { get; private set; }
        public long BytesReceived { get; private set; }
        public long TotalBytesToReceive { get; private set; }
        public int ProgressPercentage { get; private set; }

        public DownloaderProgressChangedEventArgs(int fileNumber, int totalFileCount, string fileName, DownloadProgressChangedEventArgs e)
        {
            this.FileNumber = fileNumber;
            this.TotalFileCount = totalFileCount;
            this.FileName = fileName;
            this.BytesReceived = e.BytesReceived;
            this.TotalBytesToReceive = e.TotalBytesToReceive;
            this.ProgressPercentage = e.ProgressPercentage;
        }
    }
}
