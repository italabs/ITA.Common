using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ITA.Common.Microservices.Swagger
{
    public static class SwaggerExtensions
    {
        public const string BasicSecuritySchemaName = "Basic";
        public const string BearerSecuritySchemaName = "Bearer";
        public const string AuthorizationHeaderName = "Authorization";
        private const string DeprecatedApiDescription = "This API version has been deprecated.";
        private const string DefaultSwaggerDocumentVersion = "v1";
        private const string DocumentVersionPlaceHolder = "{documentVersion}";
        private const string DefaultLicenseName = "License";

        public static IServiceCollection AddSwagger(this IServiceCollection services, SwaggerSettings settings)
        {
            if (settings.Enabled)
            {
                services.AddSwaggerGen(options =>
                {
                    options.OperationFilter<SwaggerDefaultValues>();

                    var provider = services.BuildServiceProvider();
                    var versionProvider = provider.GetService<IApiVersionDescriptionProvider>();
                    if (versionProvider != null)
                    {
                        foreach (var description in versionProvider.ApiVersionDescriptions)
                        {
                            options.SwaggerDoc(
                                description.GroupName,
                                settings.ToApiInfo(description));
                        }
                    }
                    else
                    {
                        options.SwaggerDoc(
                            DefaultSwaggerDocumentVersion,
                            settings.ToApiInfo());
                    }

                    options
                        .AddBasicSecurity(settings.UseBasicAuthentication)
                        .AddBearerSecurity(settings.UseBearerAuthentication);

                    var xmlFilesPaths = settings.XmlCommentsFileNames ?? Array.Empty<string>();
                    foreach (var filesPath in xmlFilesPaths)
                    {
                        var xmlPath = Path.Combine(AppContext.BaseDirectory, filesPath);
                        if (File.Exists(xmlPath))
                        {
                            options.IncludeXmlComments(xmlPath);
                        }
                    }
                });
            }

            return services;
        }

        public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder, SwaggerSettings settings)
        {
            if (settings.Enabled)
            {
                builder.UseSwagger(options =>
                {
                    options.SerializeAsV2 = settings.SwaggerVersion2;
                });

                builder.UseSwaggerUI(options =>
                {
                    var versionProvider = builder.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
                    if (versionProvider != null)
                    {
                        foreach (var description in versionProvider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint(
                                GetSwaggerJsonUrl(settings.SwaggerJsonUrl, description.GroupName),
                                description.GroupName.ToUpperInvariant());
                        }
                    }
                    else
                    {
                        options.SwaggerEndpoint(
                            GetSwaggerJsonUrl(settings.SwaggerJsonUrl, DefaultSwaggerDocumentVersion),
                            settings.ApiTitle);
                    }
                    options.EnableDeepLinking();
                });
            }

            return builder;
        }

        private static SwaggerGenOptions AddBasicSecurity(this SwaggerGenOptions options, bool enabled)
        {
            if (enabled)
            {
                options.AddSecurityDefinition(BasicSecuritySchemaName, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = BasicSecuritySchemaName,
                    Name = AuthorizationHeaderName
                });

                if (options.OperationFilterDescriptors.All(filter => filter.Type != typeof(AuthOperationFilter)))
                {
                    options.OperationFilter<AuthOperationFilter>();
                }
            }

            return options;
        }

        private static SwaggerGenOptions AddBearerSecurity(this SwaggerGenOptions options, bool enabled)
        {
            if (enabled)
            {
                options.AddSecurityDefinition(BearerSecuritySchemaName, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = BearerSecuritySchemaName,
                    Name = AuthorizationHeaderName
                });

                if (options.OperationFilterDescriptors.All(filter => filter.Type != typeof(AuthOperationFilter)))
                {
                    options.OperationFilter<AuthOperationFilter>();
                }
            }

            return options;
        }

        private static OpenApiInfo ToApiInfo(this SwaggerSettings settings, ApiVersionDescription description)
        {
            var apiInfo = new OpenApiInfo
            {
                Version = description.ApiVersion.ToString(),
                Title = GetDocumentTitle(settings.ApiTitle, description),
            };

            var apiDescriptions = new []
            { 
                settings.ApiDescription, 
                description.IsDeprecated ? DeprecatedApiDescription : null
            };

            apiInfo.Description = string.Join(" ", apiDescriptions.Where(desc => !string.IsNullOrEmpty(desc)));

            if (!string.IsNullOrWhiteSpace(settings.LicenseUrl))
            {
                apiInfo.License = new OpenApiLicense
                {
                    Name = settings.LicenseName ?? DefaultLicenseName,
                    Url = new Uri(settings.LicenseUrl)
                };
            }

            return apiInfo;
        }

        private static OpenApiInfo ToApiInfo(this SwaggerSettings settings)
        {
            var apiInfo = new OpenApiInfo
            {
                Version = settings.ApiVersion,
                Title = settings.ApiTitle,
            };

            if (!string.IsNullOrWhiteSpace(settings.LicenseUrl))
            {
                apiInfo.License = new OpenApiLicense
                {
                    Name = settings.LicenseName ?? "License",
                    Url = new Uri(settings.LicenseUrl)
                };
            }

            return apiInfo;
        }
        
        private static string GetDocumentTitle(string apiTitle, ApiVersionDescription description)
        {
            return $"{apiTitle} {description.GroupName.ToUpperInvariant()}";
        }

        private static string GetSwaggerJsonUrl(string swaggerJsonUrlFormat, string documentVersion)
        {
            return !swaggerJsonUrlFormat.Contains(DocumentVersionPlaceHolder)
                ? swaggerJsonUrlFormat
                : swaggerJsonUrlFormat.Replace(DocumentVersionPlaceHolder, documentVersion);
        }

    }
}
