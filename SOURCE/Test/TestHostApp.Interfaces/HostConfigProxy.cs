using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using ITA.Common.Host;

namespace Interfaces
{
    public class HostConfigProxy
    {
        private const string HOST_CONFIG_FILENAME_PARAM = "HostConfigFileName";
        private const string DEFAULT_HOST_CONFIG_FILENAME = "HostConfig.xml";

        private static HostConfigProxy _instance;
        private readonly InstallHelperConfig _installHelperConfig;

        private HostConfigProxy()
        {
            string hostConfigFileName = ConfigurationManager.AppSettings[HOST_CONFIG_FILENAME_PARAM] ??
                                        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", DEFAULT_HOST_CONFIG_FILENAME);
            _installHelperConfig = InstallHelperConfig.FromFile(hostConfigFileName);
        }

        public static HostConfigProxy Instance
        {
            get { return _instance ?? (_instance = new HostConfigProxy()); }
        }

        public string HostInstanceHame
        {
            get { return _installHelperConfig.HostConfig.InstanceName; }
        }

        public string EventLogName
        {
            get { return _installHelperConfig.EventLogConfig.Log; }
        }

        public string EventLogSource
        {
            get { return _installHelperConfig.EventLogConfig.Source; }
        }

        public string PerfCounterCategoryPrefix
        {
            get { return _installHelperConfig.PerfCounterCategoryPrefix; }
        }
    }

}
