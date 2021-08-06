namespace ITA.Common.Microservices.Versioning
{
    public interface IVersionsConfig
    {
        VersionInfo[] SupportedVersions { get; }
    }
}
