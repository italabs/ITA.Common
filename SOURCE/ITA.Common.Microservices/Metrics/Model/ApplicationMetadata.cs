using System.Collections.Generic;

namespace ITA.Common.Microservices.Metrics
{
    /// <inheritdoc cref="IApplicationMetadata"/>
    public sealed class ApplicationMetadata : IApplicationMetadata
    {
        /// <inheritdoc cref="IApplicationMetadata.ServiceName"/>
        public string ServiceName { get; }

        /// <inheritdoc cref="IApplicationMetadata.ServiceName"/>
        public IReadOnlyDictionary<string, string> AdditionalData { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public ApplicationMetadata(
            string serviceName,
            Dictionary<string, string> additionalData = null)
        {
            ServiceName = serviceName;
            AdditionalData = additionalData ?? new Dictionary<string, string>();
        }
    }
}