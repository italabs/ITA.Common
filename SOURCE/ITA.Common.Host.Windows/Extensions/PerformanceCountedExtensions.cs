using System.Diagnostics;

namespace ITA.Common.Host.Windows.Extensions
{
    public static class PerformanceCountedExtensions
    {
        public static PerformanceCounter GetCounter(this IPerformanceCounted performanceCounted, string counterName)
        {
            return performanceCounted?.GetCounter(counterName).GetUnit() as PerformanceCounter;
        }

        public static PerformanceCounter AsPerformanceCounter(this IPerformanceCounted performanceCounted, string counterID)
        {
            return performanceCounted?[counterID].GetUnit() as PerformanceCounter;
        }
    }
}
