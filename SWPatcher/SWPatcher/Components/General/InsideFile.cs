using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPatcher.Components.General
{
    public class InsideFile : File
    {
        public string PathInData { get; private set; }

        public InsideFile(string name, string filePath, string pathInData) : base(name, filePath)
        {
            this.PathInData = pathInData;
        }
    }
}
