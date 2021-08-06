using System;
using System.Threading;
using System.Threading.Tasks;
using ITA.Common.Host.Interfaces;
using ITA.Common.Tracing;
using log4net;
using Microsoft.Extensions.Hosting;

namespace ITA.Common.Host
{
    /// <summary>
    /// Cross-platformed hosted service implementation
    /// </summary>
    [Trace]
    public class CommonHostedService : IHostedService, IDebugRunner
    {
        private static ILog _logger = LogManager.GetLogger(typeof(CommonHostedService));
        IApplicationLifetime appLifetime;
        IApplicationHost _applicationHost;
        public CommonHostedService(
            IApplicationLifetime appLifetime,
            IApplicationHost applicationHost)
        {
            this.appLifetime = appLifetime;
            this._applicationHost = applicationHost;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Debug($"{nameof(CommonHostedService)}: StartAsync method called.");

            try
            {
                this.appLifetime.ApplicationStarted.Register(OnStarted);
                this.appLifetime.ApplicationStopping.Register(OnStopping);
                this.appLifetime.ApplicationStopped.Register(OnStopped);

                return Task.CompletedTask;
            }
            catch (Exception exc)
            {
                _logger.Error($"An error occurred on {nameof(CommonHostedService)} StartAsync", exc);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Debug($"{nameof(CommonHostedService)}: StopAsync method called.");

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            try
            {
                this._applicationHost.Start();
            }
            catch (Exception exc)
            {
                _logger.Error($"An error occurred on {nameof(CommonHostedService)} OnStarted", exc);
                throw;
            }
        }

        private void OnStopping()
        {
            try
            {
                this._applicationHost.Stop();
            }
            catch (Exception exc)
            {
                _logger.Error($"An error occurred on {nameof(CommonHostedService)} OnStopping", exc);
                throw;
            }
        }

        private void OnStopped()
        {
        }

        public void RunDebug()
        {
            try
            {
                this._applicationHost.RunDebug();
            }
            catch (Exception exc)
            {
                _logger.Error($"An error occurred on {nameof(CommonHostedService)} RunDebug", exc);
                throw;
            }
        }
    }
}
