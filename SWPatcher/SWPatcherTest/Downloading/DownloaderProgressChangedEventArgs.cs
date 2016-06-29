using System;

namespace SWPatcherTEST.Downloading
{
    public class DownloaderProgressChangedEventArgs : EventArgs
    {
        public int FileNumber { get; private set; }
        public int FileCount { get; private set; }
        public string FileName { get; private set; }
        public int Progress { get; private set; }

        public DownloaderProgressChangedEventArgs(int fileNumber, int fileCount, string fileName, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.FileNumber = fileNumber;
            this.FileCount = fileCount;
            this.FileName = fileName;
            this.Progress = e.BytesReceived == e.TotalBytesToReceive ? int.MaxValue : Convert.ToInt32(((double)e.BytesReceived / e.TotalBytesToReceive) * int.MaxValue);
        }
    }
}
