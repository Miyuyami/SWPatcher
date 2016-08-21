namespace SWPatcherTEST.Forms
{
    partial class MainForm
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonPlay = new SWPatcherTEST.General.SplitButton();
            this.contextMenuStripPlay = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemStartRaw = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.comboBoxLanguages = new System.Windows.Forms.ComboBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSWWebpageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadLogToPastebinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.labelNewTranslations = new System.Windows.Forms.Label();
            this.contextMenuStripPlay.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel.SetColumnSpan(this.buttonPlay, 2);
            this.buttonPlay.ContextMenuStripSplit = this.contextMenuStripPlay;
            this.buttonPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlay.Location = new System.Drawing.Point(117, 210);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(130, 26);
            this.buttonPlay.TabIndex = 3;
            this.buttonPlay.Text = "Ready To Play!";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonPlay_MouseDown);
            // 
            // contextMenuStripPlay
            // 
            this.contextMenuStripPlay.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemStartRaw});
            this.contextMenuStripPlay.Name = "contextMenuStrip";
            this.contextMenuStripPlay.Size = new System.Drawing.Size(158, 26);
            // 
            // toolStripMenuItemStartRaw
            // 
            this.toolStripMenuItemStartRaw.Name = "toolStripMenuItemStartRaw";
            this.toolStripMenuItemStartRaw.Size = new System.Drawing.Size(157, 22);
            this.toolStripMenuItemStartRaw.Text = "Start Game Raw";
            this.toolStripMenuItemStartRaw.Click += new System.EventHandler(this.toolStripMenuItemStartRaw_Click);
            // 
            // buttonDownload
            // 
            this.buttonDownload.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel.SetColumnSpan(this.buttonDownload, 2);
            this.buttonDownload.Location = new System.Drawing.Point(112, 171);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(140, 24);
            this.buttonDownload.TabIndex = 2;
            this.buttonDownload.Text = "Download Translations";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // comboBoxLanguages
            // 
            this.comboBoxLanguages.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tableLayoutPanel.SetColumnSpan(this.comboBoxLanguages, 2);
            this.comboBoxLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLanguages.FormattingEnabled = true;
            this.comboBoxLanguages.Location = new System.Drawing.Point(112, 129);
            this.comboBoxLanguages.Name = "comboBoxLanguages";
            this.comboBoxLanguages.Size = new System.Drawing.Size(140, 21);
            this.comboBoxLanguages.TabIndex = 0;
            this.comboBoxLanguages.SelectedIndexChanged += new System.EventHandler(this.comboBoxLanguages_SelectedIndexChanged);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipText = "Double click to restore...";
            this.notifyIcon.BalloonTipTitle = "Patcher is hidden here";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Soul Worker Patcher";
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // menuStrip
            // 
            this.menuStrip.BackgroundImage = global::SWPatcherTEST.Properties.Resources.fadegray;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(364, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.forceStripMenuItem,
            this.openSWWebpageToolStripMenuItem,
            this.uploadLogToPastebinToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // forceStripMenuItem
            // 
            this.forceStripMenuItem.Name = "forceStripMenuItem";
            this.forceStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.forceStripMenuItem.Text = "Force Patch";
            this.forceStripMenuItem.Click += new System.EventHandler(this.forceStripMenuItem_Click);
            // 
            // openSWWebpageToolStripMenuItem
            // 
            this.openSWWebpageToolStripMenuItem.Name = "openSWWebpageToolStripMenuItem";
            this.openSWWebpageToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.openSWWebpageToolStripMenuItem.Text = "Open SW Webpage";
            this.openSWWebpageToolStripMenuItem.Click += new System.EventHandler(this.openSWWebpageToolStripMenuItem_Click);
            // 
            // uploadLogToPastebinToolStripMenuItem
            // 
            this.uploadLogToPastebinToolStripMenuItem.Name = "uploadLogToPastebinToolStripMenuItem";
            this.uploadLogToPastebinToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.uploadLogToPastebinToolStripMenuItem.Text = "Upload Log to PasteBin";
            this.uploadLogToPastebinToolStripMenuItem.Click += new System.EventHandler(this.uploadLogToPastebinToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.statusStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripSeparator,
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 324);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip.Size = new System.Drawing.Size(364, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 2;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar.MarqueeAnimationSpeed = 40;
            this.toolStripProgressBar.Maximum = 2147483647;
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripProgressBar.Size = new System.Drawing.Size(160, 16);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripSeparator.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripSeparator.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(4, 22);
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackgroundImage = global::SWPatcherTEST.Properties.Resources.fadegray;
            this.tableLayoutPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.buttonPlay, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonDownload, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.comboBoxLanguages, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.pictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonExit, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.labelNewTranslations, 0, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(364, 300);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // pictureBox
            // 
            this.pictureBox.BackgroundImage = global::SWPatcherTEST.Properties.Resources.fadegray;
            this.tableLayoutPanel.SetColumnSpan(this.pictureBox, 2);
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Image = global::SWPatcherTEST.Properties.Resources.logo;
            this.pictureBox.Location = new System.Drawing.Point(7, 7);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(350, 116);
            this.pictureBox.TabIndex = 7;
            this.pictureBox.TabStop = false;
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tableLayoutPanel.SetColumnSpan(this.buttonExit, 2);
            this.buttonExit.Location = new System.Drawing.Point(132, 265);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(100, 28);
            this.buttonExit.TabIndex = 4;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.exit_Click);
            // 
            // labelNewTranslations
            // 
            this.labelNewTranslations.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelNewTranslations.AutoSize = true;
            this.labelNewTranslations.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel.SetColumnSpan(this.labelNewTranslations, 2);
            this.labelNewTranslations.ForeColor = System.Drawing.Color.Red;
            this.labelNewTranslations.Location = new System.Drawing.Point(182, 151);
            this.labelNewTranslations.Name = "labelNewTranslations";
            this.labelNewTranslations.Size = new System.Drawing.Size(0, 13);
            this.labelNewTranslations.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonPlay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(364, 346);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.contextMenuStripPlay.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private SWPatcherTEST.General.SplitButton buttonPlay;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.ComboBox comboBoxLanguages;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSeparator;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSWWebpageToolStripMenuItem;
        private System.Windows.Forms.Label labelNewTranslations;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadLogToPastebinToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPlay;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStartRaw;
    }
}

