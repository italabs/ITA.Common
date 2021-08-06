using ITA.Common.Microservices.Swagger;
using Microsoft.Extensions.DependencyInjection;

namespace ITA.Common.Microservices.Components
{
    public class BaseServicesOptions
    {
        public IServiceCollection Services { get; set; }

        public IWebServiceConfig WebServiceConfig { get; set; }

        public string CorsPolicyName { get; set; }

        public string AuthenticationSchemeName { get; set; }

        public SwaggerSettings SwaggerSettings { get; set; }
    }
}