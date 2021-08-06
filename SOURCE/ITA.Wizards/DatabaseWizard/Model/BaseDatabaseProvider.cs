using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using ITA.Common;
using ITA.Wizards.DatabaseWizard.Pages;
using ITA.Wizards.UpdateWizard.Model;

namespace ITA.Wizards.DatabaseWizard.Model
{
    /// <summary>
    /// Базовая реализация провайдера в БД.
    /// </summary>
    public abstract class BaseDatabaseProvider : IDatabaseProvider
    {
        protected SqlScriptHelper _scriptHelper;

        protected UpdateManager _updateManager;

        private string _connString;

        private List<ProgressPage.Stage> _stages = new List<ProgressPage.Stage>();

        protected BaseDatabaseProvider(UpdateManager updateManager)
        {
            if (updateManager == null)
                throw new ArgumentNullException("updateManager");

            _updateManager = updateManager;
            _updateManager.DatabaseProvider = this;
        }

        public abstract string Name { get; }

        public virtual bool IsUpdateRequired
        {
            get
            {
                try
                {
                    Version currentVersion = _updateManager.GetDatabaseVersion();

                    if (currentVersion < _updateManager.GetActualDatabaseVersion())
                    {
                        return true;
                    }
                    else if (currentVersion == _updateManager.GetActualDatabaseVersion())
                        return false;
                    else
                        throw new NotSupportedException(string.Format(Messages.E_ITA_DB_HAS_OLD_VERSION, currentVersion, _updateManager.GetActualDatabaseVersion()));

                }
                catch (Exception ex)
                {
                    throw new Exception(Messages.E_ITA_UPDATE_UNKNOWN_VERSION, ex);
                }
            }
        }

        public string DatabaseName
        {
            get;
            set;
        }

        public bool CreateNewDatabase
        {
            get;
            set;
        }

        public bool CreateNewDatabaseLogin
        {
            get;
            set;
        }

        /// <summary>
        /// Шифровать административное соединение.
        /// </summary>
        public bool EncryptAdminConnection { get; set; }

        /// <summary>
        /// Шифровать соединение с БД.
        /// </summary>
        public bool EncryptDatabaseConnection { get; set; }
       
        public string ConnectionString
        {
            get { return _connString; }
            set
            {
                CheckConnectionString(value);
                _connString = value;
            }
        }

        public ELoginType ServerCredentialType
        {
            get;
            set;
        }

        public abstract Regex BatchScriptSeparatorRegex { get; }

        public UpdateManager UpdateManager
        {
            get { return _updateManager; }
        }

        public string DefaultDatabaseName { get; set; }
        public string DefaultUserName { get; set; }
        public string ServerLogin { get; set; }
        public ELoginType DatabaseCredentialType { get; set; }

        /// <summary>
        /// Логин к БД для сохранения
        /// </summary>
        public string DatabaseLogin { get; set; }

        /// <summary>
        /// Пароль (для SQL)
        /// </summary>
        public string ServerPassword { get; set; }

        /// <summary>
        /// Пароль к БД для сохранения
        /// </summary>
        public string DatabasePassword { get; set; }

        /// <summary>
        /// Имя сервера
        /// </summary>
        public string ServerName { get; set; }

        public string ServerDatabasePort { get; set; }
        public string ServerDatabaseSID { get; set; }
        public abstract string MasterDatabase { get; }

        public abstract DbProviderType ProviderType
        {
            get;
        }

        protected virtual void CheckConnectionString(string connString)
        {
            Helpers.CheckNullOrEmpty(connString, "connString");
        }

        public abstract DbConnection GetConnection();

        public abstract DbConnection GetConnection(string connectionString);

        public abstract string GetConnectionString(bool isServerCredentials, string defaultDatabase);

        /// <summary>
        /// Строка подключения к "серверу" - используется для заведения объектов БД
        /// </summary>
        /// <returns></returns>
        public string GetServerConnectionString()
        {
            return this.GetConnectionString(true, MasterDatabase);
        }

        public abstract string GetConnectionString(bool isServerCredentials, bool isMarsAllowed, string defaultDatabase);

        public void ExecuteScript(DbTransaction transaction, int timeout, string script)
        {
            _scriptHelper.ExecuteScript(transaction, timeout, script);
        }

        public void ExecuteScript(string connectionString, string script)
        {
            _scriptHelper.ExecuteScript(GetConnection(connectionString), script);
        }

        public void ExecuteScript(DbConnection connection, string script, params DbParameter[] parameters)
        {
            _scriptHelper.ExecuteScript(connection, script, parameters);
        }

        public void ExecuteScript(DbConnection connection, int commandTimeout, string script, params DbParameter[] parameters)
        {
            _scriptHelper.ExecuteScript(connection, commandTimeout, script, parameters);
        }

        public abstract void CreateDb();

        public abstract void GrantUserPermissions();

        public abstract void CheckDbPresence();

        public void AddStage(ProgressPage.Stage stage)
        {

            if (stage.Step == null)
            {
                stage.Step = () =>
                                 {
                                     var resourceAssembly = UpdateManager.ResourceAssembly;
                                     var resourceNamspace = UpdateManager.ResourceNamespace;
                                     var script = SqlScriptHelper.GetScript(resourceAssembly, stage.SqlResourceName, resourceNamspace);
                                     using (var connection = GetConnection(GetConnectionString(true, MasterDatabase)))
                                     {
                                         connection.Open();
                                         ExecuteScript(connection, script, stage.Parameters);
                                     }
                                 };
            }
            _stages.Add(stage);
        }

        public List<ProgressPage.Stage> GetStages()
        {
            return _stages;
        }

        public abstract DbParameter CreateParameter(string parameterName, object parameterValue);


        public void Execute(BackgroundWorker bw)
        {
            if (CreateNewDatabase)
            {
                using (DbConnection connection = GetConnection(GetConnectionString(true, MasterDatabase)))
                {
                    connection.Open();

                    //foreach (var stage in GetStages())
                    for (int i = 0; i < _stages.Count; i++)
                    {

                        if (_stages[i].Step != null)
                        {
                            //execute step
                            _stages[i].Step.Invoke();
                        }
                        if (bw != null)
                        {
                            int percent = (int) ((float) (i + 1)/((float) _stages.Count)*100);
                            
                            bw.ReportProgress(percent, _stages[i].Label.Text);
                        }
                        
                        Thread.Sleep(300);
                    }
                }
            }
            else if (UpdateManager.IsUpdateRequired)
            {
                if (bw != null)
                {
                    UpdateManager.UpdateEvent += (s, a) =>
                                                     {
                                                         if (a.Type == UpdateEventType.StepFinished)
                                                         {
                                                             bw.ReportProgress(a.Progress, a.Step.Description);
                                                         }
                                                     };
                }
                Version version = UpdateManager.ExecuteUpdates();
                if (bw != null)
                    bw.ReportProgress(100, String.Format(Messages.WIZ_SUCCESS_UPDATE, version));
            }
            else
            {
                if (bw != null)
                    bw.ReportProgress(100, Messages.WIZ_DB_DOESNT_NEED_UPDATE);
            }

        }

        public virtual void Init(DbProviderSettings settings)
        {
            if (settings != null)
            {
                if (settings.ConnectionString != null)
                    ConnectionString = settings.ConnectionString;
                if (settings.DatabaseName != null)
                    DatabaseName = settings.DatabaseName;
                if (settings.CreateNewDatabase != null)
                    CreateNewDatabase = settings.CreateNewDatabase.Value;
                if (settings.CreateNewDatabaseLogin != null)
                    CreateNewDatabaseLogin = settings.CreateNewDatabaseLogin.Value;
                if (settings.ServerLogin != null)
                    ServerLogin = settings.ServerLogin;
                if (settings.DatabaseLogin != null)
                    DatabaseLogin = settings.DatabaseLogin;
                if (settings.ServerPassword != null)
                    ServerPassword = settings.ServerPassword;
                if (settings.DatabasePassword != null)
                    DatabasePassword = settings.DatabasePassword;
                if (settings.ServerName != null)
                    ServerName = settings.ServerName;
                if (settings.ServerDatabasePort != null)
                    ServerDatabasePort = settings.ServerDatabasePort;
                if (settings.ServerDatabaseSID != null)
                    ServerDatabaseSID = settings.ServerDatabaseSID;
                if (settings.DatabaseCredentialType != null)
                    DatabaseCredentialType = settings.DatabaseCredentialType.Value;
                if (settings.ServerCredentialType != null)
                    ServerCredentialType = settings.ServerCredentialType.Value;
            }
        }
    }
}