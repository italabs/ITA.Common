using System.Threading;
using System.Threading.Tasks;

namespace ITA.Common.Microservices.Components
{
    public interface IHostComponent
    {
        string Name { get; }

        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}