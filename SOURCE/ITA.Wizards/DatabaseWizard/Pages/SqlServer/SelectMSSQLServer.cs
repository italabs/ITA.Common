using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using ITA.Common.Passwords;
using ITA.Common.UI;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;

namespace ITA.Wizards.DatabaseWizard.Pages.SqlServer
{
    /// <summary>
    /// Шаг выбора сервера БД
    /// </summary>
    public class SelectMSSQLServer : CustomPage
    {
        private const string CREATE_DATABASE_PERMISSION = "CREATE ANY DATABASE";
        private const string CREATE_LOGIN_PERMISSION = "ALTER ANY LOGIN";

        private Button buttonTest;
        private ComboBox comboBoxServer;
        private IContainer components;
        private GroupBox groupBox3;
        private Label label1;
        private Label label2;
        private Label labelLogin;
        private Label labelPassword;
        private Label labelPasswordHint;
        private LanguageIndicator languageIndicator1;
        private bool m_bServersListComplete;
        private Panel panel1;
        private Panel panel2;
        private PictureBox pictureBoxProgress;
        private PictureBox pictureBoxSearch;
        private RadioButton rbnNT;
        private RadioButton rbnSQL;
        private TextBox textBoxLogin;
        private TextBox textBoxPassword;
        private ToolTip toolTip1;

        BackgroundWorker _worker = new BackgroundWorker();
        private CheckBox chbUseSSL;
        private bool isCanceled = false;

        public SelectMSSQLServer()
        {
            InitializeComponent();
            _worker.WorkerSupportsCancellation = true;
			this.labelHint.Text = Messages.WIZ_SQL_SELECTING_CONNECTION_SETTINGS;
			this.labelDescription.Text = Messages.WIZ_SQL_SET_SERVER_NAME_AND_AUTH_MODE;
			this.label1.Text = Messages.WIZ_SELECT_SERVER_NAME;
			this.label2.Text = Messages.WIZ_SQL_PROVIDE_AUTH_MODE;
			this.labelLogin.Text = Messages.WIZ_SQL_LOGIN;
			this.labelPassword.Text = Messages.WIZ_SQL_PASSWORD;
			this.buttonTest.Text = Messages.WIZ_SQL_TEST_CONNECTION;
			this.groupBox3.Text = Messages.WIZ_SQL_CONFIGURING_ADMIIN_CONNECTION;
			this.toolTip1.SetToolTip(this.pictureBoxSearch, Messages.WIZ_SQL_PRESS_FOR_UPDATING_SERVER_LIST);
			this.toolTip1.SetToolTip(this.pictureBoxProgress, Messages.WIZ_SQL_SERVER_LIST_IS_UPDATING);
			this.rbnSQL.Text = Messages.WIZ_SQL_AUTH;
			this.rbnNT.Text = Messages.WIZ_SQL_NT_AUTH;
            this.chbUseSSL.Text = Messages.WIZ_SQL_USE_SSL;
        }

        public DatabaseWizardContext Context
        {
            get { return Wizard.Context.ValueOf<DatabaseWizardContext>(DatabaseWizardContext.ClassName); }
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

        /// <summary>
        /// Тестирование соединения
        /// </summary>
        /// <returns></returns>
        private bool TestConnection()
        {
            if (string.IsNullOrEmpty(Context.DBProvider.ServerName.Trim()))
            {
                MessageBox.Show(this, Messages.WIZ_SRV_SELECT, Wizard.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return false;
            }

            using (var connection = new SqlConnection(Context.GetServerConnectionString()))
            using (SqlCommand command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();

                    if (Context.DBProvider.EncryptAdminConnection)
                    {
                        command.CommandText = @"SELECT encrypt_option FROM sys.dm_exec_connections WHERE session_id = @@SPID";
                        string res = (string)command.ExecuteScalar();

                        if (res != "TRUE")
                        {
                            MessageBox.Show(this, Messages.WIZ_SRV_SSL_FAILED, this.Wizard.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception x)
                {
                    ErrorMessageBox.Show(this, x, string.Format(Messages.WIZ_SRV_LOGIN_FAILED, comboBoxServer.Text),
                                         Wizard.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }


        /// <summary>
        /// Проверка наличия у текущего пользователя разрешения
        /// </summary>
        /// <returns></returns>
        private bool HasPermission(string permissionName)
        {
            using (var connection = new SqlConnection(Context.GetServerConnectionString()))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = string.Format("SELECT has_perms_by_name(null, null, '{0}')", permissionName);

                var hasPermission = command.ExecuteScalar();

                if (hasPermission == null)
                {
                    return false;
                }

                int result;

                if (!int.TryParse(hasPermission.ToString(), out result))
                {
                    return false;
                }

                connection.Close();

                return result > 0;
            }
        }

        /// <summary>
        /// Проверка наличия у текущего пользователя прав, нобходимых для работы визарда
        /// </summary>
        /// <returns></returns>
        private bool TestAdminPermissions()
        {
            try
            {
                if (!HasPermission(CREATE_DATABASE_PERMISSION))
                {
                    throw new Exception(Messages.WIZ_SRV_NO_CREATE_DB_PERMISSION);
                }

                if (!HasPermission(CREATE_LOGIN_PERMISSION))
                {
                    throw new Exception(Messages.WIZ_SRV_NO_CREATE_LOGIN_PERMISSION);
                }

                return true;
            }
            catch (Exception exception)
            {
                ErrorMessageBox.Show(this, exception, Messages.WIZ_SRV_INVALID_DB_PERMISSIONS, Wizard.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }            
        }
    
        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            if (rbnSQL.Checked)
            {
                ValidatePassword();
            }
        }

        private void textBoxPassword_Enter(object sender, EventArgs e)
        {
            textBoxPassword.SelectAll();
            if (rbnSQL.Checked)
            {
                ValidatePassword();
            }
        }

        #region [Async server search]


        /// <summary>
        /// UI-thread safe invoke
        /// </summary>
        /// <param name="d"></param>
        private void SyncImpl(SyncDelegate d)
        {
            d.Invoke();
        }

        /// <summary>
        /// Synchronize UI and work threads
        /// </summary>
        /// <param name="d"></param>
        private void Sync(SyncDelegate d)
        {
            if (InvokeRequired)
            {
                Invoke(new SyncImplDelegate(SyncImpl), d);
            }
            else
            {
                SyncImpl(d);
            }
        }
        private void ListServersStart()
        {
            pictureBoxProgress.Enabled = true;
            pictureBoxProgress.Visible = true;
            pictureBoxSearch.Enabled = false;
            pictureBoxSearch.Visible = false;
            if(!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        private string[] ListServersAsyncProc(BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                return ServerEnumerator.EnumServers();
            }
            return new string[0];
        }

        #region Nested type: SyncDelegate

        private delegate void SyncDelegate();

        #endregion

        #region Nested type: SyncImplDelegate

        private delegate void SyncImplDelegate(SyncDelegate d);

        #endregion

        #endregion

        #region [UI event handlers]

        private void buttonTest_Click(object sender, EventArgs e)
        {
            if (OnValidate())
            {
                MessageBox.Show(this, Messages.WIZ_SRV_LOGIN_SUCCEEDED, Wizard.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        private void comboBoxServer_DropDown(object sender, EventArgs e)
        {
            if (!m_bServersListComplete)
            {
                ListServersStart();
            }
        }

        private void SelectServer_Load(object sender, EventArgs e)
        {
            rbnNT.Checked = true;
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            ListServersStart();
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Sync(delegate
                         {
                             pictureBoxProgress.Enabled = false;
                             pictureBoxProgress.Visible = false;
                             pictureBoxSearch.Enabled = true;
                             pictureBoxSearch.Visible = true;
                         });


                if (e.Error != null && !e.Cancelled && !isCanceled)
                {
                    Sync(
                    delegate
                    {
                        ErrorMessageBox.Show(this, e.Error, Messages.E_DBWIZARD_LIST_SERVERS, Wizard.Text,
                                             MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                    
                }
                else if (!e.Cancelled && !isCanceled)
                {
                    string[] Servers = (string[])e.Result;
                    Sync(delegate
                             {
                                 m_bServersListComplete = true;
                                 comboBoxServer.Items.Clear();

                                 comboBoxServer.Items.AddRange(Servers);
                                 if (Visible)
                                 {
                                     if (comboBoxServer.Items.Count == 1 &&
                                         (string.IsNullOrEmpty(comboBoxServer.Text) ||
                                          comboBoxServer.Text.ToUpperInvariant() ==
                                          ((string)comboBoxServer.Items[0]).ToUpperInvariant()))
                                     {
                                         //
                                         // Automatically select the first item if it is the only item and 
                                         // user hasn't typed anything else in the edit part of the combo
                                         //
                                         comboBoxServer.SelectedIndex = 0;
                                     }
                                     else
                                     {
                                         comboBoxServer.DroppedDown = true;
                                     }
                                 }
                             });
                }
            }
            catch (Exception x)
            {
                Sync(
                    delegate
                    {
                        ErrorMessageBox.Show(this, x, Messages.E_DBWIZARD_LIST_SERVERS, Wizard.Text,
                                             MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
            }
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = ListServersAsyncProc(worker, e);
        }

        private void rbnNT_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLogin.Enabled = !rbnNT.Checked;
            textBoxPassword.Enabled = !rbnNT.Checked;
            labelPasswordHint.Visible = !rbnNT.Checked;
            labelPasswordHint.Text = String.Empty;
        }

        /// <summary>
        /// Принудительное обновление списка серверов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxProgress_Click(object sender, EventArgs e)
        {
            m_bServersListComplete = false;
            ListServersStart();
        }

        #endregion

        #region [Wizard overridables]

        public override bool OnValidate()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                Context.DBProvider.ServerName = comboBoxServer.Text;
                Context.DBProvider.EncryptAdminConnection = this.chbUseSSL.Checked;

                if (this.chbUseSSL.Checked)
                {
                    Context.DBProvider.EncryptDatabaseConnection = true;
                }

                if (rbnNT.Checked)
                {
                    Context.DBProvider.ServerCredentialType = ELoginType.NT;
                }
                else
                {
                    Context.DBProvider.ServerCredentialType = ELoginType.SQL;
                    Context.DBProvider.ServerLogin = textBoxLogin.Text.Trim();
                    Context.DBProvider.ServerPassword = textBoxPassword.Text.Trim();

                    if (string.IsNullOrEmpty(Context.DBProvider.ServerLogin))
                    {
                        MessageBox.Show(this, Messages.WIZ_DB_ENTER_USER,
                                        Wizard.Text,
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        textBoxLogin.Focus();
                        return false;
                    }

                    if (!ValidatePassword())
                    {
                        MessageBox.Show(this, Messages.WIZ_ORACLE_PASSWORD_IS_NOT_PROVIDED,
                                        Wizard.Text,
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        textBoxPassword.Focus();
                        return false;
                    }
                }

                if (!TestConnection())
                    return false;

                if (!TestAdminPermissions())
                    return false;

                return true;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        public override void OnActive()
        {
            Wizard.EnableButton(Wizard.EButtons.CancelButton);
            Wizard.EnableButton(Wizard.EButtons.NextButton);
            Wizard.EnableButton(Wizard.EButtons.BackButton);
            Wizard.DisableButton(Wizard.EButtons.FinishButton);

            comboBoxServer.Text = Context.DBProvider.ServerName;
            rbnNT.Checked = Context.DBProvider.ServerCredentialType == ELoginType.NT;
            rbnSQL.Checked = !rbnNT.Checked;
            textBoxLogin.Text = Context.DBProvider.ServerLogin;
            textBoxPassword.Text = Context.DBProvider.ServerPassword;

            comboBoxServer.Focus();

        	rbnNT.Enabled = ((DatabaseWizard) Wizard).EnableChangingServerCredentialType;
			rbnSQL.Enabled = ((DatabaseWizard)Wizard).EnableChangingServerCredentialType;

            base.OnActive();
        }

        public override void OnCancel()
        {
            _worker.CancelAsync();
            isCanceled = true;
            base.OnCancel();
        }

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxServer = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.labelLogin = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chbUseSSL = new System.Windows.Forms.CheckBox();
            this.pictureBoxSearch = new System.Windows.Forms.PictureBox();
            this.pictureBoxProgress = new System.Windows.Forms.PictureBox();
            this.rbnSQL = new System.Windows.Forms.RadioButton();
            this.rbnNT = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelPasswordHint = new System.Windows.Forms.Label();
            this.languageIndicator1 = new ITA.Common.UI.LanguageIndicator();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.panelPlaceholder.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Text = "Select connection settings";
            // 
            // labelDescription
            // 
            this.labelDescription.Size = new System.Drawing.Size(333, 32);
            this.labelDescription.Text = "Specify the database server and its authentication";
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this.groupBox3);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Specify the &database server";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxServer
            // 
            this.comboBoxServer.Location = new System.Drawing.Point(254, 16);
            this.comboBoxServer.Name = "comboBoxServer";
            this.comboBoxServer.Size = new System.Drawing.Size(188, 21);
            this.comboBoxServer.TabIndex = 1;
            this.comboBoxServer.DropDown += new System.EventHandler(this.comboBoxServer_DropDown);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(14, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(413, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Specify the authentication method for the administrative connection:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.Location = new System.Drawing.Point(254, 106);
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.Size = new System.Drawing.Size(181, 20);
            this.textBoxLogin.TabIndex = 5;
            // 
            // labelLogin
            // 
            this.labelLogin.Location = new System.Drawing.Point(246, 25);
            this.labelLogin.Name = "labelLogin";
            this.labelLogin.Size = new System.Drawing.Size(100, 16);
            this.labelLogin.TabIndex = 0;
            this.labelLogin.Text = "&Логин:";
            this.labelLogin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelPassword
            // 
            this.labelPassword.Location = new System.Drawing.Point(245, 75);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(100, 16);
            this.labelPassword.TabIndex = 1;
            this.labelPassword.Text = "&Пароль:";
            this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(254, 154);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(181, 20);
            this.textBoxPassword.TabIndex = 6;
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            this.textBoxPassword.Enter += new System.EventHandler(this.textBoxPassword_Enter);
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(251, 144);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(113, 23);
            this.buttonTest.TabIndex = 0;
            this.buttonTest.Text = global::ITA.Wizards.Messages.WIZ_SQL_TEST_CONNECTION;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chbUseSSL);
            this.groupBox3.Controls.Add(this.textBoxPassword);
            this.groupBox3.Controls.Add(this.pictureBoxSearch);
            this.groupBox3.Controls.Add(this.pictureBoxProgress);
            this.groupBox3.Controls.Add(this.textBoxLogin);
            this.groupBox3.Controls.Add(this.rbnSQL);
            this.groupBox3.Controls.Add(this.rbnNT);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.comboBoxServer);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Location = new System.Drawing.Point(12, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(471, 237);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Configuring administrative connection";
            // 
            // chbUseSSL
            // 
            this.chbUseSSL.AutoSize = true;
            this.chbUseSSL.Location = new System.Drawing.Point(38, 39);
            this.chbUseSSL.Name = "chbUseSSL";
            this.chbUseSSL.Size = new System.Drawing.Size(116, 17);
            this.chbUseSSL.TabIndex = 19;
            this.chbUseSSL.Text = "Secure connection";
            this.chbUseSSL.UseVisualStyleBackColor = true;
            // 
            // pictureBoxSearch
            // 
            this.pictureBoxSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxSearch.Image = global::ITA.Wizards.Properties.Resources.indicator_arrows_static1;
            this.pictureBoxSearch.Location = new System.Drawing.Point(443, 16);
            this.pictureBoxSearch.Name = "pictureBoxSearch";
            this.pictureBoxSearch.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxSearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxSearch.TabIndex = 13;
            this.pictureBoxSearch.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxSearch, global::ITA.Wizards.Messages.WIZ_SQL_PRESS_FOR_UPDATING_SERVER_LIST);
            this.pictureBoxSearch.Click += new System.EventHandler(this.pictureBoxProgress_Click);
            // 
            // pictureBoxProgress
            // 
            this.pictureBoxProgress.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBoxProgress.Enabled = false;
            this.pictureBoxProgress.Image = global::ITA.Wizards.Properties.Resources.indicator_arrows;
            this.pictureBoxProgress.Location = new System.Drawing.Point(443, 16);
            this.pictureBoxProgress.Name = "pictureBoxProgress";
            this.pictureBoxProgress.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxProgress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxProgress.TabIndex = 13;
            this.pictureBoxProgress.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxProgress, global::ITA.Wizards.Messages.WIZ_SQL_SERVER_LIST_IS_UPDATING);
            this.pictureBoxProgress.Visible = false;
            this.pictureBoxProgress.Click += new System.EventHandler(this.pictureBoxProgress_Click);
            // 
            // rbnSQL
            // 
            this.rbnSQL.AutoSize = true;
            this.rbnSQL.Location = new System.Drawing.Point(19, 109);
            this.rbnSQL.Name = "rbnSQL";
            this.rbnSQL.Size = new System.Drawing.Size(145, 17);
            this.rbnSQL.TabIndex = 4;
            this.rbnSQL.TabStop = true;
            this.rbnSQL.Text = "Средствами &SQL Server";
            this.rbnSQL.UseVisualStyleBackColor = true;
            // 
            // rbnNT
            // 
            this.rbnNT.AutoSize = true;
            this.rbnNT.Location = new System.Drawing.Point(19, 85);
            this.rbnNT.Name = "rbnNT";
            this.rbnNT.Size = new System.Drawing.Size(134, 17);
            this.rbnNT.TabIndex = 3;
            this.rbnNT.TabStop = true;
            this.rbnNT.Text = "Средствами &Windows";
            this.rbnNT.UseVisualStyleBackColor = true;
            this.rbnNT.CheckedChanged += new System.EventHandler(this.rbnNT_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.languageIndicator1);
            this.panel1.Controls.Add(this.buttonTest);
            this.panel1.Controls.Add(this.labelPassword);
            this.panel1.Controls.Add(this.labelLogin);
            this.panel1.Location = new System.Drawing.Point(6, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(459, 174);
            this.panel1.TabIndex = 10;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelPasswordHint);
            this.panel2.Location = new System.Drawing.Point(16, 122);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(436, 16);
            this.panel2.TabIndex = 15;
            // 
            // labelPasswordHint
            // 
            this.labelPasswordHint.AutoEllipsis = true;
            this.labelPasswordHint.AutoSize = true;
            this.labelPasswordHint.BackColor = System.Drawing.Color.Gold;
            this.labelPasswordHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelPasswordHint.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelPasswordHint.Location = new System.Drawing.Point(401, 0);
            this.labelPasswordHint.Name = "labelPasswordHint";
            this.labelPasswordHint.Size = new System.Drawing.Size(35, 13);
            this.labelPasswordHint.TabIndex = 13;
            this.labelPasswordHint.Text = "label4";
            this.labelPasswordHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelPasswordHint.Visible = false;
            // 
            // languageIndicator1
            // 
            this.languageIndicator1.AttachedTo = this.textBoxPassword;
            this.languageIndicator1.BackColor = System.Drawing.Color.RoyalBlue;
            this.languageIndicator1.ForeColor = System.Drawing.Color.White;
            this.languageIndicator1.Location = new System.Drawing.Point(431, 98);
            this.languageIndicator1.Name = "languageIndicator1";
            this.languageIndicator1.Size = new System.Drawing.Size(21, 19);
            this.languageIndicator1.TabIndex = 2;
            this.languageIndicator1.Visible = false;
            // 
            // SelectMSSQLServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SelectMSSQLServer";
            this.Load += new System.EventHandler(this.SelectServer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.panelPlaceholder.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}