/*
 * This file is part of Closers Patcher.
 * Copyright (C) 2016-2017 Miyu, Dramiel Leayal
 * 
 * Closers Patcher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Closers Patcher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Closers Patcher. If not, see <http://www.gnu.org/licenses/>.
 */

using SWPatcher.Helpers.GlobalVariables;
using System;

namespace SWPatcher.General
{
    internal class Language
    {
        internal string Id { get; }
        internal string Name { get; }
        internal DateTime LastUpdate { get; }
        internal string ApplyingRegionId { get; }
        internal string Path => System.IO.Path.Combine(this.ApplyingRegionId, this.Name);
        internal string BackupPath => System.IO.Path.Combine(this.ApplyingRegionId, Strings.FolderName.Backup);

        internal Language(string id)
        {
            this.Id = id;
        }

        private Language(string id, string name, string applyingRegionId)
        {
            this.Id = id;
            this.Name = name;
            this.ApplyingRegionId = applyingRegionId;
        }

        internal Language(string id, string name, DateTime lastUpdate, string applyingRegionId)
        {
            this.Id = id;
            this.Name = name;
            this.LastUpdate = lastUpdate;
            this.ApplyingRegionId = applyingRegionId;
        }

        internal static Language BackupLanguage(string applyingRegionId)
        {
            return new Language("bak", Strings.FolderName.Backup, applyingRegionId);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            Language language = obj as Language;
            return this.Id == language.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
