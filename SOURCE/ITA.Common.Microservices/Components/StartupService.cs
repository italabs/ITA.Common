using ITA.Common.Microservices.Authentication;
using ITA.Common.Microservices.Cors;
using ITA.Common.Microservices.Helpers;
using ITA.Common.Microservices.Swagger;
using ITA.Common.Microservices.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace ITA.Common.Microservices.Components
{
    public abstract class StartupBase<TAuthenticationHandler> where TAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        protected const string CorsPolicyName = "DefaultCorsPolicy";
        private const string SwaggerJsonUrlTemplate = "/swagger/{documentVersion}/swagger.json";
        protected readonly IWebServiceConfig _serviceConfig;
        protected readonly Lazy<SwaggerSettings> _swaggerSettings;
        protected readonly HostUrlInfo _hostInfo;
        protected bool _useAuthentication;

        protected StartupBase(IConfiguration configuration, IWebServiceConfig serviceConfig)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.NotNull(serviceConfig, nameof(serviceConfig));

            _serviceConfig = serviceConfig;
            _hostInfo = new HostUrlInfo(serviceConfig.Address);
            _swaggerSettings = new Lazy<SwaggerSettings>(() => CreateSwaggerSettings());
        }

        protected abstract void ConfigureBaseOptions(BaseServicesOptions options);

        protected abstract void ConfigureSwagger(SwaggerSettings settings);

        protected virtual void Configure(IServiceCollection services)
        {
        }

        protected virtual void Configure(IMvcCoreBuilder mvcBuilder)
        {
        }

        protected virtual void ConfigureApplication(IApplicationBuilder applicationBuilder)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BaseServicesOptions>(option =>
            {
                option.Services = services;
                option.WebServiceConfig = _serviceConfig;
                option.CorsPolicyName = CorsPolicyName;
                option.SwaggerSettings = _swaggerSettings.Value;

                ConfigureBaseOptions(option);
            });

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<BaseServicesOptions>>();
            var settings = options.Value;

            if (!string.IsNullOrWhiteSpace(settings.CorsPolicyName))
            {
                services.AddCorsPolicy(settings.CorsPolicyName, settings.WebServiceConfig);
            }

            _useAuthentication = !string.IsNullOrWhiteSpace(settings.AuthenticationSchemeName);
            if (_useAuthentication)
            {
                services.AddAuthentication<TAuthenticationHandler>(settings.AuthenticationSchemeName);
            }

            services.AddOptions();

            Configure(services);

            var mvcBuilder = services
                .AddMvcCore(option => option.EnableEndpointRouting = false)
                .AddApiExplorer()
                .AddControllersAsServices();

            Configure(mvcBuilder);

            services.AddVersioning(settings.WebServiceConfig);

            services.AddSwagger(settings.SwaggerSettings);
        }

        public virtual void Configure(IApplicationBuilder application)
        {
            var basePath = _hostInfo.Path.HasValue ? _hostInfo.Path.Value : null;
            application.ConfigureBasePath(
                basePath,
                app =>
                {
                    app.UseCors(CorsPolicyName);
                
                    ConfigureApplication(app);

                    app.UseApiVersioning();

                    app.UseSwagger(_swaggerSettings.Value);

                    if (_useAuthentication)
                    {
                        app.UseAuthentication();
                    }

                    app.UseMvc();
                });
        }

        private SwaggerSettings CreateSwaggerSettings()
        {
            var hostInfo = new HostUrlInfo(_serviceConfig.Address);
            var jsonUrl = hostInfo.Path.HasValue
                ? $"{hostInfo.Path.Value}{SwaggerJsonUrlTemplate}"
                : SwaggerJsonUrlTemplate;

            var settings = new SwaggerSettings
            {
                Enabled = _serviceConfig.SwaggerHelpEnabled,
                SwaggerJsonUrl = jsonUrl,
            };

            ConfigureSwagger(settings);

            return settings;
        }
    }
}