using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host
{
    public interface IPerformanceCounted
    {
        ICounterUnit GetCounter(string CounterName);
        ICounterUnit this[string CounterID] { get; }

        void ResetCounters();
        bool GetTicks(out long value);
        bool GetFrequency(out long value);
    }

}
