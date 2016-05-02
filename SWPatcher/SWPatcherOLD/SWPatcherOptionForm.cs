using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SWPatcher
{
    public partial class SWPatcherOptionForm : Form
    {
        Ini.IniFile theSettingIni;
        public SWPatcherOptionForm(Ini.IniFile SettingIni)
        {
            InitializeComponent();
            this.theSettingIni = SettingIni;
        }

        private void SWPatcherOptionForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.patcher;
            if (this.theSettingIni.GetValue("patcher", "overwrite", "1") == "0")
                checkBoxPatchOverwrite.Checked = false;
            else
                checkBoxPatchOverwrite.Checked = true;
            if (this.theSettingIni.GetValue("patcher", "openIE", "0") == "1")
                checkBoxStartwithIE.Checked = true;
            else
                checkBoxStartwithIE.Checked = false;
        }

        private void checkBoxPatchOverwrite_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPatchOverwrite.Checked)
                this.theSettingIni.SetValue("patcher", "overwrite", "1");
            else
                this.theSettingIni.SetValue("patcher", "overwrite", "0");
            this.theSettingIni.Save();
        }

        private void checkBoxStartwithIE_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxStartwithIE.Checked)
                this.theSettingIni.SetValue("patcher", "openIE", "1");
            else
                this.theSettingIni.SetValue("patcher", "openIE", "0");
            this.theSettingIni.Save();
        }
    }
}
