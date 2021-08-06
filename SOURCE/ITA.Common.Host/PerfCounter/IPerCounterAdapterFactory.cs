using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.PerfCounter
{
    public interface IPerCounterAdapterFactory
    {
        IPerformanceCounterAdapter CreateAdapter();
    }
}