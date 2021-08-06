using System;
using System.Reflection;

using log4net;
using log4net.Core;

namespace ITA.Common.Tracing
{
    /// <summary>
    /// Log4Net extentions
    /// </summary>
    public static class ILogExtentions
    {
        public static bool IsVerboseEnabled(this ILog log)
        {
            return log.Logger.IsEnabledFor(Level.Verbose);
        }

        public static void Verbose(this ILog log, string Message)
        {
            log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, Level.Verbose, Message, null);
        }

        public static void VerboseFormat(this ILog log, string Format, params object[] Params)
        {
            log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, Level.Verbose, string.Format(Format, Params), null);
        }

        public static void VerboseFormat(this ILog log, IFormatProvider Provider, string Format, params object[] Params)
        {
            log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType, Level.Verbose, string.Format(Provider, Format, Params), null);
        }
    }
}
