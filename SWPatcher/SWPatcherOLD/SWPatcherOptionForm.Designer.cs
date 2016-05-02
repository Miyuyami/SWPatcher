namespace SWPatcher
{
    partial class SWPatcherOptionForm
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
            this.checkBoxPatchOverwrite = new System.Windows.Forms.CheckBox();
            this.checkBoxStartwithIE = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBoxPatchOverwrite
            // 
            this.checkBoxPatchOverwrite.AutoSize = true;
            this.checkBoxPatchOverwrite.Location = new System.Drawing.Point(13, 13);
            this.checkBoxPatchOverwrite.Name = "checkBoxPatchOverwrite";
            this.checkBoxPatchOverwrite.Size = new System.Drawing.Size(174, 17);
            this.checkBoxPatchOverwrite.TabIndex = 0;
            this.checkBoxPatchOverwrite.Text = "Patching with overwrite method";
            this.checkBoxPatchOverwrite.UseVisualStyleBackColor = true;
            this.checkBoxPatchOverwrite.Visible = false;
            this.checkBoxPatchOverwrite.CheckedChanged += new System.EventHandler(this.checkBoxPatchOverwrite_CheckedChanged);
            // 
            // checkBoxStartwithIE
            // 
            this.checkBoxStartwithIE.AutoSize = true;
            this.checkBoxStartwithIE.Location = new System.Drawing.Point(13, 36);
            this.checkBoxStartwithIE.Name = "checkBoxStartwithIE";
            this.checkBoxStartwithIE.Size = new System.Drawing.Size(165, 17);
            this.checkBoxStartwithIE.TabIndex = 0;
            this.checkBoxStartwithIE.Text = "Open IE when Ready to Start";
            this.checkBoxStartwithIE.UseVisualStyleBackColor = true;
            this.checkBoxStartwithIE.CheckedChanged += new System.EventHandler(this.checkBoxStartwithIE_CheckedChanged);
            // 
            // SWPatcherOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 61);
            this.Controls.Add(this.checkBoxStartwithIE);
            this.Controls.Add(this.checkBoxPatchOverwrite);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SWPatcherOptionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SWPatcher Option";
            this.Load += new System.EventHandler(this.SWPatcherOptionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxPatchOverwrite;
        private System.Windows.Forms.CheckBox checkBoxStartwithIE;
    }
}