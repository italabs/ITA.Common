using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace ITA.Common.Host.Failover
{
    [Guid("C3BAE43A-3C88-4FFD-A713-B6F4984A3A74")]
    public class FailoverClusterResource : IFailoverClusterResource
    {
        private string _url = "";
        private string _windowsService = "";
        private int _wait = 10000;

        public FailoverClusterResource()
        {
        }

        public string WindowsService
        {
            get { return _windowsService; }
            set { _windowsService = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public int WaitTimeout
        {
            get { return _wait; }
            set { _wait = value; }
        }

        public bool Online()
        {
            try
            {
                //
                // Start the service
                //
                if (!StartService())
                {
                    return false;
                }
                //
                // Start logically the product
                //
                return Start();
            }
            catch (Exception)
            {
                // TBD: Log exception details
                return false;
            }
        }

        public bool Offline()
        {
            try
            {
                //
                // Stop logically the product
                //
                return Stop();
            }
            catch (Exception)
            {
                // TBD: Log exception details
                return false;
            }
        }

        public bool LooksAlive()
        {
            return CheckAlive();
        }

        public bool IsAlive()
        {
            return CheckAlive();
        }

        public bool Open()
        {
            return true;
        }

        public bool Close()
        {
            return true;
        }

        public bool Terminate()
        {
            // Stop service
            return StopService();
        }

        private bool CheckAlive()
        {
            try
            {
                var client = new ControlClient(_url);

                var status = client.ServiceStatus;
                if (status != EComponentStatus.Running)
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        /// <returns></returns>
        private bool StopService()
        {
            using (var service = new ServiceController(_windowsService))
            {
                switch (service.Status)
                {
                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.ContinuePending:
                    case ServiceControllerStatus.StartPending:
                        return false;

                    case ServiceControllerStatus.Running:
                    case ServiceControllerStatus.Paused:
                        service.Stop();
                        break;

                    case ServiceControllerStatus.StopPending:
                    case ServiceControllerStatus.Stopped:
                        break;
                }

                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(_wait));
            }

            return true;
        }

        /// <summary>
        /// Start the service
        /// </summary>
        /// <returns></returns>
        private bool StartService()
        {
            using (var service = new ServiceController(_windowsService))
            {
                switch (service.Status)
                {
                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.ContinuePending:
                    case ServiceControllerStatus.Running:
                        break;

                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.StopPending:
                        return false;

                    case ServiceControllerStatus.Paused:
                        service.Continue();
                        break;

                    case ServiceControllerStatus.Stopped:
                        service.Start();
                        break;
                }

                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(_wait));
            }

            return true;
        }

        /// <summary>
        /// Start logically the product
        /// </summary>
        /// <returns></returns>
        private bool Start()
        {
            var client = new ControlClient(_url);

            switch (client.ServiceStatus)
            {
                case EComponentStatus.Stopped:
                case EComponentStatus.Error:
                    client.Start();
                    break;

                case EComponentStatus.Paused: 
                    client.Continue(); 
                    break;

                case EComponentStatus.Running:
                case EComponentStatus.Starting:
                    break;
            }

            client.WaitForStatus(new[] { EComponentStatus.Running, EComponentStatus.Error }, TimeSpan.FromMilliseconds(_wait));

            return client.ServiceStatus != EComponentStatus.Error;
        }

        /// <summary>
        /// Stop logically the product
        /// </summary>
        /// <returns></returns>
        private bool Stop()
        {
            var client = new ControlClient(_url);

            switch (client.ServiceStatus)
            {
                case EComponentStatus.Error:
                case EComponentStatus.Stopped: 
                    break;

                case EComponentStatus.Paused: 
                case EComponentStatus.Running: 
                    client.Stop(); 
                    break;

                case EComponentStatus.Starting: 
                    return false;
            }

            client.WaitForStatus(new[] { EComponentStatus.Stopped, EComponentStatus.Error }, TimeSpan.FromMilliseconds(_wait));

            return true;
        }
    }
}
