using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using ITA.Wizards.UpdateWizard.Model;
using MySql.Data.MySqlClient;

namespace ITA.Wizards.DatabaseWizard.Model
{
    /// <summary>
    /// Поставщик подключения к MySQL Server
    /// </summary>
	public class MySqlServerDatabaseProvider : BaseDatabaseProvider
	{		
		private readonly Regex _goRegex = new Regex(@"^\s*(g|G)(o|O)\s*$",
													RegexOptions.ExplicitCapture | RegexOptions.Compiled |
													RegexOptions.Multiline);


		public MySqlServerDatabaseProvider(UpdateManager updateManager)
			: base(updateManager)
		{
			DatabaseCredentialType = ELoginType.NT;
			_scriptHelper = new SqlScriptHelper(_goRegex);
		}

        public override string Name
        {
            get { return Messages.WIZ_MYSQL; }
        }

		public override Regex BatchScriptSeparatorRegex
		{
			get { return _goRegex; }
		}

		public override string MasterDatabase
		{
			get { return "sys"; }
		}

		public override DbProviderType ProviderType
		{
			get { return DbProviderType.MySQL; }
		}

		public override DbConnection GetConnection()
		{
			return ConnectionString != null ? new MySqlConnection(ConnectionString) : new MySqlConnection();
		}

		public override DbConnection GetConnection(string connectionString)
		{
			ConnectionString = connectionString;
			return GetConnection();
		}

		protected override void CheckConnectionString(string connString)
		{
			try
			{
				base.CheckConnectionString(connString);
				using (var conn = new MySqlConnection(connString))
				{
					conn.Open();

					if (conn.State == ConnectionState.Open)
					{
						ServerName = ServerName == null || ServerName.Trim().Length == 0
										 ? conn.DataSource
										 : ServerName;
						DatabaseName = DatabaseName == null || DatabaseName.Trim().Length == 0
										   ? conn.Database
										   : DatabaseName;
						conn.Close();
					}
				}
			}
			catch (Exception e)
			{
				throw new Exception(Messages.WIZ_INVALID_CONNECTION_STRING, e);
			}
		}
	       
		public override string GetConnectionString(bool isServerCredentials, string defaultDatabase)
		{
			return GetConnectionString(isServerCredentials, false, defaultDatabase);
		}

        public override string GetConnectionString(bool isServerCredentials, bool isMarsAllowed, string defaultDatabase)
        {            
            string login = isServerCredentials ? ServerLogin : DatabaseLogin;
            string password = isServerCredentials ? ServerPassword : DatabasePassword;

            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder();
            connectionStringBuilder.Server = this.ServerName;
            connectionStringBuilder.Port = uint.Parse(this.ServerDatabasePort);
            connectionStringBuilder.UserID = login;
            connectionStringBuilder.Password = password;

            if (defaultDatabase != DatabaseWizardContext.MasterDatabase)
            {
                connectionStringBuilder.Database = defaultDatabase;
            }
            
            return connectionStringBuilder.ToString();
        }

        /// <summary>
        /// Создание новой БД.
        /// </summary>
		public override void CreateDb()
		{
			string createScript = SqlScriptHelper.GetScript("CreateDB.sql", "ITA.Wizards.DatabaseWizard.DbScripts.MySql");
			createScript = createScript.Replace("@DB", DatabaseName);
			ExecuteScript(GetConnectionString(true, MasterDatabase), createScript);

			// create users

		    if (this.CreateNewDatabaseLogin)
		    {
		        string createUser = SqlScriptHelper.GetScript("CreateUser.sql", "ITA.Wizards.DatabaseWizard.DbScripts.MySql");
                createUser = createUser.Replace("@DB", DatabaseName);
                createUser = createUser.Replace("@login", DatabaseLogin);
                createUser = createUser.Replace("@password", DatabasePassword);
                ExecuteScript(GetConnectionString(true, MasterDatabase), createUser);
		    }           

            this.GrantUserPermissions();
		}

        /// <summary>
        /// Предоcтавление прав пользователю
        /// </summary>
        public override void GrantUserPermissions()
        {
            if (!this.DatabaseLogin.Equals("root", StringComparison.InvariantCultureIgnoreCase) &&
                !this.DatabaseLogin.Equals("mysql.sys", StringComparison.InvariantCultureIgnoreCase))
            {
                //grant privileges
                string grantScript = SqlScriptHelper.GetScript("GrantUser.sql", "ITA.Wizards.DatabaseWizard.DbScripts.MySql");
                grantScript = grantScript.Replace("@DB", DatabaseName);
                grantScript = grantScript.Replace("@login", DatabaseLogin);
                ExecuteScript(GetConnectionString(true, MasterDatabase), grantScript);
            }
        }

        /// <summary>
        /// Проверка наличия сборки MySql.Data.dll
        /// </summary>
		public override void CheckDbPresence()
		{
            using (MySqlConnection connection = new MySqlConnection())
            {

            }
		}

		public override DbParameter CreateParameter(string parameterName, object parameterValue)
		{
			return new SqlParameter(parameterName, parameterValue);
		}
	}
}