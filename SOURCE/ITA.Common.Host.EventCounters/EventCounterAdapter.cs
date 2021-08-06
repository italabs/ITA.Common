using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using ITA.Common.Host.Enums;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.EventCounters
{
    public class EventCounterAdapter : IPerformanceCounterAdapter
    {
        private readonly EventCounterSourceCollection _eventSources;

        public EventCounterAdapter()
        {
            _eventSources = new EventCounterSourceCollection();
        }

        public IPerformanceCounted GetPerformanceCounted(Type objectType, string appName, string instanceName)
        {
            return new EventCounterPerformanceCounted(objectType, appName, instanceName, _eventSources);
        }

        public IEnumerable<string> EventSourceNames
        {
            get => _eventSources.GetEventSources().Select(x => x.Name);
        }

        public bool CategoryExists(string categoryName)
        {
            return true;
        }

        public ICounterUnit CreateCounterUnit(
            string category,
            string counterName,
            ItaPerformanceCounterType counterType,
            string instanceName,
            bool readOnly)
        {
            var eventSource = _eventSources.GetOrCreateEventSource(category);

            switch (counterType)
            {
                case ItaPerformanceCounterType.RateOfCountsPerSecond64:
                    return CreateIncrementingCounterUnit(counterName, eventSource);
                case ItaPerformanceCounterType.AverageCount64:
                    return CreateAverageCounterUnit(counterName, eventSource);
                case ItaPerformanceCounterType.NumberOfItems32:
                case ItaPerformanceCounterType.NumberOfItems64:
                    return CreatePollingCounterUnit(counterName, eventSource);
                default:
                    throw new NotSupportedException($"Unsupported counter type: {counterType}");
            }
        }

        private ICounterUnit CreateAverageCounterUnit(string counterName, EventSource eventSource)
        {
            return new AverageEventCounterUnit(counterName, eventSource);
        }

        private ICounterUnit CreatePollingCounterUnit(string counterName, EventSource eventSource)
        {
            return new PollingCounterUnit(counterName, eventSource);
        }

        private ICounterUnit CreateIncrementingCounterUnit(string counterName, EventSource eventSource)
        {
            var counter = new IncrementingEventCounter(counterName, eventSource);
            counter.DisplayName = counterName;
            counter.DisplayUnits = "";
            return new IncrementingEventCounterUnit(counter);
        }
    }
}
