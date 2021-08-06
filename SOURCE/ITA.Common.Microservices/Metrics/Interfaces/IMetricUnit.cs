namespace ITA.Common.Microservices.Metrics
{
    /// <summary>
    /// Metric.
    /// </summary>
    public interface IMetricUnit
    {
        /// <summary>
        /// Unique metric name.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Set metric value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="labels">Additional metric dimension values.</param>
        void SetValue(long value, string[] labels = null);

        /// <summary>
        /// Increases the value of the metric by 1.
        /// </summary>
        /// <param name="labels">Additional metric dimension values.</param>
        void Increment(string[] labels = null);
        
        /// <summary>
        /// Increases the value of the metric by <see cref="value"/>.
        /// </summary>
        /// <param name="value">Value for increase.</param>
        /// <param name="labels">Additional metric dimension values.</param>
        void IncrementBy(long value, string[] labels = null);
        
        /// <summary>
        /// Decreases the value of the metric by 1.
        /// </summary>
        /// <param name="labels">Additional metric dimension values.</param>
        void Decrement(string[] labels = null);
    }
}