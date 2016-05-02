using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPatcher
{
    public class PatchData
    {
        string sFileName;
        string sTargetData;
        string sParam;
        string sInsidePath;

        public string FileName
        {
            get { return this.sFileName; }
        }

        public string InsidePath
        {
            get { return this.sInsidePath; }
        }

        public string TargetData
        {
            get { return this.sTargetData; }
        }
        public string Param
        {
            get { return this.sParam; }
            set { this.sParam = value; }
        }

        public PatchData(string vFileName, string vTargetData, string vInsidePath, string vParam)
        {
            this.sFileName = vFileName;
            this.sTargetData = vTargetData;
            this.sParam = vParam;
            this.sInsidePath = vInsidePath;
        }

        public PatchData(string vFileName, string vTargetData, string vInsidePath) : this(vFileName, vTargetData, vInsidePath, string.Empty) { }
        /*tb_achievement_script.txt data12
        5 4 4 len 2 len 2
        tb_booster_script.txt data12
        5 4 2 len 2 len 2 len 2
        tb_buff_script.txt data12
        5 4 2 len 2 len 2 len 2 len 2
        tb_cinema_string.txt data12
        5 4 2 len 2 len 2 len 2 len 2 len 2 len 2 len 2 len 2
        tb_item_script.txt data12
        5 4 4 len 2 len 2 len 2 len 2 len 2 len 2 1 1 1 1 1 len 2 len 2
        tb_monster_script.txt data12
        5 4 4 len 2
        tb_npc_script.txt data12
        5 4 4 len 2
        tb_quest_script.txt data12
        5 4 4 len 2 len 2 len 2 len 2 len 2
        tb_shop_string.txt data12
        5 4 4 len 2
        tb_skill_script.txt data12
        5 4 2 len 2 len 2 len 2 len 2 4 4 len 2
        tb_soul_metry_string.txt data12
        5 4 2 len 2
        tb_speech_string.txt data12
        5 4 4 len 2
        tb_systemmail.txt data12
        5 4 1 2 2 len 2 len 2 len 2 len 2
        tb_title_string.txt data12
        5 4 4 len 2 len 2 len 2
        tb_tooltip_string.txt data12
        5 4 4 len 2 len 2
        tb_ui_string.txt data12
        5 4 2 1 len 2
        Achievement.html data14
        AkashicCompose.html data14
        AkashicLiberate.html data14
        AkashicRecords.html data14
        AkashicSeal.html data14
        AkashicTrade.html data14
        ItemSocket.html data14 */
    }
}
