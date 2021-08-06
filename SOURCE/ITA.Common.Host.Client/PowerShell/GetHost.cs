using System;
using System.Management.Automation;

namespace ITA.Common.Host.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "ServiceHost", DefaultParameterSetName = "Default")]
    [OutputType(typeof(IControlService))]
    public class GetHostCommand : PSCmdlet
    {
        const string UrlTemplate = "net.pipe://localhost/{0}/{1}";
        const string DefaultInstance = "default";

        private string _url;
        private string _instance;
        private string _service;

        [Parameter(
           Position = 0,
           Mandatory = true,
           ParameterSetName = "Default",
           ValueFromPipeline = true,
           ValueFromPipelineByPropertyName = true,
           HelpMessage = "The name of the servicehost to connect to.")]
        [Alias("ServiceHost", "ServiceName", "Name")]
        [ValidateNotNullOrEmpty]
        public string Service
        {
            get { return _service; }
            set { _service = value; }
        }

        [Parameter(
           Position = 1,
           Mandatory = false,
           ParameterSetName = "Default",
           ValueFromPipeline = false,
           ValueFromPipelineByPropertyName = false,
           HelpMessage = "The name of the servicehost instance to connect to. Default instance name is 'default'")]
        [Alias("InstanceName")]
        [ValidateNotNullOrEmpty]
        public string Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        [Parameter(
           Position = 0,
           ParameterSetName = "URL",
           ValueFromPipeline = true,
           ValueFromPipelineByPropertyName = true,
           HelpMessage = "Complete servicehost url in the following format: 'net.pipe://localhost/<service host name>/<instance name>'")]
        [ValidateNotNullOrEmpty]
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        protected override void ProcessRecord()
        {
            string url = _url ?? string.Format(UrlTemplate, _service, _instance ?? DefaultInstance);

            ControlClient _client = new ControlClient(url);
            if (!_client.Connected)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new Exception("Unable to connect to servicehost"),
                        "UnableToConnect",
                        ErrorCategory.ResourceUnavailable,
                        null));
                return;            
            }
            
            WriteObject((IControlService)_client);
        }
    }
}

