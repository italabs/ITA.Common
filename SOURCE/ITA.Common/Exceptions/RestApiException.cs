using System;
using System.Runtime.Serialization;

namespace ITA.Common.Exceptions
{
    [Serializable]
    public class RestApiException : Exception
    {
        public ServiceExceptionDetail Detail { get; set; }

        public int HttpStatusCode { get; set; }

        public RestApiException(ServiceExceptionDetail detail, int httpStatusCode) : this(null, null, detail, httpStatusCode)
        {
        }

        public RestApiException(string message, ServiceExceptionDetail detail, int httpStatusCode) : this(message, null, detail, httpStatusCode)
        {
        }

        public RestApiException(string message, Exception innerException, ServiceExceptionDetail detail, int httpStatusCode) : base(message, innerException)
        {
            Detail = detail;
            HttpStatusCode = httpStatusCode;
        }

        protected RestApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
