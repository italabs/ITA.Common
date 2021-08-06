using System;
using System.Threading;
using ITA.Common.Host.Interfaces;
using log4net;

namespace ITA.Common.Host.DatabaseManager
{
    /// <summary>
    /// Вспомогательный класс для мониторинга соединения с базой данных. При исчерпании лимитов сбоев или некорректной версии БД - останавливает сервер.
    /// </summary>
    public class DatabaseConnectionMonitor
    {       
        /// <summary>
        /// log4net logger instance
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Количества неудачных попыток проверить соединение до останова сервера.
        /// </summary>
        private int _maxConnectionAttempts;

        private int _connectionErrorAttemptDelayInterval;

        private int _connectionSuccessAttemptDelayInterval;

        private IComponentWithEvents _databaseManager;

        private IDatabaseVersionProvider _versionProvider;
        
        public DatabaseConnectionMonitor(IComponentWithEvents databaseManager): this(databaseManager, 10, 60, 5)
        {

        }

        public DatabaseConnectionMonitor(IComponentWithEvents databaseManager, int maxConnectionAttempts, int connectionSuccessAttemptDelayInterval, int сonnectionErrorAttemptDelayInterval)
        {
            this._versionProvider = databaseManager as IDatabaseVersionProvider;

            if (this._versionProvider == null)
                throw new ArgumentException("DatabaseManager component should implement IDatabaseVersionProvider interface.");

            this._databaseManager = databaseManager;

            this._log = Log4NetItaHelper.GetLogger(this._databaseManager.Name);

            this._maxConnectionAttempts = maxConnectionAttempts;
            this._connectionSuccessAttemptDelayInterval = connectionSuccessAttemptDelayInterval;
            this._connectionErrorAttemptDelayInterval = сonnectionErrorAttemptDelayInterval;
        }

        #region Проверка соединения с БД и версии
       
        /// <summary>
        /// Проверка версии и соединения с БД при ручном старте
        /// </summary>
        public void CheckDatabaseStateSync(bool isAutoStart)
        {
            _log.Info("DatabaseManager::CheckDatabaseStateSync begin.");

            int connectionErrorCount = 0;

            try
            {
                while (true)
                {
                    Version databaseVersion = null;

                    try
                    {
                        //Проверяем соединение к БД
                        databaseVersion = this._versionProvider.GetDatabaseVersion();
                    }
                    catch (Exception ex)
                    {
                        //Сбой соединения с сервером БД
                        connectionErrorCount++;

                        _log.WarnFormat("Database connection error count is '{0}' from '{1}'", connectionErrorCount, this._maxConnectionAttempts);

                        //Кол-во сбоев превысило все допустимые нормы - останавливаем сервер
                        if (!isAutoStart || connectionErrorCount >= this._maxConnectionAttempts)
                        {
                            _log.ErrorFormat("Stopping host");
                            throw new ITAException(Messages.E_ITA_DATABASE_DB_NA, ITAException.E_ITA_DATABASE_MANAGER_ERROR, ex, "MonitorDatabaseStateImpl");
                        }

                        //Ждем указанный в настройках период
                        Thread.Sleep(this._connectionErrorAttemptDelayInterval * 1000);

                        continue;
                    }

                    if (this._versionProvider.EnableVersionCheck)
                    {
                        if (databaseVersion != this._versionProvider.CurrentDatabaseVersion)
                        {
                            throw new ITAException(string.Format(
                                    Messages.E_ITA_DATABASE_DB_HAS_OLD_VERSION,
                                                        databaseVersion,
                                                        this._versionProvider.CurrentDatabaseVersion),
                                                        ITAException.E_ITA_DATABASE_MANAGER_ERROR, null, "CheckDatabaseStateSync");
                        }
                    }

                    return;
                }
            }
            finally
            {
                _log.Info("DatabaseManager::CheckDatabaseStateSync end.");
            }
        }

        private Thread _monitorThread = null;

        /// <summary>
        /// Создание потока на мониторинг соединения с сервером БД
        /// </summary>
        public void MonitorDatabaseState()
        {
            this.AbortMonitorThread();

            this._monitorThread = new Thread(new ParameterizedThreadStart(this.MonitorDatabaseStateImpl));
            this._monitorThread.IsBackground = true;
            this._monitorThread.Start(true);
        }

        /// <summary>
        /// Принудительное завершение процедуры мониторинга коннекта к БД
        /// Иначе при большом значении таймаута штатный механизм приводит к длительной задержке при остановке
        /// </summary>
        public void AbortMonitorThread()
        {
            try
            {
                if (this._monitorThread != null)
                    this._monitorThread.Abort();
            }
            catch (Exception)
            {
                _log.Warn("Error while database monitor thread aborting. Ignoring...");
            }
        }

        /// <summary>
        /// Создание потока на мониторинг соединения с сервером БД
        /// </summary>
        /// <param name="isAutoRunObj"></param>
        private void MonitorDatabaseStateImpl(object isAutoRunObj)
        {
            _log.Info("DatabaseManager::MonitorDatabaseStateImpl begin.");

            int connectionErrorCount = 0;

            while (true)
            {
                try
                {
                    //Проверяем соединение к БД
                    Version databaseVersion = this._versionProvider.GetDatabaseVersion();

                    if (this._versionProvider.EnableVersionCheck)
                    {
                        if (databaseVersion != this._versionProvider.CurrentDatabaseVersion)
                        {
                            this._databaseManager.FireFatalError(new ITAException(string.Format(
                                    Messages.E_ITA_DATABASE_DB_HAS_OLD_VERSION,
                                                                      databaseVersion,
                                                                      this._versionProvider.CurrentDatabaseVersion),
                                                                      ITAException.E_ITA_DATABASE_MANAGER_ERROR, null, "MonitorDatabaseStateImpl"));

                            break;
                        }
                    }

                    //Если все ОК - сбрасываем счетчик количества ошибок
                    connectionErrorCount = 0;

                    Thread.Sleep(this._connectionSuccessAttemptDelayInterval * 1000);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    //Сбой соединения с сервером БД
                    connectionErrorCount++;

                    _log.WarnFormat("Database connection error count is '{0}' from '{1}'", connectionErrorCount, this._maxConnectionAttempts);

                    //Кол-во сбоев превысило все допустимые нормы - останавливаем сервер
                    if (connectionErrorCount >= this._maxConnectionAttempts)
                    {
                        _log.ErrorFormat("Stopping host");
                        this._databaseManager.FireFatalError(new ITAException(Messages.E_ITA_DATABASE_DB_NA, ITAException.E_ITA_DATABASE_MANAGER_ERROR, ex, "MonitorDatabaseStateImpl"));
                        break;
                    }

                    //Ждем указанный в настройках период
                    Thread.Sleep(this._connectionErrorAttemptDelayInterval * 1000);
                }
            }

            _log.Info("DatabaseManager::MonitorDatabaseStateImpl end.");
        }       
        
        #endregion
    }
}
