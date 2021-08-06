using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ITA.WizardFramework
{
    public class WelcomePage : WizardPage
    {
        protected Label labelWelcome;
		private Panel panelContent;
        private Panel panel2;
        protected Label labelAbout;
        protected Label labelContinue;
        protected Label labelNotes;
        private Panel panelLeft;
		private IContainer components = null;
        protected Color backColor;

		public WelcomePage ()
		{
			InitializeComponent();
			
            this.labelWelcome.Text = Messages.I_ITA_WELCOME_MESSAGE;
			this.labelContinue.Text = Messages.I_ITA_CONTINUE_MESSAGE;
			this.labelAbout.Text = Messages.I_ITA_ABOUT_MESSAGE;
            backColor = panelLeft.BackColor;
		}
        /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
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
        public string About
        {
            get { return labelAbout.Text; }
            set { labelAbout.Text = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Welcome
        {
            get { return labelWelcome.Text; }
            set { labelWelcome.Text = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Notes
        {
            get { return labelNotes.Text; }
            set { labelNotes.Text = value; }
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
			Wizard.EnableButton ( Wizard.EButtons.CancelButton );
			Wizard.EnableButton ( Wizard.EButtons.NextButton );
			Wizard.DisableButton ( Wizard.EButtons.BackButton );
			Wizard.DisableButton ( Wizard.EButtons.FinishButton );

			base.OnActive ();
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.labelWelcome = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelNotes = new System.Windows.Forms.Label();
            this.labelContinue = new System.Windows.Forms.Label();
            this.labelAbout = new System.Windows.Forms.Label();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelContent.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelWelcome
            // 
            this.labelWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.labelWelcome.Location = new System.Drawing.Point(24, 28);
            this.labelWelcome.Name = "labelWelcome";
            this.labelWelcome.Size = new System.Drawing.Size(293, 73);
            this.labelWelcome.TabIndex = 1;
            this.labelWelcome.Text = Messages.I_ITA_WELCOME_MESSAGE;
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
            this.panel2.Controls.Add(this.labelNotes);
            this.panel2.Controls.Add(this.labelContinue);
            this.panel2.Controls.Add(this.labelAbout);
            this.panel2.Controls.Add(this.labelWelcome);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(160, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(337, 313);
            this.panel2.TabIndex = 2;
            // 
            // labelNotes
            // 
            this.labelNotes.Location = new System.Drawing.Point(24, 161);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(293, 106);
            this.labelNotes.TabIndex = 4;
            // 
            // labelContinue
            // 
            this.labelContinue.Location = new System.Drawing.Point(24, 283);
            this.labelContinue.Name = "labelContinue";
            this.labelContinue.Size = new System.Drawing.Size(293, 28);
            this.labelContinue.TabIndex = 3;
            this.labelContinue.Text = Messages.I_ITA_CONTINUE_MESSAGE;
            // 
            // labelAbout
            // 
            this.labelAbout.Location = new System.Drawing.Point(24, 113);
            this.labelAbout.Name = "labelAbout";
            this.labelAbout.Size = new System.Drawing.Size(293, 48);
            this.labelAbout.TabIndex = 3;
            this.labelAbout.Text = Messages.I_ITA_ABOUT_MESSAGE;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(105)))), ((int)(((byte)(166)))));
            this.panelLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(160, 313);
            this.panelLeft.TabIndex = 3;
            // 
            // WelcomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContent);
            this.Name = "WelcomePage";
            this.panelContent.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
	}
}

