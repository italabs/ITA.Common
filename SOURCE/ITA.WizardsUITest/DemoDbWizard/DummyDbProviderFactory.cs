using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using ITA.Wizards.DatabaseWizard.Model;
using ITA.Wizards.DatabaseWizard.Pages;

namespace ITA.WizardsUITest.DemoDbWizard
{
    public class DummyDbProviderFactory : IDatabaseProviderFactory
    {
        private readonly bool _loadInvalidRules;
        private readonly bool _throwException;

        #region Implementation of IDatabaseProviderFactory

        public DummyDbProviderFactory(bool loadInvalidRules, bool throwException)
        {
            _loadInvalidRules = loadInvalidRules;
            _throwException = throwException;
        }

        public IDatabaseProvider GetProvider(DbProviderType dbType)
        {
			var updateManagerSqlServer = new DummyUpdateManager("ITA.WizardsUITest.DemoDbWizard.SQL");
			var updateManagerOracle = new DummyUpdateManager("ITA.WizardsUITest.DemoDbWizard.Oracle");
            var updateManagerMySql = new DummyUpdateManager("ITA.WizardsUITest.DemoDbWizard.MySql");

            // загружаем шаги обновления из ресурсов
            try
            {
                if (!_loadInvalidRules)
                {
					updateManagerSqlServer.LoadRules(Assembly.GetExecutingAssembly(), "ITA.WizardsUITest.DemoDbWizard.Default.xml");
					updateManagerOracle.LoadRules(Assembly.GetExecutingAssembly(), "ITA.WizardsUITest.DemoDbWizard.DefaultOracle.xml");
                    updateManagerMySql.LoadRules(Assembly.GetExecutingAssembly(), "ITA.WizardsUITest.DemoDbWizard.MySql.DefaultMySql.xml");
                }
                else
                {
					updateManagerSqlServer.LoadRules(Assembly.GetExecutingAssembly(), "ITA.WizardsUITest.DemoDbWizard.InvalidUpdateSteps.xml");
					updateManagerOracle.LoadRules(Assembly.GetExecutingAssembly(), "ITA.WizardsUITest.DemoDbWizard.InvalidUpdateSteps.xml");
                    updateManagerMySql.LoadRules(Assembly.GetExecutingAssembly(), "ITA.WizardsUITest.DemoDbWizard.InvalidUpdateSteps.xml");
                }
            }
            catch (Exception x)
            {
                throw new Exception("Ошибка загрузки сценария обновления Проверьте структуру xml-файла, содержащего описание сценария.", x);
            }

            if (dbType == DbProviderType.MSSQL)
            {
                SqlServerDatabaseProvider sqlDbProvider = new SqlServerDatabaseProvider(updateManagerSqlServer)
                                                              {
                                                                  DefaultDatabaseName = "test",
                                                                  DefaultUserName = "testUser",
                                                                  ServerCredentialType = ELoginType.NT,
                                                                  DatabaseCredentialType = ELoginType.NT,
                                                                  ServerName = ".\\sqlexpress",
                                                                  CreateNewDatabase = false,
                                                                  CreateNewDatabaseLogin = false
                                                              };

                // назначаем действия, которые будут выполняться на определенных шагах создании базы

                // передаем делегат
                sqlDbProvider.AddStage(new ProgressPage.Stage("Создание БД", () => sqlDbProvider.CreateDb()));
                // передаем имя ресурса, которое содержит скрипт и параметры для его заполнения
                sqlDbProvider.AddStage(new ProgressPage.Stage("Заполнение справочников", "SQL.create_dictionaries.sql",
                                                              new SqlParameter("@testParam", "test")));
                // передаем другие делегаты
                if (!_throwException)
                    sqlDbProvider.AddStage(new ProgressPage.Stage("Создание хранимых процедур", SomeAction));
                else // эмулируем ошибку во время выполнения одного из действий создания БД
                    sqlDbProvider.AddStage(new ProgressPage.Stage("Создание хранимых процедур",
                                                                  () => { throw new Exception("Test exeption"); }));
                sqlDbProvider.AddStage(new ProgressPage.Stage("Создание таблиц", SomeAction));
                sqlDbProvider.AddStage(new ProgressPage.Stage("Создание пользователей", SomeAction));
                sqlDbProvider.AddStage(new ProgressPage.Stage("Инициализация БД", SomeAction));
                sqlDbProvider.AddStage(new ProgressPage.Stage("Создание хранимых процедур", SomeAction));
                sqlDbProvider.AddStage(new ProgressPage.Stage("Наполнение БД тестовыми данными", SomeAction));
                sqlDbProvider.AddStage(new ProgressPage.Stage("Проверка тестовых данных", SomeAction));
                sqlDbProvider.AddStage(new ProgressPage.Stage("Создание резервной копии БД", SomeAction));
                sqlDbProvider.AddStage(new ProgressPage.Stage("Сохранение конфигурации", SomeAction));

                return sqlDbProvider;
            }

            if (dbType == DbProviderType.Oracle)
            {
                IDatabaseProvider oracleDbProvider = null;
                oracleDbProvider = new OracleBaseDatabaseProvider(updateManagerOracle)
                                       {
                                           DefaultDatabaseName = "test",
                                           DefaultUserName = "testUser"
                                       };
                oracleDbProvider.AddStage(new ProgressPage.Stage("Создание БД", oracleDbProvider.CreateDb));
                // передаем имя ресурса, которое содержит скрипт и параметры для его заполнения
                oracleDbProvider.AddStage(new ProgressPage.Stage("Заполнение справочников",
                                                                 "SQL.create_dictionaries_oracle.sql",
                                                                 oracleDbProvider.CreateParameter("testParam", 1)));
                // передаем другие делегаты
                if (!_throwException)
                    oracleDbProvider.AddStage(new ProgressPage.Stage("Создание хранимых процедур", SomeAction));
                else // эмулируем ошибку во время выполнения одного из действий создания БД
                    oracleDbProvider.AddStage(new ProgressPage.Stage("Создание хранимых процедур",
                                                                     () => { throw new Exception("Test exeption"); }));
                oracleDbProvider.AddStage(new ProgressPage.Stage("Создание таблиц", SomeAction));
                oracleDbProvider.AddStage(new ProgressPage.Stage("Создание пользователей", SomeAction));
                oracleDbProvider.AddStage(new ProgressPage.Stage("Инициализация БД", SomeAction));
                oracleDbProvider.AddStage(new ProgressPage.Stage("Создание хранимых процедур", SomeAction));
                oracleDbProvider.AddStage(new ProgressPage.Stage("Наполнение БД тестовыми данными", SomeAction));
                oracleDbProvider.AddStage(new ProgressPage.Stage("Проверка тестовых данных", SomeAction));
                oracleDbProvider.AddStage(new ProgressPage.Stage("Создание резервной копии БД", SomeAction));
                oracleDbProvider.AddStage(new ProgressPage.Stage("Сохранение конфигурации", SomeAction));

                return oracleDbProvider;

            }

            if (dbType == DbProviderType.MySQL)
            {
                IDatabaseProvider mysqlDbProvider = null;
                mysqlDbProvider = new MySqlServerDatabaseProvider(updateManagerMySql)
                {
                    DefaultDatabaseName = "MYSQLDEMO",
                    DefaultUserName = "mysqluser"
                };
                mysqlDbProvider.AddStage(new ProgressPage.Stage("Создание БД", mysqlDbProvider.CreateDb));
                mysqlDbProvider.AddStage(new ProgressPage.Stage("Создание таблиц", SomeAction));

                // передаем имя ресурса, которое содержит скрипт и параметры для его заполнения
                mysqlDbProvider.AddStage(new ProgressPage.Stage("Создание таблиц", "MySql.Create.create_tables_mssql.sql"));
                mysqlDbProvider.AddStage(new ProgressPage.Stage("Создание хранимых процедур", "MySql.Create.create_stored_procedures.sql"));
               
                mysqlDbProvider.AddStage(new ProgressPage.Stage("Сохранение конфигурации", SomeAction));

                return mysqlDbProvider;

            }

            if (dbType == DbProviderType.AzureSQL)
            {
                IDatabaseProvider azureSqlDbProvider = null;
                azureSqlDbProvider = new AzureSqlDatabaseProvider(updateManagerMySql)
                {
                    DefaultDatabaseName = "MYSQLDEMO",
                    DefaultUserName = "mysqluser"
                };
                azureSqlDbProvider.AddStage(new ProgressPage.Stage("Создание БД", azureSqlDbProvider.CreateDb));
                azureSqlDbProvider.AddStage(new ProgressPage.Stage("Создание таблиц", SomeAction));
                azureSqlDbProvider.AddStage(new ProgressPage.Stage("Создание хранимых процедур", SomeAction));
                azureSqlDbProvider.AddStage(new ProgressPage.Stage("Сохранение конфигурации", SomeAction));

                return azureSqlDbProvider;
            }

            return null;
        }

        #endregion

        private void SomeAction()
        {
            Thread.Sleep(1000);
            Debug.WriteLine("SomeAction");
        }
    }
}