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

using System.IO;

namespace SWPatcher.Patching
{
    internal class XorMemoryStream : MemoryStream
    {
        internal byte XorByte { get; private set; }

        internal XorMemoryStream(byte xorByte) : this(0, xorByte)
        {

        }

        internal XorMemoryStream(int capacity, byte xorByte) : base(capacity)
        {
            this.XorByte = xorByte;
        }

        internal XorMemoryStream(byte[] buffer, byte xorByte) : this(buffer, true, xorByte)
        {

        }

        internal XorMemoryStream(byte[] buffer, bool writable, byte xorByte) : base(buffer, writable)
        {
            this.XorByte = xorByte;
        }

        internal XorMemoryStream(byte[] buffer, int index, int count, byte xorByte) : this(buffer, index, count, true, false, xorByte)
        {

        }

        internal XorMemoryStream(byte[] buffer, int index, int count, bool writable, byte xorByte) : this(buffer, index, count, writable, false, xorByte)
        {

        }

        internal XorMemoryStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible, byte xorByte) : base(buffer, index, count, writable, publiclyVisible)
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
