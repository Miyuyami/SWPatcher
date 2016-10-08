using SWPatcherTest.Helpers;
using SWPatcherTest.Helpers.GlobalVariables;
using System;
using System.Windows.Forms;

namespace SWPatcherTest.Forms
{
    partial class AboutBox : Form
    {
        private int ImagesCount = 72;

        public AboutBox()
        {
            InitializeComponent();
            InitializeTextComponent();
        }

        private void InitializeTextComponent()
        {
            this.buttonOk.Text = StringLoader.GetText("button_ok");
            this.Text = $"About {AssemblyAccessor.Title}";
            this.labelProductName.Text = AssemblyAccessor.Product;
            this.labelVersion.Text = $"Version {AssemblyAccessor.Version}";
            this.textBoxDescription.Text = StringLoader.GetText("patcher_description");
            this.linkLabelWebsite.Links.Add(0, this.linkLabelWebsite.Text.Length, Urls.SoulworkerWebsite);
            this.logoPictureBox.ImageLocation = $"https://raw.githubusercontent.com/Miyuyami/SWPatcher/master/Images/{(new Random()).Next(ImagesCount) + 1}.png";
        }

        private void linkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelWebsite.LinkVisited = true;
            System.Diagnostics.Process.Start(Urls.SoulworkerWebsite);
        }
    }
}
