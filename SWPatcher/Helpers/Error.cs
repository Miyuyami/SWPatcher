using System;
using System.IO;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Helpers
{
    public static class Error
    {
        private static string LogFormat = "--------------------{2}{0}{2}--------------------{2}{1}{2}";

        public static void Log(string message)
        {
            using (StreamWriter sw = new StreamWriter(Strings.FileName.Log, true))
                sw.Write(String.Format(LogFormat, Methods.DateToString(DateTime.UtcNow), message, Environment.NewLine));
        }

        public static void Log(Exception ex)
        {
            using (StreamWriter sw = new StreamWriter(Strings.FileName.Log, true))
                sw.Write(String.Format(LogFormat, Methods.DateToString(DateTime.UtcNow), StackTraceExceptionParser(ex), Environment.NewLine));
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
