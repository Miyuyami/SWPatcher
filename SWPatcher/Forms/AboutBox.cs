/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016 Miyu
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

using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using System;
using System.Windows.Forms;

namespace SWPatcher.Forms
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

        private void LinkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabelWebsite.LinkVisited = true;
            System.Diagnostics.Process.Start(Urls.SoulworkerWebsite);
        }
    }
}
