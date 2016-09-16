using System;
using System.Net;

namespace SWPatcher.Helpers
{
    public class RTPatchDownloadProgressChangedEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public int Progress { get; private set; }

        public RTPatchDownloadProgressChangedEventArgs(string fileName, DownloadProgressChangedEventArgs e)
        {
            this.FileName = fileName;
            this.Progress = e.BytesReceived == e.TotalBytesToReceive ? int.MaxValue : Convert.ToInt32(((double)e.BytesReceived / e.TotalBytesToReceive) * int.MaxValue);
        }
    }
}
