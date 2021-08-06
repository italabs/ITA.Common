using System;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host
{
    /// <summary>
    /// Summary description for IMonitoringEngine
    /// </summary>
    public interface IMonitoringEngine
    {
        string InstanceName { get; }

        LocaleMessages Messages { get; }
        EventHandlerCollection EventHandlers { get; }
        IEventLog EventLog { get; }

        void OnEvent(IComponent Source, string ID, EEventType Type, params object[] Args);
        void OnError(IComponent Source, Exception Error);
    }
}