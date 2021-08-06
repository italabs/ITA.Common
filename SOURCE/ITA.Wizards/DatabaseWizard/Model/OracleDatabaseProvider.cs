using System;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using ITA.Common;
using ITA.Wizards.UpdateWizard.Model;
using Microsoft.Win32;
using Oracle.DataAccess.Client;

namespace ITA.Wizards.DatabaseWizard.Model
{
    /// <summary>
    /// Поставщик подключения к Oracle
    /// </summary>
    public class OracleBaseDatabaseProvider : BaseDatabaseProvider
    {
        private Regex _goRegex = new Regex(@"^\s*(;)\s*$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Multiline);

        public const string ConnectionStringTemplate = "User Id={0}; Password={1}; Data Source=(DESCRIPTION = (ADDRESS_LIST =  (ADDRESS = ( SDU = 32768 )(PROTOCOL = TCP)(HOST = {2})(PORT = {3} )))(CONNECT_DATA = (SERVER =  DEDICATED) (SERVICE_NAME = {4})));";

        public OracleBaseDatabaseProvider(UpdateManager updateManager)
            : base(updateManager)
        {
            _scriptHelper = new SqlScriptHelper(_goRegex);
        }

        public override string Name
        {
            get { return Messages.WIZ_ORACLE; }
        }

        public override string MasterDatabase
        {
            get { return ServerLogin; }
        }

        public override DbProviderType ProviderType
        {
            get { return DbProviderType.Oracle; }
        }

        protected override void CheckConnectionString(string connString)
        {
            try
            {
                base.CheckConnectionString(connString);

                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(Messages.WIZ_INVALID_CONNECTION_STRING, e);
            }
        }

        public override DbConnection GetConnection()
        {
            return ConnectionString != null ? new OracleConnection(ConnectionString) : new OracleConnection();
        }

        public override Regex BatchScriptSeparatorRegex
        {
            get
            {
                return _goRegex;
            }
        }

        public override DbConnection GetConnection(string connectionString)
        {
            ConnectionString = connectionString;
            return GetConnection();
        }
      
        public override string GetConnectionString(bool isServerCredentials, string defaultDatabase)
        {
            string userName = isServerCredentials ? ServerLogin : DatabaseLogin;
            string password = isServerCredentials ? ServerPassword : DatabasePassword;

            Helpers.CheckNullOrEmpty(userName, "userName");
            Helpers.CheckNullOrEmpty(password, "password");
            Helpers.CheckNullOrEmpty(ServerName, "ServerName");
            Helpers.CheckNullOrEmpty(ServerDatabasePort, "ServerDatabasePort");
            Helpers.CheckNullOrEmpty(ServerDatabaseSID, "ServerDatabaseSID");

            return String.Format(ConnectionStringTemplate, userName, password, ServerName, ServerDatabasePort, ServerDatabaseSID);
        }

        public override string GetConnectionString(bool isServerCredentials, bool isMarsAllowed, string defaultDatabase)
        {
            // Oracle doesn't have Multiple Active Result Sets (MARS) limitation.
            return GetConnectionString(isServerCredentials, defaultDatabase);
        }

        public override void CreateDb()
        {
            var createScript = SqlScriptHelper.GetScript("CreateDB.sql", "ITA.Wizards.DatabaseWizard.DbScripts.Oracle");
            createScript = createScript.Replace("@DB", DatabaseName);
            ExecuteScript(GetConnectionString(true, MasterDatabase), createScript);

            if (CreateNewDatabaseLogin)
            {
                createScript = SqlScriptHelper.GetScript("CreateUser.sql", "ITA.Wizards.DatabaseWizard.DbScripts.Oracle");
                createScript = createScript.Replace("@login", DatabaseLogin);
                createScript = createScript.Replace("@password", (0 == DatabasePassword.Length) ? "null" : "\"" + DatabasePassword + "\"");
                ExecuteScript(GetConnectionString(true, MasterDatabase), createScript);
            }

            this.GrantUserPermissions();
        }

        /// <summary>
        /// Предоcтавление прав пользователю
        /// </summary>
        public override void GrantUserPermissions()
        {
           
        }

        public override DbParameter CreateParameter(string parameterName, object parameterValue)
        {
            OracleParameter parameter = null;
            parameter = new OracleParameter(parameterName, parameterValue);
            return parameter;
        }

        /// <summary>
        /// Проверка наличия сборки Oracle.DataAccess.dll
        /// </summary>
        public override void CheckDbPresence()
        {
            using(RegistryKey reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ORACLE"))
            {
                if(reg == null)
                    throw new Exception("Oracle client is not found.");
            }

            using (OracleConnection connection = new OracleConnection())
            {

            }
        }      
    }
}