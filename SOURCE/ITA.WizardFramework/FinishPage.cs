using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ITA.WizardFramework
{
	/// <summary>
	/// Summary description for Finish.
	/// </summary>
    public class FinishPage : WizardPage
    {
        protected Label labelThankyou;
        protected Label labelCompletedText;
		protected Panel panelContent;
		protected Panel panel2;
        protected Label labelContinue;
        protected Color backColor;
        private Panel panelLeft;
        /// <summary> 
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public FinishPage ()
		{
			InitializeComponent();
		
            this.labelThankyou.Text = Messages.I_ITA_THANKYOU_MESSAGE;
			this.labelCompletedText.Text = Messages.I_ITA_COMPLETE_MESSAGE;
			this.labelContinue.Text = Messages.I_ITA_CLICK_CLOSE;
            backColor = panelLeft.BackColor;
        }

        #region Properties

        public Image VerticalBanner
        {
            get
            {
                return panelContent.BackgroundImage;
            }
            set
            {
                panelContent.BackgroundImage = value;
                if (panelContent.BackgroundImage != null)
                {
                    panelLeft.BackColor = Color.Transparent;
                }
                else
                {
                    panelLeft.BackColor = backColor;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Thankyou
        {
            get { return labelThankyou.Text; }
            set { labelThankyou.Text = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CompletedNotes
        {
            get { return labelCompletedText.Text; }
            set { labelCompletedText.Text = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Continue
        {
            get { return labelContinue.Text; }
            set { labelContinue.Text = value; }
        }

        #endregion

		public override void OnActive()
		{
			Wizard.DisableButton ( Wizard.EButtons.CancelButton );
			Wizard.DisableButton ( Wizard.EButtons.BackButton );
			Wizard.DisableButton ( Wizard.EButtons.NextButton );
			Wizard.EnableButton ( Wizard.EButtons.FinishButton );

			Wizard.bCanClose = false;
		}

		public override void OnFinish ( ref bool bCancel )
		{
			Wizard.bCanClose = true;		
			base.OnFinish ( ref bCancel );
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.labelThankyou = new System.Windows.Forms.Label();
            this.labelCompletedText = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelContinue = new System.Windows.Forms.Label();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelContent.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelThankyou
            // 
            this.labelThankyou.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.labelThankyou.Location = new System.Drawing.Point(24, 32);
            this.labelThankyou.Name = "labelThankyou";
            this.labelThankyou.Size = new System.Drawing.Size(288, 40);
            this.labelThankyou.TabIndex = 0;
            this.labelThankyou.Text = Messages.I_ITA_THANKYOU_MESSAGE;
            // 
            // labelCompletedText
            // 
            this.labelCompletedText.Location = new System.Drawing.Point(25, 89);
            this.labelCompletedText.Name = "labelCompletedText";
            this.labelCompletedText.Size = new System.Drawing.Size(287, 178);
            this.labelCompletedText.TabIndex = 2;
            this.labelCompletedText.Text = Messages.I_ITA_COMPLETE_MESSAGE;
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.White;
            this.panelContent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelContent.Controls.Add(this.panel2);
            this.panelContent.Controls.Add(this.panelLeft);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(497, 313);
            this.panelContent.TabIndex = 13;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.labelContinue);
            this.panel2.Controls.Add(this.labelCompletedText);
            this.panel2.Controls.Add(this.labelThankyou);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(160, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(337, 313);
            this.panel2.TabIndex = 3;
            // 
            // labelContinue
            // 
            this.labelContinue.Location = new System.Drawing.Point(25, 283);
            this.labelContinue.Name = "labelContinue";
            this.labelContinue.Size = new System.Drawing.Size(287, 28);
            this.labelContinue.TabIndex = 4;
            this.labelContinue.Text = Messages.I_ITA_CLICK_CLOSE;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(105)))), ((int)(((byte)(166)))));
            this.panelLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(160, 313);
            this.panelLeft.TabIndex = 4;
            // 
            // FinishPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContent);
            this.Name = "FinishPage";
            this.panelContent.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
	}
}
