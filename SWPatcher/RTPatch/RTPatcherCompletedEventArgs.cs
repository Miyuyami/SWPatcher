/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2017 Miyu
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

using SWPatcher.General;
using System;

namespace SWPatcher.RTPatch
{
    internal class RTPatcherCompletedEventArgs : EventArgs
    {
        internal bool Cancelled { get; private set; }
        internal Exception Error { get; private set; }
        internal Language Language { get; private set; }

        internal RTPatcherCompletedEventArgs(bool cancelled, Exception error, Language language)
        {
            this.Cancelled = cancelled;
            this.Error = error;
            this.Language = language;
        }
    }
}
