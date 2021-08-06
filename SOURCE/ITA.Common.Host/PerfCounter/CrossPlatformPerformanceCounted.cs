using System;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.PerfCounter
{
    public class CrossPlatformPerformanceCounted : PerformanceCountedBase
    {
        public CrossPlatformPerformanceCounted(Type objectType, string instanceName) : base(objectType, instanceName)
        {
        }

        public CrossPlatformPerformanceCounted(Type objectType, string appName, string instanceName) : base(objectType, appName, instanceName)
        {
        }

        public override ICounterUnit CreateCounterUnit(string category, string counterName, string instanceName,
            bool readOnly)
        {
            return new CrossPlatformPerfCounterUnit(category, counterName, instanceName, readOnly);
        }
    }
}
