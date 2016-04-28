using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPatcher.General
{
    public class File
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public File(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
