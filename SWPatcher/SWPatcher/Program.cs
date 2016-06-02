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
        private static string mutexId = String.Format("Global\\{{{0}}}", appGuid);
        private static Mutex mutex = null;

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
            if (IsAppAlreadyRunning())
            {
                SWPatcher.Helpers.MsgBox.Error("Multiple instances of the program are not allowed.\nMaybe it's hiding in your Windows's tray?");
                SWPatcher.Helpers.Error.Log("Multiple instances of the program are not allowed");

                return;
            }

            Directory.SetCurrentDirectory(SWPatcher.Helpers.GlobalVar.Paths.PatcherRoot);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.Run(new SWPatcher.Forms.MainForm());

            mutex.ReleaseMutex();
        }
    }
}
