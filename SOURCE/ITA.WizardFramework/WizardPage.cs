using System.ComponentModel;
using System.Windows.Forms;

namespace ITA.WizardFramework
{
	/// <summary>
	/// Summary description for WizardPage.
	/// </summary>
	public class WizardPage : UserControl
	{
        protected Wizard m_Parent;
        protected int NextPageIndex = 1;

        /// <summary> 
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public WizardPage()
		{
			InitializeComponent();
		    SuppressPageHistory = false;
		}

        public Wizard Wizard
		{
			get
			{
				return m_Parent;
			}
			set
			{
				m_Parent = value;
			}
		}

        /// <summary>   Gets or sets a value indicating whether to suppress page history. </summary>
        ///
        /// <value> true if page should not be put in page history list. </value>
        public bool SuppressPageHistory { get; set; }

		public virtual bool OnValidate ()
		{
			return true;
		}

		public virtual void OnFinish ( ref bool bCancel )
		{
		}


		public virtual void OnCancel()
		{
		}

		public virtual void OnActive()
		{
		}

		public virtual void OnNext ( ref int NextIndex )
		{
		}

		public virtual void OnPrev ( ref int Steps )
		{
		}

		public virtual int GetNextIndex ( int CurrentIndex )
		{
			return CurrentIndex + NextPageIndex;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardPage));
            this.SuspendLayout();
            // 
            // WizardPage
            // 
            this.Name = "WizardPage";
            resources.ApplyResources(this, "$this");
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ResumeLayout(false);
		}
		#endregion
	}
}
