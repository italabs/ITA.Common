using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Threading;

namespace ITA.Common.Host.PowerShell
{
    [Cmdlet("Wait", "ServiceHost", DefaultParameterSetName = "Default")]
    [OutputType(typeof(IControlService))]
    public class WaitForStatusCommand : PSCmdlet
    {
        const int DefaultFrequency = 500;

        private ControlClient _inputObject;
        private EComponentStatus _status;
        private TimeSpan _timeout;
        private int? _frequency;

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
           Position = 1,
           ParameterSetName = "Default",
           Mandatory = true,
           ValueFromPipeline = false,
           ValueFromPipelineByPropertyName = false,
           HelpMessage = "The servicehost status to fait for.")]
        [ValidateNotNullOrEmpty]
        public EComponentStatus Status
        {
            get { return this._status; }
            set { this._status = value; }
        }

        [Parameter(
           Position = 2,
           ParameterSetName = "Default",
           Mandatory = true,
           ValueFromPipeline = false,
           ValueFromPipelineByPropertyName = false,
           HelpMessage = "The timeout to for for in milliseconds.")]
        [ValidateRange(0,int.MaxValue)]
        public int Timeout
        {
            get { return (int)this._timeout.TotalMilliseconds; }
            set { this._timeout = TimeSpan.FromMilliseconds((double)value); }
        }

        [Parameter(
           Position = 3,
           ParameterSetName = "Default",
           Mandatory = false,
           ValueFromPipeline = false,
           ValueFromPipelineByPropertyName = false,
           HelpMessage = "The polling frequency. Default is 500 msec.")]
        [ValidateRange(10, 1000)]
        [Alias("Interval")]
        public int Frequency
        {
            get { return this._frequency.Value; }
            set { this._frequency = value; }
        }

        protected override void ProcessRecord()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (_inputObject.ServiceStatus != _status)
            {
                Thread.Sleep(_frequency.HasValue ? _frequency.Value : DefaultFrequency);

                if (watch.Elapsed >= _timeout)
                {
                    ThrowTerminatingError(
                        new ErrorRecord(
                            new TimeoutException(),
                            "WaitTimeout",
                            ErrorCategory.OperationTimeout,
                            null));
                    return;
                }
            }

            watch.Stop();
            WriteObject((IControlService)_inputObject);
        }
    }
}
