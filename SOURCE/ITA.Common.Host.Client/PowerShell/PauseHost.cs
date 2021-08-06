using System.Management.Automation;

namespace ITA.Common.Host.PowerShell
{
    [Cmdlet("Suspend", "ServiceHost", DefaultParameterSetName = "Default")]
    [OutputType(typeof(IControlService))]
    public class PauseHostCommand : PSCmdlet
    {
        private ControlClient _inputObject;

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

        protected override void ProcessRecord()
        {
            _inputObject.Pause();
            WriteObject((IControlService)_inputObject);
        }
    }
}
