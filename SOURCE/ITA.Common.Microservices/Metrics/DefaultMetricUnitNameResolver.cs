using System.Linq;

namespace ITA.Common.Microservices.Metrics
{
    public sealed class DefaultMetricUnitNameResolver : IMetricUnitNameResolver
    {
        #region Implementation of IMetricUnitNameResolver

        public string Resolve(MetricUnitMetadata metadata)
        {
            return string.Join(
                "_",
                new[] {metadata.ServiceName, metadata.Category, metadata.MethodName, metadata.UnitName}
                    .Where(item => !string.IsNullOrWhiteSpace(item)))
                .Replace("-", "_")
                .Replace(" ", "_")
                .ToLowerInvariant();
        }

        #endregion
    }
}