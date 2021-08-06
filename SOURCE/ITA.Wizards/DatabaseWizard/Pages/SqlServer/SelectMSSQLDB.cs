using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ITA.Common.Passwords;
using ITA.Common.UI;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;

namespace ITA.Wizards.DatabaseWizard.Pages.SqlServer
{
	/// <summary>
	/// Шаг выбора базы данных
	/// </summary>
	public class SelectMSSQLDB : CustomPage
	{
		private readonly Regex _validateRegex = new Regex(@"^[0-9a-zA-Z_а-яА-Я]+$");

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private readonly Container components;
     		
        private CheckBox checkBoxCreate;
		private ComboBox comboBoxDB;
		private ComboBox comboBoxUserID;
		private GroupBox groupBoxCredentials;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label6;
		private Label label7;
		private Label label9;
		private Label labelConfirmationHint;
		private Label labelPasswordHint;
		private LanguageIndicator languageIndicator1;
		private LanguageIndicator languageIndicator2;
        private Panel panel1;
		private Panel panel2;
		private RadioButton rbnNT;
		private RadioButton rbnSQL;
		private TextBox textBoxConfirm;
		private Controls.TextBoxWithValidator textBoxDBName;
		private TextBox textBoxPassword;
        private CheckBox chbUseSSL;
		private Controls.TextBoxWithValidator textBoxUserID;

		public SelectMSSQLDB()
		{
			InitializeComponent();

			textBoxDBName.HintEvent += textBoxDBName_HintEvent;
			textBoxUserID.HintEvent += textBoxUserID_HintEvent;

			this.labelHint.Text = Messages.WIZ_SELECT_DB_MESSAGE;
			this.labelDescription.Text = Messages.WIZ_SQL_SELECT_DB_AND_SETTINGS;
			this.label1.Text = Messages.WIZ_SQL_SELECT_DB_NAME;
			this.label6.Text = Messages.WIZ_SQL_PASSWORD;
			this.label7.Text = Messages.WIZ_SQL_PASSWORD_CONFIRM;
			this.groupBoxCredentials.Text = Messages.WIZ_SQL_CONNECTION_SETTINGS;
			this.label2.Text = Messages.WIZ_SQL_AUTH_MODE;
			this.label9.Text = Messages.WIZ_SQL_SELECT_DB_NAME;
			this.rbnSQL.Text = Messages.WIZ_SQL_AUTH;
			this.rbnNT.Text = Messages.WIZ_SQL_NT_AUTH;
			this.checkBoxCreate.Text = Messages.WIZ_SQL_CREATE_NEW_LOGIN;
			this.label3.Text = Messages.WIZ_SQL_LOGIN;
            this.chbUseSSL.Text = Messages.WIZ_SQL_USE_SSL;
		}

		public Model.DatabaseWizardContext Context
		{
			get { return Wizard.Context.ValueOf<Model.DatabaseWizardContext>(Model.DatabaseWizardContext.ClassName); }
		}       
        
        private void ListDatabases()
		{
			// save previous state
			string PrevDB = null;
			if (-1 != comboBoxDB.SelectedIndex)
			{
				PrevDB = comboBoxDB.Text;
			}

			comboBoxDB.Items.Clear();

			using (SqlConnection connection = new SqlConnection(Context.GetServerConnectionString()))
            using (SqlCommand command = connection.CreateCommand())
			{
				SqlDataReader reader = null;

				try
				{
					Cursor = Cursors.WaitCursor;

                    connection.Open();
					command.CommandText = "SELECT name FROM sysdatabases";

					reader = command.ExecuteReader();

					while (reader.Read())
					{
						string DBName = reader.GetValue(0).ToString();
						comboBoxDB.Items.Add(DBName);
					}
				}
				catch (Exception)
				{

				}
				finally
				{
					if (reader != null)
						reader.Close();

					Cursor = Cursors.Default;
				}
			}

			int SelectIndex = -1;

			if (null != Context.DBProvider.DatabaseName)
			{
				SelectIndex = comboBoxDB.FindString(Context.DBProvider.DatabaseName);
			}

			if (-1 == SelectIndex && null != PrevDB)
			{
				SelectIndex = comboBoxDB.FindString(PrevDB);
			}

			if (-1 == SelectIndex)
			{
				SelectIndex = comboBoxDB.FindString(Context.DBProvider.DefaultDatabaseName);
			}

			if (-1 != SelectIndex)
			{
				comboBoxDB.SelectedIndex = SelectIndex;
			}
		}


		private void ListUsers()
		{		   
			// Store prev state
			string PrevLogin = null;

			if (!string.IsNullOrEmpty(comboBoxUserID.Text))
			{
				PrevLogin = comboBoxUserID.Text;
			}
		    
			comboBoxUserID.Items.Clear();

			using (var conn = new SqlConnection(Context.GetServerConnectionString()))
			using (SqlCommand command = conn.CreateCommand())
			{
				SqlDataReader reader = null;

				try
				{
					Cursor = Cursors.WaitCursor;

					conn.Open();
                    command.CommandText = "SELECT * FROM master.dbo.syslogins order by loginname";

					reader = command.ExecuteReader();

					while (reader.Read())
					{
						int iNameIndex = reader.GetOrdinal("loginname");
						int iNTName = reader.GetOrdinal("isntname");

						string login = reader.GetValue(iNameIndex).ToString();
						string bNTName = reader.GetValue(iNTName).ToString();

						// skip windows logins
						if ("1" != bNTName)
						{
							comboBoxUserID.Items.Add(login);
						}
					}
				}
				catch (Exception)
				{
				}
				finally
				{
					if (reader != null)
						reader.Close();

					Cursor = Cursors.Default;
				}
			}

			if (null != Context.DBProvider.DatabaseLogin)
			{
				comboBoxUserID.Text = Context.DBProvider.DatabaseLogin;
			}
			else if (null != PrevLogin)
			{
				comboBoxUserID.Text = PrevLogin;
			}
			else
			{
				comboBoxUserID.Text = "";
			}
		}

		public override void OnNext(ref int NextIndex)
		{
			base.OnNext(ref NextIndex);

            NextIndex = this.Wizard.GetPageIndex(typeof(ActionPage).Name);		   
		}

		public override void OnActive()
		{
			try
			{
				Update();

				if (Context.DBProvider.CreateNewDatabase)
				{
					checkBoxCreate.Enabled = true;
					comboBoxDB.Visible = false;
					textBoxDBName.Visible = true;
					label1.Visible = false;
					label9.Visible = true;
				}
				else
				{
					checkBoxCreate.Checked = false;
					checkBoxCreate.Enabled = false;
					comboBoxDB.Visible = true;
					textBoxDBName.Visible = false;
					label1.Visible = true;
					label9.Visible = false;
				}

			    if (Context.DBProvider.EncryptDatabaseConnection)
			    {
			        this.chbUseSSL.Checked = true;
			    } 

				//
				// TODO: Perform this operation asynchronously
				//
				ListDatabases();
				ListUsers();
				UpdateUserMode();

				if (Context.DBProvider.CreateNewDatabase && string.IsNullOrEmpty(textBoxDBName.Text.Trim()))
				{
					//
					// Lets choose unique database name
					//
					string szNewDBName = Context.DBProvider.DefaultDatabaseName;
					int i = 1;
                    
                    while (this.IsComboboxItemExists(comboBoxDB, szNewDBName))
					{
						szNewDBName = Context.DBProvider.DefaultDatabaseName + "_" + i;
						i++;
					}

					textBoxDBName.Text = szNewDBName;

					textBoxPassword.Focus();
				}
				else
				    {
					comboBoxDB.Focus();
				}
			}
			catch (Exception x)
			{
				ErrorMessageBox.Show(this, x, Messages.E_DBWIZARD_LIST_DB_ERROR, Wizard.Text, MessageBoxButtons.OK,
									 MessageBoxIcon.Error);
			}

			Wizard.EnableButton(Wizard.EButtons.CancelButton);
			Wizard.EnableButton(Wizard.EButtons.NextButton);
			Wizard.EnableButton(Wizard.EButtons.BackButton);
			Wizard.DisableButton(Wizard.EButtons.FinishButton);

			rbnSQL.Enabled = rbnNT.Enabled = true;
			rbnNT.Checked = Context.DBProvider.DatabaseCredentialType == ELoginType.NT;
			rbnSQL.Checked = !rbnNT.Checked;
			
			UpdateUserMode();

			rbnNT.Enabled = !((DatabaseWizard)Wizard).EnableChangingDatabaseCredentialType ? false : rbnNT.Enabled;
			rbnSQL.Enabled = !((DatabaseWizard)Wizard).EnableChangingDatabaseCredentialType ? false : rbnSQL.Enabled;
			checkBoxCreate.Enabled = !((DatabaseWizard)Wizard).EnableChangingDatabaseLoginMode ? false : checkBoxCreate.Enabled;

			base.OnActive();
		}

		public override bool OnValidate()
		{

			Context.DBProvider.CreateNewDatabaseLogin = checkBoxCreate.Checked;
			Context.DBProvider.DatabaseName = Context.DBProvider.CreateNewDatabase
												? textBoxDBName.Text.Trim()
												: comboBoxDB.Text.Trim();
			Context.DBProvider.DatabaseLogin = Context.DBProvider.CreateNewDatabaseLogin
												? textBoxUserID.Text.Trim()
												: comboBoxUserID.Text.Trim();
			Context.DBProvider.DatabasePassword = textBoxPassword.Text.Trim();
			Context.DBProvider.DatabaseCredentialType = rbnNT.Checked ? ELoginType.NT : ELoginType.SQL;
		    Context.DBProvider.EncryptDatabaseConnection = this.chbUseSSL.Checked;
			
			if (!ValidateWindowsLogin())
				return false;
			if (!Context.DBProvider.CreateNewDatabase && string.IsNullOrEmpty(Context.DBProvider.DatabaseName))
			{
				MessageBox.Show(this,
								Messages.WIZ_DB_SELECT_DB,
								Wizard.Text,
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				comboBoxDB.Focus();
				return false;
			}

			if (Context.DBProvider.CreateNewDatabase && string.IsNullOrEmpty(Context.DBProvider.DatabaseName))
			{
				MessageBox.Show(this,
								Messages.WIZ_DB_ENTER_DB,
								Wizard.Text,
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				textBoxDBName.Focus();
				return false;
			}

			if (Context.DBProvider.CreateNewDatabase && !textBoxDBName.IsValid)
			{
				MessageBox.Show(this,
								Messages.WIZ_DB_INVALID_DATABASE_NAME_EXT,
								Wizard.Text,
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				textBoxDBName.Focus();
				return false;
			}

			if (Context.DBProvider.CreateNewDatabase && this.IsComboboxItemExists(comboBoxDB, Context.DBProvider.DatabaseName))
			{
				MessageBox.Show(this, string.Format(Messages.WIZ_DB_ALREADY_EXISTS, textBoxDBName.Text.Trim()),
								Wizard.Text,
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				textBoxDBName.Focus();
				return false;
			}

			if (Context.DBProvider.DatabaseCredentialType != ELoginType.NT)
			{
				if (string.IsNullOrEmpty(Context.DBProvider.DatabaseLogin))
				{
					MessageBox.Show(this, Messages.WIZ_DB_ENTER_USER,
									Wizard.Text,
									MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					textBoxUserID.Focus();
					return false;
				}

				if (!textBoxUserID.IsValid)
				{
					MessageBox.Show(this, Messages.WIZ_DB_INVALID_LOGIN_EXT,
									Wizard.Text,
									MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					textBoxUserID.Focus();
					return false;
				}
			    
				// validate login name
                if (checkBoxCreate.Checked && this.IsComboboxItemExists(comboBoxUserID, Context.DBProvider.DatabaseLogin))
				{
                    MessageBox.Show(this,
                                    string.Format(Messages.WIZ_DB_USER_ALREADY_EXISTS, textBoxUserID.Text.Trim()),
                                    Wizard.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBoxUserID.Focus();
                    
                    return false;
				}

				// validate existing login
				if (!checkBoxCreate.Checked)
				{
					try
					{
						// but we dont validate password for existing user
						using (var connection = new SqlConnection(Context.GetConnectionString(false, DatabaseWizardContext.MasterDatabase)))
						using (SqlCommand command = connection.CreateCommand())
						{

							Cursor = Cursors.WaitCursor;
							connection.Open();
							connection.Close();
						}
					}
					catch (Exception x)
					{
						Cursor = Cursors.Default;

						ErrorMessageBox.Show(this, x, string.Format(Messages.WIZ_DB_LOGIN_FAILED, Context.DBProvider.ServerName,
																	Context.DBProvider.DatabaseLogin),
											 Wizard.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

						textBoxPassword.Focus();
						return false;
					}
					finally
					{
						Cursor = Cursors.Default;
					}
				}
				else if (checkBoxCreate.Checked) // validate password
				{
					if (!ValidatePassword())
					{
						MessageBox.Show(this, Messages.WIZ_DB_ENTER_PASSWORD,
										Wizard.Text,
										MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

						textBoxPassword.Focus();
						return false;
					}

					if (!ValidatePasswordConfirmation())
					{
						MessageBox.Show(this,
										Messages.WIZ_DB_RECONFIRM_PASSWORD,
										Wizard.Text,
										MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						textBoxConfirm.Focus();
						return false;
					}
				}
			}

			if (!Context.DBProvider.CreateNewDatabase && ValidateDatabaseVersion() == false)
				return false;

			return true;
		}


		/// <summary>
		/// Проверка версии базы данных
		/// </summary>
		/// <returns></returns>
		public bool ValidateDatabaseVersion()
		{
			try
			{
				Version currentVersion = Context.DBProvider.UpdateManager.GetDatabaseVersion();

				//Если версия ниже - позволим обновить БД
				//Если версия равна текущей = все ОК
				if (currentVersion <= Context.DBProvider.UpdateManager.GetActualDatabaseVersion())
				{
					return true;
				}
				else
				{
					MessageBox.Show(this,
									string.Format(Messages.E_ITA_DB_HAS_OLD_VERSION, currentVersion,
												  Context.DBProvider.UpdateManager.GetActualDatabaseVersion()),
									Wizard.Text,
									MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					return false;
				}
			}
			catch (SqlException ex)
			{
				if (ex.Number == 207 || ex.Number == 208)
				{
					ErrorMessageBox.Show(this, ex, Messages.E_ITA_UPDATE_INVALID_DATABASE,
									Wizard.Text,
									MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}

				ErrorMessageBox.Show(this, ex, Messages.E_ITA_UPDATE_UNKNOWN_VERSION,
								Wizard.Text,
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			return false;
		}

		/// <summary>
		/// Валидация логина DOMAIN\WORKSTATION$ на возможность добавления в БД
		/// Если логин добавить нельзя - запрещаем Windows-аутентификацию - disable checkbox
		/// </summary>
		/// <returns></returns>
		private bool ValidateWindowsLogin()
		{
			using (var conn = new SqlConnection(Context.GetServerConnectionString()))
			using (SqlCommand command = conn.CreateCommand())
			{
                string workstationAccountName = DatabaseWizardContext.WorkstationAccountName;

				try
				{
					Cursor = Cursors.WaitCursor;

                    conn.Open();

					if (Context.DBProvider.DatabaseCredentialType == ELoginType.NT
						&& ((DatabaseWizard)Wizard).PerformWindowsUserValidation
                        && !String.IsNullOrEmpty(workstationAccountName))
					{

						command.CommandText =
						string.Format(
							@"if not exists (select * from master.dbo.syslogins where loginname = '{0}')
                                            BEGIN
	                                            CREATE LOGIN [{0}] FROM WINDOWS WITH DEFAULT_DATABASE=[master]
	                                            DROP LOGIN [{0}]
                                            END",
                            workstationAccountName);

						command.ExecuteNonQuery();

						// информируем  пользователя, что в случае запуска визарда под учетной записью Local System 
						// могут быть проблемы с дальнейшим подключением
						MessageBox.Show(this,
                                        String.Format(Messages.WIZ_SELECT_DB_WIN_AUTH_SERVER_MESSAGE, workstationAccountName),
						                Messages.WIZ_SELECT_DB_WIN_AUTH_SERVER_CAPTION,
						                MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					return true;
				}
				catch (Exception x)
				{
					ErrorMessageBox.Show(this, x,
                                            String.Format(Messages.WIZ_SQL_CANNOT_VALIDATE_USER, workstationAccountName),
											Messages.WIZ_SELECT_DB_WIN_AUTH_SERVER_CAPTION,
											MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
					return false;
				}
				finally
				{
					Cursor = Cursors.Default;
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

		private void checkBoxCreate_CheckedChanged(object sender, EventArgs e)
		{
			UpdateUserMode();
		}

        private void UpdateUserMode()
        {
            checkBoxCreate.Visible = Context.DBProvider.CreateNewDatabase;
            label7.Visible = Context.DBProvider.CreateNewDatabase;
            textBoxConfirm.Visible = Context.DBProvider.CreateNewDatabase;

            checkBoxCreate.Enabled = rbnSQL.Checked;
            comboBoxUserID.Enabled = rbnSQL.Checked;
            textBoxUserID.Enabled = rbnSQL.Checked;
            textBoxPassword.Enabled = rbnSQL.Checked;

            if (rbnSQL.Checked && !checkBoxCreate.Checked)
            {
                textBoxConfirm.Visible = false;
                label7.Visible = false;
            }
            else
            {
                textBoxConfirm.Visible = true;
                label7.Visible = true;
            }

            textBoxConfirm.Enabled = rbnSQL.Checked && checkBoxCreate.Checked;

            comboBoxUserID.Visible = !checkBoxCreate.Checked;
            textBoxUserID.Visible = checkBoxCreate.Checked;

            if (checkBoxCreate.Checked && string.IsNullOrEmpty(textBoxUserID.Text))
            {
                //
                // Lets choose unique user name
                //
                string szNewUserName = Context.DBProvider.DefaultDatabaseName;
                int i = 1;
                while (comboBoxUserID.FindString(szNewUserName) != -1)
                {
                    szNewUserName = string.Format("{0}_{1}", Context.DBProvider.DefaultUserName, i);
                    i++;
                }

                textBoxUserID.Text = szNewUserName;
            }

            if (rbnSQL.Checked && checkBoxCreate.Checked)
            {
                ValidatePassword();

                if (!string.IsNullOrEmpty(textBoxPassword.Text) || !string.IsNullOrEmpty(textBoxConfirm.Text))
                {
                    ValidatePasswordConfirmation();
                }
            }
            else if (rbnSQL.Checked && !Context.DBProvider.CreateNewDatabase)
            {
                labelConfirmationHint.Visible = false;
                textBoxPassword.BackColor = Color.Empty; // Reset to default
                textBoxConfirm.BackColor = Color.Empty; // Reset to default
            }
            else
            {
                labelPasswordHint.Visible = false;
                labelConfirmationHint.Visible = false;
                textBoxPassword.BackColor = Color.Empty; // Reset to default
                textBoxConfirm.BackColor = Color.Empty; // Reset to default
            }
        }

		private void rbnNT_CheckedChanged(object sender, EventArgs e)
		{
			UpdateUserMode();
		}

		private bool ValidatePassword()
		{
		    string errorMessage;

            PasswordQualityValidator.Validate(this.textBoxPassword.Text, checkBoxCreate.Checked ? this.Context.PasswordQuality : this.Context.CurrentPasswordQuality, out errorMessage);
						
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

		private bool ValidatePasswordConfirmation()
		{
			bool RetVal = false;

			if (textBoxPassword.Text != textBoxConfirm.Text)
			{
				labelConfirmationHint.Text = Messages.WIZ_DB_RECONFIRM_PASSWORD;
				textBoxConfirm.BackColor = Color.LightPink;
				labelConfirmationHint.Visible = true;
			}
			else
			{
				labelConfirmationHint.Text = "";
				textBoxConfirm.BackColor = Color.LightGreen;
				labelConfirmationHint.Visible = false;
				RetVal = true;
			}
			return RetVal;
		}

		private void textBoxPassword_TextChanged(object sender, EventArgs e)
		{
			if (rbnSQL.Checked && checkBoxCreate.Checked)
			{
				ValidatePassword();

				if (!string.IsNullOrEmpty(textBoxConfirm.Text))
				{
					ValidatePasswordConfirmation();
				}
			}
			else if (rbnSQL.Checked && !Context.DBProvider.CreateNewDatabase)
			{
				ValidatePassword();
			}
		}

		private void textBoxConfirm_TextChanged(object sender, EventArgs e)
		{
			if (rbnSQL.Checked && checkBoxCreate.Checked)
			{
				ValidatePasswordConfirmation();
			}
		}

		private void textBoxPassword_Enter(object sender, EventArgs e)
		{
			textBoxPassword.SelectAll();
			if (rbnSQL.Checked && checkBoxCreate.Checked)
			{
				ValidatePassword();
			}
		}

		private void textBoxConfirm_Enter(object sender, EventArgs e)
		{
			textBoxConfirm.SelectAll();
			if (rbnSQL.Checked)
			{
				if (!string.IsNullOrEmpty(textBoxPassword.Text) || !string.IsNullOrEmpty(textBoxConfirm.Text))
				{
					ValidatePasswordConfirmation();
				}
			}
		}

		/// <summary>
		/// Login validation
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private string textBoxUserID_HintEvent(string text)
		{
			if (string.IsNullOrEmpty(text))
				return Messages.WIZ_DB_ENTER_USER;

			if (!_validateRegex.IsMatch(text))
				return Messages.WIZ_DB_INVALID_LOGIN;

			return null;
		}

		/// <summary>
		/// Database name validation
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private string textBoxDBName_HintEvent(string text)
		{
			if (string.IsNullOrEmpty(text))
				return Messages.WIZ_DB_ENTER_DB;

			if (!_validateRegex.IsMatch(text))
				return Messages.WIZ_DB_INVALID_DATABASE_NAME;

			return null;
		}

		private void comboBoxUserID_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBoxUserID.SelectedIndex != -1)
				Context.DBProvider.DatabaseLogin = comboBoxUserID.Text.Trim();
		}

        private bool IsComboboxItemExists(ComboBox comboBox, string itemToSearch)
        {
            foreach (string item in comboBox.Items)
            {
                if (string.Equals(item, itemToSearch, StringComparison.InvariantCultureIgnoreCase))
                {                    
                    return true;
                }
            }

            return false;
        }

	    #region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDB = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxConfirm = new System.Windows.Forms.TextBox();
            this.groupBoxCredentials = new System.Windows.Forms.GroupBox();
            this.chbUseSSL = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.languageIndicator2 = new ITA.Common.UI.LanguageIndicator();
            this.languageIndicator1 = new ITA.Common.UI.LanguageIndicator();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelPasswordHint = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelConfirmationHint = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.rbnSQL = new System.Windows.Forms.RadioButton();
            this.rbnNT = new System.Windows.Forms.RadioButton();
            this.checkBoxCreate = new System.Windows.Forms.CheckBox();
            this.comboBoxUserID = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxUserID = new ITA.Wizards.DatabaseWizard.Controls.TextBoxWithValidator();
            this.textBoxDBName = new ITA.Wizards.DatabaseWizard.Controls.TextBoxWithValidator();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.groupBoxCredentials.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Text = "Select database";
            // 
            // labelDescription
            // 
            this.labelDescription.Text = "Specify the database connection settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(15, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Specify the database name";
            // 
            // comboBoxDB
            // 
            this.comboBoxDB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDB.Location = new System.Drawing.Point(244, 10);
            this.comboBoxDB.Name = "comboBoxDB";
            this.comboBoxDB.Size = new System.Drawing.Size(181, 21);
            this.comboBoxDB.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(241, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(184, 16);
            this.label6.TabIndex = 4;
            this.label6.Text = "&Password:";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(244, 138);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(181, 20);
            this.textBoxPassword.TabIndex = 8;
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            this.textBoxPassword.Enter += new System.EventHandler(this.textBoxPassword_Enter);
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(241, 176);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(184, 16);
            this.label7.TabIndex = 6;
            this.label7.Text = "Password co&nfirmation:";
            // 
            // textBoxConfirm
            // 
            this.textBoxConfirm.Location = new System.Drawing.Point(244, 194);
            this.textBoxConfirm.Name = "textBoxConfirm";
            this.textBoxConfirm.Size = new System.Drawing.Size(181, 20);
            this.textBoxConfirm.TabIndex = 9;
            this.textBoxConfirm.UseSystemPasswordChar = true;
            this.textBoxConfirm.TextChanged += new System.EventHandler(this.textBoxConfirm_TextChanged);
            this.textBoxConfirm.Enter += new System.EventHandler(this.textBoxConfirm_Enter);
            // 
            // groupBoxCredentials
            // 
            this.groupBoxCredentials.Controls.Add(this.chbUseSSL);
            this.groupBoxCredentials.Controls.Add(this.label2);
            this.groupBoxCredentials.Controls.Add(this.label1);
            this.groupBoxCredentials.Controls.Add(this.languageIndicator2);
            this.groupBoxCredentials.Controls.Add(this.languageIndicator1);
            this.groupBoxCredentials.Controls.Add(this.panel2);
            this.groupBoxCredentials.Controls.Add(this.panel1);
            this.groupBoxCredentials.Controls.Add(this.label9);
            this.groupBoxCredentials.Controls.Add(this.rbnSQL);
            this.groupBoxCredentials.Controls.Add(this.comboBoxDB);
            this.groupBoxCredentials.Controls.Add(this.rbnNT);
            this.groupBoxCredentials.Controls.Add(this.checkBoxCreate);
            this.groupBoxCredentials.Controls.Add(this.comboBoxUserID);
            this.groupBoxCredentials.Controls.Add(this.textBoxPassword);
            this.groupBoxCredentials.Controls.Add(this.label3);
            this.groupBoxCredentials.Controls.Add(this.label6);
            this.groupBoxCredentials.Controls.Add(this.label7);
            this.groupBoxCredentials.Controls.Add(this.textBoxConfirm);
            this.groupBoxCredentials.Controls.Add(this.textBoxUserID);
            this.groupBoxCredentials.Controls.Add(this.textBoxDBName);
            this.groupBoxCredentials.Location = new System.Drawing.Point(12, 69);
            this.groupBoxCredentials.Name = "groupBoxCredentials";
            this.groupBoxCredentials.Size = new System.Drawing.Size(473, 237);
            this.groupBoxCredentials.TabIndex = 0;
            this.groupBoxCredentials.TabStop = false;
            this.groupBoxCredentials.Text = "Configuring Server connection to DB";
            // 
            // chbUseSSL
            // 
            this.chbUseSSL.AutoSize = true;
            this.chbUseSSL.Location = new System.Drawing.Point(38, 39);
            this.chbUseSSL.Name = "chbUseSSL";
            this.chbUseSSL.Size = new System.Drawing.Size(116, 17);
            this.chbUseSSL.TabIndex = 18;
            this.chbUseSSL.Text = "Secure connection";
            this.chbUseSSL.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(13, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(210, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "Specify the authentication method";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // languageIndicator2
            // 
            this.languageIndicator2.AttachedTo = this.textBoxPassword;
            this.languageIndicator2.BackColor = System.Drawing.Color.RoyalBlue;
            this.languageIndicator2.ForeColor = System.Drawing.Color.White;
            this.languageIndicator2.Location = new System.Drawing.Point(427, 138);
            this.languageIndicator2.Name = "languageIndicator2";
            this.languageIndicator2.Size = new System.Drawing.Size(21, 19);
            this.languageIndicator2.TabIndex = 17;
            this.languageIndicator2.Visible = false;
            // 
            // languageIndicator1
            // 
            this.languageIndicator1.AttachedTo = this.textBoxConfirm;
            this.languageIndicator1.BackColor = System.Drawing.Color.RoyalBlue;
            this.languageIndicator1.ForeColor = System.Drawing.Color.White;
            this.languageIndicator1.Location = new System.Drawing.Point(427, 194);
            this.languageIndicator1.Name = "languageIndicator1";
            this.languageIndicator1.Size = new System.Drawing.Size(21, 19);
            this.languageIndicator1.TabIndex = 16;
            this.languageIndicator1.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelPasswordHint);
            this.panel2.Location = new System.Drawing.Point(16, 159);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(409, 16);
            this.panel2.TabIndex = 14;
            // 
            // labelPasswordHint
            // 
            this.labelPasswordHint.AutoEllipsis = true;
            this.labelPasswordHint.AutoSize = true;
            this.labelPasswordHint.BackColor = System.Drawing.Color.Gold;
            this.labelPasswordHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelPasswordHint.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelPasswordHint.Location = new System.Drawing.Point(374, 0);
            this.labelPasswordHint.Name = "labelPasswordHint";
            this.labelPasswordHint.Size = new System.Drawing.Size(35, 13);
            this.labelPasswordHint.TabIndex = 13;
            this.labelPasswordHint.Text = "label4";
            this.labelPasswordHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelPasswordHint.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelConfirmationHint);
            this.panel1.Location = new System.Drawing.Point(16, 216);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(409, 15);
            this.panel1.TabIndex = 14;
            // 
            // labelConfirmationHint
            // 
            this.labelConfirmationHint.AutoEllipsis = true;
            this.labelConfirmationHint.AutoSize = true;
            this.labelConfirmationHint.BackColor = System.Drawing.Color.Gold;
            this.labelConfirmationHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelConfirmationHint.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelConfirmationHint.Location = new System.Drawing.Point(374, 0);
            this.labelConfirmationHint.Name = "labelConfirmationHint";
            this.labelConfirmationHint.Size = new System.Drawing.Size(35, 13);
            this.labelConfirmationHint.TabIndex = 13;
            this.labelConfirmationHint.Text = "label4";
            this.labelConfirmationHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelConfirmationHint.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(15, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(136, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Specify the database name";
            // 
            // rbnSQL
            // 
            this.rbnSQL.AutoSize = true;
            this.rbnSQL.BackColor = System.Drawing.Color.Transparent;
            this.rbnSQL.Location = new System.Drawing.Point(19, 111);
            this.rbnSQL.Name = "rbnSQL";
            this.rbnSQL.Size = new System.Drawing.Size(80, 17);
            this.rbnSQL.TabIndex = 6;
            this.rbnSQL.Text = global::ITA.Wizards.Messages.WIZ_SQL_AUTH;
            this.rbnSQL.UseVisualStyleBackColor = false;
            // 
            // rbnNT
            // 
            this.rbnNT.AutoSize = true;
            this.rbnNT.BackColor = System.Drawing.Color.Transparent;
            this.rbnNT.Checked = true;
            this.rbnNT.Location = new System.Drawing.Point(19, 84);
            this.rbnNT.Name = "rbnNT";
            this.rbnNT.Size = new System.Drawing.Size(87, 17);
            this.rbnNT.TabIndex = 5;
            this.rbnNT.TabStop = true;
            this.rbnNT.Text = "&Windows NT";
            this.rbnNT.UseVisualStyleBackColor = false;
            this.rbnNT.CheckedChanged += new System.EventHandler(this.rbnNT_CheckedChanged);
            // 
            // checkBoxCreate
            // 
            this.checkBoxCreate.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxCreate.Checked = true;
            this.checkBoxCreate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCreate.Location = new System.Drawing.Point(38, 129);
            this.checkBoxCreate.Name = "checkBoxCreate";
            this.checkBoxCreate.Size = new System.Drawing.Size(192, 24);
            this.checkBoxCreate.TabIndex = 7;
            this.checkBoxCreate.Text = global::ITA.Wizards.Messages.WIZ_SQL_CREATE_NEW_LOGIN;
            this.checkBoxCreate.UseVisualStyleBackColor = false;
            this.checkBoxCreate.CheckedChanged += new System.EventHandler(this.checkBoxCreate_CheckedChanged);
            // 
            // comboBoxUserID
            // 
            this.comboBoxUserID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUserID.Location = new System.Drawing.Point(243, 82);
            this.comboBoxUserID.Name = "comboBoxUserID";
            this.comboBoxUserID.Size = new System.Drawing.Size(182, 21);
            this.comboBoxUserID.TabIndex = 4;
            this.comboBoxUserID.SelectedIndexChanged += new System.EventHandler(this.comboBoxUserID_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(241, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "&Login:";
            // 
            // textBoxUserID
            // 
            this.textBoxUserID.BackColor = System.Drawing.Color.Transparent;
            this.textBoxUserID.IsValid = false;
            this.textBoxUserID.Location = new System.Drawing.Point(243, 81);
            this.textBoxUserID.Name = "textBoxUserID";
            this.textBoxUserID.Size = new System.Drawing.Size(182, 36);
            this.textBoxUserID.TabIndex = 4;
            // 
            // textBoxDBName
            // 
            this.textBoxDBName.BackColor = System.Drawing.Color.Transparent;
            this.textBoxDBName.IsValid = false;
            this.textBoxDBName.Location = new System.Drawing.Point(243, 10);
            this.textBoxDBName.Name = "textBoxDBName";
            this.textBoxDBName.Size = new System.Drawing.Size(182, 40);
            this.textBoxDBName.TabIndex = 1;
            // 
            // SelectMSSQLDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxCredentials);
            this.Name = "SelectMSSQLDB";
            this.Controls.SetChildIndex(this.panelPlaceholder, 0);
            this.Controls.SetChildIndex(this.groupBoxCredentials, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.groupBoxCredentials.ResumeLayout(false);
            this.groupBoxCredentials.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
	}
}