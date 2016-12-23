using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWPatcher.Helpers
{
    internal static class Extensions
    {
        internal static bool In<T>(this T obj, params T[] values)
        {
            return values.Contains(obj);
        }

        internal static string[] ToStringArray(this byte[] byteArray, Encoding encoding)
        {
            using (var ms = new MemoryStream(byteArray))
            using (var reader = new StreamReader(ms, encoding))
            {
                List<string> result = new List<string>();
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(line);
                }

                return result.ToArray();
            }
        }
    }
}
