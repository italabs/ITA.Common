using System;
using ITA.Common.Host.Enums;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.PerfCounter
{
    public class CrossPlatformPerformanceCounterAdapter : IPerformanceCounterAdapter
    {
        public IPerformanceCounted GetPerformanceCounted(Type objectType, string appName, string instanceName)
        {
            return new CrossPlatformPerformanceCounted(objectType, appName, instanceName);
        }

        public ICounterUnit CreateCounterUnit(string category, string counterName,
            ItaPerformanceCounterType counterType, string instanceName, bool readOnly)
        {
            return new CrossPlatformPerfCounterUnit(category, counterName, instanceName, readOnly);
        }

        public bool CategoryExists(string categoryName)
        {
            return true;
        }
    }
}
