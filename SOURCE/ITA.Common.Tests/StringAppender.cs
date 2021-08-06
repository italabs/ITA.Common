using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace ITA.Common.Tests
{
    public class StringAppender : AppenderSkeleton
    {
        public StringAppender()
        {
            TraceStrings = new List<string>();
        }

        private readonly object _syncObject = new object();

        public List<string> TraceStrings { get; protected set; }

        protected override void Append(LoggingEvent loggingEvent)
        {
            lock (_syncObject)
            {
                TraceStrings.Add(loggingEvent.RenderedMessage);
            }
        }

        public void ClearTrace()
        {
            lock (_syncObject)
            {
                TraceStrings.Clear();
            }
        }

        public static void Configure()
        {
            var h = (Hierarchy) LogManager.GetRepository(Assembly.GetExecutingAssembly());
            h.Root.Level = Level.All;
            h.Root.AddAppender(new StringAppender());
            h.Configured = true;
        }

        public static StringAppender GetStringAppender(string loggerName)
        {
            var logger = Log4NetItaHelper.GetLogger(loggerName);
            var appenders = logger.Logger.Repository.GetAppenders();
            return appenders.OfType<StringAppender>().FirstOrDefault();
        }
    }
}
