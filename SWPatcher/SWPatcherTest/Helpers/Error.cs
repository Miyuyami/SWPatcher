using System;
using System.IO;
using SWPatcherTest.Helpers.GlobalVar;

namespace SWPatcherTest.Helpers
{
    public static class Error
    {
        public static void Log(string message)
        {
            using (StreamWriter sw = new StreamWriter(Strings.FileName.Log, true))
                sw.Write(String.Format("--------------------{2}{0}{2}--------------------{2}{1}{2}", Methods.DateToString(DateTime.UtcNow), message, System.Environment.NewLine));
        }

        public static void Log(Exception ex)
        {
            using (StreamWriter sw = new StreamWriter(Strings.FileName.Log, true))
                sw.Write(String.Format("--------------------{2}{0}{2}--------------------{2}{1}{2}", Methods.DateToString(DateTime.UtcNow), StackTraceExceptionParser(ex), System.Environment.NewLine));
        }

        public static string ExeptionParser(Exception ex)
        {
            string result = ex.Message;
            if (ex.InnerException != null)
                result += "\n\n" + ex.InnerException.Message;
            return result;
        }

        private static string StackTraceExceptionParser(Exception ex)
        {
            return ex.ToString();
        }
    }
}
