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

namespace SWPatcher.RTPatch
{
    class RTPatcherDownloadProgressChangedEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public int Progress { get; private set; }
        public string DownloadSpeed { get; private set; }

        public RTPatcherDownloadProgressChangedEventArgs(string fileName, int progress, long bytesPerSecond)
        {
            this.FileName = fileName;
            this.Progress = progress;
            this.DownloadSpeed = this.FormatDownloadSpeed(bytesPerSecond);
        }

        private string FormatDownloadSpeed(long bytesPerSecond)
        {
            string result;

            if (bytesPerSecond < 1000L)
            {
                result = $"{bytesPerSecond} B/s";
            }
            else if (bytesPerSecond < 1000000L)
            {
                result = $"{bytesPerSecond / 1000d:F3} KB/s";
            }
            else if (bytesPerSecond < 1000000000L)
            {
                result = $"{bytesPerSecond / 1000000d:F3} MB/s";
            }
            else
            {
                result = $"{bytesPerSecond / 1000000000d:F3} GB/s";
            }


            return result;
        }
    }
}
