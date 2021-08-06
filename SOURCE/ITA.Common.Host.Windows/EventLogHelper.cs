using System.Diagnostics;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.Windows
{
    public static class EventLogHelper
    {
        public static EventLogEntryType GetEventType(EEventType Type)
        {
            switch (Type)
            {
                case EEventType.Error:
                    return EventLogEntryType.Error;
                case EEventType.Information:
                    return EventLogEntryType.Information;
                case EEventType.Warning:
                    return EventLogEntryType.Warning;
                case EEventType.SuccessAudit:
                    return EventLogEntryType.SuccessAudit;
                case EEventType.FailureAudit:
                    return EventLogEntryType.FailureAudit;
            }

            return EventLogEntryType.Error;
        }
    }
}
