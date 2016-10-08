using System;

namespace SWPatcherTest.RTPatch
{
    [Serializable]
    public class ResultException : Exception
    {
        public ulong Result { get; private set; }
        public string LogPath { get; private set; }

        public ResultException(ulong result, string logPath)
        {
            this.Result = result;
            this.LogPath = logPath;
        }
    }
}
