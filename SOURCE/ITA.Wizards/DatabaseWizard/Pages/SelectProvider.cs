using System;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ITA.Common.UI;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;
using ITA.Wizards.DatabaseWizard.Pages.SqlServer;

namespace ITA.Wizards.DatabaseWizard.Pages
{
    public class SelectProvider : CustomPage
    {
        private IContainer components;
        private Label label2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private RichTextBox richTextDetails;
        private RadioButton radioOracle;
        private RadioButton radioSQL;
        private RadioButton radioMySql;
        private Panel pnlProviders;
        private RadioButton radioAzureSQL;
        private ImageList imageList1;

        public SelectProvider()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

			this.labelHint.Text = Messages.WIZ_DB_PROVIDER;
			this.labelDescription.Text = Messages.WIZ_SELECT_DB_PROVIDER;
			this.label2.Text = Messages.WIZ_SELECT_DB_PROVIDER;
			this.groupBox3.Text = Messages.WIZ_DB_PROVIDER;

            (this.radioSQL.Image as Bitmap).MakeTransparent(Color.White);
            (this.radioOracle.Image as Bitmap).MakeTransparent(Color.White);
            (this.radioMySql.Image as Bitmap).MakeTransparent(Color.White);
        }


        private DatabaseWizardContext Context
        {
            get { return ((DatabaseWizard)Wizard).DatabaseWizardContext; }
        }

        public override void OnActive()
        {
            base.OnActive();

            //Ïîêàçûâàåì òîëüêî ïîääåðæèâàåìûå òèïû ÁÄ
            if (!this.Context.SupportedDbProviders.Contains(DbProviderType.MSSQL))
            {
                this.pnlProviders.Controls.Remove(this.radioSQL);
            }

            if (!this.Context.SupportedDbProviders.Contains(DbProviderType.Oracle))
            {
                this.pnlProviders.Controls.Remove(this.radioOracle);
            }

            if (!this.Context.SupportedDbProviders.Contains(DbProviderType.MySQL))
            {
                this.pnlProviders.Controls.Remove(this.radioMySql);
            }

            if (!this.Context.SupportedDbProviders.Contains(DbProviderType.AzureSQL))
            {
                this.pnlProviders.Controls.Remove(this.radioAzureSQL);
            }

            if (Context.DefaultDbProvider.HasValue && Context.DBProvider == null)
            {
                radioAzureSQL.Checked = Context.DefaultDbProvider == DbProviderType.AzureSQL;
                radioSQL.Checked = Context.DefaultDbProvider == DbProviderType.MSSQL;
                radioMySql.Checked = Context.DefaultDbProvider == DbProviderType.MySQL;
                radioOracle.Checked = Context.DefaultDbProvider == DbProviderType.Oracle;
            }
        }

        public override bool OnValidate()
        {
            Cursor cursor = this.Cursor;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (radioSQL.Checked)
                {
                    Context.SelectedDbProvider = DbProviderType.MSSQL;
                }
                else if (radioOracle.Checked)
                {
                    Context.SelectedDbProvider = DbProviderType.Oracle;
                }
                else if (radioMySql.Checked)
                {
                    Context.SelectedDbProvider = DbProviderType.MySQL;
                }
                else if (radioAzureSQL.Checked)
                {
                    Context.SelectedDbProvider = DbProviderType.AzureSQL;
                }
                else
                {
                    return false;
                }

                Context.DBProvider = Context.GetDbProviderByType(Context.SelectedDbProvider);
                Context.DBProvider.CheckDbPresence();

                return true;
            }
            catch (Exception x)
            {
                Wizard.DisableButton(Wizard.EButtons.NextButton);
                ErrorMessageBox.Show(this.Wizard, x,
                                     String.Format(Messages.WIZ_SRV_DB_PROVIDER_IS_NOT_AVAILABLE,
                                                   Context.SelectedDbProvider), m_Parent.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);

                return false;
            }
            finally
            {
                this.Cursor = cursor;
            }
        }
             
        public override void OnNext(ref int NextIndex)
        {
			if (!((DatabaseWizard)Wizard).CheckExistingConfiguration)
            {
                if (!((DatabaseWizard) Wizard).ShowSelectOperation)
                {
                    NextIndex = this.Wizard.GetPageIndex(typeof(SelectMSSQLServer).Name);                    
                }
                else
                {
                    NextIndex = this.Wizard.GetPageIndex(typeof(SelectOperationPage).Name);                    
                }
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
        
        private void radioSQL_CheckedChanged(object sender, EventArgs e)
        {
            this.ShowProviderDetails(DbProviderType.MSSQL);
        }

        private void radioOracle_CheckedChanged(object sender, EventArgs e)
        {
            this.ShowProviderDetails(DbProviderType.Oracle);
        }

        private void radioMySql_CheckedChanged(object sender, EventArgs e)
        {
            this.ShowProviderDetails(DbProviderType.MySQL);
        }

        private void radioAzureSQL_CheckedChanged(object sender, EventArgs e)
        {
            this.ShowProviderDetails(DbProviderType.AzureSQL);
        }

        /// <summary>
        /// Ïîêàç äåòàëüíîé èíôîðìàöèè ïî âûáðàííîìó ïîñòàâùèêó ÁÄ
        /// </summary>
        /// <param name="providerType">Òèï ïîñòàâùèêà ÁÄ</param>
        private void ShowProviderDetails(DbProviderType providerType)
        {
            Wizard.EnableButton(Wizard.EButtons.NextButton);

            try
            {
                using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("ITA.Wizards.DBInfos.{0}.rtf", providerType)))
                using (TextReader r = new StreamReader(s))
                {
                    string details = r.ReadToEnd();
                    richTextDetails.Rtf = details;
                }
            }
            catch (Exception x)
            {
                throw new Exception("Error has occured while loading script.", x);
            }
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectProvider));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pnlProviders = new System.Windows.Forms.Panel();
            this.radioAzureSQL = new System.Windows.Forms.RadioButton();
            this.radioMySql = new System.Windows.Forms.RadioButton();
            this.radioOracle = new System.Windows.Forms.RadioButton();
            this.radioSQL = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.richTextDetails = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.panelPlaceholder.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.pnlProviders.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Text = "Database provider";
            // 
            // labelDescription
            // 
            this.labelDescription.Text = "Select the database provider";
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this.label2);
            this.panelPlaceholder.Controls.Add(this.groupBox3);
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
            this.label2.Location = new System.Drawing.Point(19, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(462, 23);
            this.label2.TabIndex = 20;
            this.label2.Text = "Select the database provider";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pnlProviders);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.richTextDetails);
            this.groupBox3.Location = new System.Drawing.Point(19, 36);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(462, 211);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Database provider";
            // 
            // pnlProviders
            // 
            this.pnlProviders.AutoScroll = true;
            this.pnlProviders.Controls.Add(this.radioAzureSQL);
            this.pnlProviders.Controls.Add(this.radioMySql);
            this.pnlProviders.Controls.Add(this.radioOracle);
            this.pnlProviders.Controls.Add(this.radioSQL);
            this.pnlProviders.Location = new System.Drawing.Point(8, 15);
            this.pnlProviders.Name = "pnlProviders";
            this.pnlProviders.Size = new System.Drawing.Size(221, 193);
            this.pnlProviders.TabIndex = 25;
            // 
            // radioAzureSQL
            // 
            this.radioAzureSQL.BackColor = System.Drawing.SystemColors.Control;
            this.radioAzureSQL.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioAzureSQL.Image = global::ITA.Wizards.Properties.Resources.AzureSQL;
            this.radioAzureSQL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.radioAzureSQL.Location = new System.Drawing.Point(0, 195);
            this.radioAzureSQL.Name = "radioAzureSQL";
            this.radioAzureSQL.Size = new System.Drawing.Size(204, 65);
            this.radioAzureSQL.TabIndex = 25;
            this.radioAzureSQL.UseVisualStyleBackColor = false;
            this.radioAzureSQL.CheckedChanged += new System.EventHandler(this.radioAzureSQL_CheckedChanged);
            // 
            // radioMySql
            // 
            this.radioMySql.BackColor = System.Drawing.SystemColors.Control;
            this.radioMySql.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioMySql.Image = global::ITA.Wizards.Properties.Resources.MySql;
            this.radioMySql.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.radioMySql.Location = new System.Drawing.Point(0, 130);
            this.radioMySql.Name = "radioMySql";
            this.radioMySql.Size = new System.Drawing.Size(204, 65);
            this.radioMySql.TabIndex = 24;
            this.radioMySql.UseVisualStyleBackColor = false;
            this.radioMySql.CheckedChanged += new System.EventHandler(this.radioMySql_CheckedChanged);
            // 
            // radioOracle
            // 
            this.radioOracle.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioOracle.Image = global::ITA.Wizards.Properties.Resources.Oracle;
            this.radioOracle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.radioOracle.Location = new System.Drawing.Point(0, 65);
            this.radioOracle.Name = "radioOracle";
            this.radioOracle.Size = new System.Drawing.Size(204, 65);
            this.radioOracle.TabIndex = 23;
            this.radioOracle.CheckedChanged += new System.EventHandler(this.radioOracle_CheckedChanged);
            // 
            // radioSQL
            // 
            this.radioSQL.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioSQL.Image = global::ITA.Wizards.Properties.Resources.MSSQL;
            this.radioSQL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.radioSQL.Location = new System.Drawing.Point(0, 0);
            this.radioSQL.Name = "radioSQL";
            this.radioSQL.Size = new System.Drawing.Size(204, 65);
            this.radioSQL.TabIndex = 22;
            this.radioSQL.CheckedChanged += new System.EventHandler(this.radioSQL_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(232, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(2, 196);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            // 
            // richTextDetails
            // 
            this.richTextDetails.BackColor = System.Drawing.SystemColors.Control;
            this.richTextDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextDetails.Location = new System.Drawing.Point(240, 16);
            this.richTextDetails.Name = "richTextDetails";
            this.richTextDetails.ReadOnly = true;
            this.richTextDetails.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextDetails.Size = new System.Drawing.Size(216, 152);
            this.richTextDetails.TabIndex = 20;
            this.richTextDetails.Text = "";
            // 
            // SelectProvider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SelectProvider";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.panelPlaceholder.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.pnlProviders.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}

