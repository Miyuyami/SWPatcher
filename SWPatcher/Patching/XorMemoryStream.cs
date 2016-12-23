using System.IO;

namespace SWPatcher.Patching
{
    public class XorMemoryStream : MemoryStream
    {
        public byte XorByte { get; private set; }

        public XorMemoryStream(byte xorByte) : this(0, xorByte)
        {

        }

        public XorMemoryStream(int capacity, byte xorByte) : base(capacity)
        {
            this.XorByte = xorByte;
        }

        public XorMemoryStream(byte[] buffer, byte xorByte) : this(buffer, true, xorByte)
        {

        }

        public XorMemoryStream(byte[] buffer, bool writable, byte xorByte) : this(buffer, 0, buffer.Length, writable, true, xorByte)
        {
            
        }

        public XorMemoryStream(byte[] buffer, int index, int count, byte xorByte) : this(buffer, index, count, true, false, xorByte)
        {

        }

        public XorMemoryStream(byte[] buffer, int index, int count, bool writable, byte xorByte) : this(buffer, index, count, writable, true, xorByte)
        {

        }

        public XorMemoryStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible, byte xorByte) : base(buffer, index, count, writable, publiclyVisible)
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
