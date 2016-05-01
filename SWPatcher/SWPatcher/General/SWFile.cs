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
        public string PathA { get; private set; }
        public string PathD { get; private set; }
        public string Format { get; private set; }

        public SWFile(string name, string path, string pathA, string pathD, string format)
        {
            this.Name = name;
            this.Path = path;
            this.PathA = pathA;
            this.PathD = pathD;
            this.Format = format;
        }
    }
}
