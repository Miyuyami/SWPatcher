using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace SWPatcher
{
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

        public IniKeyValue(string Value)
            : this(Value, false)
        {
        }

        public IniKeyValue(string Value, bool IsComment)
        {
            this.m_IsComment = IsComment;
            this.m_Value = Value;
        }
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

        public IniSection()
            : this(false)
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

    public class ExIniFile : System.IDisposable
    {

        string m_CurrentPath;

        bool m_KeepComment;
        System.Collections.Generic.Dictionary<string, IniSection> m_ListOfSectionKeyValue;

        System.Collections.Generic.List<string> m_ParentlessComment;
        public enum NewFileOption : short
        {
            None,
            OverWriteOld,
            ReadOldFileIfExist,
            BlankIni
        }
        /*
        public void IniFile(string FilePath, NewFileOption vNewFileOption)
        {
            this.m_CurrentPath = FilePath;
            this.m_KeepComment = true;
            this.m_ListOfSectionKeyValue = new System.Collections.Generic.Dictionary<string, IniSection>();
            this.m_ParentlessComment = new System.Collections.Generic.List<string>();
            if ((vNewFileOption == NewFileOption.ReadOldFileIfExist))
            {
                if ((My.Computer.FileSystem.FileExists(FilePath) == true))
                {
                    ReadFormat();
                }
            }
            else if ((vNewFileOption == NewFileOption.OverWriteOld))
            {
                using (System.IO.FileStream FileStream = new System.IO.FileStream(FilePath, IO.FileMode.CreateNew))
                {
                    FileStream.Close();
                }
            }
            else if ((vNewFileOption == NewFileOption.BlankIni))
            {
            }
            else
            {
                if ((System.IO.File.Exists(FilePath) == false))
                {
                    using (System.IO.FileStream FileStream = new System.IO.FileStream(FilePath, System.IO.FileMode.CreateNew))
                    {
                        FileStream.Close();
                    }
                }
                else
                {
                    ReadFormat();
                }
            }
        }
        */
        public ExIniFile(System.IO.StringReader theStream, bool CloseStreamAfterRead)
        {
            this.m_CurrentPath = "";
            this.m_KeepComment = true;
            this.m_ListOfSectionKeyValue = new System.Collections.Generic.Dictionary<string, IniSection>();
            this.m_ParentlessComment = new System.Collections.Generic.List<string>();
            ReadFormatFromStream(theStream, CloseStreamAfterRead);
        }

        public System.Collections.Generic.List<string> GetAllSections()
        {
            System.Collections.Generic.List<string> TheNewList = new System.Collections.Generic.List<string>();
            foreach (System.Collections.Generic.KeyValuePair<string, IniSection> element in this.m_ListOfSectionKeyValue)
            {
                if (!element.Value.IsComment)
                    TheNewList.Add(element.Key);
			}
			return TheNewList;
		}
        public System.Collections.ObjectModel.ReadOnlyCollection<string> GetAllKeys(string SectionName)
        {
            if ((this.m_ListOfSectionKeyValue.ContainsKey(SectionName.ToLower()) == true)) 
            {
				System.Collections.Generic.List<string> TheNewList = new System.Collections.Generic.List<string>();
                foreach (KeyValuePair<string, IniKeyValue> SectionHeader_loopVariable in this.m_ListOfSectionKeyValue[SectionName.ToLower()].IniKeyValues)
                {
                    if (SectionHeader_loopVariable.Value.IsComment == false)
                        TheNewList.Add(SectionHeader_loopVariable.Key);

                }
				return TheNewList.AsReadOnly();
			} else
                 return null;
		}

        public string GetValue(string SectionName, string KeyName, string DefaultValue)
        {
            if ((this.m_ListOfSectionKeyValue.ContainsKey(SectionName.ToLower()) == true))
            {
                if ((this.m_ListOfSectionKeyValue[SectionName.ToLower()].IniKeyValues.ContainsKey(KeyName.ToLower()) == true))
                {
                    return this.m_ListOfSectionKeyValue[SectionName.ToLower()].IniKeyValues[KeyName.ToLower()].Value;
                }
                else
                {
                    return DefaultValue;
                }
            }
            else
            {
                return DefaultValue;
            }
        }

        public void AddSection(string SectionName)
        {
            if ((this.m_ListOfSectionKeyValue.ContainsKey(SectionName.ToLower()) == false))
            {
                this.m_ListOfSectionKeyValue.Add(SectionName.ToLower(), new IniSection());
            }
            else
            {
                this.m_ListOfSectionKeyValue[SectionName.ToLower()].IsComment = false;
            }
        }

        public void SetValue(string SectionName, string KeyName, string Value)
        {
            this.AddSection(SectionName);
            if ((this.m_ListOfSectionKeyValue[SectionName.ToLower()].IniKeyValues.ContainsKey(KeyName.ToLower()) == false))
            {
                this.m_ListOfSectionKeyValue[SectionName.ToLower()].IniKeyValues.Add(KeyName.ToLower(), new IniKeyValue(Value));
            }
            else
            {
                this.m_ListOfSectionKeyValue[SectionName.ToLower()].IniKeyValues[KeyName.ToLower()].IsComment = false;
                this.m_ListOfSectionKeyValue[SectionName.ToLower()].IniKeyValues[KeyName.ToLower()].Value = Value;
            }
        }

        public void Save()
        {
            SaveFormat(this.m_CurrentPath);
        }

        public void SaveAs(string FilePath)
        {
            SaveFormat(FilePath);
        }

        #region "Property"
        public bool KeepComment
        {
            get { return this.m_KeepComment; }
            set { this.m_KeepComment = value; }
        }
        #endregion

        #region "Buffer String"
        private void SaveFormat(string FileLocation)
	{
		System.Text.StringBuilder TheStringBuilder = new System.Text.StringBuilder();
		string Header = ";Ini created by Leayal Ini Reader.";
		if ((this.m_ListOfSectionKeyValue.Count == 0)) {
			Header += " This is a blank Ini.";
		} else if ((this.m_ListOfSectionKeyValue.Count > 1)) {
			Header += " Contain: " + this.GetAllSections().Count.ToString() + " sections.";
		} else {
			Header += " Contain: " + this.GetAllSections().Count.ToString() + " section.";
		}
		int HowManyKeyValue = 0;
		foreach (KeyValuePair<string, IniSection> Section_loopVariable in this.m_ListOfSectionKeyValue) {
			if ((Section_loopVariable.Value.IsComment == false)) {
				TheStringBuilder.AppendLine("[" + Section_loopVariable.Key + "]");
			} else {
				TheStringBuilder.AppendLine(";[" + Section_loopVariable.Key + "]");
			}
			foreach (KeyValuePair<string, IniKeyValue> KeyValue_loopVariable in Section_loopVariable.Value.IniKeyValues) {
				if ((KeyValue_loopVariable.Value.IsComment == false)) {
					TheStringBuilder.AppendLine(KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value);
					HowManyKeyValue += 1;
				} else {
					TheStringBuilder.AppendLine(";" + KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value);
				}
			}
		}
		if ((HowManyKeyValue > 0)) {
			if ((HowManyKeyValue > 1)) {
				Header += " Contain: " + HowManyKeyValue.ToString() + " keys/values.";
			} else {
				Header += " Contain: " + HowManyKeyValue.ToString() + " key/value.";
			}
		}
		if ((this.m_ParentlessComment.Count > 0)) {
			string TheString = null;
			foreach (string Comment_loopVariable in this.m_ParentlessComment)
                TheString += Comment_loopVariable + "\n";
			TheStringBuilder.Insert(0, Header + "\n" + "\n" + TheString + "\n");
		} else {
			TheStringBuilder.Insert(0, Header + "\n");
		}
		using (System.IO.StreamWriter TheFileWrite = new System.IO.StreamWriter(FileLocation, false, System.Text.Encoding.UTF8)) {
			TheFileWrite.Write(TheStringBuilder.ToString());
			TheFileWrite.Flush();
		}
		TheStringBuilder.Clear();
		TheStringBuilder = null;
		Header = null;
	}

        private void TextWriteTest(string TheString)
        {
            //Using Texter As New System.IO.StreamWriter(My.Application.Info.DirectoryPath() & "\Hey.txt", True)
            //    Texter.WriteLine(TheString)
            //    Texter.Flush()
            //End Using
        }

        private void ReadFormat()
        {
            System.IO.FileInfo TheInfo = new System.IO.FileInfo(this.m_CurrentPath);
            if ((TheInfo.Length > 0))
            {
                string tmpBufferedString = null;
                string BufferedSection = null;
                string[] BufferedKeyValue = null;
                using (System.IO.FileStream TheFileStream = TheInfo.OpenRead())
                {
                    using (System.IO.StreamReader TheStreamReader = new System.IO.StreamReader(TheFileStream, System.Text.Encoding.UTF8))
                    {
                        while ((TheStreamReader.Peek() > 0))
                        {
                            tmpBufferedString = TheStreamReader.ReadLine();
                            if ((string.IsNullOrWhiteSpace(tmpBufferedString) == false))
                            {
                                if ((tmpBufferedString.Substring(0, 1) != ";"))
                                {
                                    if ((tmpBufferedString.Substring(0, 1) == "[") && (tmpBufferedString.IndexOf("]", 1) > -1))
                                    {
                                        BufferedSection = tmpBufferedString.Substring(tmpBufferedString.IndexOf("[") + 1, tmpBufferedString.IndexOf("]") - tmpBufferedString.IndexOf("[") - 1);
                                        TextWriteTest("<Ini Section Read>" + BufferedSection);
                                        if ((this.m_ListOfSectionKeyValue.ContainsKey(BufferedSection.ToLower()) == false))
                                        {
                                            this.m_ListOfSectionKeyValue.Add(BufferedSection.ToLower(), new IniSection());
                                        }
                                    }
                                    else if ((tmpBufferedString.IndexOf("=") > -1) && (tmpBufferedString.Substring(0, tmpBufferedString.IndexOf("=") + 1).Length > 1))
                                    {
                                        BufferedKeyValue = tmpBufferedString.Split(new char[] { '=' }, 2, StringSplitOptions.None);
                                        TextWriteTest("<Ini Value Read>" + BufferedKeyValue[0]);
                                        if ((this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].IniKeyValues.ContainsKey(BufferedKeyValue[0].ToLower()) == false))
                                        {
                                            this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].IniKeyValues.Add(BufferedKeyValue[0].ToLower(), new IniKeyValue(BufferedKeyValue[1]));
                                        }
                                        else
                                        {
                                            this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].IniKeyValues[BufferedKeyValue[0].ToLower()].Value = BufferedKeyValue[1];
                                        }
                                    }
                                }
                                else
                                {
                                    if ((tmpBufferedString.IndexOf("Ini created by Leayal Ini Reader.") > -1))
                                    {
                                        TextWriteTest("<File Header>");
                                    }
                                    else if ((tmpBufferedString.Substring(1, 1) == "[") && (tmpBufferedString.IndexOf("]", 1) > -1))
                                    {
                                        BufferedSection = tmpBufferedString.Substring(tmpBufferedString.IndexOf("[") + 1, tmpBufferedString.IndexOf("]") - tmpBufferedString.IndexOf("[") - 1);
                                        TextWriteTest("<Ini Commented Section Read>" + BufferedSection);
                                        if ((this.m_ListOfSectionKeyValue.ContainsKey(BufferedSection.ToLower()) == false))
                                        {
                                            this.m_ListOfSectionKeyValue.Add(BufferedSection.ToLower(), new IniSection(true));
                                        }
                                    }
                                    else if ((tmpBufferedString.IndexOf("=") > -1) && (tmpBufferedString.Substring(0, tmpBufferedString.IndexOf("=") + 1).Length > 1))
                                    {
                                        BufferedKeyValue = tmpBufferedString.Substring(1).Split(new char[] { '=' }, 2, StringSplitOptions.None);
                                        TextWriteTest("<Ini Commented Value Read>" + BufferedKeyValue[0]);
                                        if ((this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].IniKeyValues.ContainsKey(BufferedKeyValue[0].ToLower()) == false))
                                        {
                                            this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].IniKeyValues.Add(BufferedKeyValue[0].ToLower(), new IniKeyValue(BufferedKeyValue[1], true));
                                        }
                                    }
                                    else
                                    {
                                        if ((string.IsNullOrEmpty(BufferedSection) == true))
                                        {
                                            this.m_ParentlessComment.Add(tmpBufferedString);
                                            TextWriteTest("<Ini Parentless Comment Read>" + tmpBufferedString);
                                        }
                                        else
                                        {
                                            if ((this.m_ListOfSectionKeyValue.ContainsKey(BufferedSection.ToLower()) == true))
                                            {
                                                this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].CommentList.Add(tmpBufferedString);
                                                TextWriteTest("<Ini Comment Read>" + tmpBufferedString);
                                            }
                                            else
                                            {
                                                this.m_ParentlessComment.Add(tmpBufferedString);
                                                TextWriteTest("<Ini Parentless Comment Read>" + tmpBufferedString);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                BufferedSection = null;
                BufferedKeyValue = null;
                tmpBufferedString = null;
            }
        }

        private void ReadFormatFromStream(System.IO.StringReader theStream, bool CloseStreamAfterRead)
        {
            string tmpBufferedString = null;
            string BufferedSection = null;
            string[] BufferedKeyValue = null;
            while ((theStream.Peek() > 0))
            {
                tmpBufferedString = theStream.ReadLine();
                if ((string.IsNullOrWhiteSpace(tmpBufferedString) == false))
                {
                    if ((tmpBufferedString.Substring(0, 1) != ";"))
                    {
                        if ((tmpBufferedString.Substring(0, 1) == "[") && (tmpBufferedString.IndexOf("]", 1) > -1))
                        {
                            BufferedSection = tmpBufferedString.Substring(tmpBufferedString.IndexOf("[") + 1, tmpBufferedString.IndexOf("]") - tmpBufferedString.IndexOf("[") - 1);
                            TextWriteTest("<Ini Section Read>" + BufferedSection);
                            if (this.m_ListOfSectionKeyValue.ContainsKey(BufferedSection.ToLower()) == false)
                                this.m_ListOfSectionKeyValue.Add(BufferedSection.ToLower(), new IniSection());
                        }
                        else if ((tmpBufferedString.IndexOf("=") > -1) && (tmpBufferedString.Substring(0, tmpBufferedString.IndexOf("=") + 1).Length > 1))
                        {
                            BufferedKeyValue = tmpBufferedString.Split(new char[] { '=' }, 2, StringSplitOptions.None);
                            TextWriteTest("<Ini Value Read>" + BufferedKeyValue[0]);
                            if (this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].IniKeyValues.ContainsKey(BufferedKeyValue[0].ToLower()) == false)
                            {
                                this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].IniKeyValues.Add(BufferedKeyValue[0].ToLower(), new IniKeyValue(BufferedKeyValue[1]));
                            }
                            else
                            {
                                this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].IniKeyValues[BufferedKeyValue[0].ToLower()].Value = BufferedKeyValue[1];
                            }
                        }
                    }
                    else
                    {
                        if ((tmpBufferedString.IndexOf("Ini created by Leayal Ini Reader.") > -1))
                        {
                            TextWriteTest("<File Header>");
                        }
                        else if ((tmpBufferedString.Substring(1, 1) == "[") && (tmpBufferedString.IndexOf("]", 1) > -1))
                        {
                            BufferedSection = tmpBufferedString.Substring(tmpBufferedString.IndexOf("[") + 1, tmpBufferedString.IndexOf("]") - tmpBufferedString.IndexOf("[") - 1);
                            TextWriteTest("<Ini Commented Section Read>" + BufferedSection);
                            if ((this.m_ListOfSectionKeyValue.ContainsKey(BufferedSection.ToLower()) == false))
                            {
                                this.m_ListOfSectionKeyValue.Add(BufferedSection.ToLower(), new IniSection(true));
                            }
                        }
                        else if ((tmpBufferedString.IndexOf("=") > -1) && (tmpBufferedString.Substring(0, tmpBufferedString.IndexOf("=") + 1).Length > 1))
                        {
                            BufferedKeyValue = tmpBufferedString.Substring(1).Split(new char[] { '=' }, 2, StringSplitOptions.None);
                            TextWriteTest("<Ini Commented Value Read>" + BufferedKeyValue[0]);
                            if ((this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].IniKeyValues.ContainsKey(BufferedKeyValue[0].ToLower()) == false))
                            {
                                this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].IniKeyValues.Add(BufferedKeyValue[0].ToLower(), new IniKeyValue(BufferedKeyValue[1], true));
                            }
                        }
                        else
                        {
                            if ((string.IsNullOrEmpty(BufferedSection) == true))
                            {
                                this.m_ParentlessComment.Add(tmpBufferedString);
                                TextWriteTest("<Ini Parentless Comment Read>" + tmpBufferedString);
                            }
                            else
                            {
                                if ((this.m_ListOfSectionKeyValue.ContainsKey(BufferedSection.ToLower()) == true))
                                {
                                    this.m_ListOfSectionKeyValue[BufferedSection.ToLower()].CommentList.Add(tmpBufferedString);
                                    TextWriteTest("<Ini Comment Read>" + tmpBufferedString);
                                }
                                else
                                {
                                    this.m_ParentlessComment.Add(tmpBufferedString);
                                    TextWriteTest("<Ini Parentless Comment Read>" + tmpBufferedString);
                                }
                            }
                        }
                    }
                }
            }
            if ((CloseStreamAfterRead == true))
                theStream.Close();
            BufferedSection = null;
            BufferedKeyValue = null;
            tmpBufferedString = null;
        }
        #endregion

        #region "IDisposable Support"
        // To detect redundant calls
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
	{
		if (!this.disposedValue) {
			if (disposing) {
				foreach (KeyValuePair<string,IniSection> Section_loopVariable in this.m_ListOfSectionKeyValue)
                    Section_loopVariable.Value.Clear();
				this.m_ListOfSectionKeyValue.Clear();
				// TODO: dispose managed state (managed objects).
			}
			this.m_ListOfSectionKeyValue = null;
			this.m_CurrentPath = null;
			// TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
			// TODO: set large fields to null.
		}
		this.disposedValue = true;
	}

        // TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        //Protected Overrides Sub Finalize()
        //    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        //    Dispose(False)
        //    MyBase.Finalize()
        //End Sub

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
