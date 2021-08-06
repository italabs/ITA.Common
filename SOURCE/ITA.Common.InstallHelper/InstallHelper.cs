using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Xml;
using ITA.Common.Host;
using ITA.Common.Host.Windows.Extensions;
using ITA.Common.Installers;
using ITA.Common.Tracing;
using log4net;
using log4net.Config;
using ComponentPerfCounterInstaller = ITA.Common.Host.ComponentPerfCounterInstaller;
using HostInstaller = ITA.Common.Host.HostInstaller;

namespace ITA.Common.InstallHelper
{
    /// <summary>
    /// Summary description for ProjectInstaller.
    /// </summary>
    /// <remarks>To enable logging run .msi file with INSTALLHELPERLOGFILELOCATION=full_path_to_logfile parameter.</remarks>
    [RunInstaller(true)]
    [Trace]
    public class InstallHelper : Installer
    {
        private const string XmlConfigFileName_ParamName = "XMLCONFIG";
        private const string XmlConfigFileName_Key = "XmlConfigFileName";

        private bool m_bReinstall = false;

        private EventLogInstaller m_EventLogInstaller;
        private HostInstaller m_HostInstaller;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private string _savedXmlConfigFileName = null;
        private InstallHelperConfig _savedInstallConfig = null;

        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(InstallHelper).Name);
        private const string logDefaultPath = @"${APPDATA}\IT Assist\Common\Logs";
        private const string logDefaultFileName = @"InstallHelper";
        private const string log4netConfigPattern = @"<?xml version='1.0'?>
  <log4net>
    <appender name=""RollingLogFileAppender"" type=""log4net.Appender.RollingFileAppender"">
      <file type=""log4net.Util.PatternString"" value=""{0}\{1}.%processid.log"" />
      <appendToFile value=""true"" />
      <encoding value=""utf-8"" />
      <maxSizeRollBackups value=""10"" />
      <maximumFileSize value=""10MB"" />
      <rollingStyle value=""Composite"" />
      <datePattern value=""yyyyMMdd"" />
      <layout type=""log4net.Layout.PatternLayout"">
        <conversionPattern value=""%date [%thread] %-5level %logger - %message%newline"" />
      </layout>
    </appender>
    <root>
      <level value=""ALL"" />
      <appender-ref ref=""RollingLogFileAppender""/>
    </root>
  </log4net>";

        public InstallHelper()
        {
            // This call is required by the Designer.
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_EventLogInstaller = new ITA.Common.Host.EventLogInstaller();
            this.m_HostInstaller = new ITA.Common.Host.HostInstaller();
        }
        #endregion

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            try
            {
                ConfigureInstallers(savedState);
                base.OnBeforeInstall(savedState);
            }
            catch (Exception x)
            {
                string errMsg = "OnBeforeInstall error: " + FormatError(x);
                logger.Error(errMsg);
                Context.LogMessage(errMsg);
                throw;
            }
        }

        private void ConfigureInstallers(IDictionary savedState)
        {
            logger.Debug("ConfigureInstallers");
            string xmlConfigFileName = Context.Parameters[XmlConfigFileName_ParamName];
            ValidateXmlConfigName(xmlConfigFileName);

            if (savedState != null && !savedState.Contains(XmlConfigFileName_Key))
                savedState.Add(XmlConfigFileName_Key, xmlConfigFileName);
            
            // Deserializing xml file.
            InstallHelperConfig installConfig = InstallHelperConfig.FromFile(xmlConfigFileName);

            // Initializing log4net.
            logger.Debug("Configuring log4net");
            ConfigureLog4Net(installConfig);

            if (installConfig.TypesForPerfCounter != null && installConfig.TypesForPerfCounter.Count > 0)
            {
                int installerVersion = 1;//default
                if (!int.TryParse(installConfig.PerfCounterInstallerVersion, out installerVersion) || installerVersion <= 1)
                {
                    logger.Debug("Configuring nested installer: component performance counters");
                    ConfigureComponentPerfCounterInstaller(installConfig.TypesForPerfCounter, installConfig.PerfCounterCategoryPrefix);

                    logger.Debug("Configuring nested installer: method performance counters");
                    ConfigureMethodPerfCounterInstaller(installConfig.TypesForPerfCounter, installConfig.PerfCounterCategoryPrefix);
                }
                else
                {
                    logger.Debug("Configuring nested installer: common performance counters");
                    ConfigureCommonPerfCounterInstaller(installConfig.TypesForPerfCounter, installConfig.PerfCounterCategoryPrefix);
                }
            }

            if (!string.IsNullOrWhiteSpace(installConfig.EtwManifestsFolderName))
            {                
                var path = installConfig.EtwManifestsFolderName;
                if (!Directory.Exists(path))
                {
                    var targetDir = Path.GetDirectoryName(xmlConfigFileName).TrimEnd('\\');
                    path = targetDir + "\\" + installConfig.EtwManifestsFolderName.Trim('\\') + "\\";
                }

                logger.DebugFormat("Configuring nested installer: ETW providers. Folder '{0}'", path);

                var etwProviderInstaller = new EtwProviderInstaller(path);
                if (Installers.IndexOf(etwProviderInstaller) < 0)
                {
                    Installers.Add(etwProviderInstaller);
                }
            }

            if (installConfig.IsRunEventLogInstaller)
            {
                logger.Debug("Configuring nested installer: event log");
                ConfigureEventLogInstaller(installConfig.EventLogConfig);
                if (Installers.IndexOf(m_EventLogInstaller) < 0)
                {
                    logger.Debug("Adding eventlog installer to the collection of installers");
                    Installers.Add(m_EventLogInstaller);
                }
            }

            if (installConfig.IsRunHostInstaller)
            {
                logger.Debug("Configuring nested installer: host");
                ConfigureHostInstaller(installConfig.HostConfig);
                if (Installers.IndexOf(m_HostInstaller) < 0)
                {
                    logger.Debug("Adding host installer to the collection of installers");
                    Installers.Add(m_HostInstaller);
                }
            }

            //
            // Reinstall mode
            //
            m_bReinstall = !string.IsNullOrEmpty(Context.Parameters["REINSTALL"]);
            logger.DebugFormat("Reinstall mode: {0}", m_bReinstall);
            //
            // Do nothing in Repair mode (we don't support graceful repair)
            //
            if (m_bReinstall)
            {
                this.Installers.Clear();
            }            
        }

        private void ValidateXmlConfigName(string xmlConfigFileName)
        {
            if (xmlConfigFileName == null)
            {
                string errMsg = "Internal error: XMLCONFIG parameter is absent";
                Context.LogMessage(errMsg);
                throw new InstallException(errMsg);
            }
            if (!File.Exists(xmlConfigFileName))
            {
                string errMsg = String.Format("Internal error: File specified in XMLCONFIG parameter ('{0}') does not exist", xmlConfigFileName);
                Context.LogMessage(errMsg);
                throw new InstallException(errMsg);
            }
        }

        private void ConfigureLog4Net(InstallHelperConfig installConfig)
        {
            try
            {
                string logPath = installConfig.InstallHelperLogPath ?? logDefaultPath;
                string logFileName = installConfig.InstallHelperLogFileName ?? logDefaultFileName;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(String.Format(log4netConfigPattern, logPath, logFileName));

                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

                Context.LogMessage($"Assembly for configure logging: {assembly?.FullName}");

                var logRepository = assembly == null 
                    ? LogManager.GetRepository() 
                    : LogManager.GetRepository(assembly);

                XmlConfigurator.Configure(logRepository, doc.DocumentElement);
            }
            catch (Exception ex)
            {
                Context.LogMessage(String.Format("log4net configuration failed with exception:{0}", FormatError(ex)));
            }
        }

        private void ConfigureEventLogInstaller(EventLogInstallerConfig elic)
        {
            if (elic == null)
                throw new InstallException("EventLogInstallerConfig object is null!");

            m_EventLogInstaller.CategoryCount = elic.CategoryCount;
            m_EventLogInstaller.CategoryResourceFile = elic.CategoryResourceFile;
            m_EventLogInstaller.DisplayNameResourceFile = elic.DisplayNameResourceFile;
            m_EventLogInstaller.DisplayNameResourceId = elic.DisplayNameResourceId;
            m_EventLogInstaller.Log = elic.Log;
            m_EventLogInstaller.MaximumKilobytes = elic.MaximumKilobytes;
            m_EventLogInstaller.MessageResourceFile = elic.MessageResourceFile;
            m_EventLogInstaller.OverflowAction = elic.OverFlowAction.ToOverflowAction();
            m_EventLogInstaller.RetentionDays = elic.RetentionDays;
            m_EventLogInstaller.Source = elic.Source;
            m_EventLogInstaller.UninstallAction = elic.UninstallAction.ToUninstallAction();
        }

        private void ConfigureHostInstaller(HostInstallerConfig hic)
        {
            if (hic == null)
                throw new InstallException("HostInstallerConfig object is null!");

            if (hic.FailureActions == null || hic.FailureActions.Count != 3)
            {
                string errMsg = "Failure Actions configuration collection is either null or its length != 3";
                Context.LogMessage(errMsg);
                throw new InstallException(errMsg);
            }

            m_HostInstaller.Description = hic.Description;
            m_HostInstaller.DisplayName = hic.DisplayName; // TODO: Add validation for absence of required values
            m_HostInstaller.ImageName = hic.ImageName;
            m_HostInstaller.InstanceName = hic.InstanceName;
            m_HostInstaller.ServiceName = hic.ServiceName;
            m_HostInstaller.ImageDirectory = hic.ImageDirectory;

            ServiceStartMode startType;

            if (!Enum.TryParse(hic.StartType, out startType))
            {
                startType = ServiceStartMode.Automatic;
            }

            m_HostInstaller.StartType = startType;
            m_HostInstaller.ServicesDependedOn = hic.ServicesDependedOn != null ? hic.ServicesDependedOn.ToArray() : null;
            m_HostInstaller.StartAfterInstall = hic.StartAfterInstall;
            m_HostInstaller.StartTimeout = hic.StartTimeout;

            SERVICE_FAILURE_ACTIONS actions = new SERVICE_FAILURE_ACTIONS
                {
                    cActions = 3,
                    dwResetPeriod = 3600*24,
                    lpCommand = "",
                    lpsaActions = hic.FailureActions.ToArray()
                };

            m_HostInstaller.FailureActions = actions;
        }

        private void ConfigureMethodPerfCounterInstaller(List<string> typesForPerfCounter, string perfCounterCategoryPrefix)
        {
            var perfCounterInstallers = new List<MethodPerfCounterInstaller>();

            try
            {
                foreach (string typeName in typesForPerfCounter)
                {
                    Type type = Type.GetType(typeName);
                    if (type != null)
                    {
                        logger.DebugFormat("Got type '{0}' from string '{1}'", type.FullName, typeName);
                    }
                    else
                    {
                        throw new InstallException(String.Format("Failed to get type from string '{0}'", typeName));
                    }

                    try
                    {
                        perfCounterInstallers.Add(new MethodPerfCounterInstaller(type, perfCounterCategoryPrefix));
                    }
                    catch (Exception e)
                    {
                        throw new InstallException(String.Format("Failed to create MethodPerfCounterInstaller of type '{0}'", typeName), e);
                    }
                }
            }
            catch (Exception ex)
            {
                string errMsg = String.Format("Error while loading types for reflection:{0}", FormatError(ex));
                Context.LogMessage(errMsg);
                throw new InstallException(errMsg);
            }

            foreach (var installer in perfCounterInstallers)
            {
                if (Installers.IndexOf(installer) < 0)
                {
                    Installers.Add(installer);
                }
            }
        }

        private void ConfigureComponentPerfCounterInstaller(List<string> typesForPerfCounter, string appNameCategoryPrefix)
        {
            var perfCounterInstallers = new List<ComponentPerfCounterInstaller>();

            try
            {
                foreach (string typeName in typesForPerfCounter)
                {
                    Type type = Type.GetType(typeName);
                    if (type != null)
                    {
                        logger.DebugFormat("Got type '{0}' from string '{1}'", type.FullName, typeName);
                    }
                    else
                    {
                        throw new InstallException(String.Format("Failed to get type from string '{0}'", typeName));
                    }

                    try
                    {
                        perfCounterInstallers.Add( new ComponentPerfCounterInstaller(type, appNameCategoryPrefix) );
                    }
                    catch (Exception e)
                    {
                        throw new InstallException(String.Format("Failed to create ComponentPerfCounterInstaller of type '{0}'", typeName), e);
                    }
                }
            }
            catch (Exception ex)
            {
                string errMsg = String.Format("Error while loading types for reflection:{0}", FormatError(ex));
                Context.LogMessage(errMsg);
                throw new InstallException(errMsg);
            }

            foreach (var installer in perfCounterInstallers)
            {
                if (Installers.IndexOf(installer) < 0)
                {
                    Installers.Add(installer);
                }
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            try
            {
                _savedXmlConfigFileName = _savedXmlConfigFileName ?? Context.Parameters[XmlConfigFileName_ParamName];
                // Deserializing xml file.
                _savedInstallConfig = _savedInstallConfig ?? InstallHelperConfig.FromFile(_savedXmlConfigFileName);

                string moduleDir = _savedInstallConfig.HostConfig.ImageDirectory;
                if (string.IsNullOrEmpty(moduleDir) || !Directory.Exists(moduleDir))
                {
                    logger.WarnFormat("Module '{0}' not found.", args.Name);
                    return null;
                }

                var di = new DirectoryInfo(moduleDir);
                var module = SearchFile(di, args.Name);
                if (module != null)
                {
                    logger.DebugFormat("Found module '{0}' in directory '{1}'", args.Name, di.FullName);
                    return Assembly.LoadFrom(module.FullName);
                }

                return null;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }
        }

        private FileInfo SearchFile(DirectoryInfo directory, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            logger.DebugFormat("Search file '{0}' in directory '{1}'", fileName, directory.FullName);

            var name = fileName;
            if (name.Contains(","))
            {
                var assemblyName = new AssemblyName(name);
                name = assemblyName.Name;
            }
            return directory.GetFiles($"{name}.dll").FirstOrDefault();
        }

        private void ConfigureCommonPerfCounterInstaller(List<string> typesForPerfCounter, string appNameCategoryPrefix)
        {
            var perfCounterInstallers = new List<CommonPerfCounterInstaller>();

            try
            {
                List<Type> _types = new List<Type>();
                foreach (string typeName in typesForPerfCounter)
                {
                    Type type = Type.GetType(typeName);
                    if (type != null)
                    {
                        logger.DebugFormat("Got type '{0}' from string '{1}'", type.FullName, typeName);
                        _types.Add(type);
                    }
                    else
                    {
                        throw new InstallException(String.Format("Failed to get type from string '{0}'", typeName));
                    }
                }
                try
                {
                    perfCounterInstallers.Add(new CommonPerfCounterInstaller(_types, appNameCategoryPrefix));
                }
                catch (Exception e)
                {
                    throw new InstallException("Failed to create CommonPerfCounterInstaller", e);
                }
            }
            catch (Exception ex)
            {
                string errMsg = String.Format("Error while loading types for reflection:{0}", FormatError(ex));
                Context.LogMessage(errMsg);
                throw new InstallException(errMsg);
            }

            foreach (var installer in perfCounterInstallers)
            {
                if (Installers.IndexOf(installer) < 0)
                {
                    Installers.Add(installer);
                }
            }
        }

        private void RestoreState(IDictionary savedState)
        {
            try
            {
                if (savedState == null)
                {
                    logger.Debug("No saved state was found");
                    return;
                }

                string xmlConfigFileName = savedState[XmlConfigFileName_Key] as string;
                logger.DebugFormat("XML config file restored: {0}", xmlConfigFileName);

                ValidateXmlConfigName(xmlConfigFileName);
                InstallHelperConfig installConfig = InstallHelperConfig.FromFile(xmlConfigFileName);

                // Initializing log4net.
                logger.Debug("Configuring log4net");
                ConfigureLog4Net(installConfig);

                if (installConfig.IsRunEventLogInstaller)
                {
                    logger.Debug("Configuring nested installer: event log");
                    ConfigureEventLogInstaller(installConfig.EventLogConfig);
                }

                if (installConfig.IsRunHostInstaller)
                {
                    logger.Debug("Configuring nested installer: host");
                    ConfigureHostInstaller(installConfig.HostConfig);
                }
            }
            catch (Exception x)
            {
                string errMsg = String.Format("Exception inside RestoreState:{0}", FormatError(x));
                logger.Error(errMsg);
                Context.LogMessage(errMsg);
                throw;
            }
            finally
            {
                logger.Debug("RestoreState() <<<");
            }
        }

        protected override void OnBeforeRollback(IDictionary savedState)
        {
            try
            {
                RestoreState(savedState);

                if (m_bReinstall)
                {
                    this.Installers.Clear();
                }

                base.OnBeforeRollback(savedState);
            }
            catch (Exception x)
            {
                Context.LogMessage("OnBeforeRollback " + FormatError(x));
                throw;
            }
        }

        protected override void OnCommitting(IDictionary savedState)
        {
            try
            {
                RestoreState(savedState);

                //
                // Do nothing in Repair mode (we don't support graceful repair)
                //
                if (m_bReinstall)
                {
                    this.Installers.Clear();
                }

                base.OnCommitting(savedState);
            }
            catch (Exception x)
            {
                Context.LogMessage("OnCommitting " + FormatError(x));
                throw;
            }
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            try
            {
                ConfigureInstallers(savedState);
                RestoreState(savedState);
                base.OnBeforeUninstall(savedState);
            }
            catch (Exception x)
            {
                Context.LogMessage("OnBeforeUninstall " + FormatError(x));
                throw;
            }
        }

        public override void Install(IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);
            }
            catch (Exception x)
            {
                Context.LogMessage("Install " + FormatError(x));
                throw;
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
            }
            catch (Exception x)
            {
                Context.LogMessage("Uninstall " + FormatError(x));
                throw;
            }
        }

        public override void Rollback(IDictionary savedState)
        {
            try
            {
                ConfigureInstallers(savedState);
                base.Rollback(savedState);
            }
            catch (Exception x)
            {
                Context.LogMessage("Rollback " + FormatError(x));
                throw;
            }
        }

        public override void Commit(IDictionary savedState)
        {
            try
            {
                ConfigureInstallers(savedState);
                base.Commit(savedState);
            }
            catch (Exception x)
            {
                Context.LogMessage("Commit " + FormatError(x));
                throw;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private string FormatError(Exception x)
        {
            string RetVal = string.Format("Exception:{0}\nMessage:{1}\nSource:{2}\nStack:{3}\n", x.GetType(), x.Message, x.Source, x.StackTrace);

            foreach (DictionaryEntry Entry in x.Data)
            {
                RetVal += string.Format("{0}={1}\n", Entry.Key == null ? "<null>" : Entry.Key.ToString(),
                                                     Entry.Value == null ? "<null>" : Entry.Value.ToString());
            }

            if (x.InnerException != null)
            {
                RetVal += "------------Inner exception----------------\n";
                RetVal += FormatError(x.InnerException);
            }
            return RetVal;
        }
    }
}
