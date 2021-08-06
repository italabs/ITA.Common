using System;
using System.Diagnostics.Tracing;
using System.Threading;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.EventCounters
{
    internal class PollingCounterUnit : ICounterUnit
    {
        private readonly PollingCounter _counter;
        private long _rawValue;

        public PollingCounterUnit(string counterName, EventSource eventSource)
        {
            _counter = new PollingCounter(counterName, eventSource, () => _rawValue);
            _counter.DisplayName = counterName;
        }

        public string CounterName
        {
            get { return _counter.DisplayName; }
        }

        public long RawValue
        {
            get { return _rawValue; }
            set { _rawValue = value; }
        }

        public object GetUnit()
        {
            return _counter;
        }

        public void Increment()
        {
            Interlocked.Increment(ref _rawValue);
        }

        public void IncrementBy(long value)
        {
            Interlocked.Add(ref _rawValue, value);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref _rawValue);
        }
    }
}
