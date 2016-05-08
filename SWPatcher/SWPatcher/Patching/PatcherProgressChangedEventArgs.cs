using System;

namespace SWPatcher.Patching
{
    public class PatcherProgressChangedEventArgs : EventArgs
    {
        public int FileNumber { get; private set; }
        public int TotalFileCount { get; private set; }
        public string FileName { get; private set; }
        public int Progress { get; private set; }

        public PatcherProgressChangedEventArgs(int fileNumber, int totalFileCount, string fileName, int progress)
        {
            this.FileNumber = fileNumber;
            this.TotalFileCount = totalFileCount;
            this.FileName = fileName;
            this.Progress = progress;
        }

        public PatcherProgressChangedEventArgs(int fileNumber, int totalFileCount, string fileName, int currentLine, int lineCount)
        {
            this.FileNumber = fileNumber;
            this.TotalFileCount = totalFileCount;
            this.FileName = fileName;
            this.Progress = currentLine == lineCount ? int.MaxValue : Convert.ToInt32(((double)currentLine/lineCount) * int.MaxValue);
        }
    }
}
