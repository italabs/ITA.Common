using System;
using System.ComponentModel;
using System.Windows.Forms;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;
using ITA.Wizards.DatabaseWizard.Pages.AzureSql;
using ITA.Wizards.DatabaseWizard.Pages.MySql;
using ITA.Wizards.DatabaseWizard.Pages.Oracle;
using ITA.Wizards.DatabaseWizard.Pages.SqlServer;

namespace ITA.Wizards.DatabaseWizard
{
    /// <summary>
    /// Âûáîð îïåðàöèè - ïîäêëþ÷åíèå èëè ñîçäàíèå íîâîé ÁÄ
    /// </summary>
    public class SelectOperationPage : CustomPage
    {
        private IContainer components;
        private ImageList imageList1;
        private Label label1;
        private Label label2;
        private Label label3;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private RadioButton radioConnect;
        private RadioButton radioCreate;

        private string szOriginalCaption = string.Empty;

        public SelectOperationPage()
        {
            InitializeComponent();
			
			this.labelHint.Text = Messages.WIZ_SELECT_WIZARD_MODE;
			this.labelDescription.Text = Messages.WIZ_WIZARD_MODE_DESCRIPTION;
			this.radioCreate.Text = Messages.WIZ_CREATE_NEW_DB_MODE;
			this.radioConnect.Text = Messages.WIZ_CONNECT_TO_DB_MODE;
			this.label2.Text = Messages.WIZ_SELECT_WIZARD_MODE_DESC1;
			this.label1.Text = Messages.WIZ_SELECT_WIZARD_MODE_DESC2;
			this.label3.Text = Messages.WIZ_SELECT_WIZARD_MODE_DESC3;
        }

        public DatabaseWizardContext Context
        {
            get { return Wizard.Context.ValueOf<DatabaseWizardContext>(DatabaseWizardContext.ClassName); }
        }

        public override bool OnValidate()
        {
            if (radioCreate.Checked)
            {
                Context.DBProvider.CreateNewDatabase = true;
                Parent.Text = string.Format(Messages.WIZ_CAPTION_CREATE, szOriginalCaption);
            }
            else if (radioConnect.Checked)
            {
                Context.DBProvider.CreateNewDatabase = false;
                Parent.Text = string.Format(Messages.WIZ_CAPTION_CONNECT, szOriginalCaption);
            }

            return true;
        }

        public override void OnActive()
        {
            if (szOriginalCaption == string.Empty)
            {
                szOriginalCaption = m_Parent.Text;
            }
            else
            {
                m_Parent.Text = szOriginalCaption;
            }

            radioCreate.Checked = Context.DBProvider.CreateNewDatabase;
            radioConnect.Checked = !radioCreate.Checked;

            Wizard.EnableButton(Wizard.EButtons.CancelButton);
            Wizard.EnableButton(Wizard.EButtons.NextButton);
            Wizard.EnableButton(Wizard.EButtons.BackButton);
            Wizard.DisableButton(Wizard.EButtons.FinishButton);
            base.OnActive();
        }

        public override void OnPrev(ref int Steps)
        {
            
        }

        public override void OnNext(ref int NextIndex)
        {
            base.OnNext(ref NextIndex);

            DbProviderType providerType = ((DatabaseWizard) Wizard).DatabaseWizardContext.DBProvider.ProviderType;

            if (providerType == DbProviderType.MSSQL)
            {
                NextIndex = this.Wizard.GetPageIndex(typeof(SelectMSSQLServer).Name);                    
            }
            else if(providerType == DbProviderType.Oracle)
            {
                NextIndex = this.Wizard.GetPageIndex(typeof(SelectOracleServer).Name);       
            }
            else if (providerType == DbProviderType.MySQL)
            {
                NextIndex = this.Wizard.GetPageIndex(typeof(SelectMySqlServer).Name);       
            }
            else if (providerType == DbProviderType.AzureSQL)
            {
                NextIndex = this.Wizard.GetPageIndex(typeof(SelectAzureSqlServer).Name);       
            }
            else
            {
                throw new NotSupportedException(String.Format("Invalid provider type ({0}).", providerType));
            }
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectOperationPage));
            this.radioCreate = new System.Windows.Forms.RadioButton();
            this.radioConnect = new System.Windows.Forms.RadioButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.panelPlaceholder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Text = Messages.WIZ_SELECT_WIZARD_MODE;
            // 
            // labelDescription
            // 
            this.labelDescription.Text = Messages.WIZ_WIZARD_MODE_DESCRIPTION;
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this.label3);
            this.panelPlaceholder.Controls.Add(this.label1);
            this.panelPlaceholder.Controls.Add(this.pictureBox2);
            this.panelPlaceholder.Controls.Add(this.radioCreate);
            this.panelPlaceholder.Controls.Add(this.pictureBox1);
            this.panelPlaceholder.Controls.Add(this.label2);
            this.panelPlaceholder.Controls.Add(this.radioConnect);
            // 
            // radioCreate
            // 
            this.radioCreate.Checked = true;
            this.radioCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioCreate.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.radioCreate.Location = new System.Drawing.Point(50, 76);
            this.radioCreate.Name = "radioCreate";
            this.radioCreate.Size = new System.Drawing.Size(242, 24);
            this.radioCreate.TabIndex = 1;
            this.radioCreate.TabStop = true;
            this.radioCreate.Text = Messages.WIZ_CREATE_NEW_DB_MODE;
            // 
            // radioConnect
            // 
            this.radioConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioConnect.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.radioConnect.Location = new System.Drawing.Point(50, 149);
            this.radioConnect.Name = "radioConnect";
            this.radioConnect.Size = new System.Drawing.Size(272, 24);
            this.radioConnect.TabIndex = 2;
            this.radioConnect.Text = Messages.WIZ_CONNECT_TO_DB_MODE;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            // 
            // label2
            // 
            this.label2.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label2.Location = new System.Drawing.Point(12, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(440, 40);
            this.label2.TabIndex = 5;
            this.label2.Text = Messages.WIZ_SELECT_WIZARD_MODE_DESC1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 69);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(12, 146);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(67, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(381, 47);
            this.label1.TabIndex = 6;
            this.label1.Text = Messages.WIZ_SELECT_WIZARD_MODE_DESC2;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(67, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(381, 40);
            this.label3.TabIndex = 7;
            this.label3.Text = Messages.WIZ_SELECT_WIZARD_MODE_DESC3;
            // 
            // SelectOperationPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SelectOperationPage";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.panelPlaceholder.ResumeLayout(false);
            this.panelPlaceholder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}

