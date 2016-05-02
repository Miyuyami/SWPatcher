using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SWPatcher.Classes.Patch
{
    class PatchBuilder
    {
        System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, BuildData>> _buildDatalist;
        bool _building;
        string _directory;
        string _gamedirectory;
        XORprocessor xorer;
        string _SourceFolder;

        public PatchBuilder(string SDirectory, string sSWFolder, System.Collections.Generic.List<PatchData> lPatchDataList)
        {
            this._buildDatalist = new Dictionary<string, Dictionary<string, BuildData>>();
            //Process to be easier to read
            string tmpTargetName = null;
            string tmpFilename = null;
            foreach (var node in lPatchDataList)
            {
                tmpTargetName = System.IO.Path.GetFileNameWithoutExtension(node.TargetData.ToLower());
                if (!this._buildDatalist.ContainsKey(tmpTargetName))
                    this._buildDatalist.Add(tmpTargetName, new Dictionary<string, BuildData>());
                tmpFilename = System.IO.Path.GetFileNameWithoutExtension(node.FileName.ToLower());
                if (!this._buildDatalist[tmpTargetName].ContainsKey(tmpFilename))
                    this._buildDatalist[tmpTargetName].Add(tmpFilename, new BuildData(node.InsidePath, node.Param));
            }
            tmpFilename = null;
            tmpTargetName = null;
            this._building = false;
            this._directory = SDirectory;
            this._gamedirectory = sSWFolder;
            this.xorer = new XORprocessor("55");
            this._SourceFolder = fixPath(AppDomain.CurrentDomain.BaseDirectory);
        }

        public bool Build(BackgroundWorker bWorkerCheckCancel)
        {
            if (!this._building)
            {
                this._building = true;
                if (System.IO.Directory.Exists(this._SourceFolder + "\\build"))
                    EmptyFolder(new System.IO.DirectoryInfo(this._SourceFolder + "\\build"));
                else
                    System.IO.Directory.CreateDirectory(this._SourceFolder + "\\build");
                System.IO.Directory.CreateDirectory(this._SourceFolder + "\\cache");
                Ionic.Zip.ZipFile theZip = null;
                //System.IO.File.Copy(this._gamedirectory, AppDomain.CurrentDomain.BaseDirectory + "\\build\\", true);
                //this.xorer.processFile(this._gamedirectory, AppDomain.CurrentDomain.BaseDirectory + "\\build\\build.dat");
                List<System.Diagnostics.Process> listofProcess;
                System.Diagnostics.Process theProcess = null;
                if (System.IO.File.Exists(this._SourceFolder + "\\SWRFU.exe"))
                {
                    foreach (string theDataFile in this._buildDatalist.Keys)
                    {
                        bWorkerCheckCancel.ReportProgress(2, "Building " + theDataFile);
                        System.IO.Directory.CreateDirectory(this._SourceFolder + "\\build\\" + theDataFile + "\\translated");
                        System.IO.Directory.CreateDirectory(this._SourceFolder + "\\build\\" + theDataFile + "\\raw");
                        this.xorer.processFile(this._gamedirectory + "\\datas\\" + theDataFile + ".v", this._SourceFolder + "\\build\\" + theDataFile + ".dat");
                        theZip = Ionic.Zip.ZipFile.Read(this._SourceFolder + "\\build\\" + theDataFile + ".dat");
                        theZip.ExtractAll(this._SourceFolder + "\\build\\" + theDataFile + "\\raw\\raw", Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                        theZip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestSpeed;
                        listofProcess = new List<System.Diagnostics.Process>();
                        foreach (var hue in this._buildDatalist[theDataFile])
                        {
                            theProcess = new System.Diagnostics.Process();
                            theProcess.StartInfo.FileName = this._SourceFolder + "\\SWRFU.exe";
                            theProcess.StartInfo.Arguments = "\"" + this._directory + "\\" + theDataFile + "\\" + hue.Key + ".txt\" " +
                                "\"" + this._SourceFolder + "\\build\\" + theDataFile + "\\raw\\" + fixPath(hue.Value.Path) + "\\" + hue.Key + ".res\" " +
                                "\"" + this._SourceFolder + "\\build\\" + theDataFile + "\\translated\\" + hue.Key + ".res\" " +
                                hue.Value.Param.Trim();
                            //System.Windows.Forms.MessageBox.Show(theProcess.StartInfo.Arguments);
                            theProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            listofProcess.Add(theProcess);
                            //theProcess.Start();
                            //theProcess.WaitForExit();
                            //System.IO.File.Delete(this._directory + "\\" + filename + ".res");
                            //"\"" + currentDir + "\\build\\" + filename + ".res\""
                            /*
            "C:\Users\Dramiel Leayal\AppData\Roaming\SWPatcher\English\data12\tb_item_script.txt"
            "C:\Users\Dramiel Leayal\AppData\Roaming\SWPatcher\tb_item_script.res"
            "C:\Users\Dramiel Leayal\AppData\Roaming\SWPatcher\English\data12\tb_item_script.res"
            5 4 4 len 2 len 2 len 2 len 2 len 2 len 2 1 1 1 1 1 len 2 len 2
                             */
                        }
                        if (bWorkerCheckCancel.CancellationPending)
                            return false;
                        if (listofProcess.Count > 0)
                        {
                            for (int cou = 0; cou < listofProcess.Count; cou++)
                                listofProcess[cou].Start();
                            for (int cou = 0; cou < listofProcess.Count; cou++)
                            {
                                listofProcess[cou].WaitForExit();
                                listofProcess[cou].Dispose();
                                if (bWorkerCheckCancel.CancellationPending)
                                    return false;
                            }
                        }
                        foreach (var hue in this._buildDatalist[theDataFile])
                        {
                            if (bWorkerCheckCancel.CancellationPending)
                                return false;
                            if (System.IO.File.Exists(this._SourceFolder + "\\build\\" + theDataFile + "\\translated\\" + hue.Key + ".res"))
                            {
                                theZip.UpdateEntry(hue.Value.Path + hue.Key + ".res", System.IO.File.ReadAllBytes(this._SourceFolder + "\\build\\" + theDataFile + "\\translated\\" + hue.Key + ".res"));
                            }
                        }
                        
                        theZip.Save();
                        xorer.processFile(this._SourceFolder + "\\build\\" + theDataFile + ".dat", this._SourceFolder + "\\cache\\" + theDataFile + ".v");
                        EmptyFolder(new System.IO.DirectoryInfo(this._SourceFolder + "\\build"));
                        listofProcess.Clear();
                        try
                        { System.IO.Directory.Delete(this._SourceFolder + "\\build", true); }
                        catch
                        { }
                        theZip.Dispose();
                    }
                }
                theZip = null;
                this._building = false;
                return true;
            }
            return true;
        }

        private void EmptyFolder(System.IO.DirectoryInfo directoryInfo)
        {
            foreach (System.IO.FileInfo file in directoryInfo.GetFiles("*", System.IO.SearchOption.TopDirectoryOnly))
            {
                try
                { file.Delete(); }
                catch
                { }
            }
            foreach (System.IO.DirectoryInfo subfolder in directoryInfo.GetDirectories("*", System.IO.SearchOption.TopDirectoryOnly))
            {
                try
                { subfolder.Delete(true); }
                catch
                { }
            }
        }

        private string fixPath(string sPath)
        {
            if (sPath.StartsWith("..\\"))
                sPath = sPath.Remove(0, 3);
            if (sPath.EndsWith("\\"))
                sPath = sPath.Substring(0, sPath.Length - 1);
            return sPath;
        }

        public void Close()
        {
            foreach (var asd in this._buildDatalist.Values)
                asd.Clear();
            this._buildDatalist.Clear();
            //Process to be easier to read
            this._building = false;
            this._directory = null;
            this._gamedirectory = null;
            this.xorer = null;
            this._SourceFolder = null;
        }
    }

    class BuildData
    {
        string insidePath;
        string vparam;
        bool bCompile;

        public string Path
        {
            get { return this.insidePath; }
        }

        public string Param
        {
            get { return this.vparam; }
        }

        public bool Compile
        {
            get { return this.bCompile; }
        }

        public BuildData(string path, string p, bool vCompile)
        {
            this.insidePath = path;
            this.vparam = p;
            this.bCompile = vCompile;
        }

        public BuildData(string path, string p) : this(path, p, true) { }
    }
}
