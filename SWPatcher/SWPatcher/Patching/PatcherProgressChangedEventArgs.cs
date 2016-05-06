using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SWPatcher.Patching
{
    public class PatcherProgressChangedEventArgs : EventArgs
    {
        public int FileNumber { get; private set; }
        public int TotalFileCount { get; private set; }
        public string FileName { get; private set; }
        public int Progress { get; private set; }

        public PatcherProgressChangedEventArgs(int fileNumber, int totalFileCount, string fileName)//, ProgressChangedEventArgs e)
        {
            this.FileNumber = fileNumber;
            this.TotalFileCount = totalFileCount;
            this.FileName = fileName;
            //this.Progress = e.BytesReceived == e.TotalBytesToReceive ? int.MaxValue : Convert.ToInt32(((double) e.BytesReceived / e.TotalBytesToReceive) * int.MaxValue);
        }
    }
}
