using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Logging
{
    public static class Log4NetExtensions
    {
        [Obsolete("Use Log4NetAppliedLogger and AddApplied instead")]
        public static ILoggingBuilder AddLog4Net(
            this ILoggingBuilder builder,
            IConfiguration configuration,
            LogLevel minimumLogLevel)
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
                    Trace.Listeners.Add(new DefaultTraceListener()
                    {
                        Name = "InternalTracingListener",
                        LogFileName = settings.InternalOptions.FileName,
                        TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ProcessId
                    });
                    Trace.Flush();
                }
            }

            log4net.Util.LogLog.InternalDebugging = settings.InternalOptions.Debug;

            return builder
                .AddLog4Net(settings.ProviderOptions)
                .SetMinimumLevel(minimumLogLevel);
        }

        public static ILoggingBuilder AddApplied(
            this ILoggingBuilder builder,
            IAppliedLoggerRegistrar appliedLoggerRegistrar)
        {
            var provider = builder.Services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            appliedLoggerRegistrar.Register(builder, configuration);
            return builder;
        }
    }
}