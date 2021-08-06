using ITA.Common.Microservices.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITA.Common.Microservices.Versioning
{
    public sealed class VersionInfo
    {
        public string ApiVersion { get; }

        public Version Version { get; }

        public bool Deprecated { get; }

        public VersionInfo(string apiVersion, string fullVersion, bool deprecated = false)
        {
            Guard.NotNullOrWhiteSpace(apiVersion, nameof(apiVersion));
            Guard.NotNullOrWhiteSpace(fullVersion, nameof(fullVersion));

            ApiVersion = apiVersion;
            Version = Version.Parse(fullVersion);
            Deprecated = deprecated;
        }

        public override string ToString()
        {
            return Deprecated ? $"{Version}-deprecated" : $"{Version}";
        }
    }
}
