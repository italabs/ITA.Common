namespace ITA.Common.Microservices.Metrics
{
    public sealed class MetricFactoryInstance
    {
        private static IMetricsFactory _metricsFactory;

        public MetricFactoryInstance(IMetricsFactory metricsFactory)
        {
            _metricsFactory = _metricsFactory ?? metricsFactory;
        }

        public static IMetricsFactory GetFactory() => _metricsFactory;
    }
}