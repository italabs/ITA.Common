using System.Collections.Generic;
using ITA.Common.Microservices.Helpers;

namespace ITA.Common.Microservices.Metrics
{
    /// <summary>
    /// Metric unit metadata.
    /// </summary>
    public sealed class MetricUnitMetadata
    {
        /// <summary>
        /// Application prefix.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Category is target class name or custom name.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Target method name.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Metric unit name.
        /// </summary>
        public string UnitName { get; }

        /// <summary>
        /// Additional metadata items.
        /// </summary>
        public IReadOnlyDictionary<string, string> AdditionalData { get; }

        public MetricUnitMetadata(
            string serviceName,
            string category,
            string methodName,
            string unitName,
            IReadOnlyDictionary<string, string> additionalData)
        {
            Guard.NotNullOrWhiteSpace(serviceName, nameof(serviceName));
            Guard.NotNullOrWhiteSpace(unitName, nameof(unitName));

            ServiceName = serviceName;
            Category = category;
            MethodName = methodName;
            UnitName = unitName;
            AdditionalData = additionalData ?? new Dictionary<string, string>();
        }
    }
}