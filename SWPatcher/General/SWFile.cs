/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016 Miyu
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

namespace SWPatcher.General
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
