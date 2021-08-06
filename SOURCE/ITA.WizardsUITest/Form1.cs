using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using ITA.Common.Passwords;
using ITA.Common.UI;
using ITA.Wizards.DatabaseWizard.Model;
using ITA.WizardsUITest.DemoDbWizard;

namespace ITA.WizardsUITest
{
    public partial class Form1 : Form
    {
        private readonly string[] _args;

        public Form1(string[] args)
        {
            _args = args;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(cmbCulture.SelectedItem.ToString());
			Thread.CurrentThread.CurrentCulture = new CultureInfo(cmbCulture.SelectedItem.ToString());

            // создаем компонент для обновления версии базы
            string sqlServerConnectionString = null;
            string oracleConnectionString = null;
            string mySqlConnectionString = null;

            if (ConfigurationManager.ConnectionStrings != null && ConfigurationManager.ConnectionStrings["SqlServerConnection"] != null)
                sqlServerConnectionString = ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString;
           
            if (ConfigurationManager.ConnectionStrings != null && ConfigurationManager.ConnectionStrings["OracleConnection"] != null)
                oracleConnectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;

            if (ConfigurationManager.ConnectionStrings != null && ConfigurationManager.ConnectionStrings["MySqlConnection"] != null)
                mySqlConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            // создаем визард БД
            using (var wizard = new Wizard(new DummyDbProviderFactory(chbxInvalidUpdateRules.Checked,chbxError.Checked),"testDB","testUser"))
            {
                // меняем банеры
                wizard.HorizontalBanner = Properties.Resources.HorizontalBanner;
                wizard.VerticalBanner = Properties.Resources.VerticalBanner;
                
                List<DbProviderType> supportedDbProviders = new List<DbProviderType>();

                if (chMSSQL.Checked)
                    supportedDbProviders.Add(DbProviderType.MSSQL);

                if (chOracle.Checked)
                    supportedDbProviders.Add(DbProviderType.Oracle);

                if (chMySql.Checked)
                    supportedDbProviders.Add(DbProviderType.MySQL);

                if (chAzureSql.Checked)
                    supportedDbProviders.Add(DbProviderType.AzureSQL);

                wizard.DatabaseWizardContext.SupportedDbProviders = supportedDbProviders.ToArray();
                wizard.DatabaseWizardContext.DefaultDbProvider =
                    Enum.TryParse<DbProviderType>(cbDefaultDbProvider.SelectedItem?.ToString(), false, out var defaultDbProvider)
                        ? (DbProviderType?)defaultDbProvider
                        : null;
                wizard.DatabaseWizardContext.PasswordQuality = new PasswordQuality() { Min = 8, Max = 80 };

                // указываем существующее подключение к БД
                if (rdbUpdateSqlServer.Checked)
                {
                    wizard.ExistingConnString = sqlServerConnectionString;
                    // в тестовых целях явно указываем провайдера БД
                    wizard.DatabaseWizardContext.SelectedDbProvider = DbProviderType.MSSQL;
                }

                if (rdbUpdateOracle.Checked)
                {
                    wizard.ExistingConnString = oracleConnectionString;
                    // в тестовых целях явно указываем провайдера БД
                    wizard.DatabaseWizardContext.SelectedDbProvider = DbProviderType.Oracle;
                }

                if (rdbUpdateMySql.Checked)
                {
                    wizard.ExistingConnString = mySqlConnectionString;
                    // в тестовых целях явно указываем провайдера БД
                    wizard.DatabaseWizardContext.SelectedDbProvider = DbProviderType.MySQL;
                }

                // нужно ли выбирать ДБ провайдер?
                wizard.ShowSelectProvider = chbSelectProvider.Checked;
            	wizard.EnableChangingServerCredentialType = chkbChangeServerCredentialType.Checked;
            	wizard.EnableChangingDatabaseCredentialType = chkbChangeUserCredentialType.Checked;
            	wizard.ShowSelectOperation = chkbShowSelectOperation.Checked;
            	wizard.EnableChangingDatabaseLoginMode = chkbEnableChangingDatabaseLogin.Checked;                

                // запускаем визард
                wizard.ShowDialog();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;

            progressText.ForeColor = Color.Black;
            progressText.Text = "";
            progressBar1.Value = 0;

            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;

            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button2.Enabled = true;
            if (e.Error != null)
            {
                progressText.Text = e.Error.Message;
                progressText.ForeColor = Color.Red;

                ErrorMessageBox.Show(this, e.Error, "Ошибка во время выполнения авторазвертывания.", "Ошибка авторазвертывания.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                progressText.Text += "\n\rАвторазвертывание успешно завершено.";
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            progressText.Text = (string)e.UserState;
        }

        private IDatabaseProvider GetDBProviderForAutotest()
        {
            IDatabaseProviderFactory dummyDbProviderFactory = new DummyDbProviderFactory(chbxInvalidUpdateRules.Checked, chbxError.Checked);
            SqlServerDatabaseProvider sqlDbProvider = (SqlServerDatabaseProvider)dummyDbProviderFactory.GetProvider(DbProviderType.MSSQL);

            sqlDbProvider.ServerCredentialType = rbnNT.Checked ? ELoginType.NT : ELoginType.SQL;
            sqlDbProvider.CreateNewDatabase = chkbCreateNewDB.Checked;
            sqlDbProvider.CreateNewDatabaseLogin = !rdbtNTDatabase.Checked && checkBoxCreateNewUser.Checked;
            sqlDbProvider.DatabaseCredentialType = rdbtNTDatabase.Checked ? ELoginType.NT : ELoginType.SQL;
            sqlDbProvider.ServerName = txtServerName.Text;
            sqlDbProvider.ServerLogin = textBoxLogin.Text;
            sqlDbProvider.ServerPassword = textBoxPassword.Text;
            sqlDbProvider.DatabaseName = textBoxDBName.Text;
            sqlDbProvider.DatabaseLogin = textBoxUserID.Text;
            sqlDbProvider.DatabasePassword = txtDBPassword.Text;

            return sqlDbProvider;
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            IDatabaseProvider provider = GetDBProviderForAutotest();
            provider.Execute((BackgroundWorker)sender);
        }

        private void rdbtNTDatabase_CheckedChanged(object sender, EventArgs e)
        {
            textBoxUserID.Enabled = !rdbtNTDatabase.Checked;
            txtDBPassword.Enabled = !rdbtNTDatabase.Checked;
            checkBoxCreateNewUser.Enabled = !rdbtNTDatabase.Checked;
        }

        private void rbnNT_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLogin.Enabled = !rbnNT.Checked;
            textBoxPassword.Enabled = !rbnNT.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cmbCulture.SelectedItem.ToString());
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cmbCulture.SelectedItem.ToString());

            Exception ex = new Exception("test exception", new Exception("Inner exception"));
            ErrorMessageBox.Show(ex, "text", this.Text,
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.cmbCulture.SelectedIndex = 0;
        }

        /// <summary>
        /// Авторазвертывание MySQL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            IDatabaseProviderFactory dummyDbProviderFactory = new DummyDbProviderFactory(false, false);
            MySqlServerDatabaseProvider sqlDbProvider = (MySqlServerDatabaseProvider)dummyDbProviderFactory.GetProvider(DbProviderType.MySQL);

            sqlDbProvider.ServerCredentialType = ELoginType.SQL;
            sqlDbProvider.CreateNewDatabase = true;
            sqlDbProvider.CreateNewDatabaseLogin = true;
            sqlDbProvider.DatabaseCredentialType = ELoginType.SQL;
            sqlDbProvider.ServerName = "localhost";
            sqlDbProvider.ServerDatabasePort = "3306";
            sqlDbProvider.ServerLogin = "root";
            sqlDbProvider.ServerPassword = "vbfgrt45$%";
            sqlDbProvider.DatabaseName = "mysql_auto_db_1";
            sqlDbProvider.DatabaseLogin = "mysql_auto_user_1";
            sqlDbProvider.DatabasePassword = "zxasqwq12!@";
            sqlDbProvider.Execute(null);
        }
    }
}
