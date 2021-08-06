using System.Diagnostics;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.Windows
{
    public class PerformanceCounterUnit : ICounterUnit
    {
        private PerformanceCounter _counter = null;
        public PerformanceCounterUnit(PerformanceCounter counter)
        {
            _counter = counter;
        }

        public long RawValue
        {
            get { return _counter.RawValue; }
            set { _counter.RawValue = value; }
        }

        public string CounterName
        {
            get { return _counter.CounterName; }
        }

        public object GetUnit()
        {
            return _counter;
        }

        public void Increment()
        {
            _counter.Increment();
        }

        public void IncrementBy(long value)
        {
            _counter.IncrementBy(value);
        }

        public void Decrement()
        {
            _counter.Decrement();
        }
    }
}
