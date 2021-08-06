using System;
using System.ServiceProcess;
using ITA.Common.Host.Interfaces;
using log4net;
using Microsoft.Extensions.Hosting;

namespace ITA.Common.Host.Windows
{
    /// <summary>
    /// WinService (services.msc) class wrapper
    /// </summary>
    public class CommonWinService : ServiceBase, IDebugRunner
    {
        private ILog _logger = Log4NetItaHelper.GetLogger(typeof(CommonWinService).Name);

        private readonly IApplicationHost _commonApplicationHost;
        private readonly IApplicationLifetime _applicationLifetime;
        protected SYSTEM_POWER_STATUS m_PowerStatus;

        public CommonWinService(IApplicationHost commonApplicationHost, IApplicationLifetime applicationLifetime)
        {
            _commonApplicationHost = commonApplicationHost;
            _applicationLifetime = applicationLifetime;
            CanHandlePowerEvent = true;
            CanPauseAndContinue = false;
            CanShutdown = true;
            AutoLog = false;

            m_PowerStatus = new SYSTEM_POWER_STATUS();

            if (!WinInterops.GetSystemPowerStatus(ref m_PowerStatus))
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error quering power status: %s", WinInterops.GetLastError()));
            }

            _commonApplicationHost.UpdateFieldsHandler += CommonApplicationHostOnUpdateFieldsHandler;
        }

        #region ServiceBase Overridables

        protected override void OnStart(string[] args)
        {
            _logger.Debug("OnStart winservice");
            base.OnStart(args);
            _commonApplicationHost.Start();
        }

        protected override void OnStop()
        {
            _logger.Debug("OnStop winservice");
            base.OnStop();
            _applicationLifetime?.StopApplication();
            _commonApplicationHost.Stop();
        }

        protected override void OnShutdown()
        {
            _logger.Debug("OnShutdown winservice");
            base.OnShutdown();
            _commonApplicationHost.Stop();
        }

        protected override void OnContinue()
        {
            _logger.Debug("OnContinue winservice");
            base.OnContinue();
            _commonApplicationHost.Continue();
        }

        protected override void OnPause()
        {
            _logger.Debug("OnPause winservice");
            base.OnPause();
            _commonApplicationHost.Pause();
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            _logger.Debug("OnPowerEvent winservice");
            var Power = _commonApplicationHost.Component as IPowerEventHandler;
            if (Power != null)
            {
                switch (powerStatus)
                {
                    case PowerBroadcastStatus.BatteryLow:
                        {
                            Power.OnBatteryLow();
                            break;
                        }
                    case PowerBroadcastStatus.PowerStatusChange:
                        {
                            var ps = new SYSTEM_POWER_STATUS();
                            if (!WinInterops.GetSystemPowerStatus(ref ps))
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    string.Format("Power event: Error quering power status: %s", WinInterops.GetLastError()));
                                break;
                            }

                            if (ps.ACLineStatus == 1 && m_PowerStatus.ACLineStatus == 0)
                            {
                                // switched to AC
                                System.Diagnostics.Debug.WriteLine(string.Format("Power event: Switched to AC"));
                                Power.OnAC();
                            }
                            else if (ps.ACLineStatus == 0 && m_PowerStatus.ACLineStatus == 1)
                            {
                                // Switched to battery
                                System.Diagnostics.Debug.WriteLine(string.Format("Power event: Switched to battery"));
                                Power.OnBattery();
                            }
                            else
                            {
                                // who the hell knows
                            }

                            m_PowerStatus = ps; // remember new status
                            break;
                        }
                    case PowerBroadcastStatus.QuerySuspend:
                        {
                            return Power.IsSuspendEnabled();
                        }
                    case PowerBroadcastStatus.ResumeAutomatic:
                        {
                            Power.OnResume(EPowerResumeType.Automatic);
                            break;
                        }
                    case PowerBroadcastStatus.ResumeCritical:
                        {
                            Power.OnResume(EPowerResumeType.Critical);
                            break;
                        }
                    case PowerBroadcastStatus.ResumeSuspend:
                        {
                            Power.OnResume(EPowerResumeType.User);
                            break;
                        }
                    case PowerBroadcastStatus.Suspend:
                        {
                            Power.OnSuspend();
                            break;
                        }
                    case PowerBroadcastStatus.QuerySuspendFailed:
                        {
                            Power.OnSuspendDenied();
                            break;
                        }
                    case PowerBroadcastStatus.OemEvent:
                        break; //ignore
                }
            }

            return base.OnPowerEvent(powerStatus);
        }

        #endregion

        private void UpdateFields()
        {
            if (_commonApplicationHost.InstanceName != BaseApplicationHost.cDefaultInstance)
            {
                base.ServiceName = _commonApplicationHost.ServiceName + "_" + _commonApplicationHost.InstanceName;
            }
            else
            {
                base.ServiceName = _commonApplicationHost.ServiceName;
            }
        }

        private void CommonApplicationHostOnUpdateFieldsHandler(object sender, EventArgs eventArgs)
        {
            UpdateFields();
        }

        public void RunDebug()
        {
            _commonApplicationHost.RunDebug();
        }

        public void Run()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] {this};
            Run(ServicesToRun);
        }
    }
    
}
