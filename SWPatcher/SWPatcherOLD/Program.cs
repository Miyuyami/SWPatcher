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

namespace SWPatcher
{
    static class Program
    {
        private static string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
        private static string mutexId = string.Format("Global\\{{{0}}}", appGuid);
        private static Mutex mutex = null;

        /*
        private static bool IsUserAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        */
        private static bool IsAppAlreadyRunning()
        {
            bool createdNew;
            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            mutex = new Mutex(false, mutexId, out createdNew, securitySettings);
            return !mutex.WaitOne(TimeSpan.Zero, true);
        }

        private static string getLatestVersionFile()
        {
            string result = string.Empty;
            using (WebClient webClient = new WebClient())
            {
                webClient.BaseAddress = new Uri("https://raw.githubusercontent.com/Miyuyami/SWHQPatcher/master/").AbsoluteUri;
                for (short i = 0; i < 2; i++)
                {
                    result = webClient.DownloadString("version");
                    if (string.IsNullOrEmpty(result) == false)
                        break;
                }
            }
            return result;
        }

        private static bool NewPatcherUpdateAvailable()
        {
            string patcherVersionFile = getLatestVersionFile();
            if (string.IsNullOrEmpty(patcherVersionFile) == true) // in case user disconnected (or reconnecting) or loss packet too much.
            {
                MessageBox.Show("Failed to get latest version.");
                return false;
            }
            string[] lines = patcherVersionFile.Split('\n');
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Version readVersion = new Version(lines[0]);
            if (currentVersion.CompareTo(readVersion) < 0)
            {
                DialogResult newVersionDialog = MessageBox.Show("There is a new patcher version available!\n\nYes - Application will close and redirect you to the patcher website.\nNo - Ignore", "New Patcher Version Available!", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                if (newVersionDialog == DialogResult.Yes)
                {
                    Process.Start(lines[1]);
                    return true;
                }
                else
                {
                    DialogResult newVersionDialog2 = MessageBox.Show("Are you sure you want to ignore the update?\nIt might cause unknown problems!", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
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
            /*
            if (!IsUserAdministrator())
            {
                MessageBox.Show("You must run this application as administrator.", "Administrator rights", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                return;
            }
            */
            if (IsAppAlreadyRunning())
                return;
            if (NewPatcherUpdateAvailable())
                return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            mutex.ReleaseMutex();
        }
    }
}
