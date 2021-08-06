using System;
using ITA.Common.Host.Enums;

namespace ITA.Common.Host
{
    [Serializable]
    public class PerformanceCounterInfo
    {
        public string CounterName;
        public string CounterDecription;
        public ItaPerformanceCounterType CounterType;
    }
}