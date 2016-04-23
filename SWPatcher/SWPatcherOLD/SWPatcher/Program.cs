using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SWPatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            System.Reflection.Assembly CurrentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Runtime.InteropServices.GuidAttribute theObj = (System.Runtime.InteropServices.GuidAttribute)(CurrentAssembly.GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), true)[0]);
            bool instanceCountOne = false;
            using (System.Threading.Mutex mtex = new System.Threading.Mutex(true, "Global\\" + theObj.Value, out instanceCountOne))
            {
                if (instanceCountOne)
                {
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    Application.ApplicationExit += new EventHandler(MyApplication_OnExit);
                    Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(MyApplication_ThreadException);
                    for (short i = 1; i <= 3; i++)
                    {
                        try
                        {
                            /*i think for some reasons, user internet may get unstable and get stuck on waiting request.
                             *So we might need TimeOut from HTTP Request....WebClient can but for simple, HTTP serve its purpose ...
                             */
                            System.Collections.Generic.List<string> responseArray = new List<string>();
                            System.Net.HttpWebRequest TheRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("https://raw.githubusercontent.com/Miyuyami/SWHQPatcher/master/version");
                            TheRequest.Timeout = 5000;
                            TheRequest.UserAgent = "SWHQ";
                            TheRequest.AllowAutoRedirect = true;
                            TheRequest.Proxy = null;
                            System.Net.HttpWebResponse TheResponse = (System.Net.HttpWebResponse)TheRequest.GetResponse();
                            if (TheResponse != null)
                            {

                                using (System.IO.Stream TheResponseStream = TheResponse.GetResponseStream())
                                using (System.IO.StreamReader theStreamReader = new System.IO.StreamReader(TheResponseStream))
                                    while (theStreamReader.EndOfStream == false)
                                    {
                                        responseArray.Add(theStreamReader.ReadLine());
                                    }
                                TheResponse.Close();

                                System.Version TheVersionObj;
                                if (System.Version.TryParse(responseArray[0], out TheVersionObj) == false)
                                    TheVersionObj = new System.Version("0.0.0.1");
                                System.Version CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                                int compareResult = TheVersionObj.CompareTo(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                                if (compareResult > 0)
                                {
                                    // Ask user to update or skip. I think i should make a Update Changes box for this .... ?
                                    if (MessageBox.Show("Latest version: " + TheVersionObj.ToString() + "\nCurrent version: " + CurrentVersion.ToString() + "\nDo you want to go to SWHQ ?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                    {
                                        string defaultBrowser = GetDefaultBrowserPath();
                                        if (string.IsNullOrWhiteSpace(defaultBrowser) == false)
                                        {
                                            System.Diagnostics.Process.Start(defaultBrowser, responseArray[1]);
                                            Application.Exit();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Unknown Error: Cannot found default browser.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            Application.Run(new Form1());
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("*Insert the skip song*", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        Application.Run(new Form1());
                                    }
                                }
                                else if (compareResult == 0)
                                    Application.Run(new Form1());
                                else
                                    Application.Run(new Form1());
                                TheVersionObj = null; // i know this is not necessary, but i think it attract the GC.;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                    MessageBox.Show("It's running .......");
                mtex.ReleaseMutex();
            }
        }

        static void MyApplication_OnExit(object sender, EventArgs e)
        {

        }

        static void MyApplication_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {

        }
        
        private static string CleanifyBrowserPath(string p)
        {
            string[] url = p.Split('"');
            string clean = url[1];
            return clean;
        }

        public static string GetDefaultBrowserPath()
        {
            string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
            string browserPathKey = @"$BROWSER$\shell\open\command";
            Microsoft.Win32.RegistryKey userChoiceKey = null;
            string browserPath = "";
            try
            {
                userChoiceKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);
                if (userChoiceKey == null)
                {
                    var browserKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);
                    if (browserKey == null)
                        browserKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(urlAssociation, false);
                    var path = CleanifyBrowserPath(browserKey.GetValue(null) as string);
                    browserKey.Close();
                    return path;
                }
                else
                {
                    string progId = (userChoiceKey.GetValue("ProgId").ToString());
                    userChoiceKey.Close();
                    string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                    var kp = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
                    browserPath = CleanifyBrowserPath(kp.GetValue(null) as string);
                    kp.Close();
                    return browserPath;
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
