using System;
using System.Windows.Forms;
using SWPatcherTest.Helpers.GlobalVar;

namespace SWPatcherTest.Forms
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
            this.linkLabelWebsite.Links.Add(0, 17, Urls.SWHQWebsite);
            this.logoPictureBox.ImageLocation = String.Format("https://raw.githubusercontent.com/Miyuyami/SWHQPatcher/master/Images/{0}.png", (new Random()).Next(72) + 1);
        }

        private void linkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelWebsite.LinkVisited = true;
            System.Diagnostics.Process.Start(Urls.SWHQWebsite);
        }
    }
}
