using System;

namespace ITA.Common.Host.Interfaces
{
    /// <summary>
    /// Summary description for IContext.
    /// </summary>
    public interface IContext
    {
        string InstanceName { get; }

        IMonitoringEngine MonitoringEngine { get; }

        string GetMessage(Type type, string ID);
    }
}