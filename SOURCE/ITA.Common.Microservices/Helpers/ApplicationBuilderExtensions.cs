using System;
using Microsoft.AspNetCore.Builder;

namespace ITA.Common.Microservices.Helpers
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureBasePath(
            this IApplicationBuilder app,
            string basePath, 
            Action<IApplicationBuilder> configureAction)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                configureAction(app);
                return app;
            }
            
            return app.Map($"/{basePath.Trim('/')}", configureAction);
        }
    }
}