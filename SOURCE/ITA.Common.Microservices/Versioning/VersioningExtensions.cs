using ITA.Common.Microservices.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace ITA.Common.Microservices.Versioning
{
    public static class VersioningExtensions
    {
        public static IServiceCollection AddVersioning(
            this IServiceCollection services,
            IVersionsConfig versionsConfig)
        {
            services.AddApiVersioning(option =>
            {
                option.ApiVersionReader = new UrlSegmentApiVersionReader();
                option.ReportApiVersions = false;
                option.ErrorResponses = new CustomErrorResponseProvider();
            });
            services.AddVersionedApiExplorer(option =>
            {
                option.GroupNameFormat = "'v'V";
                //option.SubstitutionFormat = "V";
                option.SubstituteApiVersionInUrl = true;
            });

            services.AddSingleton<IComponentVersionsManager>(new ComponentVersionsManager(versionsConfig));

            return services;
        }
    }
}
