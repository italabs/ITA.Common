using System;
using System.IO;
using System.Threading;
using Interfaces;
using ITA.Common.Host;
using ITA.Common.Host.Interfaces;
using ITA.Common.Tracing;

namespace BusinessComponent
{
    [Trace]
    public class BusinessLogic : UnboundComponentWithEvents, IBusinessLogic
    {
        #region variables

        public static string COMPONENT_NAME = "BusinessLogic";

        private readonly ICommandContext _context;

        private IServiceConfig _serviceConfig;
        private IServiceConfig ServiceConfig
        {
            get { return _serviceConfig ?? (_serviceConfig = (IServiceConfig)_context.GetComponent<IConfigManager>().GetConfig("Engine")); }
        }

        private bool _stopped = false;

        private FileWriterService _fileWriterService;
        private CancellationToken _cancelToken;

        private readonly BusinessLogicConfig _config;

        #endregion

        public BusinessLogic(ICommandContext context, IConfigManager configManager, ISettingsStorage storage)
        {
            try
            {
                _context = context;

                _config = new BusinessLogicConfig(configManager);
                configManager.AddConfig(_config, storage);

                _config.CommitRequiredParameters();
            }
            catch (Exception exc)
            {
                Logger.Error(exc.Message);
            }
        }

        #region Component implementation

        public override string Name
        {
            get { return COMPONENT_NAME; }
        }

        public override void OnInitialize()
        {
            try
            {
                base.OnInitialize();

                _fileWriterService = new FileWriterService(_config.MyTimeout, _config.TestPath);
                _cancelToken = new CancellationToken();
            }
            catch (Exception exc)
            {
                Logger.Error(exc);
            }
        }

        public override void OnShutdown()
        {
            _fileWriterService.Dispose();

            base.OnShutdown();
        }

        public override void OnStart()
        {
            try
            {
                base.OnStart();

                Logger.InfoFormat("Starting component... ");

                _fileWriterService.StartAsync(_cancelToken);
                _stopped = false;
            }
            catch (Exception exc)
            {
                Logger.Error(exc);
            }
        }

        public override void OnStop()
        {
            if (!_stopped)
            {
                _fileWriterService.StopAsync(_cancelToken);
            }

            base.OnStop();

            _stopped = true;
        }

        #endregion

        #region Server control

        public void DoStart()
        {
            _context.Host.Start();
        }

        public void DoStop()
        {
            _context.Host.Stop();
        }

        public void DoPause()
        {
            _context.Host.Pause();
        }

        public void DoContinue()
        {
            _context.Host.Continue();
        }

        public bool AutoStart
        {
            get { return ServiceConfig.AutoStart; }
            set { ServiceConfig.AutoStart = value; }
        }

        public bool EnableSuspend
        {
            get { return ServiceConfig.EnableSuspend; }
            set { ServiceConfig.EnableSuspend = value; }
        }

        public EOnPowerEventBehaviour OnBatteryAction
        {
            get { return ServiceConfig.OnBatteryAction; }
            set { ServiceConfig.OnBatteryAction = value; }
        }

        public EOnPowerEventBehaviour OnLowBatteryAction
        {
            get { return ServiceConfig.OnLowBatteryAction; }
            set { ServiceConfig.OnLowBatteryAction = value; }
        }

        public EOnPowerEventBehaviour OnSuspendAction
        {
            get { return ServiceConfig.OnSuspendAction; }
            set { ServiceConfig.OnSuspendAction = value; }
        }

        public string InstanceID
        {
            get { return ServiceConfig.ID; }
        }

        public string InstanceName
        {
            get { return _context.InstanceName; }
        }

        public string ServiceName
        {
            get { return _context.Service.ServiceName; }
        }

        public string ServiceDisplayName
        {
            get { return _context.Service.ServiceDisplayName; }
        }

        public EComponentStatus ServiceStatus
        {
            get { return _context.Status; }
        }


        #endregion
    }

}
