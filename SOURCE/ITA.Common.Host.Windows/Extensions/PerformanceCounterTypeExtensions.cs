using System.Diagnostics;
using ITA.Common.Host.Enums;

namespace ITA.Common.Host.Windows.Extensions
{
    public static class PerformanceCounterTypeExtensions
    {
        public static PerformanceCounterType ToPerformanceCounterType(this ItaPerformanceCounterType counterType)
        {
            return (PerformanceCounterType)counterType;
        }
    }
}
