namespace ITA.Common.Microservices.Metrics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMetricsFactory
    {
        /// <summary>
        /// Returns information about application.
        /// </summary>
        IApplicationMetadata GetApplicationMetadata();

        /// <summary>
        /// Returns metric name resolver.
        /// </summary>
        /// <returns></returns>
        IMetricUnitNameResolver GetMetricUnitNameResolver();

        /// <summary>
        /// Create metric object.
        /// </summary>
        /// <param name="name">Metric name.</param>
        /// <param name="description">Human readable information about metric.</param>
        /// <param name="type">Metric type <see cref="MetricType"/>.</param>
        /// <param name="labels">Additional metric dimension names.</param>
        /// <returns></returns>
        IMetricUnit CreateUnit(
            string name,
            string description,
            MetricType type,
            string[] labels = null);
    }
}