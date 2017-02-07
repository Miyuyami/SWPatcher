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

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Security.AccessControl;

namespace SWPatcher.Patching
{
    internal class XorFileStream : FileStream
    {
        internal byte XorByte { get; private set; }

        internal XorFileStream(string path, FileMode mode, byte xorByte) : base(path, mode)
        {
            this.XorByte = xorByte;
        }

        internal XorFileStream(string path, FileMode mode, FileAccess access, byte xorByte) : base(path, mode, access)
        {
            this.XorByte = xorByte;
        }

        internal XorFileStream(string path, FileMode mode, FileAccess access, FileShare share, byte xorByte) : base(path, mode, access, share)
        {
            this.XorByte = xorByte;
        }

        internal XorFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, byte xorByte) : base(path, mode, access, share, bufferSize)
        {
            this.XorByte = xorByte;
        }

        internal XorFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, byte xorByte) : base(path, mode, access, share, bufferSize, options)
        {
            this.XorByte = xorByte;
        }

        internal XorFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync, byte xorByte) : base(path, mode, access, share, bufferSize, useAsync)
        {
            this.XorByte = xorByte;
        }

        internal XorFileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, byte xorByte) : base(path, mode, rights, share, bufferSize, options)
        {
            this.XorByte = xorByte;
        }

        internal XorFileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity, byte xorByte) : base(path, mode, rights, share, bufferSize, options, fileSecurity)
        {
            this.XorByte = xorByte;
        }

        [Obsolete("This constructor has been deprecated. Please use new XorFileStream(SafeFileHandle handle, FileAccess access, byte xorByte) instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        internal XorFileStream(IntPtr handle, FileAccess access, byte xorByte) : this(handle, access, true, 4096, false, xorByte)
        {

        }

        [Obsolete("This constructor has been deprecated. Please use new XorFileStream(SafeFileHandle handle, FileAccess access, byte xorByte) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")]
        internal XorFileStream(IntPtr handle, FileAccess access, bool ownsHandle, byte xorByte) : this(handle, access, ownsHandle, 4096, false, xorByte)
        {

        }

        [Obsolete("This constructor has been deprecated. Please use new XorFileStream(SafeFileHandle handle, FileAccess access, int bufferSize, byte xorByte) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")]
        internal XorFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, byte xorByte) : this(handle, access, ownsHandle, bufferSize, false, xorByte)
        {

        }

        [Obsolete("This constructor has been deprecated. Please use new XorFileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync, byte xorByte) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")]
        internal XorFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync, byte xorByte) : this(new SafeFileHandle(handle, ownsHandle), access, bufferSize, isAsync, xorByte)
        {

        }

        internal XorFileStream(SafeFileHandle handle, FileAccess access, byte xorByte) : this(handle, access, 4096, false, xorByte)
        {

        }

        internal XorFileStream(SafeFileHandle handle, FileAccess access, int bufferSize, byte xorByte) : this(handle, access, bufferSize, false, xorByte)
        {

        }

        internal XorFileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync, byte xorByte) : base(handle, access, bufferSize, isAsync)
        {
            this.XorByte = xorByte;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = base.Read(buffer, offset, count);
            InternalXorArray(buffer, offset, count);

            return bytesRead;
        }

        public override int ReadByte()
        {
            return base.ReadByte() ^ this.XorByte;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            InternalXorArray(buffer, offset, count);
            base.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            value ^= this.XorByte;
            base.WriteByte(value);
        }

        private void InternalXorArray(byte[] buffer, int offset, int count)
        {
            int length = offset + count;
            for (int i = offset; i < length; i++)
            {
                buffer[i] ^= this.XorByte;
            }
        }
    }
}
