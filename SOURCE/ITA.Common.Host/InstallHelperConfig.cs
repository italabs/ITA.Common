using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ITA.Common.Tracing;

namespace ITA.Common.Host
{
    [Serializable]
    public class InstallHelperConfig
    {
        /// <summary>Install helper log file directory. Default value - "${APPDATA}\IT Assist\Common\Logs"</summary>
        public string InstallHelperLogPath { get; set; }
        /// <summary>File name of the install helper log, without extension. Default value - "InstallHelper".</summary>
        public string InstallHelperLogFileName { get; set; }
        /// <summary>Specifies whether install helper will install host service.</summary>
        public bool IsRunHostInstaller { get; set; }
        /// <summary>Collection of <see cref="HostInstaller"/> parameters.</summary>
        public HostInstallerConfig HostConfig { get; set; }
        /// <summary>Specifies whether install helper will install eventlog.</summary>
        public bool IsRunEventLogInstaller { get; set; }
        /// <summary>Collection of <see cref="Host.EventLogInstaller"/> parameters.</summary>
        public EventLogInstallerConfig EventLogConfig { get; set; }
        /// <summary>String (usually the application name) that will be used as a prefix to performance counter attributes category names.</summary>
        public string PerfCounterCategoryPrefix { get; set; }
        /// <summary>List of assembly-qualified type names containing CounterAttribute or MethodPerfCountedAttribute annotations.</summary>
        public List<string> TypesForPerfCounter { get; set; }
        /// <summary>Specifies of perfomance counters installer. 
        /// 1 - use separated installer for types containing CounterAttribute or MethodPerfCountedAttribute annotains (obsolete)
        /// >=2 - use common installer for registering performance counters from all types for any attributes
        /// </summary>
        public string PerfCounterInstallerVersion { get; set; }
        /// <summary>Any additional parameters.</summary>
        /// <remarks>Currently only RegistryPlatformName and RegistryApplicationName are used by HostConfigManager</remarks>
        public StringKeyValuePairCollection CustomParams { get; set; }
        /// <summary>
        /// Folder contains *etwManifest.man and *etwManifest.dll files
        /// </summary>
        public string EtwManifestsFolderName { get; set; }

        [Trace]
        public static InstallHelperConfig FromFile(string fileName)
        {
            InstallHelperConfig installConfig;

            try
            {
                XmlSerializer xmlSer = new XmlSerializer(typeof(InstallHelperConfig));
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    installConfig = xmlSer.Deserialize(fs) as InstallHelperConfig;
                }
            }
            catch (Exception ex)
            {
                string errMsg = (ex.InnerException is XmlException) ? ex.InnerException.Message : ex.Message;

                //throw new ITAException(Messages.E_ITA_INSTALL_XML_DESERIALIZING_ERROR1,
                //                       ITAException.E_ITA_INSTALL_XML_DESERIALIZING_ERROR1, errMsg);

                throw new ITAException(errMsg, ITAException.E_ITA_INSTALL_XML_DESERIALIZING_ERROR1, ex);
            }

            return installConfig;
        }

        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();

            retVal.AppendFormat("InstallHelperLogPath: {0}\n", InstallHelperLogPath);
            retVal.AppendFormat("InstallHelperLogFileName: {0}\n", InstallHelperLogFileName);
            retVal.AppendFormat("IsRunHostInstaller: {0}\n", IsRunHostInstaller);
            retVal.AppendFormat("IsRunEventLogInstaller: {0}\n", IsRunEventLogInstaller);
            retVal.AppendFormat("PerfCounterCategoryPrefix: {0}\n", PerfCounterCategoryPrefix);
            retVal.AppendFormat("TypesForPerfCounter: {0}\n", String.Join(", ", TypesForPerfCounter));

            retVal.AppendFormat("HostConfig: {0}\n", HostConfig);
            retVal.AppendFormat("EventLogConfig: {0}\n", EventLogConfig);

            return retVal.ToString();
        }

    }
    
    [Serializable]
    public class EventLogInstallerConfig
    {
        /// <summary>Action applied to event log on application uninstall.</summary>
        public UninstallAction UninstallAction { get; set; }

        /// <summary>Total count of eventlog categories.</summary>
        public int CategoryCount { get; set; }

        /// <summary>
        /// Full path to the assembly containing resource with eventlog categories. 
        /// For example - @"%CommonProgramFiles%\ITA Shared\Eventlog\ITA.Common.EventlogCategories.dll"
        /// </summary>
        public string CategoryResourceFile { get; set; }

        /// <summary>
        /// Full path to the assembly containing resource with eventlog messages. 
        /// For example - @"%CommonProgramFiles%\ITA Shared\Eventlog\ITA.Common.EventlogMessages.dll"
        /// </summary>
        public string MessageResourceFile { get; set; }

        /// <summary>Eventlog name.</summary>
        public string Log { get; set; }

        /// <summary>Eventlog source.</summary>
        public string Source { get; set; }

        /// <summary>
        /// Full path to the assembly containing resource with eventlog display name. 
        /// For example - @"%CommonProgramFiles%\ITA Shared\Eventlog\ITA.Common.EventlogCategories.dll"
        /// </summary>
        public string DisplayNameResourceFile { get; set; }

        /// <summary>Id of the resource in the <see cref="DisplayNameResourceFile"/> containing eventlog display name.</summary>
        public int DisplayNameResourceId { get; set; }

        /// <summary>Action taken when eventlog overflows.</summary>
        public OverflowAction OverFlowAction { get; set; }


        /// <summary>Maximum eventlog size in kilobytes.</summary>
        public int MaximumKilobytes { get; set; }

        /// <summary>
        /// The minimum number of days each event log entry is retained. 
        /// This parameter is used only if <see cref="OverflowAction"/> is set to <see cref="OverflowAction.OverwriteOlder"/>. 
        /// </summary>
        /// <remarks>Value must be in the range 1..365 inclusive.</remarks>
        public int RetentionDays { get; set; }

        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();

          //  retVal.AppendFormat("UninstallAction: {0}\n", UninstallAction);
            retVal.AppendFormat("CategoryCount: {0}\n", CategoryCount);
            retVal.AppendFormat("CategoryResourceFile: {0}\n", CategoryResourceFile);
            retVal.AppendFormat("MessageResourceFile: {0}\n", MessageResourceFile);
            retVal.AppendFormat("Log: {0}\n", Log);
            retVal.AppendFormat("Source: {0}\n", Source);
            retVal.AppendFormat("DisplayNameResourceFile: {0}\n", DisplayNameResourceFile);
            retVal.AppendFormat("DisplayNameResourceId: {0}\n", DisplayNameResourceId);
            retVal.AppendFormat("OverFlowAction: {0}\n", OverFlowAction);
            retVal.AppendFormat("MaximumKilobytes: {0}\n", MaximumKilobytes);
            retVal.AppendFormat("RetentionDays: {0}\n", RetentionDays);

            return retVal.ToString();
        }
    }

    [Serializable]
    public class HostInstallerConfig
    {
        /// <summary>Name of the service executable file. For example - "ITA.Common.Service.exe".</summary>
        public string ImageName { get; set; }

        /// <summary>Service description. For example - "Implements common background tasks"</summary>
        public string Description { get; set; }

        /// <summary>Service display name. For example - "ITA Common Service".</summary>
        public string DisplayName { get; set; }

        /// <summary>Instance name - it is appended to both <see cref="ServiceName"/> and <see cref="DisplayName"/>.</summary>
        public string InstanceName { get; set; }

        /// <summary>Service name. For example - "ITACommonSvc".</summary>
        public string ServiceName { get; set; }

        /// <summary>If set to <c>true</c>, service started immediately after installation.</summary>
        public bool StartAfterInstall { get; set; }

        /// <summary>Allow timeout for service startup.</summary>
        public int StartTimeout { get; set; }

        /// <summary>Automatic (default)|Manual|Disabled.</summary>
        public string StartType { get; set; }

        /// <summary>Indicates the services that must be running for this service to run.</summary>
        public List<string> ServicesDependedOn { get; set; }

        /// <summary>Contains actions takes in case of service failure.</summary>
        /// <remarks>Must contain three items.</remarks>
        public List<SC_ACTION> FailureActions { get; set; }

        /// <summary>The directiry to the service executable file.</summary>
        public string ImageDirectory { get; set; }

        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder ();

            retVal.AppendFormat("ImageName: {0}\n", ImageName);
            retVal.AppendFormat("Description: {0}\n", Description);
            retVal.AppendFormat("DisplayName: {0}\n", DisplayName);
            retVal.AppendFormat("InstanceName: {0}\n", InstanceName);
            retVal.AppendFormat("ServiceName: {0}\n", ServiceName);
            retVal.AppendFormat("StartAfterInstall: {0}\n", StartAfterInstall);
            retVal.AppendFormat("StartTimeout: {0}\n", StartTimeout);
            retVal.AppendFormat("StartType: {0}\n", StartType);
            retVal.AppendFormat("ImageDirectory: {0}\n", ImageDirectory);

            retVal.AppendFormat("ServicesDependedOn: {0}\n", String.Join(", ", ServicesDependedOn));
            retVal.AppendFormat("FailureActions: {0}\n", String.Join(", ", FailureActions.Select(action=>action.ToString()).ToArray()));
            
            return retVal.ToString();
        }
    }

    /// <summary>
    /// Wrapper for List of StringKeyValuePair to fix dictionary XML-serialization problem.
    /// </summary>
    [Serializable]
    public class StringKeyValuePairCollection : List<StringKeyValuePair>
    {
        [NonSerialized]
        [XmlIgnore]
        private Dictionary<string, string> m_Dictionary = null;

        /// <summary>
        /// Get or fill dictionary.
        /// </summary>
        [XmlIgnore]
        private Dictionary<string, string> Dictionary
        {
            get
            {
                if (m_Dictionary == null)
                {
                    m_Dictionary = new Dictionary<string, string>();

                    foreach (StringKeyValuePair pair in this)
                    {
                        m_Dictionary[pair.Key] = pair.Value;
                    }
                }

                return m_Dictionary;
            }
        }

        /// <summary>
        /// Get single value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return this.Dictionary[key];
        }

        /// <summary>
        /// Get single value or null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueOrDefault(string key)
        {
            string value = null;

            this.Dictionary.TryGetValue(key, out value);

            return value;
        }
    }

    [Serializable]
    public struct StringKeyValuePair
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
    }  
}
