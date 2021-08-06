using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using ITA.Common.Passwords;
using ITA.Common.UI;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;

namespace ITA.Wizards.DatabaseWizard.Pages.AzureSql
{
    public class SelectAzureSqlServer : CustomPage
    {
        private IContainer components;
        private IDbConnection m_Connection;
        private GroupBox groupBox3;
        private Label labelLogin;
        private TextBox textBoxLogin;
        private ComboBox comboBoxServer;
        private Label label1;
        private Button buttonTest;
        private Label labelPassword;
        private TextBox textBoxPassword;
		private LanguageIndicator languageIndicator1;
		private Panel panel2;
		private Label labelPasswordHint;
        private Label label3;
        private TextBox textBoxDatabaseName;
        private bool _connectionValid;

        public SelectAzureSqlServer()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
			this.labelHint.Text = Messages.WIZ_AZURE_SQL_CONNECTION;
			this.labelDescription.Text = Messages.WIZ_AZURE_SQL_SELECT_SERVER;
			this.groupBox3.Text = Messages.WIZ_AZURE_SQL_CONNECTING;
			this.labelLogin.Text = Messages.WIZ_AZURE_SQL_USER_NAME_LABEL;
			this.label3.Text = Messages.WIZ_AZURE_SQL_ENTER_DB_NAME;
			this.label1.Text = Messages.WIZ_AZURE_SQL_ENTER_SERVER_NAME;
			this.buttonTest.Text = Messages.WIZ_AZURE_SQL_CHECK_CONNECTION;
			this.labelPassword.Text = Messages.WIZ_AZURE_SQL_PASSWORD;            
        }

        public DatabaseWizardContext Context => Wizard.Context.ValueOf<DatabaseWizardContext>(DatabaseWizardContext.ClassName);

        public override bool OnValidate()
        {
			if (!ValidatePassword())
			{
				MessageBox.Show(this, Messages.WIZ_AZURE_SQL_PASSWORD_IS_NOT_PROVIDED,
								Wizard.Text,
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				textBoxPassword.Focus();
				return false;
			}

            if (string.IsNullOrWhiteSpace(comboBoxServer.Text))
            {
                MessageBox.Show(this,
                    Messages.WIZ_SRV_SELECT,
                    m_Parent.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBoxServer.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxDatabaseName.Text))
            {
                MessageBox.Show(this,
                    Messages.WIZ_DB_SELECT_DB,
                    m_Parent.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxDatabaseName.Focus();
                return false;
            }

            if (!_connectionValid)
            {
                TestConnection();

                if (!_connectionValid)
                {
                    return false;
                }
            }

            return true;
        }

        public override void OnNext(ref int NextIndex)
        {
            base.OnNext(ref NextIndex);

            if (Context.DBProvider.CreateNewDatabase)
            {
                var azureSqlProvider = (AzureSqlDatabaseProvider)Context.DBProvider;
                azureSqlProvider.CheckAbilityForDatabaseCreation();
            }
        }

        public override void OnActive()
        {
            comboBoxServer.Focus();
            labelPasswordHint.Text = string.Empty;
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void TestConnection()
        {
            if (null != m_Connection && m_Connection.State == ConnectionState.Open)
            {
                m_Connection.Close();
            }

            using (m_Connection = new SqlConnection())
            {
                var connectionStringBuilder = new SqlConnectionStringBuilder();
                connectionStringBuilder.DataSource = comboBoxServer.Text; 
                connectionStringBuilder.UserID = textBoxLogin.Text;
                connectionStringBuilder.Password = textBoxPassword.Text;
                connectionStringBuilder.InitialCatalog = textBoxDatabaseName.Text;

                var connectionString = connectionStringBuilder.ToString();
                try
                {
                    // validation is performed in ConnectionString setter
                    var databaseProvider = ((DatabaseWizard)Wizard).DatabaseWizardContext.DBProvider;
                    databaseProvider.ConnectionString = connectionString;

                    databaseProvider.ServerLogin = textBoxLogin.Text;
                    databaseProvider.ServerPassword = textBoxPassword.Text;
                    databaseProvider.ServerName = comboBoxServer.Text;
                    databaseProvider.DatabaseName = textBoxDatabaseName.Text;
                    databaseProvider.DatabaseLogin = textBoxLogin.Text;
                    databaseProvider.DatabasePassword = textBoxPassword.Text;
                    databaseProvider.DatabaseCredentialType = ELoginType.SQL;

                    _connectionValid = true;
                }
                catch (Exception x)
                {
                    ErrorMessageBox.Show(this, x,
                                         String.Format(Messages.WIZ_SRV_LOGIN_FAILED, comboBoxServer.Text),
                                         m_Parent.Text, MessageBoxButtons.OK,
                                         MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            TestConnection();

            if (_connectionValid)
            {
                MessageBox.Show(this, Messages.WIZ_SRV_LOGIN_SUCCEEDED, m_Parent.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ConnectionParametersChanged(object sender, EventArgs e)
        {
            _connectionValid = false;
            ValidatePassword();
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDatabaseName = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelPasswordHint = new System.Windows.Forms.Label();
            this.languageIndicator1 = new ITA.Common.UI.LanguageIndicator();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.labelLogin = new System.Windows.Forms.Label();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.comboBoxServer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonTest = new System.Windows.Forms.Button();
            this.labelPassword = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.panelPlaceholder.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Text = "Connecting to Azure SQL";
            // 
            // labelDescription
            // 
            this.labelDescription.Text = "Specify the Azure SQL server and connection parameters for the administrator";
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this.groupBox3);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textBoxDatabaseName);
            this.groupBox3.Controls.Add(this.panel2);
            this.groupBox3.Controls.Add(this.languageIndicator1);
            this.groupBox3.Controls.Add(this.labelLogin);
            this.groupBox3.Controls.Add(this.textBoxLogin);
            this.groupBox3.Controls.Add(this.comboBoxServer);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.buttonTest);
            this.groupBox3.Controls.Add(this.labelPassword);
            this.groupBox3.Controls.Add(this.textBoxPassword);
            this.groupBox3.Location = new System.Drawing.Point(19, 16);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(463, 238);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Connection";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(31, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 24);
            this.label3.TabIndex = 25;
            this.label3.Text = "Database name:";
            // 
            // textBoxDatabaseName
            // 
            this.textBoxDatabaseName.Location = new System.Drawing.Point(216, 61);
            this.textBoxDatabaseName.Name = "textBoxDatabaseName";
            this.textBoxDatabaseName.Size = new System.Drawing.Size(219, 20);
            this.textBoxDatabaseName.TabIndex = 2;
            this.textBoxDatabaseName.TextChanged += new System.EventHandler(this.ConnectionParametersChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelPasswordHint);
            this.panel2.Location = new System.Drawing.Point(34, 138);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(401, 16);
            this.panel2.TabIndex = 23;
            // 
            // labelPasswordHint
            // 
            this.labelPasswordHint.AutoEllipsis = true;
            this.labelPasswordHint.AutoSize = true;
            this.labelPasswordHint.BackColor = System.Drawing.Color.Gold;
            this.labelPasswordHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelPasswordHint.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelPasswordHint.Location = new System.Drawing.Point(366, 0);
            this.labelPasswordHint.Name = "labelPasswordHint";
            this.labelPasswordHint.Size = new System.Drawing.Size(35, 13);
            this.labelPasswordHint.TabIndex = 13;
            this.labelPasswordHint.Text = "label4";
            this.labelPasswordHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // languageIndicator1
            // 
            this.languageIndicator1.AttachedTo = this.textBoxPassword;
            this.languageIndicator1.BackColor = System.Drawing.Color.RoyalBlue;
            this.languageIndicator1.ForeColor = System.Drawing.Color.White;
            this.languageIndicator1.Location = new System.Drawing.Point(439, 114);
            this.languageIndicator1.Name = "languageIndicator1";
            this.languageIndicator1.Size = new System.Drawing.Size(21, 19);
            this.languageIndicator1.TabIndex = 22;
            this.languageIndicator1.Visible = false;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(216, 113);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(219, 20);
            this.textBoxPassword.TabIndex = 4;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.ConnectionParametersChanged);
            this.textBoxPassword.Enter += new System.EventHandler(this.textBoxPassword_Enter);
            // 
            // labelLogin
            // 
            this.labelLogin.Location = new System.Drawing.Point(31, 87);
            this.labelLogin.Name = "labelLogin";
            this.labelLogin.Size = new System.Drawing.Size(150, 24);
            this.labelLogin.TabIndex = 19;
            this.labelLogin.Text = "User name:";
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.Location = new System.Drawing.Point(216, 87);
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.Size = new System.Drawing.Size(219, 20);
            this.textBoxLogin.TabIndex = 3;
            this.textBoxLogin.TextChanged += new System.EventHandler(this.ConnectionParametersChanged);
            // 
            // comboBoxServer
            // 
            this.comboBoxServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboBoxServer.Location = new System.Drawing.Point(216, 34);
            this.comboBoxServer.Name = "comboBoxServer";
            this.comboBoxServer.Size = new System.Drawing.Size(219, 21);
            this.comboBoxServer.TabIndex = 1;
            this.comboBoxServer.TextChanged += new System.EventHandler(this.ConnectionParametersChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(31, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 24);
            this.label1.TabIndex = 15;
            this.label1.Text = "Server name:";
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(322, 157);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(113, 23);
            this.buttonTest.TabIndex = 5;
            this.buttonTest.Text = global::ITA.Wizards.Messages.WIZ_AZURE_SQL_CHECK_CONNECTION;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // labelPassword
            // 
            this.labelPassword.Location = new System.Drawing.Point(31, 113);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(150, 24);
            this.labelPassword.TabIndex = 21;
            this.labelPassword.Text = "Password:";
            // 
            // SelectAzureSqlServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SelectAzureSqlServer";
            this.Load += new System.EventHandler(this.ConnectionParametersChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.panelPlaceholder.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion

		private void textBoxPassword_Enter(object sender, EventArgs e)
		{
			ValidatePassword();
		}

        private bool ValidatePassword()
        {
            PasswordQualityValidator.Validate(this.textBoxPassword.Text, this.Context.CurrentPasswordQuality, out var errorMessage);

            bool retVal;

            if (!string.IsNullOrEmpty(errorMessage))
            {
                labelPasswordHint.Text = errorMessage;
                textBoxPassword.BackColor = Color.LightPink;
                labelPasswordHint.Visible = true;
                retVal = false;
            }
            else
            {
                labelPasswordHint.Text = "";
                textBoxPassword.BackColor = Color.LightGreen;
                labelPasswordHint.Visible = false;
                retVal = true;
            }

            return retVal;
        }
    }
}

