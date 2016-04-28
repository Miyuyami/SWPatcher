using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

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

        [STAThread]
        static void Main()
        {
            /*IniReader r = new IniReader(@"C:\Users\Miyu\Documents\GitHub\SWHQPatcher\TranslationPackData.ini");
            foreach (var v in r.GetSectionNames())
            {
                r.Section = v.ToString();
                MessageBox.Show(Path.Combine(r.ReadString("path"), r.ReadString("format")) + v);
            }
            DateTime dt = DateTime.ParseExact("22/Apr/2016 7:00 PM", "dd/MMM/yyyy h:mm tt", CultureInfo.InvariantCulture);
            MessageBox.Show(dt.ToString("dd MMMM yyyy h:mm tt"));*/
            if (IsAppAlreadyRunning())
                return;
            if (!IsUserAdministrator())
            {
                SWPatcher.Helpers.MsgBox.Default("You must run this application as administrator.", "Administrator rights", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Directory.SetCurrentDirectory(SWPatcher.Helpers.GlobalVars.Paths.PatcherRoot);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            mutex.ReleaseMutex();
        }
    }
}
