using System;
using System.Drawing;
using System.Windows.Forms;

namespace ITA.Common.UI
{
    public partial class BriefErrorDescription : UserControl
    {
        private IErrorSource m_Error;
        private LocaleMessages m_ErrorMessages;
        private String m_ErrorTitle = String.Empty;
        private int m_Level;
        private DateTime m_Timestamp;

        public BriefErrorDescription()
        {
            InitializeComponent();

        	Image arrow = ErrorMessageBox.GetPicture(EPictureType.Arrow);

			if (arrow != null)
			{
				pictureBoxArrow.Image = arrow;
			}


        }

        #region Events

        private void BriefErrorDescription_Load(object sender, EventArgs e)
        {
            try
            {
                string Message = Error.Message;

                if (!string.IsNullOrEmpty(Error.LocalizedMessage))
                {
                    Message = Error.LocalizedMessage;
                }

                labelMessage.Text = ExceptionHelper.TerminateMessage(Message);
                ReLayout();
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                ReLayout();
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var Details = new ExceptionViewForm
                                  {
                                      ErrorMessages = m_ErrorMessages,
                                      Error = m_Error,
                                      ErrorTitle = ErrorTiltle,
                                      Timestamp = m_Timestamp
                                  };
                Details.ShowDialog(this);
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void label1_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                linkLabelDetails.Left = labelMessage.Right;
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        private void BriefErrorDescription_ParentChanged(object sender, EventArgs e)
        {
            try
            {
                if (Parent != null)
                {
                    Parent.SizeChanged += Parent_SizeChanged;
                }
            }
            catch (Exception Unexpected)
            {
                Utils.HandleUnexpectedError(Unexpected);
            }
        }

        #endregion

        #region Properties

        public LocaleMessages ErrorMessages
        {
            get { return m_ErrorMessages; }
            set { m_ErrorMessages = value; }
        }

        public String ErrorTiltle
        {
            get { return m_ErrorTitle; }
            set { m_ErrorTitle = value; }
        }

        public bool Details
        {
            get 
            { 
                return linkLabelDetails.Visible; 
            }
            set
            {
                linkLabelDetails.Visible = value;
                linkLabelDetails.Enabled = value;
            }
        }

        public int Level
        {
            get 
            { 
                return m_Level; 
            }
            set
            {
                m_Level = value;
                panelIdent.Width = 16*value + 10;

                pictureBoxArrow.Visible = m_Level > 0;
            }
        }

        public IErrorSource Error
        {
            get { return m_Error; }
            set { m_Error = value; }
        }

        public DateTime Timestamp
        {
            get { return m_Timestamp; }
            set { m_Timestamp = value; }
        }

        #endregion

        public int RecommendedWidth
        {
            get
            {
                Size RecommendedLabelSize = TextRenderer.MeasureText(labelMessage.Text, labelMessage.Font);
                return panelContents.Left + RecommendedLabelSize.Width + linkLabelDetails.Width + Padding.Horizontal;
            }
        }

        private void ReLayout()
        {
            //
            // Always be the same width as a parent
            //
            Width = Parent.ClientSize.Width;
            //
            // Let's figure out the width of a text. It might be less or equal to available space 
            // so it never goes behind the right side
            //
            Size RecommendedLabelSize = TextRenderer.MeasureText(labelMessage.Text, labelMessage.Font);

            int MaxAvailableWidth = Width - panelContents.Left - linkLabelDetails.Width - Padding.Horizontal;
            int NewLabelWidth = Math.Min(MaxAvailableWidth, RecommendedLabelSize.Width);
            labelMessage.Width = NewLabelWidth;
            //
            // So, now having the width determined let's calculate the height assuming that text will be wrapped instead of clipped
            //
            RecommendedLabelSize = TextRenderer.MeasureText(labelMessage.Text, labelMessage.Font, labelMessage.Size,
                                                            TextFormatFlags.WordBreak);
            Height = RecommendedLabelSize.Height + Padding.Vertical;
        }
    }
}