/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016 Miyu
 * 
 * Soulworker Patcher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Soulworker Patcher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Soulworker Patcher. If not, see <http://www.gnu.org/licenses/>.
 */

using Ionic.Zip;
using MadMilkman.Ini;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SWPatcher.Patching
{
    public delegate void PatcherProgressChangedEventHandler(object sender, PatcherProgressChangedEventArgs e);
    public delegate void PatcherCompletedEventHandler(object sender, PatcherCompletedEventArgs e);

    public class Patcher
    {
        public enum State
        {
            Idle = 0,
            Load,
            Patch,
            Save,
            ExePatch
        }

        private State _state;
        private readonly BackgroundWorker Worker;
        private Language Language;
        private const byte SecretByte = 0x55;
        public State CurrentState
        {
            get
            {
                return this._state;
            }
            private set
            {
                Logger.Info($"State=[{value}]");
                if (value != State.Idle)
                {
                    this.Worker.ReportProgress(-1);
                }

                this._state = value;
            }
        }

        public Patcher()
        {
            this.Worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += this.Worker_DoWork;
            this.Worker.ProgressChanged += this.Worker_ProgressChanged;
            this.Worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
        }

        public event PatcherProgressChangedEventHandler PatcherProgressChanged;
        public event PatcherCompletedEventHandler PatcherCompleted;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Logger.Debug(Methods.MethodFullName("Patcher", Thread.CurrentThread.ManagedThreadId.ToString(), this.Language.ToString()));

            this.CurrentState = State.Load;
            IEnumerable<ArchivedSWFile> archivedSWFiles = SWFileManager.GetFiles().OfType<ArchivedSWFile>();
            Dictionary<string, string> passwordDictionary = LoadPasswords();
            int archivedSWFilesCount = archivedSWFiles.Count();
            var archives = archivedSWFiles.Select(f => f.Path).Distinct().ToDictionary(p => p, p =>
            {
                string archivePath = Path.Combine(UserSettings.GamePath, p);
                Logger.Info($"Loading archive=[{archivePath}]");
                byte[] fileBytes = File.ReadAllBytes(archivePath);
                var xms = new XorMemoryStream(fileBytes, SecretByte);
                return ZipFile.Read(xms);
            });

            this.CurrentState = State.Patch;
            int count = 1;
            foreach (ArchivedSWFile archivedSWFile in archivedSWFiles)
            {
                if (this.Worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                this.Worker.ReportProgress(count++ == archivedSWFilesCount ? int.MaxValue : Convert.ToInt32(((double)count / archivedSWFilesCount) * int.MaxValue));

                string archiveFileNameWithoutExtension = Path.GetFileNameWithoutExtension(archivedSWFile.Path);
                string archivePassword = null;

                if (passwordDictionary.ContainsKey(archiveFileNameWithoutExtension))
                {
                    archivePassword = passwordDictionary[archiveFileNameWithoutExtension];
                }

                Logger.Info($"Patching file=[{archivedSWFile.PathA}] archive=[{archivedSWFile.Path}]");
                if (archivedSWFile is PatchedSWFile patchedSWFile)
                {
                    MemoryStream ms = Methods.GetZippedFileStream(archives[patchedSWFile.Path], patchedSWFile.PathA, archivePassword);
                    MemoryStream msDest = new MemoryStream();

                    string[] fullFormatArray = patchedSWFile.Format.Split(' ');
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
                    {
                        if (formatArray[i] == "len")
                        {
                            lineCount++;
                            i++;
                        }
                    }

                    Dictionary<ulong, string[]> inputTable = this.ReadInputFile(patchedSWFile.Data, lineCount, idIndex);

                    using (var br = new BinaryReader(ms))
                    using (var bw = new BinaryWriter(msDest, new UTF8Encoding(false, true), true))
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
                        {
                            hash[i] = Convert.ToByte(hashString[i]);
                        }

                        bw.Write(hash);
                    }
                    #endregion

                    Methods.ZipFileStream(archives[patchedSWFile.Path], patchedSWFile.PathA, msDest, archivePassword);
                }
                else
                {
                    var ms = new MemoryStream(archivedSWFile.Data);

                    if (Path.GetExtension(archivedSWFile.PathD) == ".zip")
                    {
                        Methods.AddZipToZip(archives[archivedSWFile.Path], archivedSWFile.PathA, ms, archivePassword);
                    }
                    else
                    {
                        Methods.ZipFileStream(archives[archivedSWFile.Path], archivedSWFile.PathA, ms, archivePassword);
                    }
                }
            }

            this.CurrentState = State.Save;
            foreach (KeyValuePair<string, ZipFile> archive in archives)
            {
                if (this.Worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                string zipFileName = archive.Key;
                ZipFile zipFile = archive.Value;
                string archivePath = Path.Combine(this.Language.Name, zipFileName);
                string archivePathDirectory = Path.GetDirectoryName(archivePath);

                Directory.CreateDirectory(archivePathDirectory);

                using (var xfs = new XorFileStream(archivePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, SecretByte))
                {
                    zipFile.Save(xfs);
                    zipFile.Dispose();
                }
            }

            if (UserSettings.WantToPatchExe)
            {
                this.CurrentState = State.ExePatch;
                string gameExePath = Path.Combine(UserSettings.GamePath, Strings.FileName.GameExe);
                byte[] gameExeBytes = File.ReadAllBytes(gameExePath);
                string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, Strings.FileName.GameExe);

                Methods.PatchExeFile(gameExeBytes, gameExePatchedPath);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.PatcherProgressChanged?.Invoke(sender, new PatcherProgressChangedEventArgs(this.CurrentState, e.ProgressPercentage));
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.PatcherCompleted?.Invoke(sender, new PatcherCompletedEventArgs(this.Language, e.Cancelled, e.Error));
            this.CurrentState = State.Idle;
        }

        private Dictionary<ulong, string[]> ReadInputFile(byte[] fileBytes, int lineCount, int idIndex)
        {
            int idTextLength = 3;
            int emptyLineCount = 1;
            int idLineCount = 1;
            lineCount += idLineCount;
            int entryLineCount = lineCount + emptyLineCount;
            var result = new Dictionary<ulong, string[]>();

            string[] fileLines = fileBytes.ToStringArray(Encoding.UTF8);

            for (int i = 0; i < fileLines.Length; i += entryLineCount)
            {
                string[] currentData = new string[lineCount];

                for (int j = 0; j < lineCount; j++)
                {
                    if (i + j < fileLines.Length)
                    {
                        currentData[j] = fileLines[i + j].Replace("\\n ", "\n").Replace("\\n", "\n");
                    }
                    else
                    {
                        currentData[j] = "";
                    }
                }

                ulong id = Convert.ToUInt64(currentData[idIndex].Substring(idTextLength));
                List<string> dataList = currentData.ToList();
                dataList.RemoveAt(idIndex);
                string[] data = dataList.ToArray();

                if (!result.ContainsKey(id) && data.Length <= 511)
                {
                    result.Add(id, data);
                }
            }

            return result;
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
            {
                var result = new Dictionary<string, string>();

                byte[] fileBytes = client.DownloadData(Urls.PatcherGitHubHome + Strings.IniName.DatasArchives);
                IniFile ini = new IniFile();
                using (var ms = new MemoryStream(fileBytes))
                {
                    ini.Load(ms);
                }

                IniSection section = ini.Sections[Strings.IniName.Datas.SectionZipPassword];
                foreach (IniKey key in section.Keys)
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
            {
                return;
            }

            this.Language = language;
            this.Worker.RunWorkerAsync();
        }
    }
}
