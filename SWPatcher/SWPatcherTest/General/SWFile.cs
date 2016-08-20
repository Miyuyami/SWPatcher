namespace SWPatcherTEST.General
{
    public class SWFile
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string PathA { get; private set; }
        public string PathD { get; private set; }
        public string Format { get; private set; }
        public string Password { get; private set; }

        public SWFile(string name, string path, string pathA, string pathD, string format, string password)
        {
            this.Name = name;
            this.Path = path;
            this.PathA = pathA;
            this.PathD = pathD;
            this.Format = format;
            this.Password = password;
        }

        public SWFile(string name, string path, string pathA, string pathD, string format) : this(name, path, pathA, pathD, format, string.Empty) { }
    }
}
