using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPatcher.General
{
    public class File
    {
        public string Name { get; private set; }
        public string FilePath { get; private set; }

        public File(string name, string filePath)
        {
            this.Name = name;
            this.FilePath = filePath;
        }
    }
}
