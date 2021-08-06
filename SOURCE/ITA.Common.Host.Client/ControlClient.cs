using System;
using System.Linq;
using ITA.Common.WCF;
using System.Threading;
using System.Diagnostics;

namespace ITA.Common.Host
{
    public class ControlClient : RemoteClientBase<IControlService>, IControlService
    {
        public int cDefaultPollInterval = 500;

        public ControlClient(string uri)
            : base(uri, new BindingOptions {  SecurityType = SecurityType.None })
        {

        }

        public ControlClient(string uri, SecurityType secutityType )
            : base(uri, new BindingOptions { SecurityType = secutityType })
        {

        }

        #region IControlService Members

        public void Start()
        {
            DoWithValidation(proxy => proxy.Start());
        }

        public void Stop()
        {
            DoWithValidation(proxy => proxy.Stop());
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Pause()
        {
            DoWithValidation(proxy => proxy.Pause());
        }

        public void Continue()
        {
            DoWithValidation(proxy => proxy.Continue());
        }

        public void WaitForStatus(EComponentStatus status, TimeSpan timeout)
        {
            WaitForStatus ( status, timeout, TimeSpan.FromMilliseconds (cDefaultPollInterval));
        }

        public void WaitForStatus(EComponentStatus status, TimeSpan timeout, TimeSpan pollInterval)
        {
            WaitForStatus(new EComponentStatus [] {status}, timeout, TimeSpan.FromMilliseconds(cDefaultPollInterval));
        }

        public void WaitForStatus(EComponentStatus[] statuses, TimeSpan timeout)
        {
            WaitForStatus(statuses, timeout, TimeSpan.FromMilliseconds(cDefaultPollInterval));
        }

        public void WaitForStatus(EComponentStatus[] statuses, TimeSpan timeout, TimeSpan pollInterval)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (!statuses.Contains ( ServiceStatus ))
            {
                Thread.Sleep(pollInterval);
                if (watch.Elapsed >= timeout)
                {
                    throw new TimeoutException ();
                }
            }
        }

        public EOnPowerEventBehaviour OnBatteryAction
        {
            get { return DoWithValidation(proxy => proxy.OnBatteryAction); }
            set { DoWithValidation(proxy => proxy.OnBatteryAction = value); }
        }

        public EOnPowerEventBehaviour OnLowBatteryAction
        {
            get { return DoWithValidation(proxy => proxy.OnLowBatteryAction); }
            set { DoWithValidation(proxy => proxy.OnLowBatteryAction = value); }
        }

        public EOnPowerEventBehaviour OnSuspendAction
        {
            get { return DoWithValidation(proxy => proxy.OnSuspendAction); }
            set { DoWithValidation(proxy => proxy.OnSuspendAction = value); }
        }

        public bool EnableSuspend
        {
            get { return DoWithValidation(proxy => proxy.EnableSuspend); }
            set { DoWithValidation(proxy => proxy.EnableSuspend = value); }
        }

        public bool AutoStart
        {
            get { return DoWithValidation(proxy => proxy.AutoStart); }
            set { DoWithValidation(proxy => proxy.AutoStart = value); }
        }

        public string InstanceID
        {
            get { return DoWithValidation(proxy => proxy.InstanceID); }
        }

        public string InstanceName
        {
            get { return DoWithValidation(proxy => proxy.InstanceName); }
        }

        public string ServiceName
        {
            get { return DoWithValidation(proxy => proxy.ServiceName); }
        }

        public string ServiceDisplayName
        {
            get { return DoWithValidation(proxy => proxy.ServiceDisplayName); }
        }

        public EComponentStatus ServiceStatus
        {
            get { return DoWithValidation(proxy => proxy.ServiceStatus); }
        }

        #endregion
    }
}
