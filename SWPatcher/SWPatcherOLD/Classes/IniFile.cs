using System.Linq;

namespace SWPatcher.Ini
{
    public class IniFile
    {
        string sFilename;
        System.Text.StringBuilder tmpStringBuild;
        System.Collections.Generic.Dictionary<string, IniSection> o_Sections = new System.Collections.Generic.Dictionary<string, IniSection>();

        #region "Constructors"
        public IniFile(string filePath)
        {
            this.sFilename = filePath;
            this.tmpStringBuild = new System.Text.StringBuilder();
            if (System.IO.File.Exists(filePath))
                using (System.IO.StreamReader theReader = new System.IO.StreamReader(filePath))
                    ReadIniFromTextStream(theReader);
        }
        

        public IniFile(System.IO.TextReader Stream, bool CloseAfterRead = true)
        {
            this.sFilename = string.Empty;
            this.tmpStringBuild = new System.Text.StringBuilder();
            this.ReadIniFromTextStream(Stream);
            if (CloseAfterRead)
                Stream.Close();
        }
        #endregion

        #region "Methods"
        public string GetValue(string section, string key, string defaultValue)
        {
            foreach (var theNode in this.o_Sections)
                if (theNode.Key.ToLower() == section.ToLower())
                    foreach (var theInsideNode in this.o_Sections[theNode.Key].IniKeyValues)
                        if (theInsideNode.Key.ToLower() == key.ToLower())
                            return theInsideNode.Value.Value;
            return defaultValue;
        }

        public void SetValue(string section, string key, string value)
        {
            if (checkSection(section) == false)
                this.o_Sections.Add(section, new IniSection());
            foreach (var theNode in this.o_Sections[section].IniKeyValues)
                if (theNode.Key.ToLower() == key.ToLower())
                {
                    theNode.Value.Value = value;
                    return;
                }
            this.o_Sections[section].IniKeyValues.Add(key, new IniKeyValue(value));
        }

        public void Save()
        {
            this.Save(System.Text.Encoding.UTF8);
        }

        public void Save(System.Text.Encoding encode)
        {
            if (string.IsNullOrWhiteSpace(this.sFilename))
                return;
            using (System.IO.StreamWriter theWriter = new System.IO.StreamWriter(this.sFilename, false, encode))
                WriteToStream(theWriter);
        }

        public void SaveAs(string newPath)
        {
            this.SaveAs(newPath, System.Text.Encoding.UTF8);
        }

        public void SaveAs(string newPath, System.Text.Encoding encode)
        {
            if (string.IsNullOrWhiteSpace(newPath))
                return;
            this.sFilename = newPath;
            using (System.IO.StreamWriter theWriter = new System.IO.StreamWriter(newPath, false, encode))
                WriteToStream(theWriter);
        }

        public void Close()
        {
            this.tmpStringBuild.Clear();
            this.sFilename = null;
            this.o_Sections.Clear();
            this.tmpStringBuild = null;
            this.o_Sections = null;
        }

        public override string ToString()
        {
            this.tmpStringBuild.Clear();
            foreach (var Section_loopVariable in this.o_Sections)
            {
                if (Section_loopVariable.Value.IsComment == false)
                {
                    this.tmpStringBuild.AppendLine("[" + Section_loopVariable.Key + "]");
                }
                else
                {
                    this.tmpStringBuild.AppendLine(";[" + Section_loopVariable.Key + "]");
                }
                foreach (var KeyValue_loopVariable in Section_loopVariable.Value.IniKeyValues)
                {
                    if (Section_loopVariable.Value.IsComment == false)
                    {
                        this.tmpStringBuild.AppendLine(KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value);
                    }
                    else
                    {
                        this.tmpStringBuild.AppendLine(";" + KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value);
                    }
                }
            }
            return this.tmpStringBuild.ToString();
        }
        #endregion

        #region "Properties"
        public System.Collections.ObjectModel.ReadOnlyCollection<string> Sections
        {
            get
            {
                return new System.Collections.ObjectModel.ReadOnlyCollection<string>(this.o_Sections.Keys.ToList());
            }
        }
        #endregion

        #region "Private Methods"
        private bool checkSection(string theKey)
        {
            foreach (var theNode in this.o_Sections)
                if (theNode.Key.ToLower() == theKey.ToLower())
                    return true;
            return false;
        }

        private void ReadIniFromTextStream(System.IO.TextReader Stream)
        {
            string lineBuffer = string.Empty;
            string[] splitBuffer = null;
            IniSection sectionBuffer = null;
            int pumBuffer;
            while ((pumBuffer = Stream.Read()) != -1)
            {
                //pumBuffer = Stream.Read();
                if (!string.IsNullOrWhiteSpace(lineBuffer) && ((char)pumBuffer == '\n'))
                {
                    //System.Windows.Forms.MessageBox.Show(lineBuffer);
                    if (lineBuffer.StartsWith("[") && lineBuffer.EndsWith("]"))
                    {
                        sectionBuffer = new IniSection(false);
                        this.o_Sections.Add(lineBuffer.Substring(1, lineBuffer.Length - 2), sectionBuffer);
                        lineBuffer = string.Empty;
                    }
                    else if (lineBuffer.IndexOf("=") > -1)
                    {
                        splitBuffer = lineBuffer.Split('=');
                        sectionBuffer.IniKeyValues.Add(splitBuffer[0].Trim(), new IniKeyValue(splitBuffer[1].Trim()));
                        lineBuffer = string.Empty;
                    }
                }
                else if ((char)pumBuffer == '\r')
                { }
                else if (pumBuffer == 0)
                { }
                else
                    lineBuffer += (char)pumBuffer;
            }
            if (!string.IsNullOrWhiteSpace(lineBuffer)) //This will make sure last line without \n will not be discarded
            {
                if (lineBuffer.StartsWith("[") && lineBuffer.EndsWith("]"))
                {
                    sectionBuffer = new IniSection(false);
                    this.o_Sections.Add(lineBuffer.Substring(1, lineBuffer.Length - 2), sectionBuffer);
                    lineBuffer = string.Empty;
                }
                else if (lineBuffer.IndexOf("=") > -1)
                {
                    splitBuffer = lineBuffer.Split('=');
                    sectionBuffer.IniKeyValues.Add(splitBuffer[0].Trim(), new IniKeyValue(splitBuffer[1].Trim()));
                    lineBuffer = string.Empty;
                }
            }
            sectionBuffer = null;
            lineBuffer = null;
            splitBuffer = null;
        }

        private void WriteToStream(System.IO.TextWriter theStream)
        {
            this.tmpStringBuild.Clear();
            foreach (var Section_loopVariable in this.o_Sections)
            {
                if (Section_loopVariable.Value.IsComment == false)
                {
                    this.tmpStringBuild.AppendLine("[" + Section_loopVariable.Key + "]");
                }
                else
                {
                    this.tmpStringBuild.AppendLine(";[" + Section_loopVariable.Key + "]");
                }
                foreach (var KeyValue_loopVariable in Section_loopVariable.Value.IniKeyValues)
                {
                    if (Section_loopVariable.Value.IsComment == false)
                    {
                        this.tmpStringBuild.AppendLine(KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value);
                    }
                    else
                    {
                        this.tmpStringBuild.AppendLine(";" + KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value);
                    }
                }
            }
            theStream.Write(this.tmpStringBuild.ToString());
            theStream.Flush();
        }
        #endregion
    }

    public class IniSection
    {
        bool m_IsComment;
        public bool IsComment
        {
            get { return this.m_IsComment; }
            set { this.m_IsComment = value; }
        }

        System.Collections.Generic.List<string> m_CommentList;
        public System.Collections.Generic.List<string> CommentList
        {
            get { return this.m_CommentList; }
        }

        System.Collections.Generic.Dictionary<string, IniKeyValue> m_ListOfIniKeyValue;
        public System.Collections.Generic.Dictionary<string, IniKeyValue> IniKeyValues
        {
            get { return this.m_ListOfIniKeyValue; }
        }

        public IniSection() : this(false)
        {
        }

        public IniSection(bool IsComment)
        {
            this.m_IsComment = IsComment;
            this.m_ListOfIniKeyValue = new System.Collections.Generic.Dictionary<string, IniKeyValue>();
            this.m_CommentList = new System.Collections.Generic.List<string>();
        }

        public void Clear()
        {
            this.m_ListOfIniKeyValue.Clear();
            this.m_CommentList.Clear();
        }
    }

    public class IniKeyValue
    {
        bool m_IsComment;
        public bool IsComment
        {
            get { return this.m_IsComment; }
            set { this.m_IsComment = value; }
        }

        string m_Value;
        public string Value
        {
            get { return this.m_Value; }
            set { this.m_Value = value; }
        }

        public IniKeyValue(string Value) : this(Value, false)
        {
        }

        public IniKeyValue(string Value, bool IsComment)
        {
            this.m_IsComment = IsComment;
            this.m_Value = Value;
        }
    }
}