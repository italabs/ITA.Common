using ITA.Common.Microservices.Versioning;
using System;

namespace ITA.Common.Microservices.Components
{
    public abstract class BaseWebServiceConfig : IWebServiceConfig
    {
        /// <summary>
        /// Gets the config. name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the default address.
        /// </summary>
        public abstract string DefaultAddress { get; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string Address { get; set; }

        /// <inheritdoc />
        public bool SwaggerHelpEnabled { get; set; } = true;

        /// <inheritdoc />
        public bool CorsEnabled { get; set; } = true;

        /// <inheritdoc />
        public string CorsAllowOrigins { get; } = "*";

        /// <inheritdoc />
        public string CorsAllowHeaders { get; } = "*";

        /// <inheritdoc />
        public string CorsAllowMethods { get; } = "*";

        /// <inheritdoc />
        public int CorsMaxAge { get; } = 300;

        /// <inheritdoc />
        public virtual VersionInfo[] SupportedVersions { get; } = Array.Empty<VersionInfo>();
    }
}