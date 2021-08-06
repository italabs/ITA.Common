using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Logging
{
    public sealed class Log4NetAppliedLoggerRegistrar : IAppliedLoggerRegistrar
    {
        #region Implementation of IAppliedLogging

        public void Register(ILoggingBuilder builder, IConfiguration configuration)
        {
            var settings = configuration
                .GetSection(nameof(Log4NetSettings))
                .Get<Log4NetSettings>();

            // Tracing for Log4Net internal debugging
            if (settings.InternalOptions.Enabled)
            {
                if (!string.IsNullOrEmpty(settings.InternalOptions.FileName))
                {
                    Trace.AutoFlush = true;
                    Trace.Listeners.Add(new DefaultTraceListener
                    {
                        Name = "InternalTracingListener",
                        LogFileName = settings.InternalOptions.FileName,
                        TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ProcessId
                    });
                    Trace.Flush();
                }
            }

            log4net.Util.LogLog.InternalDebugging = settings.InternalOptions.Debug;

            builder
                .AddLog4Net(settings.ProviderOptions)
                .SetMinimumLevel(LogLevel.Trace);
        }

        #endregion
    }
}