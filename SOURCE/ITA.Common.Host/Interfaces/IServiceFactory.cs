using System;

namespace ITA.Common.Host.Interfaces
{
    /// <summary>
    /// Factory to get platform specifically hosted service type
    /// </summary>
    public interface IServiceFactory
    {
        Type GetHostedServiceType();
    }
}
