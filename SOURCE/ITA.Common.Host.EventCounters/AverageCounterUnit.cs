using System.Diagnostics.Tracing;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.EventCounters
{
    internal class AverageEventCounterUnit : ICounterUnit
    {
        private readonly PollingCounter _counter;
        private readonly object _lock = new object();
        private double _currentValue; // текущее среднее значение
        private double _count;        // кол-во измерений

        public AverageEventCounterUnit(string counterName, EventSource eventSource)
        {
            _counter = new PollingCounter(counterName, eventSource, () => GetAverageValue());
            _counter.DisplayName = counterName;
        }

        public string CounterName
        {
            get { return _counter.DisplayName; }
        }

        public long RawValue
        {
            get { return (long)GetAverageValue(); }
            set { AddValue(value); }
        }

        public object GetUnit()
        {
            return _counter;
        }

        public void Increment()
        {
            AddValue(1);
        }

        public void IncrementBy(long value)
        {
            AddValue(value);
        }

        public void Decrement()
        {
            AddValue(-1);
        }

        private double GetAverageValue()
        {
            lock (_lock)
            {
               return _currentValue;
            }
        }

        private void AddValue(long value)
        {
            lock (_lock)
            {
                if (_count != 0)
                {
                    // вычисляем среднее значение исходя из предыдущего
                    // позволяет избежать возможного переполнения
                    _currentValue = ((double)_count / (_count + 1)) * (_currentValue + value / _count);
                }
                else
                {
                    _currentValue = value;
                }
                _count++;
            }
        }
    }
}