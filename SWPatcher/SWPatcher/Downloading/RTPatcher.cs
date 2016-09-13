using MadMilkman.Ini;
using SWPatcher.Helpers.GlobalVar;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace SWPatcher.Helpers
{
    public delegate void RTPatchDownloadProgressChangedEventHandler(object sender, RTPatchDownloadProgressChangedEventArgs e);
    public delegate void RTPatchProgressChangedEventHandler(object sender, RTPatchProgressChangedEventArgs e);
    public delegate string RTPatchCallback(uint id, IntPtr ptr);

    public class RTPatcher
    {
        [DllImport("patchw32.dll", EntryPoint = "RTPatchApply32@12")]
        public static extern uint RTPatchApply32(string command, RTPatchCallback func, bool waitFlag);

        [DllImport("patchw64.dll", EntryPoint = "RTPatchApply32")]
        public static extern uint RTPatchApply64(string command, RTPatchCallback func, bool waitFlag);

        private readonly BackgroundWorker Worker;
        private readonly WebClient Client;
        private string CurrentLogFilePath;
        private string FileName;
        private string Url;
        private Version ClientVersion;
        private Version ServerVersion;
        private int FileCount;
        private int FileNumber;

        public event RTPatchDownloadProgressChangedEventHandler RTPatchDownloadProgressChanged;
        public event RTPatchProgressChangedEventHandler RTPatchProgressChanged;
        public event AsyncCompletedEventHandler RTPatchCompleted;

        public RTPatcher()
        {
            this.Worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += Worker_DoWork;
            this.Worker.ProgressChanged += Worker_ProgressChanged;
            this.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            this.Client = new WebClient();
            this.Client.DownloadProgressChanged += Client_DownloadProgressChanged;
            this.Client.DownloadFileCompleted += Client_DownloadFileCompleted;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.Worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            string gamePath = UserSettings.GamePath;
            this.Apply(gamePath, Path.Combine(gamePath, Methods.VersionToRTP(this.ClientVersion)));
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.RTPatchProgressChanged?.Invoke(this, new RTPatchProgressChangedEventArgs(this.FileNumber, this.FileCount, this.FileName, e.ProgressPercentage));
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                this.RTPatchCompleted?.Invoke(sender, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.Result));
            else
            {
                IniFile ini = new IniFile(new IniOptions
                {
                    KeyDuplicate = IniDuplication.Ignored,
                    SectionDuplicate = IniDuplication.Ignored
                });
                string iniPath = Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer);
                ini.Load(iniPath);
                string serverVer = ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value;
                string clientVer = this.ClientVersion.ToString();
                if (serverVer != clientVer)
                {
                    ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value = clientVer;
                    ini.Save(iniPath);
                }

                this.DownloadNext();
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.RTPatchDownloadProgressChanged?.Invoke(sender, new RTPatchDownloadProgressChangedEventArgs(Methods.VersionToRTP(this.ClientVersion), e));
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                this.RTPatchCompleted?.Invoke(sender, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            else
                this.Worker.RunWorkerAsync();
        }

        private void LoadVersions()
        {
            IniFile serverIni = Methods.GetServerIni();
            this.ServerVersion = new Version(serverIni.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
            string address = serverIni.Sections[Strings.IniName.ServerRepository.Section].Keys[Strings.IniName.ServerRepository.Key].Value;
            this.Url = address + Strings.IniName.ServerRepository.UpdateRepository;

            IniFile clientIni = new IniFile();
            clientIni.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer));
            this.ClientVersion = new Version(clientIni.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
        }

        private void DownloadNext()
        {
            if (this.ClientVersion < this.ServerVersion)
            {
                this.ClientVersion = GetNextVersion(this.ClientVersion, this.ServerVersion);
                string RTPFileName = Methods.VersionToRTP(this.ClientVersion);
                Client.DownloadFileAsync(new Uri(this.Url + '/' + RTPFileName), Path.Combine(UserSettings.GamePath, RTPFileName));
            }
            else
            {
                this.RTPatchCompleted?.Invoke(this, new AsyncCompletedEventArgs(null, false, null));
            }
        }

        private static Version GetNextVersion(Version clientVer, Version serverVer)
        {
            var result = new Version(clientVer.Major + 1, 0, 0, 0);

            if (result > serverVer)
            {
                result = new Version(clientVer.Major, clientVer.Minor + 1, 0, 0);

                if (result > serverVer)
                {
                    result = new Version(clientVer.Major, clientVer.Minor, clientVer.Build + 1, 0);

                    if (result > serverVer)
                    {
                        result = new Version(clientVer.Major, clientVer.Minor, clientVer.Build, clientVer.Revision + 1);
                    }
                }
            }

            return result;
        }

        private void Apply(string directory, string diffFilePath)
        {
            this.CurrentLogFilePath = Path.Combine(Strings.FolderName.RTPatchLogs, Path.GetFileName(diffFilePath) + ".log");
            string logDirectory = Path.GetDirectoryName(this.CurrentLogFilePath);
            Directory.CreateDirectory(logDirectory);
            File.WriteAllText(this.CurrentLogFilePath, string.Empty);

            string command = $"/u /nos \"{directory}\" \"{diffFilePath}\"";
            ulong result = Environment.Is64BitProcess ? RTPatchApply64(command, new RTPatchCallback(RTPatchMessage), true) : RTPatchApply32(command, new RTPatchCallback(RTPatchMessage), true);
            File.Delete(diffFilePath);
            File.AppendAllText(this.CurrentLogFilePath, $"Result=[{result}]");

            if (result != 0)
                throw new Exception($"Result=[{result}]");
        }

        private string RTPatchMessage(uint id, IntPtr ptr)
        {
            switch (id)
            {
                case 1u:
                case 2u:
                case 3u:
                case 4u:
                case 8u:
                case 9u:
                case 10u:
                case 11u:
                case 12u: // outputs
                    {
                        string arg = Marshal.PtrToStringAnsi(ptr);
                        File.AppendAllText(this.CurrentLogFilePath, arg);

                        break;
                    }
                case 14u:
                case 17u:
                case 18u: // idk
                    return null;
                case 5u: // completion percentage
                    {
                        int readInt = Marshal.ReadInt32(ptr);
                        int percentage = readInt > short.MaxValue ? int.MaxValue : readInt * 0x10000;
                        this.Worker.ReportProgress(percentage);
                        percentage = readInt * 100 / 0x8000;
                        File.AppendAllText(this.CurrentLogFilePath, $"[{percentage}%]");

                        break;
                    }
                case 6u: // number of files in patch
                    {
                        int fileCount = Marshal.ReadInt32(ptr);
                        this.FileNumber = 0;
                        this.FileCount = fileCount;
                        File.AppendAllText(this.CurrentLogFilePath, $"File Count=[{fileCount}]\n");

                        break;
                    }
                case 7u: // current file
                    {
                        string fileName = Marshal.PtrToStringAnsi(ptr);
                        this.FileNumber++;
                        this.FileName = fileName;
                        File.AppendAllText(this.CurrentLogFilePath, $"Patching=[{fileName}]\n");

                        break;
                    }
                case 32u:
                case 33u: // idk
                    {
                        int[] numbers = new int[2];
                        Marshal.Copy(ptr, numbers, 0, 2);
                        File.AppendAllText(this.CurrentLogFilePath, $"number1=[{numbers[0]}] number2=[{numbers[1]}]\n");

                        break;
                    }
                default: break; // ignore rest
            }
            return "";
        }

        public void Cancel()
        {
            this.Client.CancelAsync();
            this.Worker.CancelAsync();
        }

        public void Run()
        {
            if (this.Client.IsBusy || this.Worker.IsBusy)
                return;

            Methods.RTPatchCleanup();
            this.LoadVersions();
            this.DownloadNext();
        }
    }
}
