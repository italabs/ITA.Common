using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ITA.Common.Microservices.Exceptions;
using ITA.Common.Microservices.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Components
{
    public class BaseComponentHost : IHostedService
    {
        private readonly IHostComponent[] _components;
        private readonly ILogger<BaseComponentHost> _logger;

        public BaseComponentHost(
            IHostComponentProvider componentProvider,
            ILogger<BaseComponentHost> logger)
        {
            Guard.NotNull(componentProvider, nameof(componentProvider));
            Guard.NotNull(logger, nameof(logger));

            var components = componentProvider.GetComponents()?.ToArray();

            Guard.NotNullOrEmpty(components, nameof(components));

            _components = components;
            _logger = logger;
        }

        protected virtual void PrepareHostStart(IReadOnlyCollection<IHostComponent> components)
        {
        }


        #region Implementation of IHostedService

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting ConnectorHost");

            try
            {
                PrepareHostStart(_components);

                foreach (var component in _components)
                {
                    await component.StartAsync(cancellationToken);
                }
            }
            catch (Exception exception)
            {
                var ex = exception as ITAMicroservicesLocaleException ?? new ITAMicroservicesLocaleException(ITAMicroservicesLocaleException.E_HOST_START_ERROR, exception);

                _logger.LogError(ex, $"Failed to start.");

                throw ex;
            }

            _logger.LogInformation("ConnectorHost started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping ConnectorHost");

            var isErrorOccured = false;

            foreach (var component in _components.Reverse())
            {
                try
                {
                    await component.StopAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    var exc = new ITAMicroservicesLocaleException(ITAMicroservicesLocaleException.E_HOST_COMPONENT_STOP_ERROR, ex, component.Name);

                    isErrorOccured = true;

                    _logger.LogError(exc, $"Failed to stop component '{component.Name}'.");
                }
            }
            if (isErrorOccured)
            {
                _logger.LogError($"An error occurred while stopping one or more components.");
            }

            _logger.LogInformation("ConnectorHost stopped");
        }

        #endregion
    }
}