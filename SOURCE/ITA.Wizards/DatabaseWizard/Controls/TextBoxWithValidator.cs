using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ITA.Wizards.DatabaseWizard.Controls
{
    /// <summary>
    /// TextBox with validation hint label & color
    /// </summary>
    public partial class TextBoxWithValidator : UserControl
    {
        public delegate string HintDelegate(string text);

        public event HintDelegate HintEvent;
        
        protected string OnHintEvent(string text)
        {
            if (HintEvent != null)
            {
                return this.HintEvent(text);
            }

            return null;
        }

        public TextBoxWithValidator()
        {
            InitializeComponent();
        }
       
        [Browsable(true)]
        [Localizable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TextBoxWidth
        {
            get
            {
                return this.textBoxToValidate.Width;
            }
            set
            {
                this.textBoxToValidate.Width = value;
            }
        }

        [Browsable(true)]
        [Localizable(false)]
        public new string Text
        {
            get
            {
                return this.textBoxToValidate.Text;
            }
            set
            {
                this.textBoxToValidate.Text = value;
            }
        }

        [Localizable(false)]
        public new bool Enabled
        {
            get
            {
                return this.textBoxToValidate.Enabled;
            }
            set
            {
                this.textBoxToValidate.Enabled = value;
            }
        }

        private bool _isValid = false;

        public bool IsValid
        {
            get
            {
                if (this.textBoxToValidate.Visible == false || this.textBoxToValidate.Enabled == false)
                    return true;

                return this._isValid;
            }
            set
            {
                this._isValid = value;
            }
        }

        private void textBoxConfirm_TextChanged(object sender, EventArgs e)
        {
            this.ValidateInput();
        }

        private void textBoxConfirm_Enter(object sender, EventArgs e)
        {
            this.ValidateInput();
        }

        private void ValidateInput()
        {
            if (this.textBoxToValidate.Visible)
            {
                string hint = this.OnHintEvent(this.textBoxToValidate.Text);

                if (!string.IsNullOrEmpty(hint))
                {
                    this.textBoxToValidate.BackColor = this.textBoxToValidate.Enabled ? Color.LightPink : Color.Empty;
                    this.labelHint.Visible = this.textBoxToValidate.Enabled;
                    this.labelHint.Text = hint;
                    this.IsValid = false;
                    this.RelocateHint();                    
                }
                else
                {
                    this.textBoxToValidate.BackColor = this.textBoxToValidate.Enabled ? Color.LightGreen : Color.Empty;
                    this.labelHint.Visible = false;
                    this.labelHint.Text = string.Empty;
                    this.IsValid = true;
                }
            }
            else
            {
                this.labelHint.Visible = false;
                this.labelHint.Text = string.Empty;
                this.IsValid = true;
            }
        }

        private void RelocateHint()
        {
            if (this.textBoxToValidate.Visible)
                this.labelHint.Location = new Point(this.Width - this.labelHint.Width, this.textBoxToValidate.Height);
        }       

        private void textBoxToValidate_EnabledChanged(object sender, EventArgs e)
        {
            this.ValidateInput();            
        }
    }
}
