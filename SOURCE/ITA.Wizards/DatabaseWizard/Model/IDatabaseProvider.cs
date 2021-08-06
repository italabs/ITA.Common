using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Text.RegularExpressions;
using ITA.Wizards.DatabaseWizard.Pages;
using ITA.Wizards.UpdateWizard.Model;

namespace ITA.Wizards.DatabaseWizard.Model
{
    public interface IDatabaseProviderFactory
    {
        IDatabaseProvider GetProvider(DbProviderType dbType);
    }

    public enum DbProviderType
    {
        MSSQL,
        Oracle,
        MySQL,
        AzureSQL
    }
    public interface IDatabaseProvider
    {
        string Name { get; }

        /// <summary>
        /// Необходимость Update DB
        /// </summary>
        bool IsUpdateRequired { get; }
        
        /// <summary>
        /// Имя БД
        /// </summary>
        string DatabaseName { get; set; }
        
        /// <summary>
        /// Создавать новую БД?
        /// </summary>
        bool CreateNewDatabase { get; set; }
        
        /// <summary>
        /// Создавать ли новый логин к БД
        /// </summary>
        bool CreateNewDatabaseLogin { get; set; }

        /// <summary>
        /// Шифровать административное соединение.
        /// </summary>
        bool EncryptAdminConnection { get; set; }

        /// <summary>
        /// Шифровать соединение с БД.
        /// </summary>
        bool EncryptDatabaseConnection { get; set; }
        
        string ConnectionString { get; set; }
        
        ELoginType ServerCredentialType { get; set; }
        
        Regex BatchScriptSeparatorRegex { get; }

        DbConnection GetConnection();
        DbConnection GetConnection(string connectionString);
      
        string GetConnectionString(bool isServerCredentials, string defaultDatabase);
        string GetConnectionString(bool isServerCredentials, bool isMarsAllowed, string defaultDatabase);

        void GrantUserPermissions();

        UpdateManager UpdateManager { get; }

        string DefaultDatabaseName { get; set; }
        string DefaultUserName { get; set; }

        /// <summary>
        /// Логин (для SQL)
        /// </summary>
        string ServerLogin { get; set; }

        ELoginType DatabaseCredentialType { get; set; }

        /// <summary>
        /// Логин к БД для сохранения
        /// </summary>
        string DatabaseLogin { get; set; }

        /// <summary>
        /// Пароль (для SQL)
        /// </summary>
        string ServerPassword { get; set; }

        /// <summary>
        /// Пароль к БД для сохранения
        /// </summary>
        string DatabasePassword { get; set; }

        string ServerName { get; set; }
        string ServerDatabasePort { get; set; }
        string ServerDatabaseSID { get; set; }

        string MasterDatabase { get; }

        DbProviderType ProviderType { get; }

        void ExecuteScript(DbTransaction transaction, int timeout, string script);
        void ExecuteScript(string connectionString, string script);
        void ExecuteScript(DbConnection connection, string script, params DbParameter[] parameters);
        void ExecuteScript(DbConnection connection, int commandTimeout, string script, params DbParameter[] parameters);

        void CreateDb();
        void CheckDbPresence();

        void AddStage(ProgressPage.Stage stage);

        List<ProgressPage.Stage> GetStages();

        DbParameter CreateParameter(string parameterName, object parameterValue);

        void Execute(BackgroundWorker bw);

        void Init(DbProviderSettings settings);
    }
}