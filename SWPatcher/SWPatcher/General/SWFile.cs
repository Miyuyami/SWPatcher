using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SWPatcher.General
{
    public class SWFile
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public SWFile(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
