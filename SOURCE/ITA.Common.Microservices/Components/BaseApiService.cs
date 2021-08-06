using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ITA.Common.Microservices.Exceptions;
using ITA.Common.Microservices.Helpers;
using ITA.Common.Microservices.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Components
{
    /// <summary>
    /// Components contains Web API host.
    /// </summary>
    public abstract class BaseApiService
    {
        private CancellationTokenSource _tokenSource;
        private Task _webHostTask;
        private IWebHost _webHost;
        private string _webHostEndpointAddresses;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        protected readonly IConfiguration _componentConfiguration;

        public abstract string Name { get; }

        protected virtual IAppliedLoggerRegistrar AppliedLogger { get; } = new Log4NetAppliedLoggerRegistrar();

        protected BaseApiService(
            IConfiguration configuration,
            ILogger logger)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.NotNull(logger, nameof(logger));

            _configuration = configuration;
            _componentConfiguration = configuration.GetSection(Name);
            _logger = logger;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting component {0}... ", Name);

            _tokenSource = new CancellationTokenSource();

            _logger.LogDebug("Starting web server...");

            _webHost = CreateWebHost();

            _webHostTask = _webHost.RunAsync(_tokenSource.Token);

            if (_webHostTask.Exception != null)
            {
                throw new ITAMicroservicesLocaleException(
                    ITAMicroservicesLocaleException.E_MICROSERVICE_COMPONENT_INITIALIZE_ERROR,
                    _webHostTask.Exception,
                    Name);
            }

            var serverAddresses = _webHost?.ServerFeatures?.Get<IServerAddressesFeature>();
            if (serverAddresses != null)
            {
                _webHostEndpointAddresses = string.Join(", ", serverAddresses.Addresses);
            }

            _logger.LogDebug("Web server running at {0}...", _webHostEndpointAddresses);

            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                _tokenSource = null;
            }

            if (_webHost != null)
            {
                _webHost.Dispose();
                _webHost = null;
            }

            if (_webHostTask != null && (_webHostTask.Status == TaskStatus.RanToCompletion
                                         || _webHostTask.Status == TaskStatus.Canceled
                                         || _webHostTask.Status == TaskStatus.Faulted))
            {
                _webHostTask.Dispose();
                _webHostTask = null;
            }

            _logger.LogDebug("Web server stopped at {0}", _webHostEndpointAddresses);

            return Task.CompletedTask;
        }

        protected virtual void ConfigureBuilder(IWebHostBuilder builder)
        {
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        protected virtual IWebHost CreateWebHost()
        {
            _logger.LogDebug($"Creating web hosting for component '{Name}'");

            var builder = WebHost.CreateDefaultBuilder(null)
                .UseContentRoot(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location)))
                .ConfigureServices(services =>
                {
                    services.AddSingleton(_configuration);
                    ConfigureServices(services);
                })
                .ConfigureAppConfiguration(config => config.AddConfiguration(_componentConfiguration))
                .ConfigureKestrel(options => options.Configure(_componentConfiguration.GetSection("Kestrel")))
                .ConfigureLogging(logging => logging
                    .AddConfiguration(_configuration.GetSection("Logging"))
                    .ClearProviders()
                    .AddApplied(AppliedLogger));

            ConfigureBuilder(builder);

            return builder.Build();
        }
    }
}