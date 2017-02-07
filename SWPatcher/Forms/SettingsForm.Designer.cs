/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016-2017 Miyu, Dramiel Leayal
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

namespace SWPatcher.Forms
{
    internal partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonApply = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.TabPageGame = new System.Windows.Forms.TabPage();
            this.GroupBoxGameOptions = new System.Windows.Forms.GroupBox();
            this.ButtonOpenGameOptions = new System.Windows.Forms.Button();
            this.GroupBoxPatchExe = new System.Windows.Forms.GroupBox();
            this.CheckBoxPatchExe = new System.Windows.Forms.CheckBox();
            this.GroupBoxGameDirectory = new System.Windows.Forms.GroupBox();
            this.ButtonOpenGameDirectory = new System.Windows.Forms.Button();
            this.TextBoxGameDirectory = new System.Windows.Forms.TextBox();
            this.TabPageCredentials = new System.Windows.Forms.TabPage();
            this.GroupBoxGameWantLogin = new System.Windows.Forms.GroupBox();
            this.CheckBoxWantToLogin = new System.Windows.Forms.CheckBox();
            this.GroupBoxGameUserPassword = new System.Windows.Forms.GroupBox();
            this.TextBoxPassword = new System.Windows.Forms.TextBox();
            this.GroupBoxGameUserId = new System.Windows.Forms.GroupBox();
            this.TextBoxId = new System.Windows.Forms.TextBox();
            this.TabPagePatcher = new System.Windows.Forms.TabPage();
            this.GroupBoxUILanguagePicker = new System.Windows.Forms.GroupBox();
            this.ComboBoxUILanguage = new System.Windows.Forms.ComboBox();
            this.GroupBoxPatcherDirectory = new System.Windows.Forms.GroupBox();
            this.ButtonChangePatcherDirectory = new System.Windows.Forms.Button();
            this.TextBoxPatcherDirectory = new System.Windows.Forms.TextBox();
            this.TableLayoutPanel.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.TabPageGame.SuspendLayout();
            this.GroupBoxGameOptions.SuspendLayout();
            this.GroupBoxPatchExe.SuspendLayout();
            this.GroupBoxGameDirectory.SuspendLayout();
            this.TabPageCredentials.SuspendLayout();
            this.GroupBoxGameWantLogin.SuspendLayout();
            this.GroupBoxGameUserPassword.SuspendLayout();
            this.GroupBoxGameUserId.SuspendLayout();
            this.TabPagePatcher.SuspendLayout();
            this.GroupBoxUILanguagePicker.SuspendLayout();
            this.GroupBoxPatcherDirectory.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.ColumnCount = 4;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.TableLayoutPanel.Controls.Add(this.ButtonApply, 3, 1);
            this.TableLayoutPanel.Controls.Add(this.ButtonCancel, 2, 1);
            this.TableLayoutPanel.Controls.Add(this.ButtonOk, 1, 1);
            this.TableLayoutPanel.Controls.Add(this.TabControl, 0, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 2;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(320, 252);
            this.TableLayoutPanel.TabIndex = 0;
            // 
            // ButtonApply
            // 
            this.ButtonApply.Enabled = false;
            this.ButtonApply.Location = new System.Drawing.Point(242, 224);
            this.ButtonApply.Name = "ButtonApply";
            this.ButtonApply.Size = new System.Drawing.Size(75, 23);
            this.ButtonApply.TabIndex = 3;
            this.ButtonApply.UseVisualStyleBackColor = true;
            this.ButtonApply.Click += new System.EventHandler(this.ButtonApply_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Location = new System.Drawing.Point(161, 224);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 2;
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ButtonOk
            // 
            this.ButtonOk.Location = new System.Drawing.Point(80, 224);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(75, 23);
            this.ButtonOk.TabIndex = 1;
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // TabControl
            // 
            this.TableLayoutPanel.SetColumnSpan(this.TabControl, 4);
            this.TabControl.Controls.Add(this.TabPageGame);
            this.TabControl.Controls.Add(this.TabPageCredentials);
            this.TabControl.Controls.Add(this.TabPagePatcher);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(3, 3);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(314, 215);
            this.TabControl.TabIndex = 0;
            // 
            // TabPageGame
            // 
            this.TabPageGame.Controls.Add(this.GroupBoxGameOptions);
            this.TabPageGame.Controls.Add(this.GroupBoxPatchExe);
            this.TabPageGame.Controls.Add(this.GroupBoxGameDirectory);
            this.TabPageGame.Location = new System.Drawing.Point(4, 22);
            this.TabPageGame.Name = "TabPageGame";
            this.TabPageGame.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageGame.Size = new System.Drawing.Size(306, 189);
            this.TabPageGame.TabIndex = 0;
            this.TabPageGame.UseVisualStyleBackColor = true;
            // 
            // GroupBoxGameOptions
            // 
            this.GroupBoxGameOptions.AutoSize = true;
            this.GroupBoxGameOptions.Controls.Add(this.ButtonOpenGameOptions);
            this.GroupBoxGameOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBoxGameOptions.Location = new System.Drawing.Point(3, 122);
            this.GroupBoxGameOptions.Name = "GroupBoxGameOptions";
            this.GroupBoxGameOptions.Size = new System.Drawing.Size(300, 64);
            this.GroupBoxGameOptions.TabIndex = 3;
            this.GroupBoxGameOptions.TabStop = false;
            // 
            // ButtonOpenGameOptions
            // 
            this.ButtonOpenGameOptions.Location = new System.Drawing.Point(6, 22);
            this.ButtonOpenGameOptions.Name = "ButtonOpenGameOptions";
            this.ButtonOpenGameOptions.Size = new System.Drawing.Size(99, 23);
            this.ButtonOpenGameOptions.TabIndex = 2;
            this.ButtonOpenGameOptions.UseVisualStyleBackColor = true;
            this.ButtonOpenGameOptions.Click += new System.EventHandler(this.ButtonOpenGameOptions_Click);
            // 
            // GroupBoxPatchExe
            // 
            this.GroupBoxPatchExe.AutoSize = true;
            this.GroupBoxPatchExe.Controls.Add(this.CheckBoxPatchExe);
            this.GroupBoxPatchExe.Dock = System.Windows.Forms.DockStyle.Top;
            this.GroupBoxPatchExe.Location = new System.Drawing.Point(3, 64);
            this.GroupBoxPatchExe.Name = "GroupBoxPatchExe";
            this.GroupBoxPatchExe.Size = new System.Drawing.Size(300, 58);
            this.GroupBoxPatchExe.TabIndex = 2;
            this.GroupBoxPatchExe.TabStop = false;
            // 
            // CheckBoxPatchExe
            // 
            this.CheckBoxPatchExe.AutoSize = true;
            this.CheckBoxPatchExe.Location = new System.Drawing.Point(6, 25);
            this.CheckBoxPatchExe.Name = "CheckBoxPatchExe";
            this.CheckBoxPatchExe.Size = new System.Drawing.Size(15, 14);
            this.CheckBoxPatchExe.TabIndex = 0;
            this.CheckBoxPatchExe.UseVisualStyleBackColor = true;
            this.CheckBoxPatchExe.CheckedChanged += new System.EventHandler(this.CheckBoxPatchExe_CheckedChanged);
            // 
            // GroupBoxGameDirectory
            // 
            this.GroupBoxGameDirectory.AutoSize = true;
            this.GroupBoxGameDirectory.Controls.Add(this.ButtonOpenGameDirectory);
            this.GroupBoxGameDirectory.Controls.Add(this.TextBoxGameDirectory);
            this.GroupBoxGameDirectory.Dock = System.Windows.Forms.DockStyle.Top;
            this.GroupBoxGameDirectory.Location = new System.Drawing.Point(3, 3);
            this.GroupBoxGameDirectory.Name = "GroupBoxGameDirectory";
            this.GroupBoxGameDirectory.Size = new System.Drawing.Size(300, 61);
            this.GroupBoxGameDirectory.TabIndex = 0;
            this.GroupBoxGameDirectory.TabStop = false;
            // 
            // ButtonOpenGameDirectory
            // 
            this.ButtonOpenGameDirectory.Location = new System.Drawing.Point(219, 19);
            this.ButtonOpenGameDirectory.Name = "ButtonOpenGameDirectory";
            this.ButtonOpenGameDirectory.Size = new System.Drawing.Size(75, 23);
            this.ButtonOpenGameDirectory.TabIndex = 1;
            this.ButtonOpenGameDirectory.UseVisualStyleBackColor = true;
            this.ButtonOpenGameDirectory.Click += new System.EventHandler(this.ButtonOpenGameDirectory_Click);
            // 
            // TextBoxGameDirectory
            // 
            this.TextBoxGameDirectory.Location = new System.Drawing.Point(6, 21);
            this.TextBoxGameDirectory.Name = "TextBoxGameDirectory";
            this.TextBoxGameDirectory.ReadOnly = true;
            this.TextBoxGameDirectory.Size = new System.Drawing.Size(207, 20);
            this.TextBoxGameDirectory.TabIndex = 0;
            // 
            // TabPageCredentials
            // 
            this.TabPageCredentials.Controls.Add(this.GroupBoxGameWantLogin);
            this.TabPageCredentials.Controls.Add(this.GroupBoxGameUserPassword);
            this.TabPageCredentials.Controls.Add(this.GroupBoxGameUserId);
            this.TabPageCredentials.Location = new System.Drawing.Point(4, 22);
            this.TabPageCredentials.Name = "TabPageCredentials";
            this.TabPageCredentials.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageCredentials.Size = new System.Drawing.Size(306, 189);
            this.TabPageCredentials.TabIndex = 1;
            this.TabPageCredentials.UseVisualStyleBackColor = true;
            // 
            // GroupBoxGameWantLogin
            // 
            this.GroupBoxGameWantLogin.AutoSize = true;
            this.GroupBoxGameWantLogin.Controls.Add(this.CheckBoxWantToLogin);
            this.GroupBoxGameWantLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBoxGameWantLogin.Location = new System.Drawing.Point(3, 123);
            this.GroupBoxGameWantLogin.Name = "GroupBoxGameWantLogin";
            this.GroupBoxGameWantLogin.Size = new System.Drawing.Size(300, 63);
            this.GroupBoxGameWantLogin.TabIndex = 2;
            this.GroupBoxGameWantLogin.TabStop = false;
            // 
            // CheckBoxWantToLogin
            // 
            this.CheckBoxWantToLogin.AutoSize = true;
            this.CheckBoxWantToLogin.Location = new System.Drawing.Point(6, 25);
            this.CheckBoxWantToLogin.Name = "CheckBoxWantToLogin";
            this.CheckBoxWantToLogin.Size = new System.Drawing.Size(15, 14);
            this.CheckBoxWantToLogin.TabIndex = 0;
            this.CheckBoxWantToLogin.UseVisualStyleBackColor = true;
            this.CheckBoxWantToLogin.CheckedChanged += new System.EventHandler(this.CheckBoxWantToLogin_CheckedChanged);
            // 
            // GroupBoxGameUserPassword
            // 
            this.GroupBoxGameUserPassword.AutoSize = true;
            this.GroupBoxGameUserPassword.Controls.Add(this.TextBoxPassword);
            this.GroupBoxGameUserPassword.Dock = System.Windows.Forms.DockStyle.Top;
            this.GroupBoxGameUserPassword.Location = new System.Drawing.Point(3, 63);
            this.GroupBoxGameUserPassword.Name = "GroupBoxGameUserPassword";
            this.GroupBoxGameUserPassword.Size = new System.Drawing.Size(300, 60);
            this.GroupBoxGameUserPassword.TabIndex = 1;
            this.GroupBoxGameUserPassword.TabStop = false;
            // 
            // TextBoxPassword
            // 
            this.TextBoxPassword.Location = new System.Drawing.Point(6, 21);
            this.TextBoxPassword.Name = "TextBoxPassword";
            this.TextBoxPassword.Size = new System.Drawing.Size(180, 20);
            this.TextBoxPassword.TabIndex = 0;
            this.TextBoxPassword.UseSystemPasswordChar = true;
            this.TextBoxPassword.TextChanged += new System.EventHandler(this.TextBoxPassword_TextChanged);
            // 
            // GroupBoxGameUserId
            // 
            this.GroupBoxGameUserId.AutoSize = true;
            this.GroupBoxGameUserId.Controls.Add(this.TextBoxId);
            this.GroupBoxGameUserId.Dock = System.Windows.Forms.DockStyle.Top;
            this.GroupBoxGameUserId.Location = new System.Drawing.Point(3, 3);
            this.GroupBoxGameUserId.Name = "GroupBoxGameUserId";
            this.GroupBoxGameUserId.Size = new System.Drawing.Size(300, 60);
            this.GroupBoxGameUserId.TabIndex = 0;
            this.GroupBoxGameUserId.TabStop = false;
            // 
            // TextBoxId
            // 
            this.TextBoxId.Location = new System.Drawing.Point(6, 21);
            this.TextBoxId.Name = "TextBoxId";
            this.TextBoxId.Size = new System.Drawing.Size(180, 20);
            this.TextBoxId.TabIndex = 0;
            this.TextBoxId.TextChanged += new System.EventHandler(this.TextBoxId_TextChanged);
            // 
            // TabPagePatcher
            // 
            this.TabPagePatcher.Controls.Add(this.GroupBoxUILanguagePicker);
            this.TabPagePatcher.Controls.Add(this.GroupBoxPatcherDirectory);
            this.TabPagePatcher.Location = new System.Drawing.Point(4, 22);
            this.TabPagePatcher.Name = "TabPagePatcher";
            this.TabPagePatcher.Padding = new System.Windows.Forms.Padding(3);
            this.TabPagePatcher.Size = new System.Drawing.Size(306, 189);
            this.TabPagePatcher.TabIndex = 2;
            this.TabPagePatcher.UseVisualStyleBackColor = true;
            // 
            // GroupBoxUILanguagePicker
            // 
            this.GroupBoxUILanguagePicker.AutoSize = true;
            this.GroupBoxUILanguagePicker.Controls.Add(this.ComboBoxUILanguage);
            this.GroupBoxUILanguagePicker.Dock = System.Windows.Forms.DockStyle.Top;
            this.GroupBoxUILanguagePicker.Location = new System.Drawing.Point(3, 64);
            this.GroupBoxUILanguagePicker.Name = "GroupBoxUILanguagePicker";
            this.GroupBoxUILanguagePicker.Size = new System.Drawing.Size(300, 59);
            this.GroupBoxUILanguagePicker.TabIndex = 4;
            this.GroupBoxUILanguagePicker.TabStop = false;
            // 
            // ComboBoxUILanguage
            // 
            this.ComboBoxUILanguage.FormattingEnabled = true;
            this.ComboBoxUILanguage.Location = new System.Drawing.Point(6, 19);
            this.ComboBoxUILanguage.Name = "ComboBoxUILanguage";
            this.ComboBoxUILanguage.Size = new System.Drawing.Size(121, 21);
            this.ComboBoxUILanguage.TabIndex = 0;
            this.ComboBoxUILanguage.SelectedIndexChanged += new System.EventHandler(this.ComboBoxUILanguage_SelectedIndexChanged);
            // 
            // GroupBoxPatcherDirectory
            // 
            this.GroupBoxPatcherDirectory.AutoSize = true;
            this.GroupBoxPatcherDirectory.Controls.Add(this.ButtonChangePatcherDirectory);
            this.GroupBoxPatcherDirectory.Controls.Add(this.TextBoxPatcherDirectory);
            this.GroupBoxPatcherDirectory.Dock = System.Windows.Forms.DockStyle.Top;
            this.GroupBoxPatcherDirectory.Location = new System.Drawing.Point(3, 3);
            this.GroupBoxPatcherDirectory.Name = "GroupBoxPatcherDirectory";
            this.GroupBoxPatcherDirectory.Size = new System.Drawing.Size(300, 61);
            this.GroupBoxPatcherDirectory.TabIndex = 2;
            this.GroupBoxPatcherDirectory.TabStop = false;
            // 
            // ButtonChangePatcherDirectory
            // 
            this.ButtonChangePatcherDirectory.Location = new System.Drawing.Point(219, 19);
            this.ButtonChangePatcherDirectory.Name = "ButtonChangePatcherDirectory";
            this.ButtonChangePatcherDirectory.Size = new System.Drawing.Size(75, 23);
            this.ButtonChangePatcherDirectory.TabIndex = 1;
            this.ButtonChangePatcherDirectory.UseVisualStyleBackColor = true;
            this.ButtonChangePatcherDirectory.Click += new System.EventHandler(this.ButtonChangePatcherDirectory_Click);
            // 
            // TextBoxPatcherDirectory
            // 
            this.TextBoxPatcherDirectory.Location = new System.Drawing.Point(6, 21);
            this.TextBoxPatcherDirectory.Name = "TextBoxPatcherDirectory";
            this.TextBoxPatcherDirectory.ReadOnly = true;
            this.TextBoxPatcherDirectory.Size = new System.Drawing.Size(207, 20);
            this.TextBoxPatcherDirectory.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 252);
            this.Controls.Add(this.TableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.TableLayoutPanel.ResumeLayout(false);
            this.TabControl.ResumeLayout(false);
            this.TabPageGame.ResumeLayout(false);
            this.TabPageGame.PerformLayout();
            this.GroupBoxGameOptions.ResumeLayout(false);
            this.GroupBoxPatchExe.ResumeLayout(false);
            this.GroupBoxPatchExe.PerformLayout();
            this.GroupBoxGameDirectory.ResumeLayout(false);
            this.GroupBoxGameDirectory.PerformLayout();
            this.TabPageCredentials.ResumeLayout(false);
            this.TabPageCredentials.PerformLayout();
            this.GroupBoxGameWantLogin.ResumeLayout(false);
            this.GroupBoxGameWantLogin.PerformLayout();
            this.GroupBoxGameUserPassword.ResumeLayout(false);
            this.GroupBoxGameUserPassword.PerformLayout();
            this.GroupBoxGameUserId.ResumeLayout(false);
            this.GroupBoxGameUserId.PerformLayout();
            this.TabPagePatcher.ResumeLayout(false);
            this.TabPagePatcher.PerformLayout();
            this.GroupBoxUILanguagePicker.ResumeLayout(false);
            this.GroupBoxPatcherDirectory.ResumeLayout(false);
            this.GroupBoxPatcherDirectory.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.Button ButtonApply;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage TabPageGame;
        private System.Windows.Forms.GroupBox GroupBoxGameDirectory;
        private System.Windows.Forms.Button ButtonOpenGameDirectory;
        private System.Windows.Forms.TextBox TextBoxGameDirectory;
        private System.Windows.Forms.GroupBox GroupBoxPatchExe;
        private System.Windows.Forms.CheckBox CheckBoxPatchExe;
        private System.Windows.Forms.TabPage TabPageCredentials;
        private System.Windows.Forms.GroupBox GroupBoxGameUserPassword;
        private System.Windows.Forms.TextBox TextBoxPassword;
        private System.Windows.Forms.GroupBox GroupBoxGameUserId;
        private System.Windows.Forms.TextBox TextBoxId;
        private System.Windows.Forms.GroupBox GroupBoxGameWantLogin;
        private System.Windows.Forms.CheckBox CheckBoxWantToLogin;
        private System.Windows.Forms.TabPage TabPagePatcher;
        private System.Windows.Forms.GroupBox GroupBoxPatcherDirectory;
        private System.Windows.Forms.Button ButtonChangePatcherDirectory;
        private System.Windows.Forms.TextBox TextBoxPatcherDirectory;
        private System.Windows.Forms.GroupBox GroupBoxUILanguagePicker;
        private System.Windows.Forms.ComboBox ComboBoxUILanguage;
        private System.Windows.Forms.GroupBox GroupBoxGameOptions;
        private System.Windows.Forms.Button ButtonOpenGameOptions;
    }
}