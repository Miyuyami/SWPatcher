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
    internal class RTPatcherProgressChangedEventArgs : EventArgs
    {
        internal int FileNumber { get; private set; }
        internal int FileCount { get; private set; }
        internal string FileName { get; private set; }
        internal int Progress { get; private set; }

        internal RTPatcherProgressChangedEventArgs(int fileNumber, int fileCount, string fileName, int progress)
        {
            this.FileNumber = fileNumber;
            this.FileCount = fileCount;
            this.FileName = fileName;
            this.Progress = progress;
        }
    }
}
