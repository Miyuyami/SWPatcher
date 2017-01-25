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

namespace SWPatcher.Helpers.GlobalVariables
{
    public static class Urls
    {
        public const string SoulworkerWebsite = "https://miyuyami.github.io/patcher.html";
#if DEBUG
        public static string PatcherGitHubHome = System.Environment.ExpandEnvironmentVariables(@"%userprofile%\Documents\GitHub\SWPatcher\");
        public static string TranslationGitHubHome = System.Environment.ExpandEnvironmentVariables(@"%userprofile%\Documents\GitHub\SoulWorkerHQTranslations\");
#else
        public const string PatcherGitHubHome = "https://raw.githubusercontent.com/Miyuyami/SWPatcher/master/";
        public const string TranslationGitHubHome = "https://raw.githubusercontent.com/Miyuyami/SoulWorkerHQTranslations/master/";
#endif
        public const string SoulworkerSettingsHome = "http://down.hangame.co.jp/jp/purple/plii/j_sw/";
        public const string SoulworkerJPHome = "http://soulworker.jp/";
        public const string SoulworkerKRHome = "http://soulworker.co.kr/";
        public static string SoulworkerHome
        {
            get
            {
                if (UserSettings.ClientRegion == 1)
                {
                    return SoulworkerKRHome;
                }

                return SoulworkerJPHome;
            }
        }
        public const string HangameLogin = "https://id.hangame.co.jp/login.nhn";
        public const string SoulworkerGameStart = "http://soulworker.hangame.co.jp/gamestart.nhn";
        public const string SoulworkerReactorGameStart = "http://soulworker.hangame.co.jp/reactor/gameStart.nhn";
        public const string SoulworkerReactor = "http://soulworker.hangame.co.jp/reactor/reactor.nhn";
        public const string SoulworkerRegistCheck = "http://soulworker.hangame.co.jp/reactor/registCheck.nhn";

        public const string SoulworkerKRAPI = "http://patchapi.onstove.com:80/apiv1/get_live_version";
        public const string StoveLogin = "https://member.onstove.com/auth/login/request";
        public const string SoulworkerKRGameStart = "http://soulworker.game.onstove.com/Game/GameStart";
    }
}
