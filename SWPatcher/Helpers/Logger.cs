using SWPatcher.Helpers.GlobalVariables;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace SWPatcher.Helpers
{
    public static class Logger
    {
        private class LogMessage
        {
            public DateTime DateTime;
            public string LogMode;
            public string Message;
        }

        private static readonly BlockingCollection<LogMessage> _messages = new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>());
        private const string LogFormat = "dd/MM/yyyy HH:mm:ss:fffffff - {0,-5} {1}\n";

        public static void Debug(string message)
        {
            _messages.Add(new LogMessage
            {
                DateTime = DateTime.UtcNow,
                LogMode = "DEBUG",
                Message = message
            });
        }

        public static void Info(string message)
        {
            _messages.Add(new LogMessage
            {
                DateTime = DateTime.UtcNow,
                LogMode = "INFO",
                Message = message
            });
        }

        public static void Critical(Exception exception)
        {
            Critical(exception.ToString());
        }

        public static void Critical(string message)
        {
            _messages.Add(new LogMessage
            {
                DateTime = DateTime.UtcNow,
                LogMode = "CRIT",
                Message = message
            });
        }

        public static void Error(Exception exception)
        {
            Error(exception.ToString());
        }

        public static void Error(string message)
        {
            _messages.Add(new LogMessage
            {
                DateTime = DateTime.UtcNow,
                LogMode = "ERROR",
                Message = message
            });
        }

        private static void Log(DateTime dateTime, string logMode, string message)
        {
            var messages = message.Split('\n');
            for (int i = 1; i < messages.Length; i++)
            {
                messages[i] = messages[i].Trim();
                messages[i] = messages[i].PadLeft(messages[i].Length + 9, '\t');
            }
            message = String.Join("\n", messages);
            message = String.Format(dateTime.ToString(LogFormat), logMode, message);

            byte[] messageBytes = Encoding.Unicode.GetBytes(message);
            ushort[] messageShorts = new ushort[messageBytes.Length / 2];

            using (var bw = new BinaryWriter(File.Open(Strings.FileName.Log, FileMode.Append, FileAccess.Write)))
            {
                for (int i = 0; i < messageBytes.Length; i += 2)
                {
                    int index = i / 2;

                    messageShorts[index] = (ushort)((messageBytes[i] << 8) + messageBytes[i + 1]);
                    messageShorts[index] ^= 0xAB00;
                    bw.Write(messageShorts[index]);
                }
            }
        }

        public static string ExeptionParser(Exception ex)
        {
            string result = ex.Message;
            if (ex.InnerException != null)
                result += "\n\n" + ex.InnerException.Message;
            return result;
        }

        public static void Run()
        {
            Thread thread = new Thread(() =>
            {
                foreach (var msg in _messages.GetConsumingEnumerable())
                {
                    Log(msg.DateTime, msg.LogMode, msg.Message);
                }
            });
            thread.IsBackground = true;
            thread.Start();

            Logger.Debug(Methods.MethodFullName("Logger.Run", thread.ManagedThreadId.ToString()));
        }
    }
}
