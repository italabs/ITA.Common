using System;

namespace ITA.Common.Microservices.Metrics
{
    /// <summary>
    /// Metrics collection mode.
    /// </summary>
    [Flags]
    public enum MetricsMode
    {
        SuccessfullyExecutedRequestsCounter = 0x1,
        AbandonedRequestsCounter = 0x2,
        LastExecutionTimeCounter = 0x4,
        RequestsPerSecCounter = 0x8,
        ExecutingRequestsCounter = 0x10,
        AvgExecutionTimeCounter = 0x20,
        TotalExecutedRequestsCounter = 0x40,
        All =   SuccessfullyExecutedRequestsCounter | 
                AbandonedRequestsCounter | 
                LastExecutionTimeCounter |
                RequestsPerSecCounter |
                ExecutingRequestsCounter |
                AvgExecutionTimeCounter |
                TotalExecutedRequestsCounter,
        Count = TotalExecutedRequestsCounter,
        Speed = RequestsPerSecCounter | AvgExecutionTimeCounter
    }
}