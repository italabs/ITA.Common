using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ITA.Common.Microservices.Exceptions
{
    public class ITAExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ITAExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.BadRequest;

            var error = new ServiceExceptionDetail(exception);

            await response.WriteAsync(JsonConvert.SerializeObject(error));
        }
    }
}