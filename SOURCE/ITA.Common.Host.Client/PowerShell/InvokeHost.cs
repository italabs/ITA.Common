using System;
using System.Management.Automation;

namespace ITA.Common.Host.PowerShell
{
    [Cmdlet("Invoke", "ServiceHost", DefaultParameterSetName = "Default")]
    [OutputType(typeof(IControlService))]
    public class InvokeHostCommand : PSCmdlet
    {
        private ControlClient _inputObject;
        private string _action;

        [Parameter(
           Position = 0,
           ParameterSetName = "Default",
           Mandatory = true,
           ValueFromPipeline = true,
           ValueFromPipelineByPropertyName = true,
           HelpMessage = "The instance of servicehost to work with.")]
        [Alias("InputObject")]
        public ControlClient Input
        {
            get { return this._inputObject; }
            set { this._inputObject = value; }
        }

        [Parameter(
           Position = 0,
           ParameterSetName = "Default",
           Mandatory = true,
           ValueFromPipeline = false,
           ValueFromPipelineByPropertyName = false)]
        [Alias("InputObject")]
        [ValidateNotNullOrEmpty]
        public string Action
        {
            get { return this._action; }
            set { this._action = value; }
        }

        protected override void ProcessRecord()
        {
            ThrowTerminatingError(
                new ErrorRecord(
                    new NotImplementedException(""),
                    "UnableToInvoke",
                    ErrorCategory.NotImplemented,
                    null));
            return;            
        }
    }
}
