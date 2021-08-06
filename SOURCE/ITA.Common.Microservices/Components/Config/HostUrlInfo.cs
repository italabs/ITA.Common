using ITA.Common.Microservices.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;

namespace ITA.Common.Microservices.Components
{
    public class HostUrlInfo
    {
        public HostUrlInfo(string hostUrl)
        {
            Guard.NotNullOrWhiteSpace(hostUrl, nameof(hostUrl));

            UriHelper.FromAbsolute(hostUrl, out var schema, out var host, out var path, out var query, out var fragment);

            var uriParsed = Uri.TryCreate(hostUrl, UriKind.Absolute, out var uri);

            Schema = schema;
            Host = uriParsed ? new HostString(uri.Host, uri.Port) : host;
            Path = path;
            Query = query;
            Fragment = fragment;
        }

        public string Schema { get; }

        public HostString Host { get; }

        public PathString Path { get; }

        public QueryString Query { get; }

        public FragmentString Fragment { get; }
    }
}