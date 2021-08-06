using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ITA.WizardFramework
{
	/// <summary>
	/// Summary description for Wizard.
	/// </summary>
	public class Wizard : Form
	{
		public enum EButtons
		{
			BackButton = 1,
			NextButton = 2,
			CancelButton = 4,
			FinishButton = 8
		}

		private Hashtable m_Pages  = new Hashtable();
        private ArrayList m_Path = new ArrayList();
        
        private int m_CurrentIndex = 0;
        private WizardContext m_Context;

        private Image m_VerticalBannerImage;
        private Image m_HorizontalBannerImage;
        private bool m_bWarnOnCancel = true;
        public bool bCanClose = true;
        
		private Panel PagePanel;
		private Panel ButtonsPanel;
		private Button BackButton;
		private Button NextButton;
		private Button FinishButton;
		private Button CancButton;
        private GroupBox HorizontalRule;
        /// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public Wizard()
		{
            this.InitializeComponent();

			this.BackButton.Text = Messages.I_ITA_BACK;
			this.NextButton.Text = Messages.I_ITA_NEXT;
			this.FinishButton.Text = Messages.I_ITA_FINISH;
			this.CancButton.Text = Messages.I_ITA_CANCEL;
			this.Text = Messages.I_ITA_CAPTION;

		}       

        #region Form events

        private void Wizard_Load(object sender, EventArgs e)
        {
            DisableButton(EButtons.BackButton);

            if (m_Pages.Count > 1)
            {
                HideButton(EButtons.FinishButton);
                ShowButton(EButtons.NextButton);
            }
            else
            {
                HideButton(EButtons.NextButton);
                ShowButton(EButtons.FinishButton);
            }

            m_CurrentIndex = 0;
            m_Path.Add(m_CurrentIndex);
            ShowPage(m_CurrentIndex);
        }

        private void Wizard_Closing(object sender, CancelEventArgs e)
        {
            if ( !bCanClose )
            {
                //
                // Closing wizard is explicitly prohibited by application logic
                //
                e.Cancel = true;
                return;
            }

            if ( DialogResult == DialogResult.Cancel || DialogResult == DialogResult.None )
            {
                //
                // We're about to cancel the wizard
                // It could be a Cancel button click, Close button click, Alt-F4 hot-key or similiar action
                //
                if ( WarnOnCancel )
                {
                    if ( DialogResult.Yes != MessageBox.Show ( this, Messages.I_ITA_ARE_YOU_SURE, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) )
                    {
                        //
                        // User do not confirm the exit
                        //
                        e.Cancel = true;
                        return;
                    }
                }
                //
                // Have to let all pages know that we're leaving
                //
                foreach ( WizardPage Page in m_Pages.Values )
                {
                    Page.OnCancel ();
                }
            }
        }

        #endregion

        #region Button control

        public void EnableButton ( EButtons button )
		{
            if ( EButtons.BackButton == button )
			{
				BackButton.Enabled = true;
			}
            else if ( EButtons.CancelButton == button )
			{
				CancButton.Enabled = true;
			}
            else if ( EButtons.FinishButton == button )
			{
				FinishButton.Enabled = true;
			}
            else if ( EButtons.NextButton == button )
			{
				NextButton.Enabled = true;
			}
		}

		public void DisableButton ( EButtons button )
		{
			if ( EButtons.BackButton == button )
			{
				BackButton.Enabled = false;
			}
			else if ( EButtons.CancelButton == button )
			{
				CancButton.Enabled = false;
			}
			else if ( EButtons.FinishButton == button )
			{
				FinishButton.Enabled = false;
			}
			else if ( EButtons.NextButton == button )
			{
				NextButton.Enabled = false;
			}
		}

		public void ShowButton ( EButtons button )
		{
			if ( EButtons.BackButton == button )
			{
				BackButton.Visible = true;
			}
			else if ( EButtons.CancelButton == button )
			{
				CancButton.Visible = true;
			}
			else if ( EButtons.FinishButton == button )
			{
				FinishButton.Visible = true;
			}
			else if ( EButtons.NextButton == button )
			{
				NextButton.Visible = true;
			}
		}

		public void HideButton ( EButtons button )
		{
			if ( EButtons.BackButton == button )
			{
				BackButton.Visible = false;
			}
			else if ( EButtons.CancelButton == button )
			{
				CancButton.Visible = false;
			}
			else if ( EButtons.FinishButton == button )
			{
				FinishButton.Visible = false;
			}
			else if ( EButtons.NextButton == button )
			{
				NextButton.Visible = false;
			}
        }

        #endregion

        #region Page control

        public void RemovePage(int index)
        {
            m_Pages.Remove(index);
        }

        public void AddPage(WizardPage Page)
        {
            m_Pages[m_Pages.Count] = Page;

            Page.Wizard = this;
            Page.Visible = false;
        }

        public void AddPage(int Index, WizardPage Page)
        {
            m_Pages[Index] = Page;

            Page.Wizard = this;
            Page.Visible = false;
        }

        public int GetPageIndex(string Page)
        {
            for (int i = 0; i < m_Pages.Count; i++)
            {
                if (m_Pages[i].GetType().Name == Page)
                    return i;
            }
            return -1;
        }
        
        private void ShowPage(int Index)
        {
            WizardPage Page = m_Pages[Index] as WizardPage;

            if (null != Page)
            {
                Page.Parent = this.PagePanel;
                Page.Dock = DockStyle.Fill;
                Page.Visible = true;
                Page.OnActive();
            }
        }

        private void HidePage(int Index)
        {
            WizardPage Page = m_Pages[Index] as WizardPage;
            Page.Visible = false;
        }


        private bool SwitchPage(int NextIndex, bool bValidate)
        {
            WizardPage CurPage = m_Pages[m_CurrentIndex] as WizardPage;

            if (null != CurPage)
            {
                if (bValidate && !CurPage.OnValidate())
                {
                    return false;
                }

                HidePage(m_CurrentIndex);
            }

            m_CurrentIndex = NextIndex;

            if (NextIndex > 0)
            {
                EnableButton(EButtons.BackButton);
            }
            else
            {
                DisableButton(EButtons.BackButton);
            }

            if (NextIndex < m_Pages.Count - 1)
            {
                ShowButton(EButtons.NextButton);
                HideButton(EButtons.FinishButton);

                NextButton.Focus();
            }
            else
            {
                HideButton(EButtons.NextButton);
                ShowButton(EButtons.FinishButton);

                FinishButton.Focus();
            }

            ShowPage(NextIndex);

            return true;
        }

		public void NextPage ()
		{
			NextButton_Click ( null, null );
		}

		public void PrevPage ()
		{
			BackButton_Click ( null, null );
        }

        /// <summary>
        /// Событие
        /// </summary>
        public event EventHandler OnRepeatPage;

        public void RepeatPage()
        {
            if (OnRepeatPage != null)
            {
                OnRepeatPage(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Properties

        public Image VerticalBanner
        {
            get { return m_VerticalBannerImage; }
            set
            {
                m_VerticalBannerImage = value;
                foreach (var mPage in m_Pages.Values)
                {
                    var page = mPage as FinishPage;
                    if (page != null)
                    {
                        page.VerticalBanner = value;
                    }
                    else
                    {
                        var wPage = mPage as WelcomePage;
                        if (wPage != null)
                        {
                            wPage.VerticalBanner = value;
                        }
                    }
                }
            }
        }

        public Image HorizontalBanner
        {
            get { return m_HorizontalBannerImage; }
            set { 
                m_HorizontalBannerImage = value;
                foreach (var mPage in m_Pages.Values)
                {
                    var page = mPage as CustomPage;
                    if (page != null)
                    {
                        page.HorizontalBanner = value;
                    }
                }
            }
        }

        public WizardContext Context
        {
            get 
            {
                if (m_Context == null)
                {
                    m_Context = new WizardContext();
                }
                return m_Context; 
            }
            set 
            { 
                m_Context = value; 
            }
        }

        public bool WarnOnCancel
        {
            get { return m_bWarnOnCancel; }
            set { m_bWarnOnCancel = value; }
        }

        #endregion
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wizard));
            this.PagePanel = new System.Windows.Forms.Panel();
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.HorizontalRule = new System.Windows.Forms.GroupBox();
            this.BackButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.FinishButton = new System.Windows.Forms.Button();
            this.CancButton = new System.Windows.Forms.Button();
            this.ButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // PagePanel
            // 
            this.PagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PagePanel.Location = new System.Drawing.Point(0, 0);
            this.PagePanel.Name = "PagePanel";
            this.PagePanel.Size = new System.Drawing.Size(497, 313);
            this.PagePanel.TabIndex = 1;
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Controls.Add(this.HorizontalRule);
            this.ButtonsPanel.Controls.Add(this.BackButton);
            this.ButtonsPanel.Controls.Add(this.NextButton);
            this.ButtonsPanel.Controls.Add(this.FinishButton);
            this.ButtonsPanel.Controls.Add(this.CancButton);
            this.ButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPanel.Location = new System.Drawing.Point(0, 313);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(497, 47);
            this.ButtonsPanel.TabIndex = 0;
            // 
            // HorizontalRule
            // 
            this.HorizontalRule.Location = new System.Drawing.Point(-12, 0);
            this.HorizontalRule.Name = "HorizontalRule";
            this.HorizontalRule.Size = new System.Drawing.Size(530, 2);
            this.HorizontalRule.TabIndex = 4;
            this.HorizontalRule.TabStop = false;
            // 
            // BackButton
            // 
            this.BackButton.BackColor = System.Drawing.SystemColors.Control;
            this.BackButton.Location = new System.Drawing.Point(254, 13);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(72, 23);
            this.BackButton.TabIndex = 2;
            this.BackButton.Text = global::ITA.WizardFramework.Messages.I_ITA_BACK;
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(329, 13);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(72, 23);
            this.NextButton.TabIndex = 0;
            this.NextButton.Text = global::ITA.WizardFramework.Messages.I_ITA_NEXT;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // FinishButton
            // 
            this.FinishButton.Location = new System.Drawing.Point(329, 14);
            this.FinishButton.Name = "FinishButton";
            this.FinishButton.Size = new System.Drawing.Size(72, 22);
            this.FinishButton.TabIndex = 0;
            this.FinishButton.Text = global::ITA.WizardFramework.Messages.I_ITA_FINISH;
            this.FinishButton.Click += new System.EventHandler(this.FinishButton_Click);
            // 
            // CancButton
            // 
            this.CancButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancButton.Location = new System.Drawing.Point(414, 13);
            this.CancButton.Name = "CancButton";
            this.CancButton.Size = new System.Drawing.Size(72, 23);
            this.CancButton.TabIndex = 1;
            this.CancButton.Text = global::ITA.WizardFramework.Messages.I_ITA_CANCEL;
            this.CancButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // Wizard
            // 
            this.AcceptButton = this.NextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.CancButton;
            this.ClientSize = new System.Drawing.Size(497, 360);
            this.Controls.Add(this.PagePanel);
            this.Controls.Add(this.ButtonsPanel);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Wizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = Messages.I_ITA_CAPTION;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Wizard_Closing);
            this.Load += new System.EventHandler(this.Wizard_Load);
            this.ButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        #region Button events

        private void FinishButton_Click(object sender, EventArgs e)
		{
			bool bCancel = false;
			foreach ( WizardPage Page in m_Pages.Values )
			{
				Page.OnFinish ( ref bCancel );
				if ( bCancel )
					return;
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

        private void CancelButton_Click ( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close ();
        }

		private void NextButton_Click(object sender, EventArgs e)
		{
			WizardPage CurPage = m_Pages [ m_CurrentIndex ] as WizardPage;
			int NextIndex = CurPage.GetNextIndex( m_CurrentIndex );

			CurPage.OnNext ( ref NextIndex );

			if ( SwitchPage ( NextIndex, true ) )
			{
                WizardPage nextPage = m_Pages[NextIndex] as WizardPage;

                if (!nextPage.SuppressPageHistory)
			    {
                    m_Path.Add(NextIndex);
			    }
			}
		}

		private void BackButton_Click(object sender, EventArgs e)
		{
			WizardPage CurPage = m_Pages [ m_CurrentIndex ] as WizardPage;

			int Steps = 1;
			CurPage.OnPrev ( ref Steps );

			if ( Steps > m_Path.Count - 1)
			{
				Steps = m_Path.Count - 1;
			}

		    if (CurPage.SuppressPageHistory)
		    {
		        Steps--;
		    }

			m_Path.RemoveRange ( m_Path.Count - Steps, Steps );

			int PrevIndex = (int)m_Path[m_Path.Count - 1];
			SwitchPage ( PrevIndex, false );
		}

        #endregion

        #region Unattended mode

        public virtual int RunUnattended()
        {
            return RunUnattended(null);
        }

        public virtual int RunUnattended(string[] args)
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}
