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
    public partial class TranslationOptionForm : Form
    {
        SWPatcher.Classes.TranslationOption myTranslationSelection;
        public TranslationOptionForm(Classes.TranslationOption OriginalConfig)
        {
            this.myTranslationSelection = OriginalConfig;
            InitializeComponent();
        }

        public TranslationOptionForm() : this(new Classes.TranslationOption()) { }

        private void TranslationOptionForm_Load(object sender, EventArgs e)
        {
            string[] tableOption = this.myTranslationSelection.ToString(Classes.TranslationOption.OutputType.Short).Split(',');
            string[] splitting = null;
            Control currentTarget = null;
            for (short cou = 0; cou < tableOption.Length; cou++)
            {
                currentTarget = getFromName("checkBox" + (cou+1).ToString());
                splitting = tableOption[cou].Split('=');
                currentTarget.Text = splitting[0];
                if (splitting[1] == "False")
                    ((CheckBox)currentTarget).Checked = false;
                else
                    ((CheckBox)currentTarget).Checked = true;
                currentTarget.Tag = cou;
                ((CheckBox)currentTarget).CheckedChanged += new EventHandler((object target, EventArgs em) =>
                {
                    var currentPun = (CheckBox)target;
                    this.myTranslationSelection.SetValueIndex((short)currentPun.Tag, currentPun.Checked);
                    currentPun = null;
                });

            }
            splitting = null;
            tableOption = null;
            currentTarget = null;
        }

        private Control getFromName(string sName)
        {
            foreach (Control theControl in tableLayoutPanel1.Controls)
                if (theControl.GetType().Name == "CheckBox")
                    if (theControl.Name == sName)
                        return theControl;
            return null;
        }

        public Classes.TranslationOption SelectedTranslation
        {
            get { return this.myTranslationSelection; }
        }
        
        private void buttonBuild_Click(object sender, EventArgs e)
        {
            
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Ini.IniFile theIni = new Ini.IniFile(System.IO.Directory.GetParent(Application.ExecutablePath).FullName + "\\Settings.ini");
            theIni.SetValue("Translation", "bSelectedTranslation", this.myTranslationSelection.ToString());
            theIni.Save();
            theIni.Close();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
