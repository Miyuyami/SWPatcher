using System.Windows.Forms;

namespace SWPatcherTEST.Helpers
{
    public static class MsgBox
    {
        //public static messages...

        public static void Error(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
        }

        public static DialogResult ErrorRetry(string message)
        {
            return MessageBox.Show(message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
        }

        public static DialogResult Question(string message)
        {
            return MessageBox.Show(message, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
        }

        public static void Success(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
        }

        public static void Default(string message, string title, MessageBoxButtons button, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, button, icon, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
        }
    }
}
