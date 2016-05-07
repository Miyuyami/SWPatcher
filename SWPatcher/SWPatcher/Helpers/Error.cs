using System;
using System.IO;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Helpers
{
    public static class Error
    {
        public static void Log(string message)
        {
            using (StreamWriter sw = new StreamWriter(Strings.FileName.Log, true))
                sw.Write(string.Format("--------------------\n{0}\n--------------------\n{1}\n", Strings.DateToString(DateTime.Now), message));
        }

        public static void Log(Exception ex)
        {
            using (StreamWriter sw = new StreamWriter(Strings.FileName.Log, true))
                sw.Write(string.Format("--------------------\n{0}\n--------------------\n{1}\n", Strings.DateToString(DateTime.Now), StackTraceExceptionParser(ex)));
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
            string result = ex.Message;
            if (!string.IsNullOrEmpty(ex.StackTrace))
                result += "\nStackTrace:\n" + ex.StackTrace;
            if (ex.InnerException != null)
            {
                result += "\n\n" + ex.InnerException.Message;
                if (!string.IsNullOrEmpty(ex.InnerException.StackTrace))
                    result += "\nInnerStackTrace:\n" + ex.InnerException.StackTrace;
            }
            return result;
        }
    }
}
