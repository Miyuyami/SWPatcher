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

namespace SWPatcher.General
{
    internal class ResxLanguage
    {
        internal string Language { get; private set; }
        internal string Code { get; private set; }

        internal ResxLanguage(string language, string code)
        {
            this.Language = language;
            this.Code = code;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            ResxLanguage language = obj as ResxLanguage;
            return this.Language == language.Language;
        }

        public override int GetHashCode()
        {
            return this.Language.GetHashCode();
        }

        public override string ToString()
        {
            return this.Language;
        }
    }
}
