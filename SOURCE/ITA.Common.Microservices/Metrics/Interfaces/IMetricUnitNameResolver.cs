namespace ITA.Common.Microservices.Metrics
{
    /// <summary>
    /// Metric name resolver.
    /// </summary>
    public interface IMetricUnitNameResolver
    {
        /// <summary>
        /// Returns unique metric name.
        /// </summary>
        /// <param name="metadata">Metric metadata.</param>
        string Resolve(MetricUnitMetadata metadata);
    }
}