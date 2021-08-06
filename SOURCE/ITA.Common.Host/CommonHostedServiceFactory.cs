using System;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host
{
    /// <summary>
    /// Factory to get cross-platformed hosted service type.
    /// </summary>
    public class CommonHostedServiceFactory : IServiceFactory
    {
        public Type GetHostedServiceType()
        {
            return typeof(CommonHostedService);
        }
    }
}
