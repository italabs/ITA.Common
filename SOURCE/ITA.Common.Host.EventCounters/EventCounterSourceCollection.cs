using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

namespace ITA.Common.Host.EventCounters
{
    internal class EventCounterSourceCollection
    {
        private readonly ConcurrentDictionary<string, EventSource> _eventSources;

        public EventCounterSourceCollection()
        {
            _eventSources = new ConcurrentDictionary<string, EventSource>();
        }

        public IEnumerable<EventSource> GetEventSources()
        {
            return _eventSources.Values.ToArray();
        }

        public EventSource GetOrCreateEventSource(string eventSourceName)
        {
            return _eventSources.GetOrAdd(eventSourceName, name => new EventCounterSource(name));
        }
    }
}
