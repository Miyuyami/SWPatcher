using System.IO;

namespace SWPatcher.Classes
{
    class XORprocessor
    {
        // 0x55

        byte bKey;
        public XORprocessor(byte bkey)
        {
            this.bKey = bkey;
        }

        public XORprocessor(string sKey)
        {
            this.bKey = System.Convert.ToByte(System.Convert.ToInt32(sKey, 16));
        }

        private byte[] processData(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)(data[i] ^ this.bKey);
            return data;
        }

        public void processFile(string inputFile, string outputFile)
        {
            using (FileStream inputstream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream outputstream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                this.processFile(inputstream, outputstream);
            }
        }

        public void processFile(System.IO.FileStream inputStream, System.IO.FileStream outputStream)
        {
            
            byte[] readBuffer = new byte[1025];
            int readByte = inputStream.Read(readBuffer, 0, 1024);
            while (readByte > 0)
            {
                readBuffer = processData(readBuffer);
                outputStream.Write(readBuffer, 0, 1024);
                readByte = inputStream.Read(readBuffer, 0, 1024);
                outputStream.Flush();
            }
            readBuffer = null;
        }
    }
}
