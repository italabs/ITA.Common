using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using ITA.Common.Passwords;

namespace ITA.Wizards.DatabaseWizard.Model
{
    public enum ELoginType
    {
        NT,
        SQL
    }

    public class DatabaseWizardContext
    {
        private readonly IDatabaseProviderFactory _dbProviderFactory;
        
        private IDatabaseProvider _dbProvider;
        
        public DatabaseWizardContext(IDatabaseProviderFactory dbProviderFactory) : this(dbProviderFactory, null)
        {

        }

        public DatabaseWizardContext(IDatabaseProviderFactory dbProviderFactory, string[] commandLineArguments)
        {
            SupportedDbProviders = new DbProviderType[] { DbProviderType.MSSQL, DbProviderType.Oracle, DbProviderType.MySQL, DbProviderType.AzureSQL };

            _dbProviderFactory = dbProviderFactory;

            if (commandLineArguments != null)
            {
                Settings = new DbProviderSettings(commandLineArguments);
            }

            PasswordQuality = new PasswordQuality() { Min = 8, Max = 80 };
            CurrentPasswordQuality = new PasswordQuality() { Min = 1, Max = 80 };
        }

        public const string ClassName = "DatabaseWizardContext";

        public const string MasterDatabase = "master";

        public DbProviderSettings Settings { get; private set; }

        /// <summary>
        /// Имя аккаунта рабочей станции в формате DOMAIN\NETBIOS$
        /// 
        /// Алгоритм определения - в первую очередь по pre-Windows 2000 Name
        /// Если вдруг не получилось - смотрим по NetBIOS.
        /// </summary>
        public static string WorkstationAccountName
        {
            get
            {
                try
                {
                    string domainName = Domain.GetComputerDomain().Name.ToUpper();
                    domainName = domainName.Substring(0, domainName.IndexOf('.'));
                    string workstationName = null;

                    try
                    {
                        using (var searcher = new DirectorySearcher(String.Format("(&(objectCategory=computer)(objectClass=computer)(name={0})(!userAccountControl:1.2.840.113556.1.4.803:=2))", Environment.MachineName), null, SearchScope.Subtree))
                        {
                            SearchResult result = searcher.FindOne();

                            if (result != null)
                            {
                                workstationName = (string)result.Properties["sAMAccountName"][0];
                            }
                        }
                    }
                    catch
                    {
                        workstationName = Environment.MachineName + "$";
                    }

                    return domainName + @"\" + workstationName;
                }
                catch
                {
                    // не доменная среда
                    return null;
                }
            }
        }
      
        public string GetConnectionString(bool isServerCredentials, string defaultDatabase)
        {
            return DBProvider.GetConnectionString(isServerCredentials, defaultDatabase);
        }

        public string GetConnectionString(bool isServerCredentials, bool isMarsAllowed, string defaultDatabase)
        {
            return DBProvider.GetConnectionString(isServerCredentials, isMarsAllowed, defaultDatabase);
        }

        /// <summary>
        /// Строка подключения к "серверу" - используется для заведения объектов БД
        /// </summary>
        /// <returns></returns>
        public string GetServerConnectionString()
        {
            return this.GetConnectionString(true, MasterDatabase);
        }

        /// <summary>
        /// Строка соединения к конкретной БД 
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseConnectionString()
        {

            return this.DBProvider.ConnectionString ?? this.GetConnectionString(false, this.DBProvider.DatabaseName);
        }

        /// <summary>
        /// Требования к качеству нового пароля.
        /// </summary>
        public PasswordQuality PasswordQuality { get; set; }

        /// <summary>
        /// Валидатор для проверки существующего пароля.
        /// </summary>
        internal PasswordQuality CurrentPasswordQuality { get; private set; }    

        public IDatabaseProvider DBProvider
        {
            get
            {
                return _dbProvider;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("DBProvider");

                _dbProvider = value;
            }
        }

        public DbProviderType SelectedDbProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Перечень поддерживаемых типов провайдеров.
        /// </summary>
        public DbProviderType[] SupportedDbProviders { get; set; }

        /// <summary>
        /// Db provider selected by default.
        /// </summary>
        public DbProviderType? DefaultDbProvider { get; set; }

        public IDatabaseProvider GetDbProviderByType(DbProviderType type)
        {
            IDatabaseProvider dbProvider = _dbProviderFactory.GetProvider(type);
            if (dbProvider == null)
                throw new InvalidOperationException(String.Format(Messages.WIZ_SRV_DB_PROVIDER_IS_NOT_AVAILABLE, type));
            
            dbProvider.Init(Settings);
            
            return dbProvider;
        }

        public DbProviderSettings SetDbProviderSetings(string[] args)
        {
            Settings = new DbProviderSettings(args);
            return Settings;
        }
    }
}
