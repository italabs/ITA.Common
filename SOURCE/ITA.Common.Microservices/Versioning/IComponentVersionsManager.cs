namespace ITA.Common.Microservices.Versioning
{
    public interface IComponentVersionsManager
    {
        string[] SupportedVersions { get; }

        string GetFullVersion(string apiVersion);

        bool IsDeprecated(string apiVersion);
    }
}
