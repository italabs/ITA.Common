using System;
using System.Diagnostics;
using System.Reflection;
using log4net;

namespace ITA.Common.Tracing
{
    public static class LogHelper
    {
        /// <summary>
        /// Updates logger name based on <paramref name="method"/> object.
        /// </summary>
        /// <param name="loggerName">If it is not null then method does nothing.</param>
        /// <param name="method">Method information from which type name is extracted and returned as logger name.</param>
        public static void UpdateLoggerName(ref string loggerName, MethodBase method)
        {
            if (!String.IsNullOrEmpty(loggerName))
            {
                Trace.TraceInformation("loggerName != IsNullOrEmpty, but == {0}", loggerName);
                return;
            }

            if (method != null && method.DeclaringType != null)
            {
                loggerName = method.DeclaringType.Name;
                Trace.TraceInformation("loggerName determined successfully: {0}", loggerName);
            }
            else
            {
                try
                {
                    Trace.TraceError("Either method or method.DeclaringType are null!");

                    loggerName = new StackFrame(1).GetMethod().DeclaringType.Name;
                }
                catch (Exception e)
                {
                    Trace.TraceError("Can't get type name from stack: {0}", e.Message);

                    loggerName = "Possibly_fody_attribute";
                }
            }
        }

        /// <summary>
        /// Gets logger instance based on <paramref name="loggerName"/>.
        /// </summary>
        /// <param name="loggerName">Desired name of the logger.</param>
        /// <param name="method">Used for diagnostics if getting logger failed.</param>
        /// <returns>Logger instance.</returns>
        public static ILog GetLogger(string loggerName, MethodBase method)
        {
            try
            {
                ILog logger = null;

                if (loggerName != null)
                    logger = Log4NetItaHelper.GetLogger(loggerName);

                if (logger == null)
                {
                    Trace.WriteLine(String.Format("logger is null in '{0}' class. Method name is {1}. loggerName is '{2}'.", 
                                        method.DeclaringType, method.Name, loggerName));
                }

                return logger;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return null;
            }
        }
    }
}
