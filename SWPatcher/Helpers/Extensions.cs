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
        /// <summary>
        /// Determines whether a specified element is part of a sequence by using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <returns>true if the element is located in the specified sequence; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">source is null.</exception>
        internal static bool In<T>(this T value, params T[] source)
        {
            return source.Contains(value);
        }

        /// <summary>
        /// Converts the byte array to a string sequence using the specified encoding.
        /// </summary>
        /// <param name="byteArray">The array to be converted.</param>
        /// <param name="encoding">The encoding used for converting.</param>
        /// <returns>The byte array converted to a string sequence using the specified encoding.</returns>
        /// <exception cref="System.ArgumentNullException">byteArray or encoding is null.</exception>
        /// <exception cref="System.OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
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
