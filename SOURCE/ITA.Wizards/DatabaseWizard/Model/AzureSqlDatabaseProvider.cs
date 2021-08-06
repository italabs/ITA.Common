using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using ITA.Wizards.DatabaseWizard.Exceptions;
using ITA.Wizards.UpdateWizard.Model;

namespace ITA.Wizards.DatabaseWizard.Model
{
    /// <summary>
    /// Azure SQL provider
    /// </summary>
    public class AzureSqlDatabaseProvider : BaseDatabaseProvider
    {
        private readonly Regex _goRegex = new Regex(@"^\s*(g|G)(o|O)\s*$",
            RegexOptions.ExplicitCapture | RegexOptions.Compiled |
            RegexOptions.Multiline);

        public AzureSqlDatabaseProvider(UpdateManager updateManager) : base(updateManager)
        {
            DatabaseCredentialType = ELoginType.SQL;
            _scriptHelper = new SqlScriptHelper(_goRegex);
        }

        #region Overrides of BaseDatabaseProvider

        public override string Name => Messages.WIZ_AZURE_SQL;

        /// <inheritdoc />
        public override Regex BatchScriptSeparatorRegex => _goRegex;

        public override DbConnection GetConnection()
        {
            return ConnectionString != null ? new SqlConnection(ConnectionString) : new SqlConnection();
        }

        public override DbConnection GetConnection(string connectionString)
        {
            ConnectionString = connectionString;
            return GetConnection();
        }

        public override string GetConnectionString(bool isServerCredentials, string defaultDatabase)
        {
            return GetConnectionString(isServerCredentials, false, defaultDatabase);
        }

        public override string GetConnectionString(bool isServerCredentials, bool isMarsAllowed, string defaultDatabase)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = ServerName; 
            connectionStringBuilder.UserID = DatabaseLogin;
            connectionStringBuilder.Password = DatabasePassword;
            connectionStringBuilder.InitialCatalog = DatabaseName;

            return connectionStringBuilder.ToString();
        }

        public override void CreateDb()
		{
        }

        public override void GrantUserPermissions()
        {
            if (this.DatabaseCredentialType != ELoginType.SQL)
            {
                throw new NotSupportedException(string.Format(Messages.WIZ_AZURE_SQL_CREDENTIAL_TYPE_NOT_SUPPORTED, DatabaseCredentialType));
            }
        }

        public override void CheckDbPresence()
        {
        }

        public override DbParameter CreateParameter(string parameterName, object parameterValue)
        {
            return new SqlParameter(parameterName, parameterValue);
        }

        public override string MasterDatabase => string.Empty;

        public override DbProviderType ProviderType => DbProviderType.AzureSQL;

        #endregion

        protected override void CheckConnectionString(string connectionString)
        {
            try
            {
                base.CheckConnectionString(connectionString);
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        ServerName = ServerName == null || ServerName.Trim().Length == 0
                            ? connection.DataSource
                            : ServerName;
                        DatabaseName = DatabaseName == null || DatabaseName.Trim().Length == 0
                            ? connection.Database
                            : DatabaseName;
                        connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new AzureSqlException(Messages.WIZ_INVALID_CONNECTION_STRING, e);
            }
        }

        public virtual void CheckAbilityForDatabaseCreation()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select count(1) from sys.tables";
                    var tablesCount = Convert.ToInt32(command.ExecuteScalar());
                    if (tablesCount > 0)
                    {
                        throw new AzureSqlException(Messages.WIZ_AZURE_SQL_DENY_FOR_DB_CREATION);
                    }
                }
            }
        }
    }
}

