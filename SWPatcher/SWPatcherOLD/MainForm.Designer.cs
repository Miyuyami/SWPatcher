namespace SWPatcher
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
            this.buttonPatch = new System.Windows.Forms.Button();
            this.buttonLastest = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.swHQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.placeHolderHideThisTabForNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressbarText = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOpenBackupFolder = new System.Windows.Forms.Button();
            this.buttonResetSWFolder = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.listBoxTrasnaltionlanguage = new SWPatcher.Classes.SplitButton();
            this.languageList1 = new SWPatcher.Classes.LanguageList();
            this.buttonExit = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonPatch
            // 
            this.buttonPatch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel.SetColumnSpan(this.buttonPatch, 2);
            this.buttonPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPatch.Location = new System.Drawing.Point(115, 266);
            this.buttonPatch.Name = "buttonPatch";
            this.buttonPatch.Size = new System.Drawing.Size(133, 28);
            this.buttonPatch.TabIndex = 0;
            this.buttonPatch.Text = "Ready To Play!";
            this.buttonPatch.UseVisualStyleBackColor = true;
            this.buttonPatch.Visible = false;
            // 
            // buttonLastest
            // 
            this.buttonLastest.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel.SetColumnSpan(this.buttonLastest, 2);
            this.buttonLastest.Location = new System.Drawing.Point(97, 184);
            this.buttonLastest.Name = "buttonLastest";
            this.buttonLastest.Size = new System.Drawing.Size(170, 28);
            this.buttonLastest.TabIndex = 2;
            this.buttonLastest.Text = "Get Latest Translations && Patch";
            this.buttonLastest.UseVisualStyleBackColor = true;
            this.buttonLastest.Click += new System.EventHandler(this.buttonLastest_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "SW Patcher";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.swHQToolStripMenuItem,
            this.optionToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(364, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // swHQToolStripMenuItem
            // 
            this.swHQToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placeHolderHideThisTabForNowToolStripMenuItem});
            this.swHQToolStripMenuItem.Name = "swHQToolStripMenuItem";
            this.swHQToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.swHQToolStripMenuItem.Text = "SwHQ";
            this.swHQToolStripMenuItem.Visible = false;
            // 
            // placeHolderHideThisTabForNowToolStripMenuItem
            // 
            this.placeHolderHideThisTabForNowToolStripMenuItem.Name = "placeHolderHideThisTabForNowToolStripMenuItem";
            this.placeHolderHideThisTabForNowToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.placeHolderHideThisTabForNowToolStripMenuItem.Text = "PlaceHolder. Hide this tab for now";
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            this.optionToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionToolStripMenuItem.Text = "Option";
            this.optionToolStripMenuItem.Click += new System.EventHandler(this.optionToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.statusLabel,
            this.toolStripSeparator,
            this.progressbarText});
            this.statusStrip.Location = new System.Drawing.Point(0, 362);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip.Size = new System.Drawing.Size(364, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 5;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 16);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripSeparator.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripSeparator.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(4, 22);
            // 
            // progressbarText
            // 
            this.progressbarText.ActiveLinkColor = System.Drawing.Color.Red;
            this.progressbarText.Name = "progressbarText";
            this.progressbarText.Size = new System.Drawing.Size(39, 17);
            this.progressbarText.Text = "Ready";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.89606F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.10394F));
            this.tableLayoutPanel.Controls.Add(this.buttonOpenBackupFolder, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonResetSWFolder, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.pictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonLastest, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.listBoxTrasnaltionlanguage, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonExit, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.buttonPatch, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.button1, 0, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel.RowCount = 7;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.5F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.5F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(364, 338);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // buttonOpenBackupFolder
            // 
            this.buttonOpenBackupFolder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonOpenBackupFolder.Location = new System.Drawing.Point(34, 129);
            this.buttonOpenBackupFolder.Name = "buttonOpenBackupFolder";
            this.buttonOpenBackupFolder.Size = new System.Drawing.Size(120, 20);
            this.buttonOpenBackupFolder.TabIndex = 4;
            this.buttonOpenBackupFolder.Text = "Open Backup Folder";
            this.buttonOpenBackupFolder.UseVisualStyleBackColor = true;
            // 
            // buttonResetSWFolder
            // 
            this.buttonResetSWFolder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonResetSWFolder.Location = new System.Drawing.Point(212, 129);
            this.buttonResetSWFolder.Name = "buttonResetSWFolder";
            this.buttonResetSWFolder.Size = new System.Drawing.Size(120, 20);
            this.buttonResetSWFolder.TabIndex = 5;
            this.buttonResetSWFolder.Text = "Reset SW Folder";
            this.buttonResetSWFolder.UseVisualStyleBackColor = true;
            this.buttonResetSWFolder.Click += new System.EventHandler(this.buttonResetSWFolder_Click);
            // 
            // pictureBox
            // 
            this.tableLayoutPanel.SetColumnSpan(this.pictureBox, 2);
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Image = global::SWPatcher.Properties.Resources.logo;
            this.pictureBox.Location = new System.Drawing.Point(7, 7);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(350, 116);
            this.pictureBox.TabIndex = 7;
            this.pictureBox.TabStop = false;
            // 
            // listBoxTrasnaltionlanguage
            // 
            this.listBoxTrasnaltionlanguage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.listBoxTrasnaltionlanguage.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.listBoxTrasnaltionlanguage, 2);
            this.listBoxTrasnaltionlanguage.ContextMenuStrip = this.languageList1;
            this.listBoxTrasnaltionlanguage.Location = new System.Drawing.Point(151, 155);
            this.listBoxTrasnaltionlanguage.Name = "listBoxTrasnaltionlanguage";
            this.listBoxTrasnaltionlanguage.Size = new System.Drawing.Size(61, 20);
            this.listBoxTrasnaltionlanguage.SplitMenuStrip = this.languageList1;
            this.listBoxTrasnaltionlanguage.TabIndex = 8;
            this.listBoxTrasnaltionlanguage.Text = "None";
            this.listBoxTrasnaltionlanguage.UseVisualStyleBackColor = true;
            this.listBoxTrasnaltionlanguage.Click += new System.EventHandler(this.listBoxTrasnaltionlanguage_Click);
            // 
            // languageList1
            // 
            this.languageList1.Name = "languageList1";
            this.languageList1.SelectedIndex = 0;
            this.languageList1.SelectedItem = "";
            this.languageList1.Size = new System.Drawing.Size(61, 4);
            this.languageList1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.languageList1_ItemClicked);
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tableLayoutPanel.SetColumnSpan(this.buttonExit, 2);
            this.buttonExit.Location = new System.Drawing.Point(132, 305);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(100, 26);
            this.buttonExit.TabIndex = 6;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel.SetColumnSpan(this.button1, 2);
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(115, 224);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 28);
            this.button1.TabIndex = 0;
            this.button1.Text = "Mods Manager";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonPatch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SWPatcher.Properties.Resources.fadegray;
            this.ClientSize = new System.Drawing.Size(364, 384);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Soul Worker Patcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
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
        private System.Windows.Forms.Button buttonPatch;
        private System.Windows.Forms.Button buttonLastest;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Button buttonOpenBackupFolder;
        private System.Windows.Forms.Button buttonResetSWFolder;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel progressbarText;
        private System.Windows.Forms.ToolStripMenuItem swHQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
        private Classes.SplitButton listBoxTrasnaltionlanguage;
        private Classes.LanguageList languageList1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem placeHolderHideThisTabForNowToolStripMenuItem;
        private System.Windows.Forms.Button button1;
    }
}

