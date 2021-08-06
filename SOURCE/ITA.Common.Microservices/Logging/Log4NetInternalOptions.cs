namespace ITA.Common.Microservices.Logging
{
    public sealed class Log4NetInternalOptions
    {
        public bool Enabled { get; set; }

        public bool Debug { get; set; }

        public string FileName { get; set; }
    }
}