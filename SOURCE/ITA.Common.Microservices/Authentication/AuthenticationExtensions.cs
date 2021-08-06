using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ITA.Common.Microservices.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthentication<THandler>(
            this IServiceCollection services,
            string schemeName,
            Action<AuthenticationSchemeOptions> configureAction = null) where THandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            services
                .AddAuthentication(schemeName)
                .AddScheme<AuthenticationSchemeOptions, THandler>(schemeName, configureAction);

            return services;
        }
    }
}