using System;
using System.Windows.Forms;
using ITA.Common.UI;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;
using ITA.Wizards.UpdateWizard.Model;

namespace ITA.Wizards.DatabaseWizard.Pages
{
    /// <summary>
    /// Summary description for Action.
    /// </summary>
    public class ActionPage : CustomPage
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelAction;
        private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.Label labelDB;
        private System.Windows.Forms.Label labelLogin;
        private System.Windows.Forms.Label labelLogintext;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelProvider;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ActionPage()
        {
            InitializeComponent();
			this.labelHint.Text = Messages.WIZ_PARAMETER_CONFIRMATION;
			this.labelDescription.Text = Messages.WIZ_VALIDATE_PARAMETERS;
			this.label1.Text = Messages.WIZ_CLICK_NEXT;
			this.groupBox1.Text = Messages.WIZ_CONNECTION_PARAMETERS;
			this.label2.Text = Messages.WIZ_ACTION;
			this.label3.Text = Messages.WIZ_SERVER_DB;
			this.label4.Text = Messages.WIZ_DB_NAME;
			this.labelLogintext.Text = Messages.WIZ_DB_LOGIN;
			this.label6.Text = Messages.WIZ_PROVIDER;
        }

        public DatabaseWizardContext Context
        {
            get
            {
                return Wizard.Context.ValueOf<DatabaseWizardContext>("DatabaseWizardContext");
            }
        }

        public override void OnNext(ref int NextIndex)
        {
            if (!this.Context.DBProvider.CreateNewDatabase)
            {
                //Если подключаемся к существующей БД - проверяем гранты
                this.GrantUserPermissions();

                //Проверяем необходимость выполнения обновлений

                UpdateDatabaseWizardContext updateWizardContext = new UpdateDatabaseWizardContext(Context.GetDatabaseConnectionString(), Context.DBProvider.UpdateManager);

                if (this.Context.DBProvider.IsUpdateRequired)
                {
                    //Необходимо выполнить обновление
                    this.Wizard.Context[UpdateDatabaseWizardContext.ClassName] = updateWizardContext;
                }
                else
                {
                    NextIndex = this.Wizard.GetPageIndex(typeof(FinishPage).Name);
                }
            }
            else
            {                
                NextIndex = this.Wizard.GetPageIndex(typeof(ProgressPage).Name);
            }

            base.OnNext(ref NextIndex);
        }

        /// <summary>
        /// Предоставление разрешений 
        /// </summary>
        /// <returns></returns>
        private bool GrantUserPermissions()
        {
            try
            {
                this.Context.DBProvider.GrantUserPermissions();               
            }
            catch (Exception x)
            {
                //Говорим, что автоматом выдать доступ нельзя - администратору нужно поднастроить вручную - маловероятный сценарий
                ErrorMessageBox.Show(this, x, ITA.Wizards.Messages.WIZ_DB_GRANT_PERMISSION_ERROR,
                                           this.Wizard.Text,
                                           MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //Ошибку данного типа интерпретируем как Warning. Предупреждаем администратора, что автоматически выделить права не удалось
            //и необходимо настроить их вручную согласно документации
            return true;
        }


        public override void OnActive()
        {
            try
            {
                labelAction.Text = this.Context.DBProvider.CreateNewDatabase ? Messages.WIZ_CREATE_NEW_DB : Messages.WIZ_CONNECT_TO_EXISTING_DB;
                labelProvider.Text = Context.DBProvider.Name;
                labelServer.Text = this.Context.DBProvider.ServerName;
                labelDB.Text = this.Context.DBProvider.DatabaseName;

                if (this.Context.DBProvider.DatabaseCredentialType == ELoginType.NT && Context.DBProvider.ProviderType == DbProviderType.MSSQL)
                {
                    labelLogin.Text = Messages.WIZ_NT_SECURITY;
                }
                else
                {
                    labelLogin.Text = this.Context.DBProvider.DatabaseLogin;
                }
            }
            catch (Exception x)
            {
                ErrorMessageBox.Show(this, x, ITA.Wizards.Messages.E_DBWIZARD_ERROR, this.Wizard.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelServer = new System.Windows.Forms.Label();
            this.labelProvider = new System.Windows.Forms.Label();
            this.labelLogin = new System.Windows.Forms.Label();
            this.labelDB = new System.Windows.Forms.Label();
            this.labelAction = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelLogintext = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Text = Messages.WIZ_PARAMETER_CONFIRMATION;
            // 
            // labelDescription
            // 
            this.labelDescription.Size = new System.Drawing.Size(353, 32);
            this.labelDescription.Text = Messages.WIZ_VALIDATE_PARAMETERS;
            // 
            // pictureBoxImage
            // 
            this.pictureBoxImage.Location = new System.Drawing.Point(433, 0);
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Size = new System.Drawing.Size(500, 281);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(477, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = Messages.WIZ_CLICK_NEXT;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelServer);
            this.groupBox1.Controls.Add(this.labelProvider);
            this.groupBox1.Controls.Add(this.labelLogin);
            this.groupBox1.Controls.Add(this.labelDB);
            this.groupBox1.Controls.Add(this.labelAction);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.labelLogintext);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(12, 118);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(477, 184);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = Messages.WIZ_CONNECTION_PARAMETERS;
            // 
            // labelServer
            // 
            this.labelServer.Location = new System.Drawing.Point(136, 96);
            this.labelServer.Name = "labelServer";
            this.labelServer.Size = new System.Drawing.Size(296, 16);
            this.labelServer.TabIndex = 1;
            // 
            // labelProvider
            // 
            this.labelProvider.Location = new System.Drawing.Point(136, 64);
            this.labelProvider.Name = "labelProvider";
            this.labelProvider.Size = new System.Drawing.Size(296, 16);
            this.labelProvider.TabIndex = 1;
            // 
            // labelLogin
            // 
            this.labelLogin.Location = new System.Drawing.Point(136, 160);
            this.labelLogin.Name = "labelLogin";
            this.labelLogin.Size = new System.Drawing.Size(296, 16);
            this.labelLogin.TabIndex = 1;
            // 
            // labelDB
            // 
            this.labelDB.Location = new System.Drawing.Point(136, 128);
            this.labelDB.Name = "labelDB";
            this.labelDB.Size = new System.Drawing.Size(296, 16);
            this.labelDB.TabIndex = 1;
            // 
            // labelAction
            // 
            this.labelAction.Location = new System.Drawing.Point(136, 32);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(296, 16);
            this.labelAction.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = Messages.WIZ_ACTION;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = Messages.WIZ_SERVER_DB;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = Messages.WIZ_DB_NAME;
            // 
            // labelLogintext
            // 
            this.labelLogintext.Location = new System.Drawing.Point(16, 160);
            this.labelLogintext.Name = "labelLogintext";
            this.labelLogintext.Size = new System.Drawing.Size(96, 16);
            this.labelLogintext.TabIndex = 0;
            this.labelLogintext.Text = Messages.WIZ_DB_LOGIN;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(16, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = Messages.WIZ_PROVIDER;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(0, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 23);
            this.label8.TabIndex = 0;
            // 
            // ActionPage
            // 
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "ActionPage";
            this.Controls.SetChildIndex(this.panelPlaceholder, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
