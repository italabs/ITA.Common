using System;
using System.Diagnostics.Tracing;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.EventCounters
{
    internal class IncrementingEventCounterUnit : ICounterUnit
    {
        private readonly IncrementingEventCounter _counter;

        public IncrementingEventCounterUnit(IncrementingEventCounter counter)
        {
            _counter = counter;
        }

        public object GetUnit()
        {
            return _counter;
        }

        public string CounterName
        {
            get { return _counter.DisplayName; }
        }

        public long RawValue
        {
            get { return default; }
            set { throw new NotSupportedException(); }
        }
        
        public void Increment()
        {
            _counter.Increment();
        }

        public void IncrementBy(long value)
        {
            _counter.Increment(value);
        }

        public void Decrement()
        {
            _counter.Increment(-1);
        }
    }
}
