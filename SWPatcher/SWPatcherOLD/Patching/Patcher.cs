using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Patching
{
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
            this.Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            this.Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            this.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
        }

        public event PatcherProgressChangedEventHandler PatcherProgressChanged;
        public event PatcherCompletedEventHandler PatcherCompleted;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.StepCount = 3;
            var archivedSWFiles = SWFiles.Where(f => !String.IsNullOrEmpty(f.PathA));
            var archives = archivedSWFiles.Select(f => f.Path).Distinct();
            int archivedSWFilesCount = archivedSWFiles.Count();

            this.CurrentStep = 1;
            foreach (var archive in archives) // copy and Xor archives
            {
                if (this.Worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                string archivePath = Path.Combine(this.Language.Lang, archive);
                File.Copy(Path.Combine(UserSettings.GamePath, archive), archivePath, true);
                this.Xor(archivePath, 0x55);
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
                string swFilePath = Methods.GetArchivedSWFilePath(swFile, this.Language);

                if (!String.IsNullOrEmpty(swFile.Format)) // if file should be patched(.res)
                {
                    using (var swFilePathRes = new TempFile(Path.ChangeExtension(swFilePath, ".res")))
                    {
                        Methods.DoUnzipFile(archivePath, swFile.PathA, Directory.GetCurrentDirectory());

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
                                string hashString = Methods.GetMD5(Convert.ToString(dataSum));
                                for (int i = 0; i < hashLength; i++)
                                    hash[i] = Convert.ToByte(hashString[i]);
                                bw.Write(hash);
                            }
                            #endregion
                        }

                        Methods.DoZipFile(archivePath, swFile.PathA, swFilePathRes.Path);
                    }
                }
                else // just zip other files
                {
                    if (Path.GetExtension(swFilePath) == ".zip")
                        Methods.AddZipToZip(swFilePath, archivePath, swFile.PathA);
                    else
                        Methods.DoZipFile(archivePath, swFile.PathA, swFilePath);
                }
            }

            this.CurrentStep = 3;
            foreach (var archive in archives) // copy and Xor archives
            {
                if (this.Worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                string archivePath = Path.Combine(this.Language.Lang, archive);
                this.Xor(archivePath, 0x55);
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
            OnPatcherProgressChanged(new PatcherProgressChangedEventArgs(CurrentStep, StepCount, e.ProgressPercentage));
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnPatcherComplete(sender, new PatcherCompletedEventArgs(this.Language, e.Cancelled, e.Error));
        }

        private void OnPatcherProgressChanged(PatcherProgressChangedEventArgs e)
        {
            if (this.PatcherProgressChanged != null)
                this.PatcherProgressChanged(this, e);
        }

        private void OnPatcherComplete(object sender, PatcherCompletedEventArgs e)
        {
            if (this.PatcherCompleted != null)
                this.PatcherCompleted(sender, e);
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

        private void Xor(string path, byte secretByte)
        {
            byte[] fileBytes = File.ReadAllBytes(path);

            for (int i = 0; i < fileBytes.Length; i++)
            {
                if (i % (fileBytes.Length / 8) == 0)
                    this.Worker.ReportProgress(i == fileBytes.Length ? int.MaxValue : Convert.ToInt32(((double)i / fileBytes.Length) * int.MaxValue));

                fileBytes[i] ^= secretByte;
            }

            File.WriteAllBytes(path, fileBytes);
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
