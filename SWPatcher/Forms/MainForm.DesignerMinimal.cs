namespace SWPatcher.Forms
{
    partial class MainForm
    {
        #region Windows Form Designer generated code
        private void InitializeComponentMinimal()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonPlay = new General.SplitButton();
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
            this.buttonExit = new System.Windows.Forms.Button();
            this.labelNewTranslations = new System.Windows.Forms.Label();
            this.contextMenuStripPlay.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel.SetColumnSpan(this.buttonPlay, 1);
            this.buttonPlay.ContextMenuStripSplit = this.contextMenuStripPlay;
            this.buttonPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            this.buttonPlay.Location = new System.Drawing.Point(117, 210);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(130, 26);
            this.buttonPlay.TabIndex = 3;
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
            this.toolStripMenuItemStartRaw.Click += new System.EventHandler(this.buttonStartRaw_Click);
            // 
            // buttonDownload
            // 
            this.buttonDownload.Visible = false;
            // 
            // comboBoxLanguages
            // 
            this.comboBoxLanguages.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel.SetColumnSpan(this.comboBoxLanguages, 1);
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
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // menuStrip
            // 
            this.menuStrip.BackgroundImage = Properties.Resources.fadegray;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(364, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.forceStripMenuItem,
            this.openSWWebpageToolStripMenuItem,
            this.uploadLogToPastebinToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            // 
            // forceStripMenuItem
            // 
            this.forceStripMenuItem.Name = "forceStripMenuItem";
            this.forceStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.forceStripMenuItem.Click += new System.EventHandler(this.forceStripMenuItem_Click);
            // 
            // openSWWebpageToolStripMenuItem
            // 
            this.openSWWebpageToolStripMenuItem.Name = "openSWWebpageToolStripMenuItem";
            this.openSWWebpageToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.openSWWebpageToolStripMenuItem.Click += new System.EventHandler(this.openSWWebpageToolStripMenuItem_Click);
            // 
            // uploadLogToPastebinToolStripMenuItem
            // 
            this.uploadLogToPastebinToolStripMenuItem.Name = "uploadLogToPastebinToolStripMenuItem";
            this.uploadLogToPastebinToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.uploadLogToPastebinToolStripMenuItem.Click += new System.EventHandler(this.uploadLogToPastebinToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(200, 200, 200);
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
            this.tableLayoutPanel.BackgroundImage = Properties.Resources.fadegray;
            this.tableLayoutPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.buttonPlay, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.comboBoxLanguages, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // buttonExit
            // 
            this.buttonExit.Visible = false;
            // 
            // labelNewTranslations
            // 
            this.labelNewTranslations.Visible = false;
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonPlay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(364, 100);
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
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
    }
}

