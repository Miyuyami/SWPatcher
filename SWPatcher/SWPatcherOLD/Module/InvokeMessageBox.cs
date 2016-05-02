using System.Windows.Forms;

namespace SWPatcher.Module
{
    class InvokeMessageBox
    {
        public delegate DialogResult _Show(Form targetForm, string text, string caption);
        static public DialogResult Show(Form targetForm, string text, string caption)
        {
            if (targetForm.InvokeRequired)
                return (DialogResult)targetForm.Invoke(new _Show(Show), new object[] { targetForm, text, caption });
            else
                return (DialogResult)MessageBox.Show(text, caption);
        }

        public delegate DialogResult _Show2(Form targetForm, string text, string caption, MessageBoxButtons iMessageBoxButtons, MessageBoxIcon iMessageBoxIcon);
        static public DialogResult Show(Form targetForm, string text, string caption, MessageBoxButtons iMessageBoxButtons, MessageBoxIcon iMessageBoxIcon)
        {
            if (targetForm.InvokeRequired)
                return (DialogResult)targetForm.Invoke(new _Show2(Show), new object[] { targetForm, text, caption, iMessageBoxButtons , iMessageBoxIcon });
            else
                return (DialogResult)MessageBox.Show(text, caption, iMessageBoxButtons, iMessageBoxIcon);
        }
    }
}
