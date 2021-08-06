namespace ITA.Common.Host
{
    /// <summary>
    /// ControlClient wrapper.
    /// </summary>
    public class ControlClient2 : IControlService
    {
        ControlClient _client;

        public ControlClient2()
        {
        }

        public void Connect(string url)
        {
            _client = new ControlClient(url);
        }

        public void Start()
        {
            _client.Start();
        }

        public void Stop()
        {
            _client.Stop();
        }

        public void Pause()
        {
            _client.Pause();
        }

        public void Continue()
        {
            _client.Continue();
        }

        public bool AutoStart
        {
            get
            {
                return _client.AutoStart;
            }
            set
            {
                _client.AutoStart = value;
            }
        }

        public bool EnableSuspend
        {
            get
            {
                return _client.EnableSuspend;
            }
            set
            {
                _client.EnableSuspend = value;
            }
        }

        public EOnPowerEventBehaviour OnBatteryAction
        {
            get
            {
                return _client.OnBatteryAction;
            }
            set
            {
                _client.OnBatteryAction = value;
            }
        }

        public EOnPowerEventBehaviour OnLowBatteryAction
        {
            get
            {
                return _client.OnLowBatteryAction;
            }
            set
            {
                _client.OnLowBatteryAction = value;
            }
        }

        public EOnPowerEventBehaviour OnSuspendAction
        {
            get
            {
                return _client.OnSuspendAction;
            }
            set
            {
                _client.OnSuspendAction = value;
            }
        }

        public string InstanceID
        {
            get
            {
                return _client.InstanceID;
            }
        }

        public string InstanceName
        {
            get
            {
                return _client.InstanceName;
            }
        }

        public string ServiceName
        {
            get
            {
                return _client.ServiceName;
            }
        }

        public string ServiceDisplayName
        {
            get
            {
                return _client.ServiceDisplayName;
            }
        }

        public EComponentStatus ServiceStatus
        {
            get
            {
                return _client.ServiceStatus;
            }
        }
    }
}