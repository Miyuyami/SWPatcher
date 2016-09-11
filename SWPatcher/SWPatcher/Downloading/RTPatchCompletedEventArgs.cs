using System;

namespace SWPatcher.Helpers
{
    public class RTPatchCompletedEventArgs : EventArgs
    {
        public Version Version { get; private set; }
        public string Caller { get; private set; }
        public uint Result { get; private set; }
        public bool Cancelled { get; private set; }
        public Exception Error { get; private set; }

        public RTPatchCompletedEventArgs(Version version, string caller)
        {
            this.Version = version;
            this.Caller = caller;
            this.Result = 0;
            this.Cancelled = false;
            this.Error = null;
        }

        public RTPatchCompletedEventArgs(Version version, string caller, bool cancelled, Exception error)
        {
            this.Version = version;
            this.Caller = caller;
            this.Result = 0;
            this.Cancelled = cancelled;
            this.Error = error;
        }

        public RTPatchCompletedEventArgs(Version version, string caller, uint result, bool cancelled, Exception error)
        {
            this.Version = version;
            this.Caller = caller;
            this.Result = result;
            this.Cancelled = cancelled;
            this.Error = error;
        }
    }
}
