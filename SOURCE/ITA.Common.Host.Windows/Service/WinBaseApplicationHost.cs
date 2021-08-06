using System;
using System.Security.Principal;
using ITA.Common.Host.Interfaces;
using log4net;

namespace ITA.Common.Host.Windows
{
    /// <summary>
    /// Base host implementation class for Windows
    /// </summary>
    public class WinBaseApplicationHost : BaseApplicationHost
    {
        private ILog _logger = Log4NetItaHelper.GetLogger(typeof(WinBaseApplicationHost).Name);

        private void OnError(IComponent Source, Exception x)
        {
            Console.WriteLine("[{0}] Error has occurred: {1}", Source == null ? "Unknown component" : Source.Name,
                x.Message);
            if (x.InnerException != null)
            {
                OnError(Source, x.InnerException);
            }
        }

        public override void RunDebug()
        {
            Component.OnError += OnError;

            if (!WinInterops.AttachConsole(ATTACH_PARENT_PROCESS))
            {
                WinInterops.AllocConsole();
            }

            Console.WriteLine("Account info: " + WindowsIdentity.GetCurrent().Name + "  " +
                              WindowsIdentity.GetCurrent().User.Value);
            Console.WriteLine(Copyright);

            Console.WriteLine("Instance name: {0}", InstanceName);

            Console.WriteLine("Initializing...");
            Start();
            Console.WriteLine("Ready.");

            Console.WriteLine();
            Console.WriteLine("Press ENTER to shut down...");
            Console.ReadLine();

            Console.WriteLine("Shutting down...");
            Stop();
            Console.WriteLine("Stopped.");
        }
    }
}
