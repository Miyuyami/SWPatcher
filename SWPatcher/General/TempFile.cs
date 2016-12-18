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

using System;

namespace SWPatcher.General
{
    public class TempFile : IDisposable
    {
        public string Path { get; private set; }
        private bool disposed = false;

        public TempFile()
            : this(System.IO.Path.GetTempFileName())
        {

        }
        public TempFile(string path)
        {
            this.Path = path;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing)
            {

            }
            if (this.Path != null)
            {
                try
                {
                    System.IO.File.Delete(this.Path);
                }
                catch
                {

                }
                this.Path = null;
            }
        }

        ~TempFile()
        {
            Dispose(false);
        }
    }
}
