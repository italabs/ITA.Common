using System.Threading;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.PerfCounter
{
    public class CrossPlatformPerfCounterUnit : ICounterUnit
    {
        private long _rawValue;
        private string _category;
        private string _counterName;
        private string _instanceName;
        private bool _readOnly;

        public CrossPlatformPerfCounterUnit(string category, string counterName, string instanceName,
            bool readOnly)
        {
            _category = category;
            _counterName = counterName;
            _instanceName = instanceName;
            _readOnly = readOnly;
        }

        public object GetUnit()
        {
            return null;
        }

        public long RawValue
        {
            get => _rawValue;
            set => _rawValue = value;
        }

        public string CounterName
        {
            get { return _counterName; }
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
