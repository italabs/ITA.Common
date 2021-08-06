using System;
using System.Diagnostics;
using ITA.Common.Host.Interfaces;
using ITA.Common.Host.PerfCounter;
using ITA.Common.Host.Windows;

namespace ITA.Common.Host
{
    public class PerformanceCounted : PerformanceCountedBase
    {
        public PerformanceCounted(Type objectType, string instanceName) : base(objectType, instanceName)
        {
        }

        public PerformanceCounted(Type objectType, string appName, string instanceName) : base(objectType, appName, instanceName)
        {
        }

        public override ICounterUnit CreateCounterUnit(string category, string counterName, string instanceName,
            bool readOnly)
        {
            var perfCounter = new PerformanceCounter(category, counterName, instanceName, readOnly);
            perfCounter.RawValue = 0;
            return new PerformanceCounterUnit(perfCounter);
        }
    }
}