using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SWPatcher.Helpers.Steam
{
    public class SteamManifest : SteamManifestSection
    {
        private const string SectionStart = "{";
        private const string SectionEnd = "}";

        public SteamManifest(string name, Dictionary<string, SteamManifestElement> elements = null) : base(name, elements)
        {

        }

        public static SteamManifest Load(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string nameLine = sr.ReadLine();
                SteamManifestSection mainSection = ReadSection(nameLine, sr);

                return new SteamManifest(mainSection.Name, mainSection.Elements);
            }
        }

        private static SteamManifestSection ReadSection(string nameLine, StreamReader sr)
        {
            string name = nameLine.TrimStart('"').TrimEnd('"');
            Dictionary<string, SteamManifestElement> elements = new Dictionary<string, SteamManifestElement>();

            string sectionStart = sr.ReadLine();
            sectionStart = sectionStart.TrimStart('\t').TrimEnd('\t');
            if (sectionStart != SectionStart)
            {
                throw new Exception($"Expected section start \"{SectionStart}\" but got \"{sectionStart}\".");
            }

            string line;
            do
            {
                line = sr.ReadLine();
                line = line.TrimStart('\t').TrimEnd('\t');
                if (line == SectionEnd)
                {
                    break;
                }

                string[] splitLine = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                switch (splitLine.Length)
                {
                    case 1:
                        elements.Add(splitLine[0].TrimStart('"').TrimEnd('"'), ReadSection(splitLine[0].TrimStart('"').TrimEnd('"'), sr));

                        break;
                    case 2:
                        elements.Add(splitLine[0].TrimStart('"').TrimEnd('"'), new SteamManifestEntry(splitLine[0].TrimStart('"').TrimEnd('"'), splitLine[1].TrimStart('"').TrimEnd('"')));

                        break;
                    default:
                        throw new Exception($"Unexpected line split length \"{splitLine.Length}\"");
                }
            }
            while (!sr.EndOfStream);

            if (line != SectionEnd)
            {
                throw new Exception($"Unexpected end of file.");
            }

            return new SteamManifestSection(name, elements);
        }

        public static async Task<SteamManifest> LoadAsync(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string nameLine = await sr.ReadLineAsync().ConfigureAwait(false);
                SteamManifestSection mainSection = await ReadSectionAsync(nameLine, sr);

                return new SteamManifest(mainSection.Name, mainSection.Elements);
            }
        }

        private static async Task<SteamManifestSection> ReadSectionAsync(string nameLine, StreamReader sr)
        {
            string name = nameLine.TrimStart('"').TrimEnd('"');
            Dictionary<string, SteamManifestElement> elements = new Dictionary<string, SteamManifestElement>();

            string sectionStart = await sr.ReadLineAsync().ConfigureAwait(false);
            sectionStart = sectionStart.TrimStart('\t').TrimEnd('\t');
            if (sectionStart != SectionStart)
            {
                throw new Exception($"Expected section start \"{SectionStart}\" but got \"{sectionStart}\".");
            }

            string line;
            do
            {
                line = await sr.ReadLineAsync().ConfigureAwait(false);
                line = line.TrimStart('\t').TrimEnd('\t');
                if (line == SectionEnd)
                {
                    break;
                }

                string[] splitLine = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                switch (splitLine.Length)
                {
                    case 1:
                        elements.Add(splitLine[0], await ReadSectionAsync(splitLine[0], sr));

                        break;
                    case 2:
                        elements.Add(splitLine[0], new SteamManifestEntry(splitLine[0], splitLine[1]));

                        break;
                    default:
                        throw new Exception($"Unexpected line split length \"{splitLine.Length}\"");
                }
            }
            while (!sr.EndOfStream);

            if (line != SectionEnd)
            {
                throw new Exception($"Unexpected end of file.");
            }

            return new SteamManifestSection(name, elements);
        }
    }
}
