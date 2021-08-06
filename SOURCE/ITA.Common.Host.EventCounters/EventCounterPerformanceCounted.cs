using System;
using ITA.Common.Host.Interfaces;
using ITA.Common.Host.PerfCounter;

namespace ITA.Common.Host.EventCounters
{
    internal class EventCounterPerformanceCounted : PerformanceCountedBase
    {
        private readonly EventCounterSourceCollection _eventSources;

        public EventCounterPerformanceCounted(Type objectType, string appName, string instanceName, EventCounterSourceCollection eventSources)
            : base(objectType, appName, instanceName, false)
        {
            _eventSources = eventSources;

            CreateCounters();
        }

        public override ICounterUnit CreateCounterUnit(string category, string counterName, string instanceName, bool readOnly)
        {
            var eventSource = _eventSources.GetOrCreateEventSource(category);
            return new PollingCounterUnit(counterName, eventSource);
        }
    }
}
