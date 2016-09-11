using System;

namespace SWPatcher.Downloading
{
    public class RTPatchProgressChangedEventArgs : EventArgs
    {
        public int FileNumber { get; private set; }
        public int FileCount { get; private set; }
        public string FileName { get; private set; }
        public int Progress { get; private set; }

        public RTPatchProgressChangedEventArgs(int fileNumber, int fileCount, string fileName, int progress)
        {
            this.FileNumber = fileNumber;
            this.FileCount = fileCount;
            this.FileName = fileName;
            this.Progress = progress;
        }
    }
}
