using System.Windows.Forms;

namespace ITA.Common.UI
{
    public class RichMessageBox
    {
        //
        // MessageBox compatible API: part 1 - with no owner specified
        //
        public static DialogResult Show(string Text)
        {
            return ErrorMessageBox.Show(null, "", Text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(string Text, string Caption)
        {
            return ErrorMessageBox.Show(null, "", Text, Caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(string Text, string Caption, MessageBoxButtons Buttons )
        {
            return ErrorMessageBox.Show(null, "", Text, Caption, Buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon)
        {
            return ErrorMessageBox.Show(null, "", Text, Caption, Buttons, Icon, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon, MessageBoxDefaultButton DefaultButton)
        {
            return ErrorMessageBox.Show(null, "", Text, Caption, Buttons, Icon, DefaultButton, 0);
        }
        //
        // MessageBox compatible API: part 2 - with owner specified
        //
        public static DialogResult Show(IWin32Window Owner, string Text)
        {
            return ErrorMessageBox.Show(Owner, "", Text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, string Text, string Caption)
        {
            return ErrorMessageBox.Show(Owner, "", Text, Caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, string Text, string Caption, MessageBoxButtons Buttons)
        {
            return ErrorMessageBox.Show(Owner, "", Text, Caption, Buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon)
        {
            return ErrorMessageBox.Show(Owner, "", Text, Caption, Buttons, Icon, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon, MessageBoxDefaultButton DefaultButton)
        {
            return ErrorMessageBox.Show(Owner, "", Text, Caption, Buttons, Icon, DefaultButton, 0);
        }
        //
        // Extended API: part 1 - with details provided, no owner specified
        //
        public static DialogResult Show(string Details, string Text, string Caption)
        {
            return ErrorMessageBox.Show(null, Details, Text, Caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(string Details, string Text, string Caption, MessageBoxButtons Buttons)
        {
            return ErrorMessageBox.Show(null, Details, Text, Caption, Buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(string Details, string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon)
        {
            return ErrorMessageBox.Show(null, Details, Text, Caption, Buttons, Icon, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(string Details, string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon, MessageBoxDefaultButton DefButton)
        {
            return ErrorMessageBox.Show(null, Details, Text, Caption, Buttons, Icon, DefButton, 0);
        }
        //
        // Extended API: part 2 - with details provided, owner specified
        //
        public static DialogResult Show(IWin32Window Owner, string Details, string Text, string Caption)
        {
            return ErrorMessageBox.Show(Owner, Details, Text, Caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, string Details, string Text, string Caption, MessageBoxButtons Buttons)
        {
            return ErrorMessageBox.Show(Owner, Details, Text, Caption, Buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, string Details, string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon)
        {
            return ErrorMessageBox.Show(Owner, Details, Text, Caption, Buttons, Icon, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, string Details, string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon, MessageBoxDefaultButton DefButton)
        {
            return ErrorMessageBox.Show(Owner, Details, Text, Caption, Buttons, Icon, DefButton, 0);
        }
    }
}
