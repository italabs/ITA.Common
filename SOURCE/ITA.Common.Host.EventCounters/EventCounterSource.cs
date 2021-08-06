using System.Diagnostics.Tracing;

namespace ITA.Common.Host.EventCounters
{
    public class EventCounterSource : EventSource
    {
        public const string DefaultEventSourceName = "ITA-PerfCounters";

        public EventCounterSource()
            : this(DefaultEventSourceName)
        {
        }

        public EventCounterSource(string eventSourceName)
            : base(eventSourceName)
        {
        }
    }
}
