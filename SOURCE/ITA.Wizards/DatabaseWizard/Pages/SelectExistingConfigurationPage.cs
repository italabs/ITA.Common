using System.ComponentModel;
using System.Windows.Forms;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;

namespace ITA.Wizards.DatabaseWizard.Pages
{
    /// <summary>
    /// Выбор операции - использование существующей или создание новой конфигурации БД
    /// </summary>
    public class SelectExistingConfigurationPage : CustomPage
    {
        private IContainer components;
        private ImageList imageList1;
        private Label label1;
        private Label label2;
        private Label label3;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private RadioButton radioCreateNew;
        private RadioButton radioUseExisting;

        private string szOriginalCaption = string.Empty;

        public SelectExistingConfigurationPage()
        {
            InitializeComponent();

			this.labelHint.Text = Messages.WIZ_CONFIGURATION_SELECTING;
			this.labelDescription.Text = Messages.WIZ_SELECTING_CONFIGURATION_MODE;
			this.radioUseExisting.Text = Messages.WIZ_USING_EXISTING_CONFIG;
			this.radioCreateNew.Text = Messages.WIZ_CREATE_NEW_CONFIG;
			this.label2.Text = Messages.WIZ_SELECT_CONFIG_USING_MODE;
			this.label1.Text = Messages.WIZ_USING_EXISTING_CONFIG_DESCRIPTION;
			this.label3.Text = Messages.WIZ_NEW_CONFIG_DESCRIPTION;
        }

        public DatabaseWizardContext Context
        {
            get { return Wizard.Context.ValueOf<DatabaseWizardContext>(DatabaseWizardContext.ClassName); }
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

            Wizard.EnableButton(Wizard.EButtons.CancelButton);
            Wizard.EnableButton(Wizard.EButtons.NextButton);
            Wizard.EnableButton(Wizard.EButtons.BackButton);
            Wizard.DisableButton(Wizard.EButtons.FinishButton);
            base.OnActive();
        }

        public override void OnNext(ref int Steps)
        {
            if (radioCreateNew.Checked)
            {
                Steps++; //skip CheckExistingDatabasePage
				if (!((DatabaseWizard)Wizard).ShowSelectOperation)
					Steps++; //skip SelectOperationPage
            }
            base.OnPrev(ref Steps);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectExistingConfigurationPage));
            this.radioUseExisting = new System.Windows.Forms.RadioButton();
            this.radioCreateNew = new System.Windows.Forms.RadioButton();
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
            this.labelHint.Text = Messages.WIZ_CONFIGURATION_SELECTING;
            // 
            // labelDescription
            // 
            this.labelDescription.Text = Messages.WIZ_SELECTING_CONFIGURATION_MODE;
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this.label3);
            this.panelPlaceholder.Controls.Add(this.label1);
            this.panelPlaceholder.Controls.Add(this.pictureBox2);
            this.panelPlaceholder.Controls.Add(this.radioUseExisting);
            this.panelPlaceholder.Controls.Add(this.pictureBox1);
            this.panelPlaceholder.Controls.Add(this.label2);
            this.panelPlaceholder.Controls.Add(this.radioCreateNew);
            // 
            // radioUseExisting
            // 
            this.radioUseExisting.Checked = true;
            this.radioUseExisting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioUseExisting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioUseExisting.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.radioUseExisting.Location = new System.Drawing.Point(63, 76);
            this.radioUseExisting.Name = "radioUseExisting";
            this.radioUseExisting.Size = new System.Drawing.Size(242, 24);
            this.radioUseExisting.TabIndex = 1;
            this.radioUseExisting.TabStop = true;
            this.radioUseExisting.Text = Messages.WIZ_USING_EXISTING_CONFIG;
            // 
            // radioCreateNew
            // 
            this.radioCreateNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioCreateNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioCreateNew.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.radioCreateNew.Location = new System.Drawing.Point(63, 149);
            this.radioCreateNew.Name = "radioCreateNew";
            this.radioCreateNew.Size = new System.Drawing.Size(272, 24);
            this.radioCreateNew.TabIndex = 2;
            this.radioCreateNew.Text = Messages.WIZ_CREATE_NEW_CONFIG;
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
            this.label2.Text = Messages.WIZ_SELECT_CONFIG_USING_MODE;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ITA.Wizards.Properties.Resources.Gear48;
            this.pictureBox1.Location = new System.Drawing.Point(12, 69);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ITA.Wizards.Properties.Resources.Setup_Install48;
            this.pictureBox2.Location = new System.Drawing.Point(12, 146);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(48, 48);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(80, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(381, 47);
            this.label1.TabIndex = 6;
            this.label1.Text = Messages.WIZ_USING_EXISTING_CONFIG_DESCRIPTION;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(80, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(381, 40);
            this.label3.TabIndex = 7;
            this.label3.Text = Messages.WIZ_NEW_CONFIG_DESCRIPTION;
            // 
            // SelectExistingConfigurationPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SelectExistingConfigurationPage";
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