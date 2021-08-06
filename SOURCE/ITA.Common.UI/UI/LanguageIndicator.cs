using System;
using System.Windows.Forms;

namespace ITA.Common.UI
{
    /// <summary>
    /// Language indicator UI control
    /// </summary>
    public partial class LanguageIndicator : UserControl
    {
        private Control m_AttachedTo;

        public LanguageIndicator()
        {
            InitializeComponent();
            Visible = false;
        }

        public Control AttachedTo
        {
            get { return m_AttachedTo; }
            set
            {
                if (m_AttachedTo != null)
                {
                    m_AttachedTo.Enter -= AttachedTo_Enter;
                    m_AttachedTo.Leave -= AttachedTo_Leave;
                }

                m_AttachedTo = value;

                if (m_AttachedTo != null)
                {
                    m_AttachedTo.Enter += AttachedTo_Enter;
                    m_AttachedTo.Leave += AttachedTo_Leave;
                }
            }
        }

        private void ParentForm_InputLanguageChanged(object sender, InputLanguageChangedEventArgs e)
        {
            labelLanguage.Text = e.Culture.TwoLetterISOLanguageName;
        }

        private void LanguageIndicator_Load(object sender, EventArgs e)
        {
            labelLanguage.Text = Application.CurrentInputLanguage.Culture.TwoLetterISOLanguageName;

            if (ParentForm != null)
            {
                ParentForm.InputLanguageChanged += ParentForm_InputLanguageChanged;
            }
        }

        private void AttachedTo_Leave(object sender, EventArgs e)
        {
            HideLanguageBox();
        }

        private void AttachedTo_Enter(object sender, EventArgs e)
        {
            ShowLanguageBox();
        }

        private void ShowLanguageBox()
        {
            if (m_AttachedTo != null)
            {
                Visible = true;
            }
        }

        private void HideLanguageBox()
        {
            Visible = false;
        }
    }
}