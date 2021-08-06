using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ITA.Common.Passwords;
using ITA.Common.UI;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;
using Oracle.DataAccess.Client;

namespace ITA.Wizards.DatabaseWizard.Pages.Oracle
{
    public class SelectOracleServer : CustomPage
    {
        private IContainer components;
        private IDbConnection m_Connection;
        private GroupBox groupBox3;
        private Label labelLogin;
        private TextBox textBoxLogin;
        private Label label3;
        private Label label4;
        private ComboBox comboBoxSID;
        private ComboBox comboBoxServer;
        private Label label1;
        private Button buttonTest;
        private Label label2;
        private Label labelPassword;
        private TextBox textBoxPassword;
        private NumericUpDown numPort;
		private LanguageIndicator languageIndicator1;
		private Panel panel2;
		private Label labelPasswordHint;
		private bool m_bConnectionValid;

        public SelectOracleServer()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
			this.labelHint.Text = Messages.WIZ_ORACLE_CONNECTION;
			this.labelDescription.Text = Messages.WIZ_ORACLE_SELECT_SERVER;
			this.groupBox3.Text = Messages.WIZ_ORACLE_CONNECTING;
			this.labelLogin.Text = Messages.WIZ_ORACLE_USER_NAME_LABEL;
			this.label3.Text = Messages.WIZ_ORACLE_ENTER_PORT;
			this.label4.Text = Messages.WIZ_ORACLE_ENTER_SID;
        	this.label1.Text = Messages.WIZ_ORACLE_ENTER_SERVER_NAME;
			this.buttonTest.Text = Messages.WIZ_ORACLE_CHECK_CONNECTION;
			this.label2.Text = Messages.WIZ_ORACLE_ENTER_ADMIN_USER_NAME;
			this.labelPassword.Text = Messages.WIZ_ORACLE_PASSWORD;
        }

        public DatabaseWizardContext Context
        {
            get { return Wizard.Context.ValueOf<Model.DatabaseWizardContext>(DatabaseWizardContext.ClassName); }
        } 

        public override bool OnValidate()
        {
			if (!ValidatePassword())
			{
				MessageBox.Show(this, Messages.WIZ_ORACLE_PASSWORD_IS_NOT_PROVIDED,
								Wizard.Text,
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				textBoxPassword.Focus();
				return false;
			}

            if (!m_bConnectionValid)
            {
                TestConnection();

                if (!m_bConnectionValid)
                {
                    return false;
                }
            }

            return true;
        }

        public override void OnActive()
        {
        	labelPasswordHint.Text = "";
            comboBoxServer.Focus();
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

        private void TestConnection()
        {
            if (0 == comboBoxServer.Text.Trim().Length)
            {
                MessageBox.Show(this,
                                Messages.WIZ_SRV_SELECT,
                                m_Parent.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (null != m_Connection && m_Connection.State == ConnectionState.Open)
            {
                m_Connection.Close();
            }

            using (m_Connection = new OracleConnection())
            {

                string connectionString = string.Format(OracleBaseDatabaseProvider.ConnectionStringTemplate, textBoxLogin.Text,
                                                        textBoxPassword.Text,
                                                        comboBoxServer.Text, numPort.Value, comboBoxSID.Text);
                
                try
                {
                    // validation is performed in ConnectionString setter
                    IDatabaseProvider databaseProvider = ((DatabaseWizard)Wizard).DatabaseWizardContext.DBProvider;
                    databaseProvider.ConnectionString = connectionString;

                    databaseProvider.ServerLogin = textBoxLogin.Text;
                    databaseProvider.ServerPassword = textBoxPassword.Text;
                    databaseProvider.ServerName = comboBoxServer.Text;
                    databaseProvider.ServerDatabasePort = numPort.Value.ToString();
                    databaseProvider.ServerDatabaseSID = comboBoxSID.Text;

                    m_bConnectionValid = true;
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

            if (m_bConnectionValid)
            {
                MessageBox.Show(this, Messages.WIZ_SRV_LOGIN_SUCCEEDED, m_Parent.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void comboBoxServer_TextChanged(object sender, EventArgs e)
        {
            m_bConnectionValid = false;
        }

        private void comboBoxPort_TextChanged(object sender, EventArgs e)
        {
            m_bConnectionValid = false;
        }

        private void comboBoxSID_TextChanged(object sender, EventArgs e)
        {
            m_bConnectionValid = false;
        }

        private void textBoxLogin_TextChanged(object sender, EventArgs e)
        {
            m_bConnectionValid = false;
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            m_bConnectionValid = false;
        	ValidatePassword();
        }

        private void SelectOracleServer_Load(object sender, EventArgs e)
        {
            m_bConnectionValid = false;
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.labelPasswordHint = new System.Windows.Forms.Label();
			this.languageIndicator1 = new ITA.Common.UI.LanguageIndicator();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.numPort = new System.Windows.Forms.NumericUpDown();
			this.labelLogin = new System.Windows.Forms.Label();
			this.textBoxLogin = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBoxSID = new System.Windows.Forms.ComboBox();
			this.comboBoxServer = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonTest = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.labelPassword = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
			this.panelPlaceholder.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
			this.SuspendLayout();
			// 
			// labelHint
			// 
			this.labelHint.Text = Messages.WIZ_ORACLE_CONNECTION;
			// 
			// labelDescription
			// 
			this.labelDescription.Text = Messages.WIZ_ORACLE_SELECT_SERVER;
			// 
			// pictureBoxImage
			// 
			this.pictureBoxImage.Location = new System.Drawing.Point(433, 0);
			// 
			// panelPlaceholder
			// 
			this.panelPlaceholder.Controls.Add(this.groupBox3);
			this.panelPlaceholder.Size = new System.Drawing.Size(500, 281);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.panel2);
			this.groupBox3.Controls.Add(this.languageIndicator1);
			this.groupBox3.Controls.Add(this.numPort);
			this.groupBox3.Controls.Add(this.labelLogin);
			this.groupBox3.Controls.Add(this.textBoxLogin);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.comboBoxSID);
			this.groupBox3.Controls.Add(this.comboBoxServer);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.buttonTest);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.labelPassword);
			this.groupBox3.Controls.Add(this.textBoxPassword);
			this.groupBox3.Location = new System.Drawing.Point(19, 16);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(463, 238);
			this.groupBox3.TabIndex = 28;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = Messages.WIZ_ORACLE_CONNECTING;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.labelPasswordHint);
			this.panel2.Location = new System.Drawing.Point(16, 186);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(419, 16);
			this.panel2.TabIndex = 23;
			// 
			// labelPasswordHint
			// 
			this.labelPasswordHint.AutoEllipsis = true;
			this.labelPasswordHint.AutoSize = true;
			this.labelPasswordHint.BackColor = System.Drawing.Color.Gold;
			this.labelPasswordHint.Dock = System.Windows.Forms.DockStyle.Right;
			this.labelPasswordHint.ForeColor = System.Drawing.SystemColors.InfoText;
			this.labelPasswordHint.Location = new System.Drawing.Point(384, 0);
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
			this.languageIndicator1.Location = new System.Drawing.Point(439, 160);
			this.languageIndicator1.Name = "languageIndicator1";
			this.languageIndicator1.Size = new System.Drawing.Size(21, 19);
			this.languageIndicator1.TabIndex = 22;
			this.languageIndicator1.Visible = false;
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(216, 160);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.PasswordChar = '*';
			this.textBoxPassword.Size = new System.Drawing.Size(217, 20);
			this.textBoxPassword.TabIndex = 5;
			this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
			this.textBoxPassword.Enter += new System.EventHandler(this.textBoxPassword_Enter);
			// 
			// numPort
			// 
			this.numPort.Location = new System.Drawing.Point(216, 56);
			this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.numPort.Name = "numPort";
			this.numPort.Size = new System.Drawing.Size(219, 20);
			this.numPort.TabIndex = 2;
			this.numPort.Value = new decimal(new int[] {
            1521,
            0,
            0,
            0});
			this.numPort.ValueChanged += new System.EventHandler(this.comboBoxPort_TextChanged);
			// 
			// labelLogin
			// 
			this.labelLogin.Location = new System.Drawing.Point(16, 136);
			this.labelLogin.Name = "labelLogin";
			this.labelLogin.Size = new System.Drawing.Size(100, 16);
			this.labelLogin.TabIndex = 19;
			this.labelLogin.Text = Messages.WIZ_ORACLE_USER_NAME_LABEL;
			// 
			// textBoxLogin
			// 
			this.textBoxLogin.Location = new System.Drawing.Point(16, 160);
			this.textBoxLogin.Name = "textBoxLogin";
			this.textBoxLogin.Size = new System.Drawing.Size(192, 20);
			this.textBoxLogin.TabIndex = 4;
			this.textBoxLogin.TextChanged += new System.EventHandler(this.textBoxLogin_TextChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(184, 24);
			this.label3.TabIndex = 15;
			this.label3.Text = Messages.WIZ_ORACLE_ENTER_PORT;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(184, 24);
			this.label4.TabIndex = 15;
			this.label4.Text = Messages.WIZ_ORACLE_ENTER_SID;
			// 
			// comboBoxSID
			// 
			this.comboBoxSID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.comboBoxSID.Location = new System.Drawing.Point(216, 88);
			this.comboBoxSID.Name = "comboBoxSID";
			this.comboBoxSID.Size = new System.Drawing.Size(219, 21);
			this.comboBoxSID.TabIndex = 3;
			this.comboBoxSID.TextChanged += new System.EventHandler(this.comboBoxSID_TextChanged);
			// 
			// comboBoxServer
			// 
			this.comboBoxServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.comboBoxServer.Location = new System.Drawing.Point(216, 24);
			this.comboBoxServer.Name = "comboBoxServer";
			this.comboBoxServer.Size = new System.Drawing.Size(219, 21);
			this.comboBoxServer.TabIndex = 1;
			this.comboBoxServer.TextChanged += new System.EventHandler(this.comboBoxServer_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(184, 24);
			this.label1.TabIndex = 15;
			this.label1.Text = Messages.WIZ_ORACLE_ENTER_SERVER_NAME;
			// 
			// buttonTest
			// 
			this.buttonTest.Location = new System.Drawing.Point(216, 208);
			this.buttonTest.Name = "buttonTest";
			this.buttonTest.Size = new System.Drawing.Size(219, 23);
			this.buttonTest.TabIndex = 6;
			this.buttonTest.Text = Messages.WIZ_ORACLE_CHECK_CONNECTION;
			this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 112);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(320, 24);
			this.label2.TabIndex = 17;
			this.label2.Text = Messages.WIZ_ORACLE_ENTER_ADMIN_USER_NAME;
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(208, 136);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(100, 16);
			this.labelPassword.TabIndex = 21;
			this.labelPassword.Text = Messages.WIZ_ORACLE_PASSWORD;
			// 
			// SelectOracleServer
			// 
			this.Name = "SelectOracleServer";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Load += new System.EventHandler(this.SelectOracleServer_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
			this.panelPlaceholder.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
			this.ResumeLayout(false);

        }


        #endregion

		private void textBoxPassword_Enter(object sender, EventArgs e)
		{
			ValidatePassword();
		}

        private bool ValidatePassword()
        {
            string errorMessage;

            PasswordQualityValidator.Validate(this.textBoxPassword.Text, this.Context.CurrentPasswordQuality, out errorMessage);

            bool RetVal = false;

            if (!string.IsNullOrEmpty(errorMessage))
            {
                labelPasswordHint.Text = errorMessage;
                textBoxPassword.BackColor = Color.LightPink;
                labelPasswordHint.Visible = true;
                RetVal = false;
            }
            else
            {
                labelPasswordHint.Text = "";
                textBoxPassword.BackColor = Color.LightGreen;
                labelPasswordHint.Visible = false;
                RetVal = true;
            }

            return RetVal;
        }
    }
}