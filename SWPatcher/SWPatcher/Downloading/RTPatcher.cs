using Ionic.Zip;
using MadMilkman.Ini;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace SWPatcher.Helpers
{
    public delegate void RTPatchDownloadProgressChangedEventHandler(object sender, RTPatchDownloadProgressChangedEventArgs e);
    public delegate void RTPatchProgressChangedEventHandler(object sender, RTPatchProgressChangedEventArgs e);
    public delegate void RTPatchCompletedEventHandler(object sender, RTPatchCompletedEventArgs e);
    public delegate string RTPatchCallback(uint id, IntPtr ptr);

    public class RTPatcher
    {
        [DllImport("patchw32.dll", EntryPoint = "RTPatchApply32@12")]
        public static extern uint RTPatchApply(string command, RTPatchCallback func, bool waitFlag);

        private readonly BackgroundWorker Worker;
        private readonly WebClient Client;
        private string CurrentLogFile;
        private string FileName;
        private string Caller;
        private RTPatchVersion RTVersion;
        private int FileCount;
        private int FileNumber;

        public event RTPatchDownloadProgressChangedEventHandler RTPatchDownloadProgressChanged;
        public event RTPatchProgressChangedEventHandler RTPatchProgressChanged;
        public event RTPatchCompletedEventHandler RTPatchCompleted;

        public RTPatcher()
        {
            this.FileName = "";
            this.FileCount = 0;
            this.FileNumber = 0;
            this.Worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += Worker_DoWork;
            this.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            this.Client = new WebClient();
            this.Client.DownloadProgressChanged += Client_DownloadProgressChanged;
            this.Client.DownloadFileCompleted += Client_DownloadFileCompleted;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string gamePath = UserSettings.GamePath;
            e.Result = this.Apply(gamePath, Path.Combine(gamePath, this.RTVersion.ToString(false)));
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                this.RTPatchCompleted?.Invoke(sender, new RTPatchCompletedEventArgs(this.RTVersion, this.Caller, Convert.ToUInt32(e.Result), e.Cancelled, e.Error));
            else
                this.Download();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.RTPatchDownloadProgressChanged?.Invoke(sender, new RTPatchDownloadProgressChangedEventArgs(this.RTVersion.ToString(false), e));
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                this.RTPatchCompleted?.Invoke(sender, new RTPatchCompletedEventArgs(this.RTVersion, this.Caller, e.Cancelled, e.Error));
            }
            else
            {
                this.Worker.RunWorkerAsync();
            }
        }

        private void Download()
        {
            IniFile serverIni = GetServerIni();
            Version serverVer = new Version(serverIni.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
            string address = serverIni.Sections[Strings.IniName.ServerRepository.Section].Keys[Strings.IniName.ServerRepository.Key].Value;
            Uri url = new Uri(new Uri(address), Strings.IniName.ServerRepository.UpdateRepository);

            IniFile clientIni = new IniFile();
            clientIni.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer));
            Version clientVer = new Version(clientIni.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);

            if (clientVer < serverVer)
            {
                this.RTVersion = new RTPatchVersion(clientVer);
                string RTPFileName = this.RTVersion.ToString(false);
                Client.DownloadFileAsync(new Uri(url, RTPFileName), Path.Combine(UserSettings.GamePath, RTPFileName));
            }
            else
            {
                this.RTPatchCompleted?.Invoke(null, new RTPatchCompletedEventArgs(this.RTVersion, this.Caller));
            }
        }

        private static IniFile GetServerIni()
        {
            using (var client = new WebClient())
            using (var zippedFile = new TempFile())
            {
                client.DownloadFile(Urls.SoulworkerSettingsHome + Strings.IniName.ServerVer + ".zip", zippedFile.Path);

                using (var file = new TempFile())
                {
                    using (ZipFile zip = ZipFile.Read(zippedFile.Path))
                    {
                        ZipEntry entry = zip[0];
                        entry.FileName = Path.GetFileName(file.Path);
                        entry.Extract(Path.GetDirectoryName(file.Path), ExtractExistingFileAction.OverwriteSilently);
                    }

                    IniFile ini = new IniFile(new IniOptions
                    {
                        Encoding = Encoding.Unicode
                    });
                    ini.Load(file.Path);

                    return ini;
                }
            }
        }

        private uint Apply(string directory, string diffFilePath)
        {
            this.CurrentLogFile = Strings.FileName.RTPatchLog + Path.GetFileNameWithoutExtension(diffFilePath);
            File.WriteAllText(CurrentLogFile, string.Empty);

            string command = $"/u /nos \"{directory}\" \"{diffFilePath}\"";
            RTPatchCallback func = new RTPatchCallback(RTPatchMessage);
            uint result = RTPatchApply(command, func, true);
            File.AppendAllText(CurrentLogFile, $"Result=[{result}]");

            return result;
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
                case 12u:
                    {
                        string arg = Marshal.PtrToStringAnsi(ptr);
                        File.AppendAllText(this.CurrentLogFile, arg);

                        break;
                    }
                case 14u:
                case 17u:
                case 18u:
                    return null;
                case 5u:
                    {
                        int percentage = (Marshal.ReadInt32(ptr) & 0xFFFF) * 100 / 0x8000;
                        this.RTPatchProgressChanged?.Invoke(null, new RTPatchProgressChangedEventArgs(this.FileNumber, this.FileCount, this.FileName, percentage));
                        File.AppendAllText(this.CurrentLogFile, $"[{percentage}%] ");

                        break;
                    }
                case 6u:
                    {
                        int fileCount = Marshal.ReadInt32(ptr);
                        this.FileCount = fileCount;
                        File.AppendAllText(this.CurrentLogFile, $"File Count=[{fileCount}]");

                        break;
                    }
                case 7u:
                    {
                        string fileName = Marshal.PtrToStringAnsi(ptr);
                        this.FileNumber++;
                        this.FileName = fileName;
                        File.AppendAllText(this.CurrentLogFile, $"Patching=[{fileName}]");

                        break;
                    }
                case 32u:
                case 33u:
                    {
                        int[] numbers = new int[2];
                        Marshal.Copy(ptr, numbers, 0, 2);
                        File.AppendAllText(this.CurrentLogFile, $"number1=[{numbers[0]}] number2=[{numbers[1]}]");

                        break;
                    }
                default: break;
            }
            return "";
        }

        public void Cancel()
        {
            this.Client.CancelAsync();
            this.Worker.CancelAsync();
        }

        public void Run(string caller)
        {
            if (this.Client.IsBusy || this.Worker.IsBusy)
                return;

            this.Caller = caller;
            this.Download();
        }
    }
}
