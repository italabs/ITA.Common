using System.Diagnostics.Tracing;

namespace ITA.Common.ETW
{
    public interface IStaticEventSource
    {
        void Start(string methodName, string parameters);

        void Stop(string methodName, string parameters);

        void Fail(string methodName, string exception);

        void FireEventError(string methodName, string parameters);

        void FireEventInfo(string methodName, string parameters);

        void FireEventVerbose(string methodName, string parameters);

        bool IsEnabled(EventLevel level, EventKeywords keywords);
    } 
}
