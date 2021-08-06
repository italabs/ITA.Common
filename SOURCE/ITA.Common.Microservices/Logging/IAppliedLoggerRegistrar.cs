using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Logging
{
    public interface IAppliedLoggerRegistrar
    {
        void Register(ILoggingBuilder builder, IConfiguration configuration);
    }
}