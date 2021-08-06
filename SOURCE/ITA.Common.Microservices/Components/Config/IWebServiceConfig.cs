using ITA.Common.Microservices.Cors;
using ITA.Common.Microservices.Swagger;
using ITA.Common.Microservices.Versioning;

namespace ITA.Common.Microservices.Components
{
    public interface IWebServiceConfig : ICorsConfig, ISwaggerConfig, IVersionsConfig
    {
        /// <summary>
        /// Web API address.
        /// </summary>
        string Address { get; set; }
    }
}