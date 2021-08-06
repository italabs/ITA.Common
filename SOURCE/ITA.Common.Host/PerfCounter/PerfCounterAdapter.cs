using System;
using ITA.Common.Host.Enums;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.PerfCounter
{
    public class PerfCounterAdapter
    {
        private static readonly PerfCounterAdapterFactory Factory = new PerfCounterAdapterFactory();

        private static Lazy<IPerformanceCounterAdapter> Adapter =>
            new Lazy<IPerformanceCounterAdapter>(() => Factory.CreateAdapter(), true);

        public static IPerformanceCounted GetPerformanceCounted(Type objectType, string appName, string instanceName) =>
            Adapter.Value.GetPerformanceCounted(objectType, appName, instanceName);

        public static ICounterUnit CreateCounterUnit(
            string category,
            string counterName,
            ItaPerformanceCounterType counterType,
            string instanceName,
            bool readOnly) => Adapter.Value.CreateCounterUnit(category, counterName, counterType, instanceName, readOnly);

        public static bool PerformanceCounterCategoryExists(string category) => Adapter.Value.CategoryExists(category);
    }
}
