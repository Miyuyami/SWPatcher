using System;

namespace SWPatcher.RTPatch
{
    [Serializable]
    public class ResultException : Exception
    {
        public ulong Result { get; private set; }
        public string LogPath { get; private set; }
        public string FileName { get; private set; }
        public Version ClientVersion { get; private set; }

        public ResultException(string message, ulong result, string logPath, string fileName, Version version) : base(message)
        {
            this.Result = result;
            this.LogPath = logPath;
            this.FileName = fileName;
            this.ClientVersion = version;
        }
    }
}
