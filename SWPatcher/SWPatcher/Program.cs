using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Net;
using System.Diagnostics;
using System.IO;
using SWPatcher.Helpers;
using SWPatcher.Components.Downloading;

namespace SWPatcher
{
    static class Program
    {
        private static string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
        private static string mutexId = string.Format("Global\\{{{0}}}", appGuid);
        private static Mutex mutex = null;

        private static bool IsUserAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static bool IsAppAlreadyRunning()
        {
            bool createdNew;
            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            mutex = new Mutex(false, mutexId, out createdNew, securitySettings);
            return !mutex.WaitOne(TimeSpan.Zero, true);
        }

        private static bool NewPatcherUpdateAvailable()
        {
            string patcherVersionFile = StringDownloader.DownloadString(new Uri(Uris.PatcherGitHubHome, Strings.FileNames.PatcherVersion));
            if (String.IsNullOrEmpty(patcherVersionFile))
                return true;
            string[] lines = patcherVersionFile.Split('\n');
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Version readVersion = new Version(lines[0]);
            if (currentVersion.CompareTo(readVersion) < 0)
            {
                DialogResult newVersionDialog = MsgBox.Question("There is a new patcher version available!\n\nYes - Application will close and redirect you to the patcher website.\nNo - Ignore");
                if (newVersionDialog == DialogResult.Yes)
                {
                    Process.Start(lines[1]);
                    return true;
                }
                else
                {
                    DialogResult newVersionDialog2 = MsgBox.Question("Are you sure you want to ignore the update?\nIt might cause unknown problems!");
                    if (newVersionDialog2 == DialogResult.No)
                    {
                        Process.Start(lines[1]);
                        return true;
                    }
                }
            }
            return false;
        }

        [STAThread]
        static void Main()
        {
            if (!IsUserAdministrator())
            {
                MsgBox.Default("You must run this application as administrator.", "Administrator rights", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (IsAppAlreadyRunning())
                return;
            if (NewPatcherUpdateAvailable())
                return;
            Directory.SetCurrentDirectory(SWPatcher.Helpers.Paths.PatcherRoot);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            mutex.ReleaseMutex();
        }
    }
}
