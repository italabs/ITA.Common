using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Interfaces;
using ITA.Common;
using ITA.Common.Host;
using ITA.Common.Host.Components;
using ITA.Common.Host.Enums;
using ITA.Common.Host.Interfaces;
using ITA.Common.Host.PerfCounter;
using ITA.Common.Tracing;
using ITA.Common.Unity;
using log4net;

namespace DummyHost
{
    /// <summary>
    /// Класс хоста сервера SDMS
    /// </summary>
    [Trace]
    [Counter("State", "The state of the system. 0 - Error, 1 - Running, 2 - Stopped, 3 - Paused, 4 - Starting ", "Host", "Host Counters", ItaPerformanceCounterType.NumberOfItems64)]
    public class DummyHost : UnboundCompaundComponentWithEvents, ICompaundService, IPowerEventHandler, IService
    {
        #region public vars/props

        public static string ComponentName = "Engine";

        #endregion

        #region private vars/props

        private const string cCultureParam = "Culture";
        private const string cDefaultCulture = "ru";

        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(DummyHost).Name);
        private EComponentStatus _lastStatus = EComponentStatus.Stopped;
        private IApplicationHost _service = null;
        private EngineServiceConfig _serviceConfig = null;
        private MonitoringEngine _monitor = null;

        // Counters
        private IPerformanceCounted _counters = null;
        private ICounterUnit _stateCounter = null;

        private ICommandContext _context;
        private Thread _startingThread = null;
        private AutoResetEvent _endedEvent = new AutoResetEvent(false);

        private string _version = null;
        private string Version
        {
            get
            {
                if (string.IsNullOrEmpty(_version))
                {
                    _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }

                return _version;
            }
        }

        #endregion

        internal class AsyncStartContext
        {
            public DummyHost Host;
            public bool IsStartup;

            public void ThreadProc()
            {
                Host.AsyncStart(IsStartup);
            }
        }

        public DummyHost(IApplicationHost Service, IEventLog eventLog)
        {

            _service = Service;

            _context = new CommandContext(this);

            _serviceConfig = new EngineServiceConfig(_context.GetComponent<IConfigManager>(), GetDefaultCulture());
            CultureHelper.SetCurrentCulture(_serviceConfig.DefaultCulture);

            logger.Debug("Creating monitoring engine");
            _monitor = new MonitoringEngine(_context, eventLog);
            _monitor.EventLog.Source = HostConfigProxy.Instance.EventLogSource;

            logger.Debug("Registering resource assemblies");
            _monitor.Messages.RegisterAssembly(Assembly.GetExecutingAssembly());
            _monitor.Messages.RegisterAssembly(Assembly.GetAssembly(typeof(Events)));

            logger.Debug("Registering event log category");
            _monitor.EventLog.AddCategory(this.Name, SDMS_CATID_SERVICE);

            // Host (service) events
            ListDictionary hostEvents = new ListDictionary();
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnStarting, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnStarted, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnStopping, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnStopped, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnPausing, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnPaused, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnContinuing, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnContinued, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnError, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnBattery, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnAC, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnBatteryLow, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnQuerySuspendDenied, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnResumeAutomatic, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnResumeCritical, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            hostEvents.Add(ITA.Common.Host.Interfaces.Events.OnSuspend, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            _monitor.EventHandlers.EventHandlers.Add(typeof(DummyHost), hostEvents);

            // Config Manager events
            ListDictionary cfgEvents = new ListDictionary();
            cfgEvents.Add(ITA.Common.Host.Interfaces.Events.OnConfigurationChanged, new IEventHandler[] { new WriteToEventLogHandler(_monitor), new TraceEventHandler(_monitor) });
            _monitor.EventHandlers.EventHandlers.Add(typeof(ConfigManager), cfgEvents);

            logger.Debug("Registering event handlers");
            this.OnError += new OnIComponentError(_monitor.OnError);
            this.OnEvent += new OnIComponentEvent(_monitor.OnEvent);

            //Обработка fatal error
            this.OnFatalError += new OnIComponentError(_monitor.OnError);
            this.OnFatalError += new OnIComponentError(FatalError_Handler);

            logger.Debug("Registering remoting tracking handler");

            // Counters
            try
            {
                // Initialize performance counters
                _counters = PerfCounterAdapter.GetPerformanceCounted(typeof(DummyHost), HostConfigProxy.Instance.PerfCounterCategoryPrefix, InstanceName);

                // Request counter instance
                _stateCounter = _counters.GetCounter("State");
            }
            catch (Exception x)
            {
                logger.Error("Error has occurred while initializing host performance counters", x);
                // throw;  - do not rethrow, we will work well without the counter
            }
            //logger.Debug(Messages.HostInitialized);
        }

        void FatalError_Handler(IComponent Source, Exception x)
        {
            FireEvent(ITA.Common.Host.Interfaces.Events.OnError, EEventType.Error, x);

            Host.Stop();
            this.Status = EComponentStatus.Error;
        }

        private void InternalStop(bool bShutdown)
        {
            if (bShutdown)
            {
                base.Shutdown();

                IComponent[] components = _context.GetComponents();

                foreach (IComponent c in components)
                {
                    _monitor.EventLog.RemoveCategory(c.Name);
                }

                RemoveAll();

                _monitor.Dispose();
            }
            else if (Status == EComponentStatus.Error || Status == EComponentStatus.Running || Status == EComponentStatus.Paused)
            {
                base.Stop();
            }

            //it's hang for Linux
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        #region IComponent Members

        public override string Name
        {
            get { return ComponentName; }
        }

        public override string PublishedAs
        {
            get { return null; }
        }

        public override IPerformanceCounted Counters
        {
            get { return _counters; }
        }

        public override IComponentConfig Config
        {
            get { return _serviceConfig; }
        }

        public const short SDMS_CATID_SERVICE = 1;
        public const short SDMS_CATID_CONFIGMANAGER = 2;
        public const short SDMS_CATID_DATABASEMANAGER = 3;
        public const short SDMS_CATID_BUSINESSLOGIC = 4;
        public const short SDMS_CATID_CONTROLSERVICE = 5;
        public const short SDMS_CATID_ADMINISTRATIONSERVICE = 6;
        public const short SDMS_CATID_AUDITMANAGER = 7;
        public const short SDMS_CATID_RUNTIMESTORAGE = 8;
        public const short SDMS_CATID_DEVICECONNECTIONSERVICE = 9;
        public const short SDMS_CATID_EVENTDISPATCHER = 10;

        public override void Initialize()
        {
            Start(true);

            if (_serviceConfig.AutoStart)
            {
                this.m_IsAutoStart = true;
                Start(false);
            }
        }

        public override void Start()
        {
            this.m_IsAutoStart = false;
            Start(false);
        }

        public void Start(bool bStartup)
        {
            try
            {
                //
                // Initialization upon service start up
                //

                //Refresh app.config
                ConfigurationManager.RefreshSection("appSettings");

                if (Status != EComponentStatus.Running &&
                    Status != EComponentStatus.Paused &&
                    bStartup)
                {
                    // Unity init
                    foreach (IComponent component in _context.GetComponents())
                    {
                        Add(component);
                    }

                    ConfigManager cfgManager = (ConfigManager)_context.GetComponent<IConfigManager>();
                    cfgManager.AddConfig(Config, _context.GetComponent<ISettingsStorage>("FileSettingsStorage"));

                    _monitor.EventLog.AddCategory(ITA.Common.Host.Components.ConfigManager.cName, SDMS_CATID_CONFIGMANAGER);

                    if (_serviceConfig.ID == "")
                    {
                        _serviceConfig.ID = "{" + Guid.NewGuid().ToString().ToLower() + "}";
                    }
                }
            }
            catch (Exception x)
            {
                FireError(x);
                this.Status = EComponentStatus.Error;

                InternalStop(true); // Full stop if startup failed
                throw; // Do re-throw here. Critical error. Service should not start if base components are not initialized
            }

            try
            {
                if (bStartup)
                {
                    base.Initialize();
                    this.Status = EComponentStatus.Stopped;
                }
            }
            catch (Exception x)
            {
                FireError(x);

                this.Status = EComponentStatus.Error;

                InternalStop(true); // Full stop if startup failed
                throw; // Do re-throw here. Critical error. Service should not start if base components are not initialized
            }

            bool AsyncStart = false;

            //IsStartup = false;

            AsyncStart = !bStartup;
            //AsyncStart = _serviceConfig.AutoStart;

            if (AsyncStart)
            {
                try
                {
                    AsyncStartContext Context = new AsyncStartContext();
                    Context.Host = this;
                    Context.IsStartup = bStartup;

                    _endedEvent.Reset();

                    _startingThread = new Thread(new ThreadStart(Context.ThreadProc));
                    _startingThread.Start();
                }
                catch (Exception x)
                {
                    FireError(x);
                    this.Status = EComponentStatus.Error;

                    InternalStop(true); // Full stop if startup failed
                    //throw; // Do re-throw here. Critical error. Service should not start if base components are not initialized
                }
            }
        }

        public void AsyncStart(bool bStartup)
        {
            //
            // Initialization upon logical start
            //
            try
            {
                FireEvent(ITA.Common.Host.Interfaces.Events.OnStarting, EEventType.Information, Name, Version);

                if (Status == EComponentStatus.Stopped ||
                    Status == EComponentStatus.Error)
                {
                    this.Status = EComponentStatus.Starting;
                    base.Start();
                    this.Status = EComponentStatus.Running;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                FireEvent(ITA.Common.Host.Interfaces.Events.OnStarted, EEventType.Information, Name, Version);
            }
            catch (ThreadInterruptedException)
            {
                // just exit
            }
            catch (ThreadAbortException)
            {
                // just exit
            }
            catch (Exception x)
            {
                try
                {
                    FireError(x);
                    this.Status = EComponentStatus.Error;
                    InternalStop(false); // Partial stop
                    // Do not re-throw here. Service should start anyway to let admin inteface work
                }
                catch
                {
                }
            }
            finally
            {
                _endedEvent.Set();
            }
        }

        public override void Shutdown()
        {
            Stop(true);
        }

        public override void Stop()
        {
            Stop(false);
        }

        public void Stop(bool bShutdown)
        {
            try
            {
                //
                // Full stop, occurs when service is being stopped
                //
                FireEvent(ITA.Common.Host.Interfaces.Events.OnStopping, EEventType.Information, Name, Version);

                // fisrt stop the async thread if it's running
                if (null != _startingThread && _startingThread.IsAlive)
                {
                    // _startingThread.Interrupt(); //do not interrupt the thread
                    _endedEvent.WaitOne();
                    _startingThread = null;
                }

                InternalStop(bShutdown);
                this.Status = EComponentStatus.Stopped;

                FireEvent(ITA.Common.Host.Interfaces.Events.OnStopped, EEventType.Information, Name, Version);
            }
            catch (Exception x)
            {
                if (x is ThreadAbortException)
                { }
                else
                {
                    FireError(x);
                    throw;
                }
            }
        }

        public override void Pause()
        {
            if (Status != EComponentStatus.Running)
                return;
            try
            {
                FireEvent(ITA.Common.Host.Interfaces.Events.OnPausing, EEventType.Information, Name, Version);
                //
                // Leave all components live
                //
                base.Pause();
                this.Status = EComponentStatus.Paused;

                GC.Collect();

                FireEvent(ITA.Common.Host.Interfaces.Events.OnPaused, EEventType.Information, Name, Version);
            }
            catch (Exception x)
            {
                FireError(x);
                throw;
            }
        }

        public override void Continue()
        {
            if (Status != EComponentStatus.Paused)
                return;
            try
            {
                FireEvent(ITA.Common.Host.Interfaces.Events.OnContinuing, EEventType.Information, Name, Version);
                //
                // All components are alive
                //
                base.Continue();
                this.Status = EComponentStatus.Running;

                FireEvent(ITA.Common.Host.Interfaces.Events.OnContinued, EEventType.Information, Name, Version);
            }
            catch (Exception x)
            {
                FireError(x);
                throw;
            }
        }

        #endregion

        #region IUnboundComponentWithEvents Members

        public override string[] Events
        {
            get
            {
                ArrayList RetVal = new ArrayList();
                RetVal.AddRange(base.Events);
                RetVal.AddRange(new string[]
                                    {
                                        ITA.Common.Host.Interfaces.Events.OnBattery,
                                        ITA.Common.Host.Interfaces.Events.OnAC,
                                        ITA.Common.Host.Interfaces.Events.OnBatteryLow,
                                        ITA.Common.Host.Interfaces.Events.OnQuerySuspendDenied,
                                        ITA.Common.Host.Interfaces.Events.OnResumeAutomatic,
                                        ITA.Common.Host.Interfaces.Events.OnResumeCritical,
                                        ITA.Common.Host.Interfaces.Events.OnSuspend
                                    });
                return RetVal.ToArray() as string[];
            }
        }

        #endregion

        #region ICompaundService Members

        public new EComponentStatus Status
        {
            get
            {
                return base.Status;
            }
            set
            {
                base.SetStatus(value);
                if (_stateCounter != null)
                {
                    _stateCounter.RawValue = (int)value;
                }
            }
        }

        public string InstanceName
        {
            get { return _service.InstanceName; }
        }

        public string EventLogName
        {
            get { return HostConfigProxy.Instance.EventLogName; }
        }

        public IComponent Host
        {
            get { return this; }
        }

        public IService Service
        {
            get { return this; }
        }

        #endregion

        #region IPowerEventHandler Members

        public void OnBatteryLow()
        {
            FireEvent(ITA.Common.Host.Interfaces.Events.OnBatteryLow, EEventType.Warning);
            switch (_serviceConfig.OnLowBatteryAction)
            {
                case EOnPowerEventBehaviour.Ignore:
                    {
                        break;
                    }
                case EOnPowerEventBehaviour.Pause:
                    {
                        BackupState(false); //pause processing, save current state
                        break;
                    }
                case EOnPowerEventBehaviour.Stop:
                    {
                        BackupState(true); //stop processing, save current state
                        break;
                    }
            }
        }

        public void OnBattery()
        {
            FireEvent(ITA.Common.Host.Interfaces.Events.OnBattery, EEventType.Warning);
            switch (_serviceConfig.OnBatteryAction)
            {
                case EOnPowerEventBehaviour.Ignore:
                    {
                        break;
                    }
                case EOnPowerEventBehaviour.Pause:
                    {
                        BackupState(false); //pause processing, save current state
                        break;
                    }
                case EOnPowerEventBehaviour.Stop:
                    {
                        BackupState(true); //stop processing, save current state
                        break;
                    }
            }
        }

        public void OnAC()
        {
            FireEvent(ITA.Common.Host.Interfaces.Events.OnAC, EEventType.Warning);
            RestoreState(); // restore last state if one
        }

        public bool IsSuspendEnabled()
        {
            bool Active = Status == EComponentStatus.Running || Status == EComponentStatus.Starting;
            bool Enabled = _serviceConfig.EnableSuspend;

            if (Active && !Enabled)
            {
                FireEvent(ITA.Common.Host.Interfaces.Events.OnQuerySuspendDenied, EEventType.Warning);
                return false;
            }
            return true;
        }

        public void OnResume(EPowerResumeType Type)
        {
            switch (Type)
            {
                case EPowerResumeType.Critical:
                    {
                        FireEvent(ITA.Common.Host.Interfaces.Events.OnResumeCritical, EEventType.Warning, _lastStatus);
                        //
                        // We've been critically suspended, our state is not predictable
                        // Ignore the state before we went suspended, perform full restart
                        //
                        ResetState();
                        Stop(true); //full stop
                        Start(true); //full start
                        break;
                    }
                case EPowerResumeType.Automatic:
                    {
                        FireEvent(ITA.Common.Host.Interfaces.Events.OnResumeAutomatic, EEventType.Information, _lastStatus);
                        //
                        // We've been normally suspended, lets try to resume to previous state
                        //
                        RestoreState(); // restore last state if one
                        break;
                    }
                case EPowerResumeType.User:
                    {
                        break; //ignore
                    }
            }
        }

        public void OnSuspend()
        {
            FireEvent(ITA.Common.Host.Interfaces.Events.OnSuspend, EEventType.Information);
            switch (_serviceConfig.OnSuspendAction)
            {
                case EOnPowerEventBehaviour.Ignore:
                    {
                        break;
                    }
                case EOnPowerEventBehaviour.Pause:
                    {
                        BackupState(false); //pause processing, save current state
                        break;
                    }
                case EOnPowerEventBehaviour.Stop:
                    {
                        BackupState(true); //stop processing, save current state
                        break;
                    }
            }
        }

        public void OnSuspendDenied()
        {
            ; // ignore
        }

        #endregion

        #region IService Members

        public string ServiceName
        {
            get
            {
                return _service.Debug ? "" : _service.ServiceName;
            }
        }

        public string ServiceDisplayName
        {
            get
            {
                return _service.Debug ? "" : "SDMS Service - " + InstanceName;
            }
        }

        bool IService.Debug
        {
            get
            {
                return _service.Debug;
            }
        }

        private bool m_IsAutoStart = false;

        bool IService.IsAutoStart
        {
            get
            {
                return this.m_IsAutoStart;
            }
        }


        #endregion

        protected void BackupState(bool bStop)
        {
            switch (Status)
            {
                case EComponentStatus.Error:
                case EComponentStatus.Stopped:
                case EComponentStatus.Paused:
                    break;
                case EComponentStatus.Starting:
                    Stop();
                    break;
                case EComponentStatus.Running:
                    _lastStatus = Status;
                    if (bStop)
                    {
                        Stop();
                    }
                    else
                    {
                        Pause();
                    }
                    break;
            }
        }

        protected void RestoreState()
        {
            EComponentStatus lastStatus = _lastStatus;
            ResetState();

            switch (lastStatus)
            {
                case EComponentStatus.Error:
                case EComponentStatus.Stopped:
                case EComponentStatus.Paused:
                case EComponentStatus.Starting:
                    break;
                case EComponentStatus.Running:
                    if (Status == EComponentStatus.Paused)
                    {
                        Continue();
                    }
                    else if (Status != EComponentStatus.Running && Status != EComponentStatus.Starting)
                    {
                        Start();
                    }
                    break;
            }
        }

        protected void ResetState()
        {
            _lastStatus = EComponentStatus.Stopped;
        }

        private CultureInfo GetDefaultCulture()
        {
            string cultureName = ConfigurationManager.AppSettings[cCultureParam] ?? cDefaultCulture;
            return new CultureInfo(cultureName);
        }
    }

}
