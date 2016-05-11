using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using SWPatcher.Forms;

namespace SWPatcher
{
    static class Program
    {
        private static string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
        private static string mutexId = string.Format("Global\\{{{0}}}", appGuid);
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
                return;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            Directory.SetCurrentDirectory(SWPatcher.Helpers.GlobalVar.Paths.PatcherRoot);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.Run(new MainForm());
            mutex.ReleaseMutex();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            byte[] bytes = null;
            string resourceName = "SWPatcher.Ionic.Zip.dll";
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using (var stream = currentAssembly.GetManifestResourceStream(resourceName))
            {
                bytes = new byte[(int)stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
            }

            return Assembly.Load(bytes);
        }
    }
}
