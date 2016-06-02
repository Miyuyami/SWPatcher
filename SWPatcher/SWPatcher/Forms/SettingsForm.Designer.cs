namespace SWPatcher.Forms
{
    partial class SettingsForm
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageGame = new System.Windows.Forms.TabPage();
            this.groupBoxGameDirectory = new System.Windows.Forms.GroupBox();
            this.buttonChangeDirectory = new System.Windows.Forms.Button();
            this.textBoxGameDirectory = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageGame.SuspendLayout();
            this.groupBoxGameDirectory.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 4;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel.Controls.Add(this.buttonApply, 3, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonCancel, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonOk, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.tabControl, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(320, 130);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // buttonApply
            // 
            this.buttonApply.Enabled = false;
            this.buttonApply.Location = new System.Drawing.Point(242, 102);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 0;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(161, 102);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(80, 102);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // tabControl
            // 
            this.tableLayoutPanel.SetColumnSpan(this.tabControl, 4);
            this.tabControl.Controls.Add(this.tabPageGame);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(314, 93);
            this.tabControl.TabIndex = 3;
            // 
            // tabPageGame
            // 
            this.tabPageGame.Controls.Add(this.groupBoxGameDirectory);
            this.tabPageGame.Location = new System.Drawing.Point(4, 22);
            this.tabPageGame.Name = "tabPageGame";
            this.tabPageGame.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGame.Size = new System.Drawing.Size(306, 67);
            this.tabPageGame.TabIndex = 0;
            this.tabPageGame.Text = "Soulworker";
            this.tabPageGame.UseVisualStyleBackColor = true;
            // 
            // groupBoxGameDirectory
            // 
            this.groupBoxGameDirectory.AutoSize = true;
            this.groupBoxGameDirectory.Controls.Add(this.buttonChangeDirectory);
            this.groupBoxGameDirectory.Controls.Add(this.textBoxGameDirectory);
            this.groupBoxGameDirectory.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxGameDirectory.Location = new System.Drawing.Point(3, 3);
            this.groupBoxGameDirectory.Name = "groupBoxGameDirectory";
            this.groupBoxGameDirectory.Size = new System.Drawing.Size(300, 61);
            this.groupBoxGameDirectory.TabIndex = 0;
            this.groupBoxGameDirectory.TabStop = false;
            this.groupBoxGameDirectory.Text = "Game location";
            // 
            // buttonChangeDirectory
            // 
            this.buttonChangeDirectory.Location = new System.Drawing.Point(219, 19);
            this.buttonChangeDirectory.Name = "buttonChangeDirectory";
            this.buttonChangeDirectory.Size = new System.Drawing.Size(75, 23);
            this.buttonChangeDirectory.TabIndex = 1;
            this.buttonChangeDirectory.Text = "Change";
            this.buttonChangeDirectory.UseVisualStyleBackColor = true;
            this.buttonChangeDirectory.Click += new System.EventHandler(this.buttonChangeDirectory_Click);
            // 
            // textBoxGameDirectory
            // 
            this.textBoxGameDirectory.Location = new System.Drawing.Point(6, 21);
            this.textBoxGameDirectory.Name = "textBoxGameDirectory";
            this.textBoxGameDirectory.Size = new System.Drawing.Size(207, 20);
            this.textBoxGameDirectory.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 130);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageGame.ResumeLayout(false);
            this.tabPageGame.PerformLayout();
            this.groupBoxGameDirectory.ResumeLayout(false);
            this.groupBoxGameDirectory.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageGame;
        private System.Windows.Forms.GroupBox groupBoxGameDirectory;
        private System.Windows.Forms.Button buttonChangeDirectory;
        private System.Windows.Forms.TextBox textBoxGameDirectory;
    }
}