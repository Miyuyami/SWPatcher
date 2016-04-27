namespace SWPatcher.Classes
{
    public class TranslationOption
    {
        /*
         * 0 = tb_achievement_script
         * 1 = tb_booster_script
         * 2 = tb_broach_set
         * 3 = tb_buff_script
         * 4 = tb_cinema_string
         * 5 = tb_item_script
         * 6 = tb_monster_script
         * 7 = tb_npc_script
         * 8 = tb_quest_script
         * 9 = tb_shop_string
         * 10 = tb_skill_script
         * 11 = tb_soul_metry_string
         * 12 = tb_speech_string
         * 13 = tb_systemmail
         * 14 = tb_title_string
         * 15 = tb_tooltip_string
         * 16 = tb_ui_string
         */
        bool[] privateValue;
        string[] privateHeader = { "Achievement", "Booster", "Broach", "Buff", "Cinema", "Item", "Monster", "NPC", "Quest", "Shop", "Skill", "SoulMetry", "Speech", "SystemMail", "Title", "Tooltip", "UI" };
        System.Text.StringBuilder theStringBuild;

        public TranslationOption(bool tb_achievement_script, bool tb_booster_script, bool tb_broach_set, bool tb_buff_script, bool tb_cinema_string, bool tb_item_script,
            bool tb_monster_script, bool tb_npc_script, bool tb_quest_script, bool tb_shop_string, bool tb_skill_script, bool tb_soul_metry_string, bool tb_speech_string,
            bool tb_systemmail, bool tb_title_string, bool tb_tooltip_string, bool tb_ui_string)
        {
            this.privateValue = new bool[] {  tb_achievement_script,  tb_booster_script,  tb_broach_set,  tb_buff_script,  tb_cinema_string,  tb_item_script,
             tb_monster_script,  tb_npc_script,  tb_quest_script,  tb_shop_string,  tb_skill_script,  tb_soul_metry_string,  tb_speech_string,
             tb_systemmail,  tb_title_string,  tb_tooltip_string,  tb_ui_string };
            this.theStringBuild = new System.Text.StringBuilder();
        }

        public TranslationOption() : this(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true) { }

        public TranslationOption(string ConfigString)
        {
            this.privateValue = new bool[17];
            if (ConfigString.IndexOf(",") > -1)
            {
                string[] splitting = ConfigString.Split(',');
                short parsedValue = 0;
                foreach (string theString in splitting)
                {
                    if (theString.ToLower() == "false")
                        this.privateValue[parsedValue] = false;
                    else
                        this.privateValue[parsedValue] = true;
                    parsedValue++;
                }
                splitting = null;
                for (short cou = parsedValue; cou < privateValue.Length; cou++)
                    this.privateValue[cou] = true;
            }
        }

        public override string ToString()
        {
            return this.ToString(false);
        }

        public string ToString(bool DetailInfo)
        {
            this.theStringBuild.Clear();
            if (DetailInfo == false)
            {
                this.theStringBuild.Append(privateValue[0].ToString());
                for (short cou = 1; cou < this.privateValue.Length; cou++)
                    this.theStringBuild.Append("," + privateValue[cou].ToString());
                return theStringBuild.ToString();
            }
            else
            {
                this.theStringBuild.Append(privateHeader[0] + "=" + privateValue[0].ToString());
                for (short cou = 1; cou < this.privateValue.Length; cou++)
                    this.theStringBuild.Append("," + privateHeader[cou] + "=" + privateValue[cou].ToString());
                return theStringBuild.ToString();
            }
        }

        public bool Achievement
        {
            get { return this.privateValue[0]; }
            set { this.privateValue[0] = value; }
        }

        public bool Booster
        {
            get { return this.privateValue[1]; }
            set { this.privateValue[1] = value; }
        }

        public bool Broach
        {
            get { return this.privateValue[2]; }
            set { this.privateValue[2] = value; }
        }

        public bool Buff
        {
            get { return this.privateValue[3]; }
            set { this.privateValue[3] = value; }
        }

        public bool Cinema
        {
            get { return this.privateValue[4]; }
            set { this.privateValue[4] = value; }
        }

        public bool Item
        {
            get { return this.privateValue[5]; }
            set { this.privateValue[5] = value; }
        }

        public bool Monster
        {
            get { return this.privateValue[6]; }
            set { this.privateValue[6] = value; }
        }

        public bool Npc
        {
            get { return this.privateValue[7]; }
            set { this.privateValue[7] = value; }
        }

        public bool Quest
        {
            get { return this.privateValue[8]; }
            set { this.privateValue[8] = value; }
        }

        public bool Shop
        {
            get { return this.privateValue[9]; }
            set { this.privateValue[9] = value; }
        }

        public bool Skill
        {
            get { return this.privateValue[10]; }
            set { this.privateValue[10] = value; }
        }

        public bool SoulMetry
        {
            get { return this.privateValue[11]; }
            set { this.privateValue[11] = value; }
        }

        public bool Speech
        {
            get { return this.privateValue[12]; }
            set { this.privateValue[12] = value; }
        }

        public bool SystemMail
        {
            get { return this.privateValue[13]; }
            set { this.privateValue[13] = value; }
        }

        public bool Title
        {
            get { return this.privateValue[14]; }
            set { this.privateValue[14] = value; }
        }

        public bool Tooltip
        {
            get { return this.privateValue[15]; }
            set { this.privateValue[15] = value; }
        }

        public bool UI
        {
            get { return this.privateValue[16]; }
            set { this.privateValue[16] = value; }
        }
    }
}
