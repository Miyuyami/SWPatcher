using System;
using System.Windows.Forms;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Forms
{
    partial class AboutBox : Form
    {
        private int ImagesCount = 72;

        public AboutBox()
        {
            InitializeComponent();
            this.Text = $"About {AssemblyAccessor.Title}";
            this.labelProductName.Text = AssemblyAccessor.Product;
            this.labelVersion.Text = $"Version {AssemblyAccessor.Version}";
            this.textBoxDescription.Text = AssemblyAccessor.Description;
            this.linkLabelWebsite.Links.Add(0, this.linkLabelWebsite.Text.Length, Urls.SWHQWebsite);
            this.logoPictureBox.ImageLocation = $"https://raw.githubusercontent.com/Miyuyami/SWHQPatcher/master/Images/{(new Random()).Next(ImagesCount) + 1}.png";
        }

        private void linkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelWebsite.LinkVisited = true;
            System.Diagnostics.Process.Start(Urls.SWHQWebsite);
        }
    }
}
