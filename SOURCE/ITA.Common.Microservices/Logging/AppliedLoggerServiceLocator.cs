using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Logging
{
    public sealed class AppliedLoggerFactoryInstance
    {
        private static ILoggerFactory _loggerFactory;

        public AppliedLoggerFactoryInstance(ILoggerFactory loggerFactory)
        {
            _loggerFactory = _loggerFactory ?? loggerFactory;
        }

        public static ILoggerFactory GetFactory() => _loggerFactory;
    }
}