using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using ITA.Common;
using ITA.Wizards.UpdateWizard.Model;

namespace ITA.Wizards.DatabaseWizard.Model
{
    /// <summary>
    /// Поставщик подключения к MS SQL Server
    /// </summary>
	public class SqlServerDatabaseProvider : BaseDatabaseProvider
	{
		/// <summary>
		/// Имя транспорта, используемого для локального соединенения с сервером БД
		/// Используется для идентификации локальный сервер-удаленный
		/// Чтобы локальный сервер выдать за удаленный можно явно указать в имени сервера префикс tcp:
		/// </summary>
		private const string SharedMemory = "Shared memory";

        private const int SqlServer2012Ver = 11;

		private readonly Regex _goRegex = new Regex(@"^\s*(g|G)(o|O)\s*$",
													RegexOptions.ExplicitCapture | RegexOptions.Compiled |
													RegexOptions.Multiline);


		public SqlServerDatabaseProvider(UpdateManager updateManager)
			: base(updateManager)
		{
			DatabaseCredentialType = ELoginType.NT;
			_scriptHelper = new SqlScriptHelper(_goRegex);
		}

        public override string Name
        {
            get { return Messages.WIZ_SQL_SERVER; }
        }

		public override Regex BatchScriptSeparatorRegex
		{
			get { return _goRegex; }
		}

		public override string MasterDatabase
		{
			get { return "master"; }
		}

		public override DbProviderType ProviderType
		{
			get { return DbProviderType.MSSQL; }
		}

		public override DbConnection GetConnection()
		{
			return ConnectionString != null ? new SqlConnection(ConnectionString) : new SqlConnection();
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
				using (var conn = new SqlConnection(connString))
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

		/// <summary>
		/// Является ли соединение к SQL Server удаленным или локальным
		/// </summary>
		private bool IsRemoteDbServer(string connectionString)
		{
			using (DbConnection connection = GetConnection(connectionString))
            using (DbCommand command = connection.CreateCommand())
			{
				try
				{
                    command.CommandText = "SELECT net_transport FROM sys.dm_exec_connections WHERE session_id = @@spid";

					connection.Open();
                    
                    var transport = (string)command.ExecuteScalar();
                    return string.Compare(transport, SharedMemory, StringComparison.InvariantCultureIgnoreCase) != 0;
				}
				catch (ITAException)
				{
					throw;
				}
				catch (Exception ex)
				{
					throw new ITAException(Messages.E_ITA_DATABASE_VALIDATION_FAILED,
										   ITAException.E_ITA_DATABASE_MANAGER_ERROR, ex);
				}
			}
		}
		
        /// <summary>
        /// Получение версии SQL Server
        /// </summary>
        private int GetSqlServerVersion(string connectionString)
        {
            using (DbConnection connection = GetConnection(connectionString))
            using (DbCommand command = connection.CreateCommand())
            {
                try
                {
                    command.CommandText = "SELECT @@MicrosoftVersion / 0x01000000";
                    connection.Open();

                    return (int)command.ExecuteScalar();
                }
                catch (ITAException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ITAException(Messages.E_ITA_DATABASE_VALIDATION_FAILED,
                                           ITAException.E_ITA_DATABASE_MANAGER_ERROR, ex);
                }
            }
        }
      
        public override string GetConnectionString(bool isServerCredentials, string defaultDatabase)
		{
			return GetConnectionString(isServerCredentials, false, defaultDatabase);
		}

        public override string GetConnectionString(bool isServerCredentials, bool isMarsAllowed, string defaultDatabase)
        {
            //"Мастер" - админское подключение к серверу с бд по умолчанию "мастер" - для создания БД, таблиц и заведения аккаунтов
            ELoginType loginType = isServerCredentials ? ServerCredentialType : DatabaseCredentialType;

            string login = isServerCredentials ? ServerLogin : DatabaseLogin;
            string password = isServerCredentials ? ServerPassword : DatabasePassword;

            StringBuilder sb = new StringBuilder();

            if ((isServerCredentials && this.EncryptAdminConnection) ||
               (!isServerCredentials && this.EncryptDatabaseConnection))
            {
                sb.Append("Encrypt=true;");
                //packet size too large for ssl encrypt/decrypt operations                
                sb.Append("Packet Size=16000;");
            }
            else
            {
                sb.Append("Packet Size=32767;");
            }

            switch (loginType)
            {
                case ELoginType.NT:
                    sb.Append("Integrated Security=SSPI;Persist Security Info=False;");
                    break;
                case ELoginType.SQL:
                    sb.Append("user id=").Append(login).Append(";password=").Append(password).Append(";");
                    break;
            }

            sb.Append("Initial Catalog=").Append(defaultDatabase).Append(";");
            sb.Append("Data Source=").Append(ServerName).Append(";");

            if (isMarsAllowed)
            {
                sb.Append("MultipleActiveResultSets=True;");
            }

            return sb.ToString();
        }

		public override void CreateDb()
		{
			string createScript = SqlScriptHelper.GetScript("CreateDB.sql", "ITA.Wizards.DatabaseWizard.DbScripts.MSSQL");
			createScript = createScript.Replace("@DB", DatabaseName);
			ExecuteScript(GetConnectionString(true, MasterDatabase), createScript);

			// create users
			if (DatabaseCredentialType == ELoginType.SQL)
			{
				createScript = SqlScriptHelper.GetScript("CreateUser.sql", "ITA.Wizards.DatabaseWizard.DbScripts.MSSQL");
				createScript = createScript.Replace("@DB", DatabaseName);
				createScript = createScript.Replace("@login", DatabaseLogin);
				createScript = createScript.Replace("@password", String.IsNullOrEmpty(DatabasePassword) ? "null" : DatabasePassword);
				ExecuteScript(GetConnectionString(true, MasterDatabase), createScript);
			}
			else if (DatabaseCredentialType == ELoginType.NT && (
                IsRemoteDbServer(GetConnectionString(true, MasterDatabase)) ||
                GetSqlServerVersion(GetConnectionString(true, MasterDatabase)) >= SqlServer2012Ver))
			{
				string workstationAccountName = DatabaseWizardContext.WorkstationAccountName;

				if (!string.IsNullOrEmpty(workstationAccountName))
				{
                    string script = SqlScriptHelper.GetScript("GrantLoginToDomainUser.sql", "ITA.Wizards.DatabaseWizard.DbScripts.MSSQL");
					script = script.Replace("@WS", workstationAccountName);

					ExecuteScript(GetConnectionString(true, DatabaseName), script);
				}

                ExecuteScript(GetConnectionString(true, DatabaseName),
                    SqlScriptHelper.GetScript("GrantLoginToLocalSystem.sql", "ITA.Wizards.DatabaseWizard.DbScripts.MSSQL"));
			}
		}

        /// <summary>
        /// Предоcтавление прав пользователю
        /// </summary>
        public override void GrantUserPermissions()
        {
            if (this.DatabaseCredentialType == ELoginType.SQL
                    && !String.IsNullOrEmpty(this.DatabaseName)
                    && !String.IsNullOrEmpty(this.DatabaseLogin))
                {
                    //1) if sql login => check database roles (db_owner, db_datareader, db_datawriter)      
                    string script = SqlScriptHelper.GetScript("CreateUser.sql", "ITA.Wizards.DatabaseWizard.DbScripts.MSSQL");

                    //Здесь нельзя использовать именованные параметры. Валидация значений осущесвляется после введения их пользователем
                    script = script.Replace("@DB", this.DatabaseName);
                    script = script.Replace("@login", this.DatabaseLogin);
                    script = script.Replace("@password", (String.IsNullOrEmpty(this.DatabasePassword)) ? "null" : this.DatabasePassword);

                    this.ExecuteScript(this.GetConnectionString(true, this.DatabaseName), script);
                }
                else if (this.DatabaseCredentialType == ELoginType.NT && (
                    this.IsRemoteDbServer(this.GetServerConnectionString()) ||
                    (this.GetSqlServerVersion(this.GetServerConnectionString()) >= SqlServerDatabaseProvider.SqlServer2012Ver)))
            {
                //2) if nt => grant login to domain user and local system
                string workstationAccountName = DatabaseWizardContext.WorkstationAccountName;
                if (!string.IsNullOrEmpty(workstationAccountName))
                {
                    string script = SqlScriptHelper.GetScript("GrantLoginToDomainUser.sql",
                                                              "ITA.Wizards.DatabaseWizard.DbScripts.MSSQL");
                    script = script.Replace("@WS", workstationAccountName);

                    this.ExecuteScript(this.GetConnectionString(true, this.DatabaseName), script);
                }

                this.ExecuteScript(
                    this.GetConnectionString(true, this.DatabaseName),
                    SqlScriptHelper.GetScript("GrantLoginToLocalSystem.sql", "ITA.Wizards.DatabaseWizard.DbScripts.MSSQL"));
            }
        }

        /// <summary>
        /// Ничего не делаем, т.к. провайдер к MS SQL Server - штатный компонент.
        /// </summary>
		public override void CheckDbPresence()
		{
			
		}

		public override DbParameter CreateParameter(string parameterName, object parameterValue)
		{
			return new SqlParameter(parameterName, parameterValue);
		}
	}
}