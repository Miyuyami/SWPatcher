using System;

namespace SWPatcher.Patching
{
    public class PatcherProgressChangedEventArgs : EventArgs
    {
        public int Progress { get; private set; }

        public PatcherProgressChangedEventArgs(int progress)
        {
            this.Progress = progress;
        }
    }
}
