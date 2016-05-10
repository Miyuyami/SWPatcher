using System;

namespace SWPatcher.Patching
{
    public class PatcherProgressChangedEventArgs : EventArgs
    {
        public int FileNumber { get; private set; }
        public int FileCount { get; private set; }
        public int Progress { get; private set; }

        public PatcherProgressChangedEventArgs(int fileNumber, int fileCount, int progress)
        {
            this.FileNumber = fileNumber;
            this.FileCount = fileCount;
            this.Progress = progress;
        }
    }
}
