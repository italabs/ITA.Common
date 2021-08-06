using System.Runtime.InteropServices;

namespace ITA.Common.Host.Failover
{
    [Guid("C0977E94-BB4D-4718-B696-2E336B0E7461")]
    public interface IFailoverClusterResource
    {
        [DispId(1)]
        string Url { get; set; }

        [DispId(2)]
        string WindowsService { get; set; }

        [DispId(3)]
        int WaitTimeout { get; set; }

        [DispId(4)]
        bool Online();

        [DispId(5)]
        bool Offline();

        [DispId(6)]
        bool LooksAlive();

        [DispId(7)]
        bool IsAlive();

        [DispId(8)]
        bool Open();

        [DispId(9)]
        bool Close();

        [DispId(10)]
        bool Terminate();
    }
}