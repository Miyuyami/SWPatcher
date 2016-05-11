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
            if (disposed)
                return;
            if (disposing)
            {

            }
            if (Path != null)
            {
                try
                {
                    System.IO.File.Delete(Path);
                }
                catch
                {

                }
                Path = null;
            }
        }

        ~TempFile()
        {
            Dispose(false);
        }
    }
}
