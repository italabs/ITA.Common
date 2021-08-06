using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using ITA.Common;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;
using ITA.Wizards.DatabaseWizard.Pages;
using ITA.Wizards.DatabaseWizard.Pages.AzureSql;
using ITA.Wizards.DatabaseWizard.Pages.Oracle;
using ITA.Wizards.DatabaseWizard.Pages.SqlServer;
using ITA.Wizards.DatabaseWizard.Pages.MySql;
using ITA.Wizards.UpdateWizard;
using log4net;
using log4net.Config;

namespace ITA.Wizards.DatabaseWizard
{
	/// <summary>
	/// Мастер-подключения к базе данных
	/// </summary>
	public partial class DatabaseWizard : Wizard
	{
		private static readonly ILog console = Log4NetItaHelper.GetLogger("ITA.Wizards.DatabaseWizard.DatabaseWizard");
		private readonly IDatabaseProviderFactory _dbProviderFactory;
		private const string EXISTING_CONN_STRING_KEY = "ExistingConnString";
		private LocaleMessages m_Messages = new LocaleMessages();
       
		static DatabaseWizard()
		{
		    var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository);
		}

		public DatabaseWizard(IDatabaseProviderFactory dbProviderFactory, string defaultDatabaseName, string defaultUserName)
		{
			_dbProviderFactory = dbProviderFactory;

			Helpers.CheckNull(dbProviderFactory, "dbProviderFactory");
            Helpers.CheckNullOrEmpty(defaultDatabaseName, "defaultDatabaseName");
            Helpers.CheckNullOrEmpty(defaultUserName, "defaultUserName");

			InitializeComponent();
			ShowSelectProvider = true;
			EnableChangingServerCredentialType = true;
			EnableChangingDatabaseCredentialType = true;
			ShowSelectOperation = true;
			EnableChangingDatabaseLoginMode = true;
			PerformWindowsUserValidation = true;
            
			m_Messages.RegisterAssembly(Assembly.GetExecutingAssembly());

			AddPage(new Pages.WelcomePage()
			{
				Welcome = Messages.I_ITA_DATABASE_WELCOME,
				About = Messages.I_ITA_DATABASE_INFO,
				VerticalBanner = this.VerticalBanner
			});

			var databaseContext = new DatabaseWizardContext(dbProviderFactory);

			Context[DatabaseWizardContext.ClassName] = databaseContext;

			AddPage(new SelectProvider() { HorizontalBanner = HorizontalBanner });
			AddPage(new SelectExistingConfigurationPage() { HorizontalBanner = HorizontalBanner });
			AddPage(new CheckExistingDatabasePage() { HorizontalBanner = HorizontalBanner });

			AddPage(new SelectOperationPage() { HorizontalBanner = this.HorizontalBanner });

			AddPage(new SelectMSSQLServer() { HorizontalBanner = this.HorizontalBanner });
			AddPage(new SelectMSSQLDB(){ HorizontalBanner = this.HorizontalBanner });

			AddPage(new SelectOracleServer() { HorizontalBanner = this.HorizontalBanner });
			AddPage(new SelectOracleDB() { HorizontalBanner = this.HorizontalBanner });

            AddPage(new SelectMySqlServer() { HorizontalBanner = this.HorizontalBanner });
            AddPage(new SelectMySqlDB() { HorizontalBanner = this.HorizontalBanner });

            AddPage(new SelectAzureSqlServer() { HorizontalBanner = this.HorizontalBanner });

			AddPage(new ActionPage() { HorizontalBanner = this.HorizontalBanner });
            
			AddPage(new UpdateNecessityPage() { HorizontalBanner = HorizontalBanner });
			AddPage(new ConfirmBackupPage() { HorizontalBanner = HorizontalBanner });
			AddPage(new UpdateProgress() { HorizontalBanner = HorizontalBanner });
			AddPage(new ConfirmUpdate() { HorizontalBanner = HorizontalBanner });			
            AddPage(new ProgressPage() { HorizontalBanner = this.HorizontalBanner });
			AddPage(new FinishPage() { Thankyou = ITA.Wizards.Messages.I_ITA_THANKS, VerticalBanner = this.VerticalBanner });
		}		

		/// <summary>
		/// Флаг, который определяет, нужно ли при обновлять БД при запуске.
		/// </summary>
		/// <value>true - нужно проверить базу и обновить если возможно; false - не нужно проверять</value>
		public bool CheckExistingConfiguration
		{
			get
			{
				return !String.IsNullOrEmpty(ExistingConnString);
			}
		}

		public DatabaseWizardContext DatabaseWizardContext
		{
			get
			{
				DatabaseWizardContext C = Context.ValueOf<DatabaseWizardContext>(DatabaseWizardContext.ClassName);
				if (C == null)
				{
					C = new DatabaseWizardContext(_dbProviderFactory);
					Context[DatabaseWizardContext.ClassName] = C;
				}
				return C;
			}
		}

		public string ExistingConnString
		{
			get
			{
				return Context.ValueOf<string>(EXISTING_CONN_STRING_KEY);
			}
			set
			{
				Context[EXISTING_CONN_STRING_KEY] = value;
			}
		}

        /// <summary>
		/// Показывать ли экран выбора провайдера БД.
		/// </summary>
		public bool ShowSelectProvider { get; set; }

		/// <summary>
		/// Разрешаем менять тип административного подключения к серверу?
		/// </summary>
		public bool EnableChangingServerCredentialType { get; set; }
		
		/// <summary>
		/// Разрешаем менять тип пользовательского подключения к серверу?
		/// </summary>
		public bool EnableChangingDatabaseCredentialType { get; set; }

		/// <summary>
		/// Показывать ли экран создания выбора типа конфигурации (создать Новую БД, подключиться к существующей).
		/// </summary>
		public bool ShowSelectOperation { get; set; }
		
		/// <summary>
		/// Разрешаем менять тип пользователя - использовать существующего или создавать нового пользователя.
		/// </summary>
		public bool EnableChangingDatabaseLoginMode { get; set; }
		
		/// <summary>
		/// Выполнять ли проверку доступности базы данных для пользователя БД.
		/// </summary>
		public bool PerformWindowsUserValidation { get; set; }
       
		public DbProviderSettings SetDbProviderSetings(string[] args)
		{
			return DatabaseWizardContext.SetDbProviderSetings(args);
		}

		public override int RunUnattended(string[] args)
		{
		    return RunUnattended(args, null) ? 0 : -1;
		}

		public bool RunUnattended(string[] args, BackgroundWorker bw)
		{
			if (args != null && args.Length != 0)
			{
				try
				{
					DbProviderSettings settings = SetDbProviderSetings(args);
					
					StringBuilder sb = new StringBuilder();
					settings.ValidationError += (s, a) => sb.AppendLine(a.ErrorMessage);
					if(!settings.Validate())
					{
						console.Debug(Messages.WIZ_CONFIG_PARAMETERS_HAVE_ERRORS);
						console.Debug(sb.ToString());
						console.Debug(DbProviderSettings.GetUsageMessage(AppDomain.CurrentDomain.FriendlyName));
						return false;
					}

					if (settings.UnattendedMode)
					{
						try
						{
							console.Debug(Messages.WIZ_AUTO_MODE_STARTED);

							IDatabaseProvider dbProvider = DatabaseWizardContext.GetDbProviderByType(settings.DbProviderType);
							dbProvider.Execute(bw);

						    this.DatabaseWizardContext.DBProvider = dbProvider;                            

							console.Debug(Messages.WIZ_AUTO_MODE_COMPLETED);
							return true;

						}
						catch (Exception e)
						{
							console.Error(Messages.WIZ_AUTO_MODE_ERROR);
                            console.Error(e.Message);
                            console.Error(DbProviderSettings.GetUsageMessage(AppDomain.CurrentDomain.FriendlyName));
						}
					}
				}
				catch (Exception e)
				{
                    console.Error(Messages.WIZ_AUTO_MODE_INIT_ERROR);
                    console.Error(e.Message);
                    console.Error(DbProviderSettings.GetUsageMessage(AppDomain.CurrentDomain.FriendlyName));
				}
			}
			else
			{
				console.Debug(DbProviderSettings.GetUsageMessage("AppDomain.CurrentDomain.FriendlyName"));
			}

			return false;
		}
	}
}
