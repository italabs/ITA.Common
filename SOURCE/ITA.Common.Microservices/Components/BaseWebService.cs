using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ITA.Common.Microservices.Exceptions;
using ITA.Common.Microservices.Logging;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Components
{
    public abstract class BaseWebService
    {
        private CancellationTokenSource _tokenSource;
        private Task _webHostTask;
        private IWebHost _webHost;
        private string _webHostEndpointAddresses;
        private readonly IConfiguration _configuration;
        protected readonly IConfiguration _componentConfiguration;

        public abstract string Name { get; }

        protected ILog Logger { get; }

        protected BaseWebService(IConfiguration configuration)
        {
            _configuration = configuration;
            _componentConfiguration = configuration.GetSection(Name);

            Logger = LogManager.GetLogger(GetType());
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.InfoFormat("Starting component {0}... ", Name);

            _tokenSource = new CancellationTokenSource();

            Logger.Debug("Starting web server...");

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

            Logger.DebugFormat("Web server running at {0}...", _webHostEndpointAddresses);

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

            Logger.DebugFormat("Web server stopped at {0}", _webHostEndpointAddresses);

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
            Logger.Debug($"Creating web hosting for component '{Name}'");

            var builder = WebHost.CreateDefaultBuilder(null)
                .UseContentRoot(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location)))
                .ConfigureLogging(logging =>
                {
                    logging.AddLog4Net(_configuration, LogLevel.Debug);
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton(_configuration);

                    ConfigureServices(services);
                })
                .ConfigureAppConfiguration(config =>
                {
                    config.AddConfiguration(_componentConfiguration);
                })
                .ConfigureKestrel(options =>
                {
                    options.Configure(_componentConfiguration.GetSection("Kestrel"));
                });

            ConfigureBuilder(builder);

            return builder.Build();
        }
    }
}