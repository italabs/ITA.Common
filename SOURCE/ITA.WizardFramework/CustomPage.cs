using System.Drawing;
using System.Windows.Forms;

namespace ITA.WizardFramework
{
	/// <summary>
	/// Summary description for Operation.
	/// </summary>
    public class CustomPage : WizardPage
    {
		private Panel panelTop;
        private GroupBox HorizontalRule;
        protected Label labelHint;
        protected Label labelDescription;

        protected PictureBox pictureBoxImage;
        protected Panel panelPlaceholder;

		public CustomPage ()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		public override bool OnValidate()
		{
			return true;
		}

        #region Properties

        public Image HorizontalBanner
        {
            get 
            { 
                return panelTop.BackgroundImage; 
            }
            set
            {
                panelTop.BackgroundImage = value;
            }
        }

        #endregion

        /// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBoxImage = new System.Windows.Forms.PictureBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelHint = new System.Windows.Forms.Label();
            this.HorizontalRule = new System.Windows.Forms.GroupBox();
            this.panelPlaceholder = new System.Windows.Forms.Panel();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelTop.Controls.Add(this.pictureBoxImage);
            this.panelTop.Controls.Add(this.labelDescription);
            this.panelTop.Controls.Add(this.labelHint);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(497, 60);
            this.panelTop.TabIndex = 10;
            // 
            // pictureBoxImage
            // 
            this.pictureBoxImage.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxImage.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBoxImage.Location = new System.Drawing.Point(430, 0);
            this.pictureBoxImage.Name = "pictureBoxImage";
            this.pictureBoxImage.Size = new System.Drawing.Size(67, 60);
            this.pictureBoxImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxImage.TabIndex = 3;
            this.pictureBoxImage.TabStop = false;
            // 
            // labelDescription
            // 
            this.labelDescription.BackColor = System.Drawing.Color.Transparent;
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.labelDescription.Location = new System.Drawing.Point(12, 24);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(372, 32);
            this.labelDescription.TabIndex = 2;
            this.labelDescription.Text = "Please choose the action you want to perform.";
            // 
            // labelHint
            // 
            this.labelHint.BackColor = System.Drawing.Color.Transparent;
            this.labelHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.labelHint.Location = new System.Drawing.Point(3, 8);
            this.labelHint.Name = "labelHint";
            this.labelHint.Size = new System.Drawing.Size(224, 16);
            this.labelHint.TabIndex = 0;
            this.labelHint.Text = "Action";
            // 
            // HorizontalRule
            // 
            this.HorizontalRule.BackColor = System.Drawing.Color.Transparent;
            this.HorizontalRule.Dock = System.Windows.Forms.DockStyle.Top;
            this.HorizontalRule.Location = new System.Drawing.Point(0, 60);
            this.HorizontalRule.Name = "HorizontalRule";
            this.HorizontalRule.Size = new System.Drawing.Size(497, 3);
            this.HorizontalRule.TabIndex = 11;
            this.HorizontalRule.TabStop = false;
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPlaceholder.Location = new System.Drawing.Point(0, 63);
            this.panelPlaceholder.Name = "panelPlaceholder";
            this.panelPlaceholder.Size = new System.Drawing.Size(497, 250);
            this.panelPlaceholder.TabIndex = 12;
            // 
            // CustomPage
            // 
            this.Controls.Add(this.panelPlaceholder);
            this.Controls.Add(this.HorizontalRule);
            this.Controls.Add(this.panelTop);
            this.Name = "CustomPage";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
	}
}
