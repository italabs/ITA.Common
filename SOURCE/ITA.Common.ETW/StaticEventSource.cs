using System;
using System.Diagnostics.Tracing;

namespace ITA.Common.ETW
{
    /// <summary>
    /// Static provider
    /// </summary>
    [EventSource(Name = "ITA-Static-Trace", LocalizationResources = "ITA.Common.ETW.Localization.Messages")]
    public class StaticEventSource : EventSource, IStaticEventSource
    {
        private static readonly Lazy<StaticEventSource> Instance = new Lazy<StaticEventSource>(() => new StaticEventSource());

        public static StaticEventSource Log
        {
            get { return Instance.Value; }
        }

        #region Event parameters

        public class EventId
        {
            public const int Start = 1;
            public const int Stop = 2;
            public const int Fail = 3;
            public const int FireEventError = 4;
            public const int FireEventVerbose = 5;
            public const int FireEventInfo = 6;
        }

        public class Keywords
        {
            public const EventKeywords Trace = (EventKeywords)0x0002;
        }

        public class Tasks
        {
            public const EventTask Trace = (EventTask)0x0004;
        }

        #endregion

        [Event(EventId.Start, Keywords = Keywords.Trace, Level = EventLevel.Verbose, Task = Tasks.Trace,
            Opcode = EventOpcode.Start, Channel = EventChannel.Analytic)]
        public void Start(string methodName, string parameters)
        {
            if (IsEnabled(EventLevel.Verbose, Keywords.Trace, EventChannel.Analytic))
                WriteEvent(EventId.Start, methodName, parameters);
        }

        [Event(EventId.Stop, Keywords = Keywords.Trace, Level = EventLevel.Verbose, Task = Tasks.Trace,
            Opcode = EventOpcode.Stop, Channel = EventChannel.Analytic)]
        public void Stop(string methodName, string parameters)
        {
            if (IsEnabled(EventLevel.Verbose, Keywords.Trace, EventChannel.Analytic))
                WriteEvent(EventId.Stop, methodName, parameters);
        }

        [Event(EventId.Fail, Keywords = Keywords.Trace, Level = EventLevel.Error,
            Task = Tasks.Trace, Channel = EventChannel.Admin, Message = "{0} {1}")]
        public void Fail(string methodName, string exception)
        {
            if (IsEnabled(EventLevel.Error, Keywords.Trace, EventChannel.Admin))
                WriteEvent(EventId.Fail, methodName, exception);
        }

        [Event(EventId.FireEventError, Keywords = Keywords.Trace, Level = EventLevel.Error, 
            Channel = EventChannel.Admin, Message = "{0} {1}")]
        public void FireEventError(string methodName, string parameters)
        {
            if (IsEnabled(EventLevel.Error, Keywords.Trace, EventChannel.Admin))
                WriteEvent(EventId.FireEventError, methodName, parameters);
        }

        [Event(EventId.FireEventVerbose, Keywords = Keywords.Trace, Level = EventLevel.Verbose, Channel = EventChannel.Debug)]
        public void FireEventVerbose(string methodName, string parameters)
        {
            if (IsEnabled(EventLevel.Verbose, Keywords.Trace, EventChannel.Debug))
                WriteEvent(EventId.FireEventVerbose, methodName, parameters);
        }

        [Event(EventId.FireEventInfo, Keywords = Keywords.Trace, Level = EventLevel.Informational, Channel = EventChannel.Operational)]
        public void FireEventInfo(string methodName, string parameters)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Trace, EventChannel.Operational))
                WriteEvent(EventId.FireEventInfo, methodName, parameters);
        }
    }          
}
