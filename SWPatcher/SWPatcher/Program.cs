using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher
{
    static class Program
    {
        /*
        private static string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
        private static string mutexId = String.Format("Global\\{{{0}}}", appGuid);
        private static Mutex mutex = null;
        */
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            /*
            if (IsAppAlreadyRunning())
                throw new Exception("Multiple instances of the program are not allowed.\nMaybe it's hiding in your Windows's tray?");
            */
            if (!Directory.Exists(Paths.PatcherRoot))
                Paths.PatcherRoot = "";
            Directory.SetCurrentDirectory(Paths.PatcherRoot);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //Application.Run(new SWPatcher.Forms.MainForm());
            var controller = new SingleInstanceController();
            controller.Run(Environment.GetCommandLineArgs());

            //mutex.ReleaseMutex();
        }
        /*
        private static bool IsAppAlreadyRunning()
        {
            bool createdNew;
            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            mutex = new Mutex(false, mutexId, out createdNew, securitySettings);

            return !mutex.WaitOne(TimeSpan.Zero, true);
        }
        */
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Error.Log(e.ExceptionObject as Exception);
            MsgBox.Error(Error.ExeptionParser(e.ExceptionObject as Exception));

            Application.Exit();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Error.Log(e.Exception);
            MsgBox.Error(Error.ExeptionParser(e.Exception));

            Application.Exit();
        }

        private class SingleInstanceController : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
        {
            public SingleInstanceController()
            {
                this.IsSingleInstance = true;
                this.StartupNextInstance += new Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventHandler(SingleInstanceController_StartupNextInstance);
            }

            private void SingleInstanceController_StartupNextInstance(object sender, Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs e)
            {
                var mainForm = this.MainForm as SWPatcher.Forms.MainForm;
                mainForm.RestoreFromTray();
            }

            protected override void OnCreateMainForm()
            {
                this.MainForm = new SWPatcher.Forms.MainForm();
            }
        }
    }
}
