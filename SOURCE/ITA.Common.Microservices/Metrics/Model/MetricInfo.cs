using System;

namespace ITA.Common.Microservices.Metrics
{
    [Serializable]
    internal sealed class MetricInfo
    {
        /// <summary>
        /// Metric unit name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the metric unit.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Metric unit type.
        /// </summary>
        public MetricType Type { get; set; }

        /// <summary>
        /// Names of metric unit additional data.
        /// </summary>
        public string[] LabelNames { get; set; }
    }
}