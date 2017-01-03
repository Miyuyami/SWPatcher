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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            using (var reader = new StreamReader(new MemoryStream(byteArray), encoding))
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
