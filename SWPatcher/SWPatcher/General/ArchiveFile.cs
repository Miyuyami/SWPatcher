using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPatcher.General
{
    public class ArchiveFile : File
    {
        public string PathInArchive { get; private set; }

        public ArchiveFile(string name, string filePath, string pathInData) : base(name, filePath)
        {
            this.PathInArchive = pathInData;
        }
    }
}
