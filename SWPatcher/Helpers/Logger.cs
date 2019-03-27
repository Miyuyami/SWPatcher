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

#define ENCRYPT_LOG

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using SWPatcher.Helpers.GlobalVariables;

namespace SWPatcher.Helpers
{
    internal static class Logger
    {
        public enum LogType : byte
        {
            Information,
            Debug,
            Error,
            Critical,
        }

        private class LogMessage
        {
			internal DateTime DateTime;
            internal LogType LogType;
            internal string Message;
        }

        private static readonly BlockingCollection<LogMessage> _messages = new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>());
        private const string _dateFormat = "dd/MM/yyyy HH:mm:ss:fffffff";
        private const string _logFormat = "{0} - {1,-5} {2}\n";
        private static bool _running;

        internal static void Log(string message, LogType logType)
        {
            _messages.Add(new LogMessage
            {
				DateTime = DateTime.UtcNow,
                LogType = logType,
                Message = message,
            });
        }

        internal static void Info(string message)
        {
            Log(message, LogType.Information);
        }

        internal static void Debug(string message)
        {
            Log(message, LogType.Debug);
        }

        internal static void Error(Exception exception)
        {
            Error(exception.ToString());
        }

        internal static void Error(string message)
        {
            Log(message, LogType.Error);
        }

        internal static void Critical(Exception exception)
        {
            Critical(exception.ToString());
        }

        internal static void Critical(string message)
        {
            Log(message, LogType.Critical);
        }

        private static void WriteLog(DateTime dateTime, string logMode, string message)
        {
            string[] messages = message.Split('\n');
            for (int i = 1; i < messages.Length; i++)
            {
                messages[i] = messages[i].Trim();
                messages[i] = messages[i].PadLeft(messages[i].Length + 9, '\t');
            }

            message = String.Join("\n", messages);
            string logEntry = String.Format(_logFormat, dateTime.ToString(_dateFormat), logMode, message);
#if DEBUG && !ENCRYPT_LOG
            File.AppendAllText(Strings.FileName.Log, logEntry);
#else
            #region Silly "Encryption"
            byte[] messageBytes = System.Text.Encoding.Unicode.GetBytes(logEntry);
            ushort[] messageShorts = new ushort[messageBytes.Length / 2];

            using (BinaryWriter bw = new BinaryWriter(File.Open(Strings.FileName.Log, FileMode.Append, FileAccess.Write)))
            {
                for (int i = 0; i < messageBytes.Length; i += 2)
                {
                    int index = i / 2;

                    messageShorts[index] = (ushort)((messageBytes[i] << 8) + messageBytes[i + 1]);
                    messageShorts[index] ^= 0xAB00;
                    bw.Write(messageShorts[index]);
                }
            }
            #endregion
#endif
        }

        internal static string ExeptionParser(Exception ex)
        {
            string result = ex.Message;
            if (ex.InnerException != null)
            {
                result += "\n\n" + ex.InnerException.Message;
            }

            return result;
        }

        private static void ConsumeLogs()
        {
            foreach (LogMessage msg in _messages.GetConsumingEnumerable())
            {
                switch (msg.LogType)
                {
                    case LogType.Information:
                        WriteLog(msg.DateTime, "INFO", msg.Message);

                        break;
                    case LogType.Debug:
                        WriteLog(msg.DateTime, "DEBUG", msg.Message);

                        break;
                    case LogType.Error:
                        WriteLog(msg.DateTime, "ERROR", msg.Message);

                        break;
                    case LogType.Critical:
                        WriteLog(msg.DateTime, "CRIT", msg.Message);

                        break;
                }
            }
        }

        internal static void Start()
        {
            if (_running)
            {
                return;
            }
            else
            {
                _running = true;
            }

            Thread thread = new Thread(ConsumeLogs)
            {
                IsBackground = true
            };
            thread.Start();

            Debug(Methods.MethodFullName("Logger.Run", thread.ManagedThreadId.ToString()));
        }
    }
}
