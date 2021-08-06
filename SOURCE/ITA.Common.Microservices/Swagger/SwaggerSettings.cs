namespace ITA.Common.Microservices.Swagger
{
    public sealed class SwaggerSettings
    {
        public bool Enabled { get; set; }

        public string ApiVersion { get; set; }

        public string ApiTitle { get; set; }

        public string ApiDescription { get; set; }

        public string LicenseName { get; set; }

        public string LicenseUrl { get; set; }

        public bool UseBasicAuthentication { get; set; }

        public bool UseBearerAuthentication { get; set; }

        public string[] XmlCommentsFileNames { get; set; }

        public string SwaggerJsonUrl { get; set; }

        public bool SwaggerVersion2 { get; set; }
    }
}
