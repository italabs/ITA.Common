using System;
using System.Drawing;
using System.Collections.Generic;

using System.Windows.Forms;

namespace ITA.Common.UI
{
    public partial class ErrorMessageBox : Form
    {
        private DateTime m_Timestamp = DateTime.Now;
        private MessageBoxButtons m_Buttons = MessageBoxButtons.OK;
        private MessageBoxDefaultButton m_DefButton = MessageBoxDefaultButton.Button1;
        private MessageBoxIcon m_Icon = MessageBoxIcon.None;
        private string m_Message = "{0}";
        private string m_Details = "";
        private MessageBoxOptions m_Options = 0;
        private bool m_bDetails;
        private int m_iButtonsAmount;

		private static Dictionary<EPictureType, Image> g_Images = new Dictionary<EPictureType, Image>();
        private static bool g_bShowTechnicalDetails = true;

        public ErrorMessageBox()
        {
            InitializeComponent();

			#region Override pictures

			Image topBack = ErrorMessageBox.GetPicture(EPictureType.TopBackground);

			if (topBack != null)
			{
				panelTop.BackgroundImage = topBack;
			}

        	Image upDetails = ErrorMessageBox.GetPicture(EPictureType.UpDetails);

			if (upDetails != null)
			{
				imageListDetails.Images.RemoveByKey("UpDetails.bmp");
				imageListDetails.Images.Add("UpDetails.bmp", upDetails);
			}

        	Image upDetailsFocused = ErrorMessageBox.GetPicture(EPictureType.UpDetailsFocused);

			if (upDetailsFocused != null)
			{
				imageListDetails.Images.RemoveByKey("UpDetailsFocused.bmp");
				imageListDetails.Images.Add("UpDetailsFocused.bmp", upDetailsFocused);
			}

        	Image downDetails = ErrorMessageBox.GetPicture(EPictureType.DownDetails);

			if (downDetails != null)
			{
				imageListDetails.Images.RemoveByKey("DownDetails.bmp");
				imageListDetails.Images.Add("DownDetails.bmp", downDetails);
			}

        	Image downDetailsFocused = ErrorMessageBox.GetPicture(EPictureType.DownDetailsFocused);

			if (downDetailsFocused != null)
			{
				imageListDetails.Images.RemoveByKey("DownDetailsFocused.bmp");
				imageListDetails.Images.Add("DownDetailsFocused.bmp", downDetailsFocused);
			}

			#endregion
		}

        #region Events

        private void AutoSizeLayout()
        {
            //
            // Let's define the maximum available space for the dialog which is:
            //    2/3 of screen width (without toolbars etc)
            //    80% of screen height (without toolbars etc)
            // So, basically, the dialog should never be clipped by screen bounds
            //
            Rectangle R = Screen.GetWorkingArea(Bounds);
            MaximumSize = new Size(R.Width*2/3, (int) (R.Height*0.8));
            //
            // Let's determine the recomended dialog width based on label text width
            //
            Size RecommendedLabelSize = TextRenderer.MeasureText(m_Message, labelMessage.Font);
            int RecommendedWidthByLabel = RecommendedLabelSize.Width + labelMessage.Padding.Horizontal +
                                          (pictureBoxIcon.Visible ? pictureBoxIcon.Width : 0);

            int NewWidth = Math.Max(MinimumSize.Width, Math.Min(MaximumSize.Width, RecommendedWidthByLabel));
            //
            // Now let's go thru the collection of controls and determine the largest width required 
            // but no wider than maximum determined
            //
            foreach (Control C in flowLayoutPanelErrors.Controls)
            {
                if (C is BriefErrorDescription)
                {
                    var D = (BriefErrorDescription)C;
                    NewWidth = Math.Max(Width, Math.Min(MaximumSize.Width, D.RecommendedWidth));
                }
            }

            Width = NewWidth;
        }

        private void ErrorMessageBox2_Load(object sender, EventArgs e)
        {
            try
            {
                //
                // Icon
                //
                switch (Picture)
                {
                    case MessageBoxIcon.Error:
                        pictureBoxIcon.Image = SystemIcons.Error.ToBitmap();
                        break;
                    case MessageBoxIcon.Warning:
                        pictureBoxIcon.Image = SystemIcons.Warning.ToBitmap();
                        break;
                    case MessageBoxIcon.Information:
                        pictureBoxIcon.Image = SystemIcons.Information.ToBitmap();
                        break;
                    case MessageBoxIcon.Question:
                        pictureBoxIcon.Image = SystemIcons.Question.ToBitmap();
                        break;
                    case MessageBoxIcon.None:
                        pictureBoxIcon.Image = null;
                        pictureBoxIcon.Width = 0;
                        break;
                    default:
                        pictureBoxIcon.Image = null;
                        pictureBoxIcon.Width = 0;
                        break;
                }
                //
                // Text
                //
                labelMessage.Text = ExceptionHelper.TerminateMessage(m_Message);

                if (String.IsNullOrEmpty(Details) && Error == null )
                {
                    panelTop.BackgroundImage = null;
                    panelTop.BackColor = panelContent.BackColor;
                    panelTop.Height = (int) ( panelTop.Height * 1.5 );
                    labelMessage.Font = panelContent.Font;
                }
                //
                // Details
                //
                if (!String.IsNullOrEmpty(Details))
                {
                    flowLayoutPanelErrors.Controls.Add(new Label () { Text = Details, TextAlign = ContentAlignment.MiddleCenter, AutoSize = true });
                }
                //
                // Exception(s)
                //
                IErrorSource X = Error;
                for (int i = 0; X != null; i++, X = X.InnerSource)
                {
                    flowLayoutPanelErrors.Controls.Add(new BriefErrorDescription
                                                           {
                                                               Level = i,
                                                               Error = X,
                                                               ErrorMessages = LocaleMessages.GlobalInstance,
                                                               ErrorTiltle = labelMessage.Text,
                                                               Timestamp = Timestamp
                                                           });
                }

                AutoSizeLayout();
                //
                // Details
                //
                bool bHaveDetails = flowLayoutPanelErrors.Controls.Count > 1;

                pictureBoxDetails.Visible = bHaveDetails;
                linkLabelDetails.Visible = bHaveDetails;
                pictureBoxDetails.Enabled = bHaveDetails;
                linkLabelDetails.Enabled = bHaveDetails;
                //
                // Buttons
                //
                switch (Buttons)
                {
                    case MessageBoxButtons.OK:
                    {
                        btn3.Text = Messages.I_ITA_COMMON_OK;
                        btn3.Visible = true;
                        btn3.DialogResult = DialogResult.OK;

                        AcceptButton = btn3;
                        CancelButton = btn3;
                        m_iButtonsAmount = 1;
                        break;
                    }
                    case MessageBoxButtons.OKCancel:
                    {
                        btn2.Text = Messages.I_ITA_COMMON_OK;
                        btn2.Visible = true;
                        btn2.DialogResult = DialogResult.OK;
                        AcceptButton = btn2;

                        btn3.Text = Messages.I_ITA_COMMON_CANCEL;
                        btn3.Visible = true;
                        btn3.DialogResult = DialogResult.Cancel;

                        CancelButton = btn3;
                        m_iButtonsAmount = 2;
                        break;
                    }
                    case MessageBoxButtons.RetryCancel:
                    {
                        btn2.Text = Messages.I_ITA_COMMON_RETRY;
                        btn2.Visible = true;
                        btn2.DialogResult = DialogResult.Retry;

                        btn3.Text = Messages.I_ITA_COMMON_CANCEL;
                        btn3.Visible = true;
                        btn3.DialogResult = DialogResult.Cancel;

                        CancelButton = btn3;
                        m_iButtonsAmount = 2;
                        break;
                    }
                    case MessageBoxButtons.YesNo:
                    {
                        btn2.Text = Messages.I_ITA_COMMON_YES;
                        btn2.Visible = true;
                        btn2.DialogResult = DialogResult.Yes;

                        btn3.Text = Messages.I_ITA_COMMON_NO;
                        btn3.Visible = true;
                        btn3.DialogResult = DialogResult.No;

                        CancelButton = btn3;
                        m_iButtonsAmount = 2;
                        break;
                    }
                    case MessageBoxButtons.YesNoCancel:
                    {
                        btn1.Text = Messages.I_ITA_COMMON_YES;
                        btn1.Visible = true;
                        btn1.DialogResult = DialogResult.Yes;

                        btn2.Text = Messages.I_ITA_COMMON_NO;
                        btn2.Visible = true;
                        btn2.DialogResult = DialogResult.No;

                        btn3.Text = Messages.I_ITA_COMMON_CANCEL;
                        btn3.Visible = true;
                        btn3.DialogResult = DialogResult.Cancel;

                        CancelButton = btn3;
                        m_iButtonsAmount = 3;
                        break;
                    }
                    case MessageBoxButtons.AbortRetryIgnore:
                    {
                        btn1.Text = Messages.I_ITA_COMMON_ABORT;
                        btn1.Visible = true;
                        btn1.DialogResult = DialogResult.Abort;

                        btn2.Text = Messages.I_ITA_COMMON_RETRY;
                        btn2.Visible = true;
                        btn2.DialogResult = DialogResult.Retry;

                        btn3.Text = Messages.I_ITA_COMMON_IGNORE;
                        btn3.Visible = true;
                        btn3.DialogResult = DialogResult.Ignore;

                        m_iButtonsAmount = 3;
                        break;
                    }
                }
                //
                // Details
                //
                ShowHideDetails();
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void ErrorMessageBox2_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                groupBoxHorizontalRule.Left = -10;
                groupBoxHorizontalRule.Width = Width + 10;
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void ErrorMessageBox2_Shown(object sender, EventArgs e)
        {
            try
            {
                //
                // DefButton
                //
                switch (DefButton)
                {
                    case MessageBoxDefaultButton.Button1:
                        {
                            switch (m_iButtonsAmount)
                            {
                                case 1:
                                    btn3.Focus();
                                    break;
                                case 2:
                                    btn2.Focus();
                                    break;
                                case 3:
                                    btn1.Focus();
                                    break;
                            }
                            break;
                        }
                    case MessageBoxDefaultButton.Button2:
                        {
                            switch (m_iButtonsAmount)
                            {
                                case 1:
                                    btn3.Focus();
                                    break;
                                case 2:
                                    btn3.Focus();
                                    break;
                                case 3:
                                    btn2.Focus();
                                    break;
                            }
                            break;
                        }
                    case MessageBoxDefaultButton.Button3:
                        {
                            switch (m_iButtonsAmount)
                            {
                                case 1:
                                    btn3.Focus();
                                    break;
                                case 2:
                                    btn3.Focus();
                                    break;
                                case 3:
                                    btn3.Focus();
                                    break;
                            }
                            break;
                        }
                }
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        #region Expand link

        private void pictureBoxDetails_Click(object sender, EventArgs e)
        {
            try
            {
                m_bDetails = !m_bDetails;
                ShowHideDetails();
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                m_bDetails = !m_bDetails;
                ShowHideDetails();
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                UpdateImage(true);
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                UpdateImage(false);
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        #endregion

        #endregion

        #region Properties

        public IErrorSource Error { get; set; }

        public MessageBoxIcon Picture
        {
            get { return m_Icon; }
            set { m_Icon = value; }
        }

        public MessageBoxButtons Buttons
        {
            get { return m_Buttons; }
            set { m_Buttons = value; }
        }

        public MessageBoxDefaultButton DefButton
        {
            get { return m_DefButton; }
            set { m_DefButton = value; }
        }

        public MessageBoxOptions Options
        {
            get { return m_Options; }
            set { m_Options = value; }
        }

        public string Message
        {
            get { return m_Message; }
            set { m_Message = !string.IsNullOrEmpty(value) ? value : ""; }
        }

        public string Details
        {
            get { return m_Details; }
            set { m_Details = !string.IsNullOrEmpty(value) ? value : ""; }
        }

        public DateTime Timestamp
        {
            get { return m_Timestamp; }
            set { m_Timestamp = value; }
        }

        #endregion

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

        public static DialogResult Show(Exception Error, string Text, string Caption, MessageBoxButtons Buttons,
                                        MessageBoxIcon Icon, DateTime created)
        {
            return Show(null, Error, Text, Caption, Buttons, Icon, MessageBoxDefaultButton.Button1, 0, created);
        }

        #endregion

        public static bool ShowTechnicalDetails
        {
            get 
            {
                return g_bShowTechnicalDetails;
            }
            set
            {
                g_bShowTechnicalDetails = value;
            }
        }

		/// <summary>
		/// Set picture for overriding defailt ErrorMessageBox pictures
		/// </summary>
		/// <param name="PType">Picture Type</param>
		/// <param name="Pic">Picture</param>
		public static void SetPicture(EPictureType PType, Image Pic)
		{
			g_Images[PType] = Pic;
		}

		internal static Image GetPicture(EPictureType PType)
		{
			if (g_Images.ContainsKey(PType))
			{
				return g_Images[PType];
			}
			else
			{
				return null;
			}
        }

        public static DialogResult Show(IWin32Window Owner, string Details, string Text, string Caption,
                                        MessageBoxButtons Buttons, MessageBoxIcon Icon,
                                        MessageBoxDefaultButton DefButton, MessageBoxOptions Options)
        {
            var dlg = new ErrorMessageBox();
            dlg.Text = Caption;
            dlg.Details = Details;
            dlg.Picture = Icon;
            dlg.Buttons = Buttons;
            dlg.DefButton = DefButton;
            dlg.Options = Options;
            dlg.Message = Text;

            if (Owner != null)
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
            }

            return dlg.ShowDialog(Owner);
        }

        public static DialogResult Show(IWin32Window Owner, IErrorSource ErrorSource, string Text, string Caption,
                                        MessageBoxButtons Buttons, MessageBoxIcon Icon,
                                        MessageBoxDefaultButton DefButton, MessageBoxOptions Options)
        {            
            var dlg = new ErrorMessageBox();
            dlg.Text = Caption;
            dlg.Error = ErrorSource;
            dlg.Picture = Icon;
            dlg.Buttons = Buttons;
            dlg.DefButton = DefButton;
            dlg.Options = Options;
            dlg.Message = Text;

            if (Owner != null)
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
            }

            return dlg.ShowDialog(Owner);
        }

        public static DialogResult Show(IWin32Window Owner, Exception Error, string Text, string Caption,
                                        MessageBoxButtons Buttons, MessageBoxIcon Icon,
                                        MessageBoxDefaultButton DefButton, MessageBoxOptions Options)
        {
            IErrorSource source = null;

            if (Error != null)
            {
                if (Error is ITARemoteException)
                {
                    source = new ExceptionDetailSource(((ITARemoteException)Error).Detail);
                }
                else
                {
                    source = new ExceptionSource(Error);
                }
            }

            var dlg = new ErrorMessageBox();
            dlg.Text = Caption;
            dlg.Error = source;
            dlg.Picture = Icon;
            dlg.Buttons = Buttons;
            dlg.DefButton = DefButton;
            dlg.Options = Options;
            dlg.Message = Text;

            if (Owner != null)
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
            }

            return dlg.ShowDialog(Owner);
        }

        public static DialogResult Show(IWin32Window Owner, Exception Error, string Text, string Caption,
                                        MessageBoxButtons Buttons, MessageBoxIcon Icon,
                                        MessageBoxDefaultButton DefButton, MessageBoxOptions Options, DateTime created)
        {
            IErrorSource source = null;

            if (Error != null)
            {
                if (Error is ITARemoteException)
                {
                    source = new ExceptionDetailSource(((ITARemoteException)Error).Detail);
                }
                else
                {
                    source = new ExceptionSource(Error);
                }
            }

            var dlg = new ErrorMessageBox();
            dlg.Text = Caption;
            dlg.Error = source;
            dlg.Picture = Icon;
            dlg.Buttons = Buttons;
            dlg.DefButton = DefButton;
            dlg.Options = Options;
            dlg.Message = Text;
            dlg.Timestamp = created;

            if (Owner != null)
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
            }

            return dlg.ShowDialog(Owner);
        }

        private void ShowHideDetails()
        {
            if (flowLayoutPanelErrors.Controls.Count == 1)
            {
                if (flowLayoutPanelErrors.Controls[0] is BriefErrorDescription)
                {
                    var C = (BriefErrorDescription)flowLayoutPanelErrors.Controls[0];
                    C.Details =  g_bShowTechnicalDetails;
                    C.TabStop =  g_bShowTechnicalDetails;
                }
            }
            else
            {
                foreach (Control C in flowLayoutPanelErrors.Controls)
                {
                    ((BriefErrorDescription) C).Details = m_bDetails && g_bShowTechnicalDetails;
                    C.TabStop = m_bDetails && g_bShowTechnicalDetails;
                }
            }

            int HeightOfStuff = Height - flowLayoutPanelErrors.Height;
            int ProposedHeight = 0;

            if (flowLayoutPanelErrors.Controls.Count == 0)
            {
                ProposedHeight = Height - panelContent.Height;
            }
            else if (m_bDetails)
            {
                int ControlsHeight = 0;
                foreach (Control C in flowLayoutPanelErrors.Controls)
                {
                    ControlsHeight += C.Height;
                }

                ProposedHeight = HeightOfStuff + ControlsHeight + flowLayoutPanelErrors.Padding.Vertical;
                linkLabelDetails.Text = Messages.I_ITA_COMMON_HIDE_DETAILS;
            }
            else
            {
                ProposedHeight = HeightOfStuff +
                                 (flowLayoutPanelErrors.Controls.Count > 0 ? flowLayoutPanelErrors.Controls[0].Height : 0) +
                                 flowLayoutPanelErrors.Padding.Vertical;
                linkLabelDetails.Text = Messages.I_ITA_COMMON_SHOW_DETAILS;
            }
            Height = Math.Max(ProposedHeight, MinimumSize.Height);
            UpdateImage(false);
        }

        private void UpdateImage(bool Hover)
        {
            if (m_bDetails)
            {
                if (Hover)
                {
                    pictureBoxDetails.Image = imageListDetails.Images["UpDetailsFocused.bmp"];
                }
                else
                {
                    pictureBoxDetails.Image = imageListDetails.Images["UpDetails.bmp"];
                }
            }
            else
            {
                if (Hover)
                {
                    pictureBoxDetails.Image = imageListDetails.Images["DownDetailsFocused.bmp"];
                }
                else
                {
                    pictureBoxDetails.Image = imageListDetails.Images["DownDetails.bmp"];
                }
            }
        }
    }

    internal class Utils
    {
        public static void HandleUnexpectedError(Exception X)
        {
            MessageBox.Show(X.Message, Messages.E_ITA_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static String BuildDetailedInfo(IErrorSource X)
        {
            if (X == null)
                return string.Empty;

            string S = string.Format(Messages.I_ITA_COMMON_EXCEPTION +
                                     Messages.I_ITA_COMMON_TYPE +
                                     Messages.I_ITA_COMMON_MESSAGE +
                                     Messages.I_ITA_COMMON_URL +
                                     Messages.I_ITA_COMMON_SOURCE +
                                     Messages.I_ITA_COMMON_METHOD +
                                     Messages.I_ITA_COMMON_PARAMS +
                                     Messages.I_ITA_COMMON_STACKTRACE +
                                     "\t-----------------------------------------------------------------------------\n",
                                     X.Type,
                                     X.Message,
                                     X.HelpLink,
                                     X.Source,
                                     X.TargetSite,
                                     X.Data,
                                     X.StackTrace);

            return X.InnerSource != null ? S += BuildDetailedInfo(X.InnerSource) : S;
        }
    }
}