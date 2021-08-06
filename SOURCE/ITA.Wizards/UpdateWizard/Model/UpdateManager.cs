using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ITA.Common;
using ITA.Wizards.DatabaseWizard.Model;
using log4net;

namespace ITA.Wizards.UpdateWizard.Model
{
    /// <summary>
    /// Менеджер обновления БД
    /// </summary>
    public abstract class UpdateManager
    {
        private const string UPDATE_RULES_SCHEMA = "ITA.Wizards.UpdateRules.xsd";

        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(UpdateManager).Name);

        private UpdateRule _rules = new UpdateRule();

        public event UpdateEventHandler UpdateEvent;

        public event UpdateVersionEventHandler UpdateVersion;

        protected UpdateManager(string resourceNamespace)
        {
            logger.Info("UpdateManager instance is created");
            ResourceNamespace = resourceNamespace;
        }

        private IDatabaseProvider _databaseProvider;

        public IDatabaseProvider DatabaseProvider
        {
            get
            {
                return _databaseProvider;
            }
            set
            {
                _databaseProvider = value;
            }
        }
        public UpdateRule Rules
        {
            get { return _rules; }
            set { _rules = value; }
        }

        public string ResourceNamespace { get; set; }

        public Assembly ResourceAssembly { get; private set; }


        /// <summary>
        /// Load update rules from resource
        /// </summary>
        public void LoadRules(Assembly resourceAssembly, string resourceName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UpdateRule));

            ResourceAssembly = resourceAssembly;

            try
            {
                new XmlValidator(Assembly.GetExecutingAssembly(), UPDATE_RULES_SCHEMA).Validate(resourceAssembly, resourceName);

                using (Stream stream = resourceAssembly.GetManifestResourceStream(resourceName))
                {
                    this._rules = (UpdateRule)serializer.Deserialize(stream);
                }

                logger.Debug("Update script was loaded from resource.");
            }
            catch (Exception ex)
            {
                logger.Error("Error while update script loading.", ex);
                throw new ArgumentException(Messages.E_ITA_UPDATE_INVALID_SCRIPT, ex);
            }
        }

        /// <summary>
        /// Get current database version
        /// </summary>
        /// <returns></returns>
        public abstract Version GetDatabaseVersion();

        /// <summary>
        /// Get current database version (transactional)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract Version GetDatabaseVersion(IUpdateContext context);

        /// <summary>
        /// Set current database version
        /// </summary>
        /// <param name="context"></param>
        /// <param name="version"></param>
        protected abstract void SetDatabaseVersion(IUpdateContext context, Version version);

        /// <summary>
        /// Gets actual (the newest one) version of database.
        /// </summary>
        /// <returns>Actual version of database.</returns>
        public abstract Version GetActualDatabaseVersion();

        protected void OnUpdateEvent(object sender, UpdateEventArg arg)
        {
            if (this.UpdateEvent != null)
                this.UpdateEvent(sender, arg);
        }

        protected void OnUpdateVersion(UpdateVersionEventArg arg)
        {
            if (UpdateVersion != null)
                UpdateVersion(this, arg);
        }

        public bool IsUpdateRequired
        {
            get
            {
                try
                {
                    Version currentVersion = GetDatabaseVersion();

                    if (currentVersion < GetActualDatabaseVersion())
                    {
                        return true;
                    }
                    else if (currentVersion == GetActualDatabaseVersion())
                        return false;
                    else
                        throw new NotSupportedException(string.Format(Messages.E_ITA_DB_HAS_OLD_VERSION, currentVersion, GetActualDatabaseVersion()));

                }
                catch (SqlException ex)
                {
                    if (ex.Number == 207 || ex.Number == 208)
                    {
                        throw new NotSupportedException(Messages.E_ITA_UPDATE_INVALID_DATABASE, ex);
                    }

                    throw new Exception(Messages.E_ITA_UPDATE_UNKNOWN_VERSION, ex);
                }
            }
        }


        /// <summary>
        /// Check database version
        /// 1) Minimal supported version
        /// 2) Supported updates
        /// </summary>
        /// <param name="context"></param>
        private void CheckDatabaseVersion(IUpdateContext context)
        {
            //Текущая версия системы - из БД
            Version currentVersion = this.GetDatabaseVersion(context);

            logger.DebugFormat("Current Database version is {0}", currentVersion);

            if (currentVersion < this._rules.Minimal)
                throw new NotSupportedException(string.Format(Messages.E_ITA_UPDATE_OLD_VERSION, currentVersion, this._rules.Minimal));

            var firstStep = Utils.FirstOrDefault(Utils.Where(this._rules.Steps, s => s.From == currentVersion));
            if (firstStep == null)
                throw new NotSupportedException(Messages.E_ITA_UPDATE_UNSUPPORTED_VERSION);
        }

        /// <summary>
        /// Execute all updates steps in single transaction
        /// </summary>
        /// <returns>
        /// Actual version
        /// </returns>
        public Version ExecuteUpdates()
        {
            try
            {
                logger.Info("UpdateManager::ExecuteUpdates begin.");
                if (_rules.Steps == null || _rules.Steps.Length == 0)
                    throw new InvalidOperationException(Messages.WIZ_NO_UPDATE_RULES);

                Version newVersion = null;

                using (DbConnection conn = DatabaseProvider.GetConnection(DatabaseProvider.GetConnectionString(true, DatabaseProvider.DatabaseName)))
                {
                    conn.Open();

                    DbTransaction transaction = conn.BeginTransaction();

                    IUpdateContext context = new UpdateContext(transaction, logger);

                    Version currentVersion = this.GetDatabaseVersion(context);

                    this.CheckDatabaseVersion(context);

                    int processedComplexity = 0;

                    var totalComplexity =
                        Utils.Sum(
                            new List<DatabaseUpdateStep>(
                                Utils.Where(_rules.Steps, (s) => s.From >= currentVersion && s.Complexity > 0))
                                .ConvertAll(s => s.Complexity));

                    var sortedSteps = new List<DatabaseUpdateStep>(Utils.Where(_rules.Steps, s => s.From >= currentVersion));
                    sortedSteps.Sort(new Comparison<DatabaseUpdateStep>((s1, s2) => s1.From.CompareTo(s2.From) == 0 ? s1.Id.CompareTo(s2.Id) : s1.From.CompareTo(s2.From)));

                    foreach (DatabaseUpdateStep step in sortedSteps)
                    {
                        try
                        {
                            if (step.Type == DatabaseUpdateStepType.None)
                            {
                                //Ничего не делать. Версия БД при этому будет инкрементироваться.
                                //Необходимо для закрытия "дырок" в истории изменений версии БД
                                //Пример - в билде 1 было обновление с 1.5.11.0 -> 1.5.12.0
                                //В билде 2 этот скрипт модифицирован и помещен уже под более новой версией

                                if (step.To != null)
                                {
                                    this.SetDatabaseVersion(context, step.To);
                                    newVersion = step.To;
                                }

                                continue;
                            }

                            this.OnUpdateEvent(this, new UpdateEventArg(UpdateEventType.StepStarted, step));

                            if (step.Type == DatabaseUpdateStepType.Sql)
                            {
                                this.ExecuteSqlStep(context, step);
                            }
                            else if (step.Type == DatabaseUpdateStepType.Net)
                            {
                                this.ExecuteNetStep(context, step);
                            }
                            else
                                throw new ArgumentException(Messages.E_ITA_UPDATE_UNSUPPORTED_ACTION);

                            if (step.To != null)
                            {
                                this.SetDatabaseVersion(context, step.To);
                                newVersion = step.To;
                            }

                            processedComplexity += step.Complexity;

                            int progress = (int)(100 * (decimal)processedComplexity / totalComplexity);

                            this.OnUpdateEvent(this, new UpdateEventArg(UpdateEventType.StepFinished, step, progress));
                            Thread.Sleep(300);

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            this.OnUpdateEvent(this, new UpdateEventArg(UpdateEventType.Failed, step, ex));

                            return null;
                        }
                    }

                    OnUpdateVersion(new UpdateVersionEventArg(transaction, currentVersion, newVersion));

                    transaction.Commit();

                    this.OnUpdateEvent(this, new UpdateEventArg(UpdateEventType.Finished, null));

                    return newVersion;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error while database update", ex);

                this.OnUpdateEvent(this, new UpdateEventArg(UpdateEventType.Failed, null, ex));

                return null;
            }
            finally
            {
                logger.Info("UpdateManager::ExecuteUpdates end.");
            }
        }

        private void ExecuteSqlStep(IUpdateContext context, DatabaseUpdateStep step)
        {
            try
            {
                logger.Info("UpdateManager::ExecuteSqlStep begin.");
                logger.InfoFormat("step.Id:'{0}'", step.Id);
                logger.InfoFormat("step.Description:'{0}'", step.Description);

                string script = SqlScriptHelper.GetScript(ResourceAssembly, step.Source, ResourceNamespace);

                if (step.Arguments != null)
                {
                    script = string.Format(script, Array.ConvertAll(step.Arguments, a => a.Value));
                }

                DatabaseProvider.ExecuteScript(context.Transaction, step.Timeout, script);
            }
            catch (Exception ex)
            {
                logger.Error("Error while executing SQL-step.", ex);
                throw;
            }
            finally
            {
                logger.Info("UpdateManager::ExecuteSqlStep end.");
            }
        }

        private void ExecuteNetStep(IUpdateContext context, DatabaseUpdateStep step)
        {
            try
            {
                logger.Info("UpdateManager::ExecuteNetStep begin.");
                logger.InfoFormat("step.Id:'{0}'", step.Id);
                logger.InfoFormat("step.Description:'{0}'", step.Description);

                Type customStepType = Type.GetType(step.Source);

                if (typeof(ICustomUpdateStep).IsAssignableFrom(customStepType))
                {
                    ICustomUpdateStep customStep = (ICustomUpdateStep)Activator.CreateInstance(customStepType);
                    customStep.Execute(context, step.Arguments);
                }
                else
                    throw new ArgumentException(Messages.E_ITA_UPDATE_NO_CUSTOM_STEP);
            }
            catch (Exception ex)
            {
                logger.Error("Error while executing NET-step.", ex);
                throw;
            }
            finally
            {
                logger.Info("UpdateManager::ExecuteNetStep end.");
            }
        }
    }

    internal class XmlValidator
    {
        private readonly Assembly _xsdResourceAssembly;
        private readonly string _xsdResourceName;
        // Validation Error Count
        static int _errorsCount = 0;

        // Validation Error Message
        static string _errorMessage = "";

        public XmlValidator(Assembly xsdResourceAssembly, string xsdResourceName)
        {
            //TODO: inputs validation
            _xsdResourceAssembly = xsdResourceAssembly;
            _xsdResourceName = xsdResourceName;

        }

        public void ValidationHandler(object sender, ValidationEventArgs args)
        {
            _errorMessage = _errorMessage + args.Message + "\r\n";
            _errorsCount++;
        }

        public void Validate(Assembly xmlResourceAssembly, string xmlResourceName)
        {
            try
            {
                _errorMessage = "";
                _errorsCount = 0;
                using (Stream xsdStream = _xsdResourceAssembly.GetManifestResourceStream(_xsdResourceName))
                {
                    XmlSchema xmlSchema = XmlSchema.Read(xsdStream, null);

                    XmlReaderSettings booksSettings = new XmlReaderSettings();
                    booksSettings.Schemas.Add(xmlSchema);
                    booksSettings.ValidationType = ValidationType.Schema;
                    booksSettings.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);

                    using (Stream xmlStream = xmlResourceAssembly.GetManifestResourceStream(xmlResourceName))
                    {
                        XmlReader books = XmlReader.Create(xmlStream, booksSettings);

                        while (books.Read())
                        {
                        }

                        // Raise exception, if XML validation fails
                        if (_errorsCount > 0)
                        {
                            throw new InvalidDataException(_errorMessage);
                        }
                    }
                }

                // XML Validation succeeded
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception error)
            {
                // XML Validation failed
                throw new InvalidDataException("XML validation failed." + "\r\n" + "Error Message: " + error.Message);
            }
        }
    }
}
