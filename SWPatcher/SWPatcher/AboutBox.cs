using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using SWPatcher.Helpers;

namespace SWPatcher
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyAccessor.Title);
            this.labelProductName.Text = AssemblyAccessor.Product;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyAccessor.Version);
            this.textBoxDescription.Text = AssemblyAccessor.Description;
            this.linkLabelWebsite.Links.Add(0, 17, Helpers.Uris.SWHQWebsite);
        }

        private void linkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelWebsite.LinkVisited = true;
            System.Diagnostics.Process.Start(Helpers.Uris.SWHQWebsite);
        }
    }
}
