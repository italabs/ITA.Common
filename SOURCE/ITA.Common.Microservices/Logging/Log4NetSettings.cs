using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Logging
{
    public sealed class Log4NetSettings
    {
        public Log4NetInternalOptions InternalOptions { get; set; }

        public Log4NetProviderOptions ProviderOptions { get; set; }
    }
}