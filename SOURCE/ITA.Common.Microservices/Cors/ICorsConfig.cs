namespace ITA.Common.Microservices.Cors
{
    /// <summary>
    /// CORS configuration.
    /// </summary>
    public interface ICorsConfig
    {
        /// <summary>
        /// True - enable CORS supported.
        /// Default - False.
        /// </summary>
        bool CorsEnabled { get; }

        /// <summary>
        /// The `Access-Control-Allow-Origin` response header indicates
        /// whether the response can be shared with requesting code from the given origin.
        /// The value may list any number of origins, separated by commas.
        /// The literal value "*" can be specified, as a wildcard;
        /// the value tells browsers to allow requesting code from any origin to access the resource.
        /// </summary>
        string CorsAllowOrigins { get; }

        /// <summary>
        /// The `Access-Control-Allow-Headers` response header is used in response
        /// to a preflight request which includes the `Access-Control-Request-Headers`
        /// to indicate which HTTP headers can be used during the actual request.
        /// The header may list any number of headers, separated by commas.
        /// </summary>
        string CorsAllowHeaders { get; }

        /// <summary>
        /// The `Access-Control-Allow-Methods` response header specifies
        /// the method or methods allowed when accessing the resource in
        /// response to a preflight request. Comma-delimited list of the allowed
        /// HTTP request methods (GET, POST, PUT, DELETE ...).
        /// </summary>
        string CorsAllowMethods { get; }

        /// <summary>
        /// The Access-Control-Max-Age response header indicates how long the
        /// results of a preflight request (that is the information contained in
        /// the Access-Control-Allow-Methods and Access-Control-Allow-Headers headers) can be cached.
        /// A value of -1 will disable caching, requiring a preflight OPTIONS check for all calls.
        /// </summary>
        int CorsMaxAge { get; }
    }
}
