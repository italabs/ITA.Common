using System.Reflection;
using log4net;

namespace ITA.Common
{
    public static class Log4NetItaHelper
    {
        public static ILog GetLogger(string name)
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return LogManager.GetLogger(assembly, name);
        }
    }
}
