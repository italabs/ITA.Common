using System;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

namespace ITA.Common.UI
{
    public partial class ExceptionViewForm : Form
    {
        private String m_ErrorTitle = string.Empty;

        public ExceptionViewForm()
        {
            InitializeComponent();

        	Image topBack = ErrorMessageBox.GetPicture(EPictureType.TopBackground);

			if (topBack != null)
			{
				panelTop.BackgroundImage = topBack;
			}
        }

        #region Properties

        public LocaleMessages ErrorMessages { get; set; }

        public String ErrorTitle
        {
            get { return m_ErrorTitle; }
            set { m_ErrorTitle = value; }
        }

        public IErrorSource Error { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion

        #region Events

        private void ExceptionViewForm_Load(object sender, EventArgs e)
        {
            try
            {
                string message = Error.Message;
                string localizedMsg = Error.LocalizedMessage;

                if (!string.IsNullOrEmpty(localizedMsg))
                {
                    message = localizedMsg;
                }

                labelTopMessage.Text = (String.IsNullOrEmpty(ErrorTitle)) ? message : ErrorTitle;
                labelMessage.Text = message;
                labelType.Text = Error.Type;
                richTextBox1.Text = Error.StackTrace;
                labelTimestamp.Text = Timestamp.ToString();

                linkLabelURL.Text = Error.HelpLink;
                linkLabelURL.Enabled = Error.HelpLinkEnabled;
                labelSource.Text = Error.Source;
                labelSite.Text = Error.TargetSite;
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void linkLabelURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(linkLabelURL.Text);
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void linkLabelClipboard_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string Info = Utils.BuildDetailedInfo(Error);
                Clipboard.SetText(Info, TextDataFormat.UnicodeText);
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void linkLabelReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string Info = Utils.BuildDetailedInfo(Error);
                Version v = Assembly.GetExecutingAssembly().GetName().Version;

                Process.Start(
                    string.Format(
                        "mailto:support@it-assist.ru?subject=Secret Disk Management Server {0}.{1}.{2}  (build {3})&body={4}",
                        v.Major, v.Minor, v.Build, v.Revision, Uri.EscapeDataString(Info)));
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        #endregion
    }
}