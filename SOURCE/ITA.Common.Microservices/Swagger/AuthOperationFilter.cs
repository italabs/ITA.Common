using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ITA.Common.Microservices.Swagger
{
    public class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .ToArray();

            if (!attributes.Any())
            {
                attributes = context.MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .ToArray();
            }

            var attribute = attributes.FirstOrDefault();
            if (attribute == null)
            {
                return;
            }

            var basicSecurityScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = attribute.AuthenticationSchemes ?? "Undefined"
                },
            };

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [basicSecurityScheme] = new string[] { }
            });
        }
    }
}
