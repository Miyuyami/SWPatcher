using MadMilkman.Ini;
using SWPatcherTest.General;
using SWPatcherTest.Helpers;
using SWPatcherTest.Helpers.GlobalVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SWPatcherTest.Patching
{
    public delegate void PatcherProgressChangedEventHandler(object sender, PatcherProgressChangedEventArgs e);
    public delegate void PatcherCompletedEventHandler(object sender, PatcherCompletedEventArgs e);

    public class Patcher
    {
        private readonly BackgroundWorker Worker;
        private readonly List<SWFile> SWFiles;
        private Language Language;
        private int CurrentStep;
        private int StepCount;

        public Patcher(List<SWFile> swFiles)
        {
            this.SWFiles = swFiles;
            this.Worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += Worker_DoWork;
            this.Worker.ProgressChanged += Worker_ProgressChanged;
            this.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        public event PatcherProgressChangedEventHandler PatcherProgressChanged;
        public event PatcherCompletedEventHandler PatcherCompleted;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Logger.Debug($"Patcher worker thread ID=[{Thread.CurrentThread.ManagedThreadId}] Language=[{this.Language.ToString()}]");

            this.StepCount = 3;
            var archivedSWFiles = SWFiles.Where(f => !String.IsNullOrEmpty(f.PathA));
            var archives = archivedSWFiles.Select(f => f.Path).Distinct();
            int archivedSWFilesCount = archivedSWFiles.Count();

            var passwordDictionary = LoadPasswords();

            this.CurrentStep = 1;
            foreach (var archive in archives) // copy and Xor archives
            {
                if (this.Worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                string archivePathPatch = Path.Combine(this.Language.Lang, archive);
                string archivePath = Path.Combine(UserSettings.GamePath, archive);
                Logger.Info($"Copying archive=[{archivePath}] archivePatch=[{archivePathPatch}]");
                File.Copy(archivePath, archivePathPatch, true);
                this.Xor(archivePathPatch, 0x55, true);
            }

            this.CurrentStep = 2;
            int count = 1;
            foreach (var swFile in archivedSWFiles)
            {
                if (this.Worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                this.Worker.ReportProgress(count++ == archivedSWFilesCount ? int.MaxValue : Convert.ToInt32(((double)count / archivedSWFilesCount) * int.MaxValue));

                string archivePath = Path.Combine(this.Language.Lang, swFile.Path);
                string archiveFileNameWithoutExtension = Path.GetFileNameWithoutExtension(archivePath);
                string archivePassword = "";
                string swFilePath = GetArchivedSWFilePath(swFile, this.Language);

                if (passwordDictionary.ContainsKey(archiveFileNameWithoutExtension))
                    archivePassword = passwordDictionary[archiveFileNameWithoutExtension];

                Logger.Debug($"Patching file=[{swFilePath}] archive=[{archivePath}] pw=[{archivePassword}]");
                if (!String.IsNullOrEmpty(swFile.Format)) // if file should be patched(.res)
                {
                    using (var swFilePathRes = new TempFile(Path.ChangeExtension(swFilePath, ".res")))
                    {
                        Methods.DoUnzipFile(archivePath, swFile.PathA, Directory.GetCurrentDirectory(), archivePassword);

                        using (var swFilePathOriginalRes = new TempFile(Path.GetFileName(swFile.PathA)))
                        {
                            string[] fullFormatArray = swFile.Format.Split(' ');
                            int idIndex = Convert.ToInt32(fullFormatArray[0]);
                            string countFormat = fullFormatArray[1];
                            string[] formatArray = fullFormatArray.Skip(2).ToArray(); // skip idIndex and countFormat

                            #region Patching the File
                            ulong dataCount = 0;
                            ulong dataSum = 0;
                            ushort hashLength = 32;
                            byte[] hash = new byte[hashLength];
                            int lineCount = 0;

                            for (int i = 0; i < formatArray.Length; i++)
                                if (formatArray[i] == "len")
                                {
                                    lineCount++;
                                    i++;
                                }

                            var inputTable = this.ReadInputFile(swFilePath, lineCount, idIndex);

                            using (var br = new BinaryReader(File.Open(swFilePathOriginalRes.Path, FileMode.Open, FileAccess.Read)))
                            using (var bw = new BinaryWriter(File.Open(swFilePathRes.Path, FileMode.OpenOrCreate, FileAccess.Write)))
                            {
                                switch (countFormat)
                                {
                                    case "1":
                                        dataCount = br.ReadByte();
                                        bw.Write(Convert.ToByte(dataCount));
                                        break;
                                    case "2":
                                        dataCount = br.ReadUInt16();
                                        bw.Write(Convert.ToUInt16(dataCount));
                                        break;
                                    case "4":
                                        dataCount = br.ReadUInt32();
                                        bw.Write(Convert.ToUInt32(dataCount));
                                        break;
                                    case "8":
                                        dataCount = br.ReadUInt64();
                                        bw.Write(Convert.ToUInt64(dataCount));
                                        break;
                                }
                                ulong value = 0;

                                for (ulong i = 0; i < dataCount; i++)
                                {
                                    if (this.Worker.CancellationPending)
                                    {
                                        e.Cancel = true;
                                        break;
                                    }

                                    #region Object Reading
                                    object[] current = new object[formatArray.Length];
                                    for (int j = 0; j < formatArray.Length; j++)
                                    {
                                        if (this.Worker.CancellationPending)
                                        {
                                            e.Cancel = true;
                                            break;
                                        }

                                        switch (formatArray[j])
                                        {
                                            case "1":
                                                current[j] = Convert.ToByte(br.ReadByte());
                                                break;
                                            case "2":
                                                current[j] = Convert.ToUInt16(br.ReadUInt16());
                                                break;
                                            case "4":
                                                current[j] = Convert.ToUInt32(br.ReadUInt32());
                                                break;
                                            case "8":
                                                current[j] = Convert.ToUInt64(br.ReadUInt64());
                                                break;
                                            case "len":
                                                switch (formatArray[++j])
                                                {
                                                    case "1":
                                                        value = br.ReadByte();
                                                        current[j] = Convert.ToByte(br.ReadByte());
                                                        break;
                                                    case "2":
                                                        value = br.ReadUInt16();
                                                        current[j] = Convert.ToUInt16(value);
                                                        break;
                                                    case "4":
                                                        value = br.ReadUInt32();
                                                        current[j] = Convert.ToUInt32(value);
                                                        break;
                                                    case "8":
                                                        value = br.ReadUInt64();
                                                        current[j] = Convert.ToUInt64(value);
                                                        break;
                                                }
                                                ulong strBytesLength = value * 2;
                                                byte[] strBytes = new byte[strBytesLength];
                                                current[j] = strBytes;

                                                for (ulong k = 0; k < strBytesLength; k++)
                                                    strBytes[k] = br.ReadByte();
                                                break;
                                        }
                                    }
                                    #endregion

                                    #region Object Writing
                                    int lenPosition = 0;
                                    for (int j = 0; j < formatArray.Length; j++)
                                    {
                                        if (this.Worker.CancellationPending)
                                        {
                                            e.Cancel = true;
                                            break;
                                        }

                                        switch (formatArray[j])
                                        {
                                            case "1":
                                                value = Convert.ToByte(current[j]);
                                                bw.Write(Convert.ToByte(value));
                                                break;
                                            case "2":
                                                value = Convert.ToUInt16(current[j]);
                                                bw.Write(Convert.ToUInt16(value));
                                                break;
                                            case "4":
                                                value = Convert.ToUInt32(current[j]);
                                                bw.Write(Convert.ToUInt32(value));
                                                break;
                                            case "8":
                                                value = Convert.ToUInt64(current[j]);
                                                bw.Write(Convert.ToUInt64(value));
                                                break;
                                            case "len":
                                                byte[] strBytes = null;
                                                j++;
                                                ulong id = Convert.ToUInt64(current[idIndex]);
                                                if (inputTable.ContainsKey(id))
                                                    strBytes = Encoding.Unicode.GetBytes(inputTable[id][lenPosition++]);
                                                else
                                                    strBytes = current[j] as byte[];
                                                value = Convert.ToUInt64(strBytes.Length / 2);

                                                switch (formatArray[j])
                                                {
                                                    case "1":
                                                        bw.Write(Convert.ToByte(value));
                                                        break;
                                                    case "2":
                                                        bw.Write(Convert.ToUInt16(value));
                                                        break;
                                                    case "4":
                                                        bw.Write(Convert.ToUInt32(value));
                                                        break;
                                                    case "8":
                                                        bw.Write(Convert.ToUInt64(value));
                                                        break;
                                                }

                                                foreach (byte b in strBytes)
                                                {
                                                    dataSum += b;
                                                    bw.Write(b);
                                                }
                                                break;
                                        }

                                        dataSum += value;
                                    }
                                    #endregion
                                }

                                bw.Write(hashLength);
                                string hashString = GetMD5(Convert.ToString(dataSum));
                                for (int i = 0; i < hashLength; i++)
                                    hash[i] = Convert.ToByte(hashString[i]);
                                bw.Write(hash);
                            }
                            #endregion
                        }

                        Methods.DoZipFile(archivePath, swFile.PathA, swFilePathRes.Path, archivePassword);
                    }
                }
                else // just zip other files
                {
                    if (Path.GetExtension(swFilePath) == ".zip")
                        Methods.AddZipToZip(swFilePath, archivePath, swFile.PathA, archivePassword);
                    else
                        Methods.DoZipFile(archivePath, swFile.PathA, swFilePath, archivePassword);
                }

                File.Delete(swFilePath);
            }

            this.CurrentStep = 3;
            foreach (var archive in archives) // Xor archives
            {
                if (this.Worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                string archivePath = Path.Combine(this.Language.Lang, archive);
                this.Xor(archivePath, 0x55, false);
            }

            if (UserSettings.WantToPatchExe)
            {
                this.CurrentStep = -1;
                this.Worker.ReportProgress(-1);
                string gameExePath = Path.Combine(UserSettings.GamePath, Strings.FileName.GameExe);
                string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, Strings.FileName.GameExe);
                
                File.Copy(gameExePath, gameExePatchedPath, true);
                Methods.PatchExeFile(gameExePatchedPath);
            }

            GC.Collect();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.PatcherProgressChanged?.Invoke(sender, new PatcherProgressChangedEventArgs(CurrentStep, StepCount, e.ProgressPercentage));
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.PatcherCompleted?.Invoke(sender, new PatcherCompletedEventArgs(this.Language, e.Cancelled, e.Error));
        }

        private Dictionary<ulong, string[]> ReadInputFile(string path, int lineCount, int idIndex)
        {
            int idTextLength = 3;
            int emptyLineCount = 1;
            int idLineCount = 1;
            lineCount += idLineCount;
            int entryLineCount = lineCount + emptyLineCount;
            var result = new Dictionary<ulong, string[]>();

            string[] fileLines = File.ReadAllLines(path, Encoding.UTF8);

            for (int i = 0; i < fileLines.Length; i += entryLineCount)
            {
                var currentData = new string[lineCount];

                for (int j = 0; j < lineCount; j++)
                {
                    if (i + j < fileLines.Length)
                        currentData[j] = fileLines[i + j].Replace("\\n", "\n");
                    else
                        currentData[j] = "";
                }

                ulong id = Convert.ToUInt64(currentData[idIndex].Substring(idTextLength));
                List<string> dataList = currentData.ToList();
                dataList.RemoveAt(idIndex);
                string[] data = dataList.ToArray();

                if (!result.ContainsKey(id))
                    result.Add(id, data);
            }

            return result;
        }

        private void Xor(string path, byte secretByte, bool check)
        {
            Logger.Debug($"Xor path=[{path}] check=[{check}]");

            byte[] fileBytes = File.ReadAllBytes(path);

            if (!(check && fileBytes[0] == 0x50 && fileBytes[1] == 0x4B))
            {
                for (int i = 0; i < fileBytes.Length; i++)
                {
                    if (i % (fileBytes.Length / 8) == 0)
                        this.Worker.ReportProgress(i == fileBytes.Length ? int.MaxValue : Convert.ToInt32(((double)i / fileBytes.Length) * int.MaxValue));

                    fileBytes[i] ^= secretByte;
                }
            }

            File.WriteAllBytes(path, fileBytes);
        }

        private static string GetArchivedSWFilePath(SWFile swFile, Language language)
        {
            string directory = Path.GetDirectoryName(Path.Combine(language.Lang, swFile.Path));
            string archiveName = Path.GetFileNameWithoutExtension(swFile.Path);
            string fileName = Path.GetFileName(swFile.PathD);

            return Path.Combine(directory, archiveName, fileName);
        }

        private static string GetMD5(string text)
        {
            using (var md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(Encoding.ASCII.GetBytes(text));
                StringBuilder sb = new StringBuilder();

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        private static Dictionary<string, string> LoadPasswords()
        {
            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                var result = new Dictionary<string, string>();

                client.DownloadFile(Urls.PatcherGitHubHome + Strings.IniName.DatasArchives, file.Path);
                IniFile ini = new IniFile();
                ini.Load(file.Path);

                var section = ini.Sections[Strings.IniName.Datas.SectionZipPassword];
                foreach (var key in section.Keys)
                {
                    result.Add(key.Name, key.Value);
                }

                return result;
            }
        }

        public void Cancel()
        {
            this.Worker.CancelAsync();
        }

        public void Run(Language language)
        {
            if (this.Worker.IsBusy)
                return;
            
            this.Language = language;
            this.Worker.RunWorkerAsync();
        }
    }
}
