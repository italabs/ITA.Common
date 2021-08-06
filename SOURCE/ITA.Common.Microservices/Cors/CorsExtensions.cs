using System;
using System.Linq;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ITA.Common.Microservices.Cors
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(
            this IServiceCollection services,
            string policyName,
            ICorsConfig corsConfig,
            Action<CorsOptions> configureAction = null)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder => builder.ApplyCorsConfig(corsConfig));
                configureAction?.Invoke(options);
            });

            return services;
        }

        public static CorsPolicyBuilder ApplyCorsConfig(
            this CorsPolicyBuilder builder,
            ICorsConfig config)
        {
            if (!config.CorsEnabled)
            {
                return builder;
            }

            ApplyPolicy(
                config.CorsAllowOrigins,
                () => builder.AllowAnyOrigin(),
                items => builder.WithOrigins(items));

            ApplyPolicy(
                config.CorsAllowHeaders,
                () => builder.AllowAnyHeader(),
                items => builder.WithHeaders(items));

            ApplyPolicy(
                config.CorsAllowMethods,
                () => builder.AllowAnyMethod(),
                items => builder.WithMethods(items));

            if (config.CorsMaxAge >= 0)
            {
                builder.SetPreflightMaxAge(TimeSpan.FromSeconds(config.CorsMaxAge));
            }

            return builder;
        }

        private static void ApplyPolicy(
            string itemValue,
            Action anyAction,
            Action<string[]> parsedItems)
        {
            if (itemValue == CorsConstants.AnyOrigin)
            {
                anyAction();
                return;
            }

            if (string.IsNullOrWhiteSpace(itemValue))
            {
                parsedItems(Array.Empty<string>());
                return;
            }

            var items = itemValue
                .Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToArray();

            parsedItems(items);
        }
    }
}
