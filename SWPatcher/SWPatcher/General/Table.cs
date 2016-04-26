using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPatcher.General
{
    public class Table : InsideFile
    {
        public string Format { get; private set; }

        public Table(string name, string filePath, string pathInData, string format) : base(name, filePath, pathInData)
        {
            this.Format = format;
        }
    }
}
