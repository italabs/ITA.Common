using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ITA.Common.Passwords;
using ITA.Common.UI;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;
using MySql2 = MySql.Data.MySqlClient;

namespace ITA.Wizards.DatabaseWizard.Pages.MySql
{
    public class SelectMySqlDB : CustomPage
    {
        private readonly Regex _validateRegex = new Regex(@"^[0-9a-zA-Z_а-яА-Я]+$");

        private readonly IContainer components;
        private Label label1;
        private ComboBox comboBoxDB;
        private GroupBox groupBoxCredentials;
        private ComboBox comboBoxUserID;
        private CheckBox checkBoxCreate;
        private TextBox textBoxPassword;
        private Label label6;
        private Label label3;
        private Label label5;
        private Label label2;
        private Label label7;
        private Label label4;
        private TextBox textBoxConfirm;
        private TextBox textBoxUserID;
        private LanguageIndicator languageIndicator2;
        private LanguageIndicator languageIndicator1;
        private Panel panel2;
        private Label labelPasswordHint;
        private Panel panel1;
        private Label labelConfirmationHint;
        private TextBox textBoxDBName;
        private Label label9;

        public SelectMySqlDB()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            this.labelHint.Text = Messages.WIZ_SELECT_DB_MESSAGE;
            this.labelDescription.Text = Messages.WIZ_SQL_SELECT_DB_AND_SETTINGS;
            this.checkBoxCreate.Text = Messages.WIZ_MYSQL_CREATE_NEW_USER;
            this.label1.Text = Messages.WIZ_MYSQL_SELECT_SCHEME;
            this.label6.Text = Messages.WIZ_SQL_PASSWORD;
            this.groupBoxCredentials.Text = Messages.WIZ_MYSQL_USER_SETTINGS;
            this.label5.Text = Messages.WIZ_MYSQL_USER_NAME;
            this.label2.Text = Messages.WIZ_MYSQL_SELECT_USER_SETTINGS;
            this.label7.Text = Messages.WIZ_SQL_PASSWORD_CONFIRM;
            this.label4.Text = Messages.WIZ_MYSQL_SETTINGS_DESCRIPTION;
            this.label9.Text = Messages.WIZ_MYSQL_SET_SCHEME_NAME;
        }       
        
        private DatabaseWizardContext Context
        {
            get
            {
                return Wizard.Context.ValueOf<DatabaseWizardContext>(DatabaseWizardContext.ClassName);
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
                labelPasswordHint.Text = "";
                labelConfirmationHint.Text = "";

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

                // save previous state
                string PrevDB = null;

                if (-1 != comboBoxDB.SelectedIndex)
                {
                    PrevDB = comboBoxDB.Text;
                }

                // List databases
                comboBoxDB.Items.Clear();

                using (IDbConnection connection = Context.DBProvider.GetConnection())
                {
                    connection.Open();

                    IDbCommand command = connection.CreateCommand();

                    command.CommandText = @"select SCHEMA_NAME from information_schema.SCHEMATA
where SCHEMA_NAME <> 'information_schema'
and SCHEMA_NAME <> 'performance_schema'
and SCHEMA_NAME <> 'sys'";

                    IDataReader reader = null;

                    try
                    {
                        reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            string dbName = reader.GetValue(0).ToString();
                            comboBoxDB.Items.Add(dbName);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        if (reader != null) reader.Close();
                        if (connection.State == ConnectionState.Open) connection.Close();
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

                if (-1 != SelectIndex)
                {
                    comboBoxDB.SelectedIndex = SelectIndex;
                }


                // Store prev state
                string PrevLogin = null;

                if (null != comboBoxUserID.Text && "" != comboBoxUserID.Text)
                {
                    PrevLogin = comboBoxUserID.Text;
                }

                // List Logins
                {
                    comboBoxUserID.Items.Clear();

                    using (IDbConnection connection = Context.DBProvider.GetConnection())
                    {
                        connection.Open();
                        IDbCommand command = connection.CreateCommand();
                        command.CommandText =
                            "SELECT User FROM mysql.user";

                        IDataReader reader = null;

                        try
                        {
                            reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                int iNameIndex = reader.GetOrdinal("USER");

                                string login = reader.GetValue(iNameIndex).ToString();

                                comboBoxUserID.Items.Add(login);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            if (reader != null) reader.Close();
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

                    if (null != Context.DBProvider.DatabasePassword)
                    {
                        textBoxPassword.Text = Context.DBProvider.DatabasePassword;
                    }
                    else
                    {
                        textBoxPassword.Text = "";
                    }
                }


                UpdateUserMode();

                if (Context.DBProvider.CreateNewDatabase && textBoxDBName.Text == "")
                {
                    //
                    // Lets choose unique database name
                    //
                    string szNewDBName = Context.DBProvider.DefaultDatabaseName;

                    int i = 1;
                    while (this.IsComboboxItemExists(comboBoxDB, szNewDBName))
                    {
                        szNewDBName = String.Format("{0}_{1}", Context.DBProvider.DefaultDatabaseName, i);
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
                ErrorMessageBox.Show(this, x, x.Message, m_Parent.Text);
            }

            Wizard.EnableButton(Wizard.EButtons.CancelButton);
            Wizard.EnableButton(Wizard.EButtons.NextButton);
            Wizard.EnableButton(Wizard.EButtons.BackButton);
            Wizard.DisableButton(Wizard.EButtons.FinishButton);

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

            if (Context.DBProvider.CreateNewDatabase && !_validateRegex.IsMatch(textBoxDBName.Text))
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

            if (string.IsNullOrEmpty(Context.DBProvider.DatabaseLogin))
            {
                MessageBox.Show(this, Messages.WIZ_DB_ENTER_USER,
                                Wizard.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxUserID.Focus();
                return false;
            }

            if (checkBoxCreate.Checked && !_validateRegex.IsMatch(textBoxUserID.Text))
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
                    using (var connection = new MySql2.MySqlConnection(Context.GetConnectionString(false, DatabaseWizardContext.MasterDatabase)))
                    using (MySql2.MySqlCommand command = connection.CreateCommand())
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
            catch (MySql2.MySqlException ex)
            {               
                ErrorMessageBox.Show(this, ex, Messages.E_ITA_UPDATE_UNKNOWN_VERSION,
                                Wizard.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return false;
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
            if (checkBoxCreate.Checked)
            {
                comboBoxUserID.Visible = false;
                textBoxUserID.Visible = true;

                textBoxConfirm.Enabled = true;
                label7.Enabled = true;

                if (textBoxUserID.Text == "")
                {
                    //
                    // Lets choose unique user name
                    //
                    string szNewUserName = Context.DBProvider.DefaultUserName;
                    int i = 1;
                    while (this.IsComboboxItemExists(comboBoxUserID, szNewUserName))
                    {
                        szNewUserName = String.Format("{0}_{1}", Context.DBProvider.DefaultUserName, i);
                        i++;
                    }

                    textBoxUserID.Text = szNewUserName;
                }

                ValidatePassword();

                if (!string.IsNullOrEmpty(textBoxPassword.Text) || !string.IsNullOrEmpty(textBoxConfirm.Text))
                {
                    ValidatePasswordConfirmation();
                }
            }
            else
            {
                comboBoxUserID.Visible = true;
                textBoxUserID.Visible = false;

                textBoxConfirm.Enabled = false;
                label7.Enabled = false;

                labelConfirmationHint.Visible = false;
                textBoxPassword.BackColor = Color.Empty; // Reset to default
                textBoxConfirm.BackColor = Color.Empty; // Reset to default

                ValidatePassword();
            }
        }

        private bool UserExists(String User)
        {
            Decimal res = 0;
            try
            {
                using (DbConnection connection = Context.DBProvider.GetConnection())
                {
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT COUNT(*) FROM mysql.user WHERE User = :user_name";

                    IDbDataParameter name = command.CreateParameter();
                    name.DbType = DbType.String;
                    name.ParameterName = "user_name";
                    name.SourceColumn = "NAME";
                    name.Size = 30;
                    name.Value = User.ToUpper();

                    command.Parameters.Add(name);

                    res = (Decimal)command.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            return res != 0;
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxUserID = new System.Windows.Forms.ComboBox();
            this.checkBoxCreate = new System.Windows.Forms.CheckBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxDB = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxCredentials = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelConfirmationHint = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelPasswordHint = new System.Windows.Forms.Label();
            this.languageIndicator1 = new ITA.Common.UI.LanguageIndicator();
            this.textBoxConfirm = new System.Windows.Forms.TextBox();
            this.languageIndicator2 = new ITA.Common.UI.LanguageIndicator();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxUserID = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxDBName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.panelPlaceholder.SuspendLayout();
            this.groupBoxCredentials.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this.textBoxDBName);
            this.panelPlaceholder.Controls.Add(this.label1);
            this.panelPlaceholder.Controls.Add(this.comboBoxDB);
            this.panelPlaceholder.Controls.Add(this.groupBoxCredentials);
            this.panelPlaceholder.Controls.Add(this.label9);
            // 
            // comboBoxUserID
            // 
            this.comboBoxUserID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUserID.Location = new System.Drawing.Point(24, 114);
            this.comboBoxUserID.Name = "comboBoxUserID";
            this.comboBoxUserID.Size = new System.Drawing.Size(192, 21);
            this.comboBoxUserID.TabIndex = 3;
            // 
            // checkBoxCreate
            // 
            this.checkBoxCreate.Checked = true;
            this.checkBoxCreate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCreate.Location = new System.Drawing.Point(24, 41);
            this.checkBoxCreate.Name = "checkBoxCreate";
            this.checkBoxCreate.Size = new System.Drawing.Size(192, 24);
            this.checkBoxCreate.TabIndex = 1;
            this.checkBoxCreate.Text = global::ITA.Wizards.Messages.WIZ_MYSQL_CREATE_NEW_USER;
            this.checkBoxCreate.CheckedChanged += new System.EventHandler(this.checkBoxCreate_CheckedChanged);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(248, 57);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(176, 20);
            this.textBoxPassword.TabIndex = 2;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            this.textBoxPassword.Enter += new System.EventHandler(this.textBoxPassword_Enter);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 24);
            this.label1.TabIndex = 20;
            this.label1.Text = "Select database scheme";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(248, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 16);
            this.label6.TabIndex = 4;
            this.label6.Text = "&Password:";
            // 
            // comboBoxDB
            // 
            this.comboBoxDB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDB.Location = new System.Drawing.Point(241, 20);
            this.comboBoxDB.Name = "comboBoxDB";
            this.comboBoxDB.Size = new System.Drawing.Size(203, 21);
            this.comboBoxDB.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 16);
            this.label3.TabIndex = 4;
            // 
            // groupBoxCredentials
            // 
            this.groupBoxCredentials.Controls.Add(this.comboBoxUserID);
            this.groupBoxCredentials.Controls.Add(this.panel1);
            this.groupBoxCredentials.Controls.Add(this.panel2);
            this.groupBoxCredentials.Controls.Add(this.languageIndicator1);
            this.groupBoxCredentials.Controls.Add(this.languageIndicator2);
            this.groupBoxCredentials.Controls.Add(this.checkBoxCreate);
            this.groupBoxCredentials.Controls.Add(this.textBoxPassword);
            this.groupBoxCredentials.Controls.Add(this.label6);
            this.groupBoxCredentials.Controls.Add(this.label3);
            this.groupBoxCredentials.Controls.Add(this.label5);
            this.groupBoxCredentials.Controls.Add(this.label2);
            this.groupBoxCredentials.Controls.Add(this.label7);
            this.groupBoxCredentials.Controls.Add(this.label4);
            this.groupBoxCredentials.Controls.Add(this.textBoxConfirm);
            this.groupBoxCredentials.Controls.Add(this.textBoxUserID);
            this.groupBoxCredentials.Location = new System.Drawing.Point(20, 52);
            this.groupBoxCredentials.Name = "groupBoxCredentials";
            this.groupBoxCredentials.Size = new System.Drawing.Size(461, 195);
            this.groupBoxCredentials.TabIndex = 1;
            this.groupBoxCredentials.TabStop = false;
            this.groupBoxCredentials.Text = "User settings";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelConfirmationHint);
            this.panel1.Location = new System.Drawing.Point(25, 141);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(399, 17);
            this.panel1.TabIndex = 21;
            // 
            // labelConfirmationHint
            // 
            this.labelConfirmationHint.AutoEllipsis = true;
            this.labelConfirmationHint.AutoSize = true;
            this.labelConfirmationHint.BackColor = System.Drawing.Color.Gold;
            this.labelConfirmationHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelConfirmationHint.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelConfirmationHint.Location = new System.Drawing.Point(364, 0);
            this.labelConfirmationHint.Name = "labelConfirmationHint";
            this.labelConfirmationHint.Size = new System.Drawing.Size(35, 13);
            this.labelConfirmationHint.TabIndex = 13;
            this.labelConfirmationHint.Text = "label4";
            this.labelConfirmationHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelConfirmationHint.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelPasswordHint);
            this.panel2.Location = new System.Drawing.Point(26, 82);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(398, 12);
            this.panel2.TabIndex = 20;
            // 
            // labelPasswordHint
            // 
            this.labelPasswordHint.AutoEllipsis = true;
            this.labelPasswordHint.AutoSize = true;
            this.labelPasswordHint.BackColor = System.Drawing.Color.Gold;
            this.labelPasswordHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelPasswordHint.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelPasswordHint.Location = new System.Drawing.Point(363, 0);
            this.labelPasswordHint.Name = "labelPasswordHint";
            this.labelPasswordHint.Size = new System.Drawing.Size(35, 13);
            this.labelPasswordHint.TabIndex = 13;
            this.labelPasswordHint.Text = "label4";
            this.labelPasswordHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelPasswordHint.Visible = false;
            // 
            // languageIndicator1
            // 
            this.languageIndicator1.AttachedTo = this.textBoxConfirm;
            this.languageIndicator1.BackColor = System.Drawing.Color.RoyalBlue;
            this.languageIndicator1.ForeColor = System.Drawing.Color.White;
            this.languageIndicator1.Location = new System.Drawing.Point(426, 114);
            this.languageIndicator1.Name = "languageIndicator1";
            this.languageIndicator1.Size = new System.Drawing.Size(21, 19);
            this.languageIndicator1.TabIndex = 19;
            this.languageIndicator1.Visible = false;
            // 
            // textBoxConfirm
            // 
            this.textBoxConfirm.Location = new System.Drawing.Point(248, 114);
            this.textBoxConfirm.Name = "textBoxConfirm";
            this.textBoxConfirm.PasswordChar = '*';
            this.textBoxConfirm.Size = new System.Drawing.Size(176, 20);
            this.textBoxConfirm.TabIndex = 4;
            this.textBoxConfirm.TextChanged += new System.EventHandler(this.textBoxConfirm_TextChanged);
            this.textBoxConfirm.Enter += new System.EventHandler(this.textBoxConfirm_Enter);
            // 
            // languageIndicator2
            // 
            this.languageIndicator2.AttachedTo = this.textBoxPassword;
            this.languageIndicator2.BackColor = System.Drawing.Color.RoyalBlue;
            this.languageIndicator2.ForeColor = System.Drawing.Color.White;
            this.languageIndicator2.Location = new System.Drawing.Point(426, 58);
            this.languageIndicator2.Name = "languageIndicator2";
            this.languageIndicator2.Size = new System.Drawing.Size(21, 19);
            this.languageIndicator2.TabIndex = 18;
            this.languageIndicator2.Visible = false;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(24, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 16);
            this.label5.TabIndex = 2;
            this.label5.Text = "&User name:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(24, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(376, 32);
            this.label2.TabIndex = 0;
            this.label2.Text = "Specify the user settings that will be used to connect to the database";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(248, 97);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(152, 14);
            this.label7.TabIndex = 6;
            this.label7.Text = "Password co&nfirmation:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(24, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(400, 30);
            this.label4.TabIndex = 9;
            this.label4.Text = "User supplied parameters will be used for server connection and will be assigned " +
    "to the selected database";
            // 
            // textBoxUserID
            // 
            this.textBoxUserID.Location = new System.Drawing.Point(24, 114);
            this.textBoxUserID.Name = "textBoxUserID";
            this.textBoxUserID.Size = new System.Drawing.Size(192, 20);
            this.textBoxUserID.TabIndex = 4;
            this.textBoxUserID.Visible = false;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(27, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(168, 24);
            this.label9.TabIndex = 19;
            this.label9.Text = "Please enter the &database schema name:";
            // 
            // textBoxDBName
            // 
            this.textBoxDBName.Location = new System.Drawing.Point(241, 20);
            this.textBoxDBName.Name = "textBoxDBName";
            this.textBoxDBName.Size = new System.Drawing.Size(203, 20);
            this.textBoxDBName.TabIndex = 0;
            // 
            // SelectMySqlDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SelectMySqlDB";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.panelPlaceholder.ResumeLayout(false);
            this.panelPlaceholder.PerformLayout();
            this.groupBoxCredentials.ResumeLayout(false);
            this.groupBoxCredentials.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            ValidatePassword();
            
            if (checkBoxCreate.Checked)
            {
                if (!string.IsNullOrEmpty(textBoxConfirm.Text))
                {
                    ValidatePasswordConfirmation();
                }
            }
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

        private void textBoxPassword_Enter(object sender, EventArgs e)
        {
            ValidatePassword();
        }

        private void textBoxConfirm_TextChanged(object sender, EventArgs e)
        {
            if (checkBoxCreate.Checked)
            {
                ValidatePasswordConfirmation();
            }
        }

        private void textBoxConfirm_Enter(object sender, EventArgs e)
        {
            textBoxConfirm.SelectAll();
            if (!string.IsNullOrEmpty(textBoxPassword.Text) || !string.IsNullOrEmpty(textBoxConfirm.Text))
            {
                ValidatePasswordConfirmation();
            }
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
    }
}