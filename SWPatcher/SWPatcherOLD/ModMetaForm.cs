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
    public partial class ModMetaForm : Form
    {
        public enum Option: short
        {
            Add = 0,
            Edit
        }

        Option _type;

        public ModMetaForm(Option type)
        {
            InitializeComponent();
            this._type = type;
        }

        private void ModMetaForm_Load(object sender, EventArgs e)
        {

        }
    }
}
