using System;
using ITA.Common.Host.Enums;

namespace ITA.Common.Host.Interfaces
{
    public interface IPerformanceCounterAdapter
    {
        IPerformanceCounted GetPerformanceCounted(Type objectType, string appName, string instanceName);
        
        ICounterUnit CreateCounterUnit(
            string category,
            string counterName,
            ItaPerformanceCounterType counterType,
            string instanceName,
            bool readOnly);
        
        bool CategoryExists(string categoryName);
    }
}
