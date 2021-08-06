using System.Diagnostics;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace ConsoleApp
{
    public static class Log4NetHelper
    {
        public static void ConfigureWithProcessId()
        {
            log4net.GlobalContext.Properties["ProcessId"] = Process.GetCurrentProcess().Id;

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var fi = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Log4NetHelper)).Location), "log4net.config"));
            XmlConfigurator.Configure(logRepository, fi);
        }
    }

}
