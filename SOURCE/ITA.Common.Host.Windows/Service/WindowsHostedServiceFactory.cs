using System;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.Windows.Service
{
    /// <summary>
    /// Factory to get Windows hosted service type
    /// </summary>
    public class WindowsHostedServiceFactory : IServiceFactory
    {
        public Type GetHostedServiceType()
        {
            return typeof(HostedWinService);
        }
    }
}
