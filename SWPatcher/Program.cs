/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016-2017 Miyu, Dramiel Leayal
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SWPatcher.Forms;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;

namespace SWPatcher
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Directory.SetCurrentDirectory(UserSettings.PatcherPath);
            Logger.Start();

            string[] args = Environment.GetCommandLineArgs();
            List<string> argsList = new List<string>(args);

            argsList.Insert(0, Thread.CurrentThread.ManagedThreadId.ToString());
            Logger.Debug(Methods.MethodFullName(System.Reflection.MethodBase.GetCurrentMethod(), argsList.ToArray()));

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            SingleInstanceController controller = new SingleInstanceController();
            controller.Run(args);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Critical(e.ExceptionObject as Exception);
            MsgBox.Error(Methods.ExeptionParser(e.ExceptionObject as Exception) + "\r\n\r\nApplication will now exit.");

            Application.Exit();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Critical(e.Exception);
            MsgBox.Error(Methods.ExeptionParser(e.Exception) + "\r\n\r\nApplication will now exit.");

            Application.Exit();
        }

        private class SingleInstanceController : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
        {
            internal SingleInstanceController()
            {
                this.IsSingleInstance = true;
                this.StartupNextInstance += new Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventHandler(this.SingleInstanceController_StartupNextInstance);
            }

            private void SingleInstanceController_StartupNextInstance(object sender, Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs e)
            {
                MainForm mainForm = this.MainForm as Forms.MainForm;
                mainForm.RestoreFromTray();
            }

            protected override void OnCreateMainForm()
            {
                this.MainForm = new Forms.MainForm();
            }
        }
    }
}
