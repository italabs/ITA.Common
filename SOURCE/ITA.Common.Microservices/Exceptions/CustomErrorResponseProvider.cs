using Microsoft.AspNetCore.Mvc.Versioning;
using System;

namespace ITA.Common.Microservices.Exceptions
{
    public sealed class CustomErrorResponseProvider : DefaultErrorResponseProvider
    {
        /// <inheritdoc />
        protected override object CreateErrorContent(ErrorResponseContext context)
        {
            return new ServiceExceptionDetail(new Exception(context.Message)) { IsITAException = false, ID = context.ErrorCode };
        }
    }
}