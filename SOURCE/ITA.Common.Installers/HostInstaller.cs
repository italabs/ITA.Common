using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Win32;
using log4net;
using ITA.Common.Tracing;

namespace ITA.Common.Host
{
    /// <summary>
    /// Summary description for HostInstaller.
    /// </summary>
    [RunInstaller(true)]
    [Trace]
    public class HostInstaller : Installer
    {
        private const string cDescription = "Description";
        private const string cImagePath = "ImagePath";

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components;

        private ITA.Common.Host.ServiceStartMode m_StartType;
        private SERVICE_FAILURE_ACTIONS m_FailureActions;
        private ServiceInstaller m_ServiceInstaller;
        private ServiceProcessInstaller m_ServiceProcessInstaller;
        private bool m_StartAfterInstall = true;
        private int m_StartTimeout = 30; //seconds
        private string m_szDescription = "";
        private string m_szDisplayName = "";

        private string m_szImageName = "";
        private string m_szInstanceName = BaseApplicationHost.cDefaultInstance;
        private string m_szServiceName = "";

        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(HostInstaller).Name);

        public HostInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();
        }

        public string ImageDirectory { get; set; }

        public string ImageName
        {
            get { return m_szImageName; }
            set { m_szImageName = value; }
        }

        public string DisplayName
        {
            get { return m_szDisplayName; }
            set
            {
                m_szDisplayName = value;
                UpdateFields();
            }
        }

        public string InstanceName
        {
            get { return m_szInstanceName; }
            set
            {
                m_szInstanceName = value;
                UpdateFields();
            }
        }

        public string ServiceName
        {
            get { return m_szServiceName; }
            set
            {
                m_szServiceName = value;
                UpdateFields();
            }
        }

        public string Description
        {
            get { return m_szDescription; }
            set { m_szDescription = value; }
        }

        public bool StartAfterInstall
        {
            get { return m_StartAfterInstall; }
            set { m_StartAfterInstall = value; }
        }

        public int StartTimeout
        {
            get { return m_StartTimeout; }
            set { m_StartTimeout = value; }
        }

        public ITA.Common.Host.ServiceStartMode StartType
        {
            get
            {
                return m_StartType;
            }
            set
            {
                m_StartType = value;

                m_ServiceInstaller.DelayedAutoStart = value == ServiceStartMode.AutomaticDelayedStart;

                switch (value)
                {
                    case ITA.Common.Host.ServiceStartMode.AutomaticDelayedStart:
                        {
                            m_ServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
                            break;
                        }
                    case ITA.Common.Host.ServiceStartMode.Manual:
                        {
                            m_ServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Manual;
                            break;
                        }
                    case ITA.Common.Host.ServiceStartMode.Disabled:
                        {
                            m_ServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Disabled;
                            break;
                        }
                    default:
                        {
                            m_ServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
                            break;
                        }
                }
            }
        }

        public SERVICE_FAILURE_ACTIONS FailureActions
        {
            get { return m_FailureActions; }
            set { m_FailureActions = value; }
        }

        public string[] ServicesDependedOn
        {
            get { return this.m_ServiceInstaller.ServicesDependedOn; }
            set { this.m_ServiceInstaller.ServicesDependedOn = value; }
        }

        #region Installer overridables

        public override void Commit(IDictionary savedState)
        {
            logger.DebugFormat("Opening registry key of w32 service: {0}", m_ServiceInstaller.ServiceName);

            RegistryKey Key =
                Registry.LocalMachine.OpenSubKey(
                    "SYSTEM\\CurrentControlSet\\Services\\" + m_ServiceInstaller.ServiceName, true);
            if (Key != null)
            {
                logger.Debug("Configuring service image");

                var szImagePath = Key.GetValue(cImagePath) as string;
                logger.DebugFormat("Previous value: {0}", szImagePath);

                if (string.IsNullOrEmpty(ImageDirectory))
                {
                    szImagePath = Path.GetDirectoryName(szImagePath.Trim('"')) + Path.DirectorySeparatorChar +
                                  m_szImageName;
                }
                else
                {
                    logger.DebugFormat("Original Image Directory name: {0}", ImageDirectory);

                    var di = new DirectoryInfo(ImageDirectory);
                    ImageDirectory = di.FullName;
                    logger.DebugFormat("Full Image Directory name: {0}", ImageDirectory);

                    szImagePath = Path.Combine(ImageDirectory, m_szImageName);
                }

                szImagePath = "\"" + szImagePath + "\" instance=" + InstanceName;

                logger.DebugFormat("New value: {0}", szImagePath);
                Key.SetValue(cImagePath, szImagePath);
                Key.Close();
            }
            else
            {
                logger.Debug("Registry key is not found");
            }

            base.Commit(savedState);
            //
            // Set up Description and FailureActions
            //
            if (m_FailureActions == null)
            {
                logger.Debug("Adjusting service settings");
                ServiceConfig.AdjustServiceSettings(m_ServiceInstaller.ServiceName, m_szDescription);
            }
            else
            {
                logger.Debug("Adjusting service settings and failure actions");
                ServiceConfig.AdjustServiceSettings(m_ServiceInstaller.ServiceName, m_szDescription, m_FailureActions);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        protected override void OnCommitted(IDictionary savedState)
        {
            try
            {
                logger.DebugFormat("Service starting mode: {0}", this.m_ServiceInstaller.StartType);

                if (StartAfterInstall && this.StartType != ServiceStartMode.Disabled)
                {
                    logger.DebugFormat("Starting the service: {0}", m_ServiceInstaller.ServiceName);

                    var c = new ServiceController(m_ServiceInstaller.ServiceName);
                    c.Start();
                    c.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(m_StartTimeout));
                }
            }
            catch
            {
                Context.LogMessage("Error has occurred while starting service " + m_ServiceInstaller.ServiceName);
            }
            base.OnCommitted(savedState);
        }

        #endregion

        private void UpdateFields()
        {
            if (0 == ServiceName.Length)
                return;

            if (m_szInstanceName != BaseApplicationHost.cDefaultInstance)
            {
                m_ServiceInstaller.ServiceName = ServiceName + "_" + m_szInstanceName;
                m_ServiceInstaller.DisplayName = DisplayName + " - " + m_szInstanceName;
            }
            else
            {
                m_ServiceInstaller.ServiceName = ServiceName;
                m_ServiceInstaller.DisplayName = DisplayName;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_ServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.m_ServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // m_ServiceProcessInstaller
            // 
            this.m_ServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.m_ServiceProcessInstaller.Password = null;
            this.m_ServiceProcessInstaller.Username = null;
            // 
            // m_ServiceInstaller
            // 
            this.m_ServiceInstaller.ServicesDependedOn = new string[] { "RPCSS", "Eventlog" };
            this.m_ServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[]
                                         {
                                             this.m_ServiceProcessInstaller,
                                             this.m_ServiceInstaller
                                         });
        }

        #endregion
    }
}