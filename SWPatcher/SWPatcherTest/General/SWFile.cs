namespace SWPatcherTEST.General
{
    public class SWFile
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string PathA { get; private set; }
        public string PathD { get; private set; }
        public string Format { get; private set; }

        public SWFile(string name, string path, string pathA, string pathD, string format)
        {
            this.Name = name;
            this.Path = path;
            this.PathA = pathA;
            this.PathD = pathD;
            this.Format = format;
        }
    }
}
