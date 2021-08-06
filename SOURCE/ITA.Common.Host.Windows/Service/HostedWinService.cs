using System;
using System.Threading;
using System.Threading.Tasks;
using ITA.Common.Host.Interfaces;
using ITA.Common.Tracing;
using log4net;
using Microsoft.Extensions.Hosting;

namespace ITA.Common.Host.Windows
{
    /// <summary>
    /// Hosted service implementation for Windows
    /// </summary>
    [Trace]
    public class HostedWinService : IHostedService, IDebugRunner
    {
        private static ILog _logger = LogManager.GetLogger(typeof(CommonHostedService));
        private IApplicationLifetime _appLifetime;
        private IApplicationHost _applicationHost;
        private CommonWinService _winService;

        public HostedWinService(
            IApplicationLifetime appLifetime,
            IApplicationHost applicationHost)
        {
            this._appLifetime = appLifetime;
            this._applicationHost = applicationHost;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Debug("StartAsync method called.");

            try
            {
                this._appLifetime.ApplicationStarted.Register(OnStarted);
                this._appLifetime.ApplicationStopping.Register(OnStopping);
                this._appLifetime.ApplicationStopped.Register(OnStopped);

                return Task.CompletedTask;
            }
            catch (Exception exc)
            {
                _logger.Error($"An error occurred on {nameof(HostedWinService)} StartAsync", exc);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Debug("StopAsync method called.");

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            try
            {
                _winService = new CommonWinService(_applicationHost, _appLifetime);

                _winService.Run();
            }
            catch (Exception exc)
            {
                _logger.Error($"An error occurred on {nameof(HostedWinService)} OnStarted", exc);
                throw;
            }
        }

        private void OnStopping()
        {
        }

        private void OnStopped()
        {
            try
            {
                _winService.Stop();
            }
            catch (Exception exc)
            {
                _logger.Error($"An error occurred on {nameof(HostedWinService)} OnStopped", exc);
                throw;
            }
        }

        public void RunDebug()
        {
            try
            {
                _winService.RunDebug();
            }
            catch (Exception exc)
            {
                _logger.Error($"An error occurred on {nameof(HostedWinService)} RunDebug", exc);
                throw;
            }
        }
    }
}
