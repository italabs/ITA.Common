using System;
using System.ServiceModel;
using System.Windows.Forms;
using ITA.Common.Exceptions;
using ITA.Common.UI;

namespace ITA.Common.WCF.UI
{
    public static class ErrorMessageBox2
    {
        public static DialogResult Show(IWin32Window Owner, Exception Error, string Text, string Caption,
                                        MessageBoxButtons Buttons, MessageBoxIcon Icon,
                                        MessageBoxDefaultButton DefButton, MessageBoxOptions Options)
        {
            if (Error is FaultException<ServiceExceptionDetail>)
            {
                IErrorSource source = new ServiceExceptionDetailSource(((FaultException<ServiceExceptionDetail>)Error).Detail);

                return ErrorMessageBox.Show(Owner, source, Text, Caption, Buttons, Icon, DefButton, Options);
            }
            else if (Error is RestApiException)
            {
                IErrorSource source = new ServiceExceptionDetailSource(((RestApiException)Error).Detail);

                return ErrorMessageBox.Show(Owner, source, Text, Caption, Buttons, Icon, DefButton, Options);
            }
            else
                return ErrorMessageBox.Show(Owner, Error, Text, Caption, Buttons, Icon, DefButton, Options);
        }

        #region Show overridables

        public static DialogResult Show(Exception Error)
        {
            return Show(null, Error, "", Messages.E_ITA_ERROR, MessageBoxButtons.OK, MessageBoxIcon.None,
                        MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(Exception Error, string Text)
        {
            return Show(null, Error, Text, Messages.E_ITA_ERROR, MessageBoxButtons.OK, MessageBoxIcon.None,
                        MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(Exception Error, string Text, string Caption)
        {
            return Show(null, Error, Text, Caption, MessageBoxButtons.OK, MessageBoxIcon.None,
                        MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(Exception Error, string Text, string Caption, MessageBoxButtons Buttons)
        {
            return Show(null, Error, Text, Caption, Buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(Exception Error, string Text, string Caption, MessageBoxButtons Buttons,
                                        MessageBoxIcon Icon)
        {
            return Show(null, Error, Text, Caption, Buttons, Icon, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(Exception Error, string Text, string Caption, MessageBoxButtons Buttons,
                                        MessageBoxIcon Icon, MessageBoxDefaultButton DefButton)
        {
            return Show(null, Error, Text, Caption, Buttons, Icon, DefButton, 0);
        }


        public static DialogResult Show(IWin32Window Owner, Exception Error)
        {
            return Show(Owner, Error, "", Messages.E_ITA_ERROR, MessageBoxButtons.OK, MessageBoxIcon.None,
                        MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, Exception Error, string Text)
        {
            return Show(Owner, Error, Text, Messages.E_ITA_ERROR, MessageBoxButtons.OK, MessageBoxIcon.None,
                        MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, Exception Error, string Text, string Caption)
        {
            return Show(Owner, Error, Text, Caption, MessageBoxButtons.OK, MessageBoxIcon.None,
                        MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, Exception Error, string Text, string Caption,
                                        MessageBoxButtons Buttons)
        {
            return Show(Owner, Error, Text, Caption, Buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, Exception Error, string Text, string Caption,
                                        MessageBoxButtons Buttons, MessageBoxIcon Icon)
        {
            return Show(Owner, Error, Text, Caption, Buttons, Icon, MessageBoxDefaultButton.Button1, 0);
        }

        public static DialogResult Show(IWin32Window Owner, Exception Error, string Text, string Caption,
                                        MessageBoxButtons Buttons, MessageBoxIcon Icon,
                                        MessageBoxDefaultButton DefButton)
        {
            return Show(Owner, Error, Text, Caption, Buttons, Icon, DefButton, 0);
        }

        #endregion
    }
}
