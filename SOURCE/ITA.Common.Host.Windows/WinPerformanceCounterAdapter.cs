using System;
using System.Diagnostics;
using ITA.Common.Host.Enums;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.Windows
{
    public class WinPerformanceCounterAdapter : IPerformanceCounterAdapter
    {
        public IPerformanceCounted GetPerformanceCounted(Type objectType, string appName, string instanceName)
        {
            return new PerformanceCounted(objectType, appName, instanceName);
        }

        public ICounterUnit CreateCounterUnit(
            string category,
            string counterName,
            ItaPerformanceCounterType counterType,
            string instanceName,
            bool readOnly)
        {
            var perfCounter = new PerformanceCounter(category, counterName, instanceName, readOnly);
            return new PerformanceCounterUnit(perfCounter);
        }

        public bool CategoryExists(string categoryName)
        {
            return PerformanceCounterCategory.Exists(categoryName);
        }
    }
}
