using System;
using System.Net;

namespace SWPatcherTest.RTPatch
{
    public class RTPatcherDownloadProgressChangedEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public int Progress { get; private set; }

        public RTPatcherDownloadProgressChangedEventArgs(string fileName, DownloadProgressChangedEventArgs e)
        {
            this.FileName = fileName;
            this.Progress = e.BytesReceived == e.TotalBytesToReceive ? int.MaxValue : Convert.ToInt32(((double)e.BytesReceived / e.TotalBytesToReceive) * int.MaxValue);
        }
    }
}
