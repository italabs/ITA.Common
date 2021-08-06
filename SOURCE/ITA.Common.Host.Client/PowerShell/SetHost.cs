using System.Management.Automation;

namespace ITA.Common.Host.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "ServiceHost", DefaultParameterSetName = "Default")]
    [OutputType(typeof(IControlService))]
    public class SetHostCommand : PSCmdlet
    {
        private ControlClient _inputObject;

        private bool? _autoStart;
        private bool? _enableSuspend;
        private EOnPowerEventBehaviour? _onBatteryAction;
        private EOnPowerEventBehaviour? _onLowBatteryAction;
        private EOnPowerEventBehaviour? _onSuspendAction;

        [Parameter(
           Position=0,
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
           ParameterSetName = "Default",
           Mandatory = false,
           ValueFromPipeline = false,
           ValueFromPipelineByPropertyName = false,
           HelpMessage = "Specifies whether servicehost starts logically immediately upon start of the windows service.")]
        public bool AutoStart
        {
            get { return _autoStart.Value; }
            set { this._autoStart = value; }
        }

        [Parameter(
           ParameterSetName = "Default",
           Mandatory = false,
           ValueFromPipeline = false,
           ValueFromPipelineByPropertyName = false,
           HelpMessage = "Specifies whether servicehost prevents system from being suspended.")]
        public bool EnableSuspend
        {
            get { return _enableSuspend.Value; }
            set { this._enableSuspend = value; }
        }

        [Parameter(
           ParameterSetName = "Default",
           Mandatory = false,
           ValueFromPipeline = false,
           ValueFromPipelineByPropertyName = false,
           HelpMessage = "Specifies servicehost action when power supply swithed to battery.")]
        public EOnPowerEventBehaviour OnBatteryAction
        {
            get { return _onBatteryAction.Value; }
            set { this._onBatteryAction = value; }
        }

        [Parameter(
           ParameterSetName = "Default",
           Mandatory = false,
           ValueFromPipeline = false,
           ValueFromPipelineByPropertyName = false,
           HelpMessage = "Specifies servicehost action when battery power supply is running low.")]
        public EOnPowerEventBehaviour OnLowBatteryAction
        {
            get { return _onLowBatteryAction.Value; }
            set { this._onLowBatteryAction = value; }
        }

        [Parameter(
           ParameterSetName = "Default",
           Mandatory = false,
           ValueFromPipeline = false,
           ValueFromPipelineByPropertyName = false,
           HelpMessage = "Specifies servicehost action when operating system is about to be suspended.")]
        public EOnPowerEventBehaviour OnSuspendAction
        {
            get { return _onSuspendAction.Value; }
            set { this._onSuspendAction = value; }
        }

        protected override void ProcessRecord()
        {
            if (_autoStart.HasValue) _inputObject.AutoStart = _autoStart.Value;
            if (_enableSuspend.HasValue) _inputObject.EnableSuspend = _enableSuspend.Value;
            if (_onBatteryAction.HasValue) _inputObject.OnBatteryAction = _onBatteryAction.Value;
            if (_onLowBatteryAction.HasValue) _inputObject.OnLowBatteryAction = _onLowBatteryAction.Value;
            if (_onSuspendAction.HasValue) _inputObject.OnSuspendAction = _onSuspendAction.Value;

            WriteObject((IControlService)_inputObject);
        }
    }
}
