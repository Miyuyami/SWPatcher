/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016 Miyu
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
            string[] messages = message.Split('\n');
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
                foreach (LogMessage msg in _messages.GetConsumingEnumerable())
                {
                    Log(msg.DateTime, msg.LogMode, msg.Message);
                }
            })
            {
                IsBackground = true
            };
            thread.Start();

            Logger.Debug(Methods.MethodFullName("Logger.Run", thread.ManagedThreadId.ToString()));
        }
    }
}
