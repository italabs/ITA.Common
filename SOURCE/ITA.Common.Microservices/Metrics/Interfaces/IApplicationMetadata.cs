using System.Collections.Generic;

namespace ITA.Common.Microservices.Metrics
{
    /// <summary>
    /// Application metadata.
    /// </summary>
    public interface IApplicationMetadata
    {
        /// <summary>
        /// Service name.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Additional data.
        /// </summary>
        IReadOnlyDictionary<string, string> AdditionalData { get; }
    }
}