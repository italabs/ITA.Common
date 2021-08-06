using ITA.Common.Microservices.Helpers;
using System;
using System.Linq;

namespace ITA.Common.Microservices.Versioning
{
    public sealed class ComponentVersionsManager : IComponentVersionsManager
    {
        private readonly VersionInfo[] _versions;

        public ComponentVersionsManager(IVersionsConfig versionsConfig)
        {
            Guard.NotNull(versionsConfig, nameof(versionsConfig));
            
            _versions = versionsConfig.SupportedVersions;
        }

        public string[] SupportedVersions => _versions.Select(item => item.ToString()).ToArray();

        public string GetFullVersion(string apiVersion)
        {
            return FindVersion(apiVersion).ToString();
        }

        public bool IsDeprecated(string apiVersion)
        {
            return FindVersion(apiVersion).Deprecated;
        }

        private VersionInfo FindVersion(string apiVersion)
        {
            Guard.NotNullOrWhiteSpace(apiVersion, nameof(apiVersion));

            var majorVersion = int.TryParse(apiVersion, out var major) ? major : 0;            
            if (majorVersion <= 0)
            {
                if (!Version.TryParse(apiVersion, out var ver))
                {
                    throw new ArgumentOutOfRangeException(nameof(apiVersion), $"API version '{apiVersion}' is invalid.");
                }
                majorVersion = ver.Major;
            }

            if (!_versions.Any(item => item.Version.Major == majorVersion))
            {
                throw new ArgumentOutOfRangeException(nameof(majorVersion), $"API version {majorVersion} is not supported.");
            }

            return _versions.First(item => item.Version.Major == majorVersion);
        }
    }
}
