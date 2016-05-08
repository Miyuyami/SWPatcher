using System;
using System.Collections.Generic;
using System.ComponentModel;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;
using System.IO;
using Ionic.Zip;
using System.Threading.Tasks;
using System.Linq;

namespace SWPatcher.Patching
{
    public class Patcher
    {
        private readonly BackgroundWorker Worker;
        private readonly List<SWFile> SWFiles;
        private Language Language;

        public Patcher(List<SWFile> swFiles)
        {
            this.SWFiles = swFiles;
            this.Worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            this.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
        }

        public event PatcherProgressChangedEventHandler PatcherProgressChanged;
        public event PatcherCompletedEventHandler PatcherCompleted;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (TranslationsExist() && IsClientNewerVersion())
            {
                foreach (var swFile in SWFiles)
                {
                    if (!string.IsNullOrEmpty(swFile.PathA))
                    {
                        string archivePath = Path.Combine(Paths.PatcherRoot, this.Language.Lang, swFile.Path);
                        File.Copy(Path.Combine(Paths.GameRoot, swFile.Path), archivePath, true); // copy .v
                        Xor(archivePath, 0x55);
                        string swFilePath = GetSWFilePath(swFile);
                        if (!string.IsNullOrEmpty(swFile.Format)) // if file should be patched(.res)
                        {
                            string swFilePathOriginalRes = Path.Combine(Paths.PatcherRoot, Path.GetFileNameWithoutExtension(swFilePath)) + ".res";
                            string swFilePathRes = Path.ChangeExtension(swFilePath, ".res");
                            DoUnzipFile(archivePath, swFile.PathA, swFilePathOriginalRes);
                            string[] formatArray = swFile.Format.Split(' ');
                            PatchFile(swFilePath, swFilePathRes, swFilePathOriginalRes, formatArray);
                            File.Delete(swFilePathOriginalRes);
                            DoZipFile(archivePath, swFile.PathA, swFilePathRes);
                            File.Delete(swFilePathRes);
                        }
                        else // just zip other files
                        {
                            if (Path.GetExtension(swFilePath) == ".zip")
                                AddZipToZip(swFilePath, archivePath);
                            else
                                DoZipFile(archivePath, swFile.PathA, swFilePath);
                        }
                        Xor(archivePath, 0x55);
                    }
                }
            }
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

        private void Xor(string path, byte secretByte)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
            {
                int b;
                while ((b = stream.ReadByte()) != -1)
                {
                    stream.Position--;
                    stream.WriteByte((byte)b);
                }
            }
        }

        private void DoUnzipFile(string zipPath, string fileName, string extractDestination)
        {
            using (var zip = ZipFile.Read(zipPath))
            {
                zip.FlattenFoldersOnExtract = true;
                zip[fileName].Extract(extractDestination, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        private void DoZipFile(string zipPath, string fileName, string filePath)
        {
            using (var zip = ZipFile.Read(zipPath))
            {
                zip.RemoveEntry(fileName);
                zip.AddFile(fileName);
                zip.Save();
            }
        }

        private void AddZipToZip(string zipPath, string destinationZipPath)
        {
            using (var zip = ZipFile.Read(zipPath))
            using (var destinationZip = ZipFile.Read(destinationZipPath))
            {
                zip.FlattenFoldersOnExtract = true;
                destinationZip.RemoveEntries(zip.Entries);
                using (var file = new TempFile())
                {
                    zip.ToList().ForEach(entry =>
                    {
                        entry.Extract(file.Path, ExtractExistingFileAction.OverwriteSilently);
                        destinationZip.AddFile(file.Path, entry.FileName);
                    });
                }
            }
        }

        private bool TranslationsExist()
        {
            string translationIniPath = Path.Combine(Paths.PatcherRoot, this.Language.Lang, Strings.IniName.Translation);
            if (!File.Exists(translationIniPath))
                throw new Exception("Translation settings file not found");
            foreach (var swFile in SWFiles)
            {
                if (!File.Exists(GetSWFilePath(swFile)))
                    throw new Exception(string.Format("Translation file {0} not found", swFile.Name));
            }
            return true;
        }

        private string GetSWFilePath(SWFile swFile)
        {
            string path = "";
            if (string.IsNullOrEmpty(swFile.Path))
                path = Path.Combine(Paths.PatcherRoot, this.Language.Lang);
            else
                path = Path.Combine(Path.GetDirectoryName(Path.Combine(Paths.PatcherRoot, this.Language.Lang, swFile.Path)), Path.GetFileNameWithoutExtension(swFile.Path));
            return Path.Combine(path, Path.GetFileName(swFile.PathD));
        }

        private bool IsClientNewerVersion()
        {
            IniReader clientIni = new IniReader(Path.Combine(Paths.GameRoot, Strings.IniName.ClientVer));
            Version client = new Version(clientIni.ReadString(Strings.IniName.Ver.Section, Strings.IniName.Ver.Key, "0.0.0.0"));
            IniReader translationIni = new IniReader(Path.Combine(Paths.PatcherRoot, this.Language.Lang, Strings.IniName.Translation));
            Version translation = new Version(translationIni.ReadString(Strings.IniName.Patcher.Section, Strings.IniName.Patcher.KeyVer, "0.0.0.0"));
            if (client > translation)
                return true;
            return false;
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
