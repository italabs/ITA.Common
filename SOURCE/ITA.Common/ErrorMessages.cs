using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using log4net;

namespace ITA.Common
{
    [Obsolete("Don't use this helper class. Use new version - ResourceMappingInfo")]
    public class ExceptionInfo
    {
        public Assembly ResourceAssembly;
        public string ResourceName;
    }

    /// <summary>
    /// ErrorMessages class
    /// </summary>
    [Obsolete("Don't use this class. Use new version - LocaleMessages")]
    public class ErrorMessages
    {
        private static readonly ILog logger = Log4NetItaHelper.GetLogger( typeof (ErrorMessages).Name);

        private readonly Hashtable m_ExceptionTypeTable;

        public ErrorMessages()
        {
            m_ExceptionTypeTable = new Hashtable();
        }

        [Obsolete("Don't use this class. Use new version - LocaleMessages")]
        public void RegisterAssembly(Assembly A)
        {
            logger.Debug("RegisterAssembly() begin.");
            try
            {
                object[] attr = A.GetCustomAttributes(typeof (AssemblyResourceInfoAttribute), false);

                foreach (AssemblyResourceInfoAttribute attribut in attr)
                {
                    var info = new ExceptionInfo();

                    info.ResourceAssembly = A;
                    info.ResourceName = attribut.ResourceName;

                    logger.DebugFormat("info.ResourceAssembly:{0}", info.ResourceAssembly);
                    logger.DebugFormat("info.ResourceName:{0}", info.ResourceName);

                    if (attribut.ExceptionType.FullName != null)
                    {
                        if (!m_ExceptionTypeTable.Contains(attribut.ExceptionType.FullName))
                        {
                            logger.DebugFormat("m_ExceptionTypeTable doesn't contain:{0}", attribut.ExceptionType.FullName);
                            m_ExceptionTypeTable.Add(attribut.ExceptionType.FullName, info);
                        }
                    }
                }
            }
            finally
            {
                logger.Debug("RegisterAssembly() end.");
            }
        }

        [Obsolete("Don't use this class. Use new version - LocaleMessages")]
        public string GetErrorString(string ExceptionTypeFullName, string MessageID, CultureInfo ci, params object[] Args)
        {
            string strMessage = null;
            logger.Debug("GetErrorString() begin.");

            logger.DebugFormat("ExceptionTypeFullName:{0}", ExceptionTypeFullName);
            logger.DebugFormat("MessageID:{0}", MessageID);
            logger.DebugFormat("ci:{0}", ci);
            if (Args != null)
            {
                logger.Debug("Args:");
                foreach (object o in Args)
                {
                    logger.DebugFormat("arg:{0}", o);
                }
            }

            try
            {
                if (m_ExceptionTypeTable.Contains(ExceptionTypeFullName))
                {
                    logger.DebugFormat("m_ExceptionTypeTable.Contains {0}.", ExceptionTypeFullName);

                    var info = (ExceptionInfo)m_ExceptionTypeTable[ExceptionTypeFullName];

                    logger.DebugFormat("ExceptionInfo.ResourceName:{0}", info.ResourceName);
                    logger.DebugFormat("ExceptionInfo.ResourceAssembly:{0}", info.ResourceAssembly);

                    var rm = new ResourceManager(info.ResourceName, info.ResourceAssembly);
                    strMessage = rm.GetString(MessageID, ci);

                    logger.DebugFormat("strMessage:{0}", strMessage);

                    if (null != strMessage && null != Args)
                    {
                        try
                        {
                            logger.DebugFormat("formatting strMessage:{0}", strMessage);
                            // trying to format message
                            strMessage = string.Format(strMessage, Args);
                            logger.DebugFormat("formatted strMessage:{0}", strMessage);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e);
                            // unable to format message
                            strMessage = string.Format("{0} (unable to format message)", strMessage);
                        }
                    }
                }
            }
            catch (MissingManifestResourceException mre)
            {
                // no resorces found;
                logger.Error(mre);
            }
            catch (InvalidOperationException ioe)
            {
                // is not a string
                logger.Error(ioe);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                logger.Debug("GetErrorString() end.");
            }

            return strMessage;
        }

        [Obsolete("Don't use this class. Use new version - LocaleMessages")]
        public string GetErrorString(Type ExceptionType, int MessageID, CultureInfo ci, params object[] Args)
        {
            return GetErrorString(ExceptionType.FullName, string.Format("{0}", MessageID), ci, Args);
        }

        [Obsolete("Don't use this class. Use new version - LocaleMessages")]
        public string GetErrorString(Type ExceptionType, string MessageID, params object[] Args)
        {
            return GetErrorString(ExceptionType.FullName, MessageID, Thread.CurrentThread.CurrentUICulture, Args);
        }

        [Obsolete("Don't use this class. Use new version - LocaleMessages")]
        public string GetErrorString(Type ExceptionType, int MessageID, params object[] Args)
        {
            return GetErrorString(ExceptionType.FullName, string.Format("{0}", MessageID), Thread.CurrentThread.CurrentUICulture, Args);
        }

        [Obsolete("Don't use this class. Use new version - LocaleMessages")]
        public string GetErrorString(string ExceptionTypeFullName, int MessageID, CultureInfo ci, params object[] Args)
        {
            return GetErrorString(ExceptionTypeFullName, string.Format("{0}", MessageID), ci, Args);
        }

        [Obsolete("Don't use this class. Use new version - LocaleMessages")]
        public string GetErrorString(string ExceptionTypeFullName, string MessageID, params object[] Args)
        {
            return GetErrorString(ExceptionTypeFullName, MessageID, Thread.CurrentThread.CurrentUICulture, Args);
        }

        [Obsolete("Don't use this class. Use new version - LocaleMessages")]
        public string GetErrorString(string ExceptionTypeFullName, int MessageID, params object[] Args)
        {
            return GetErrorString(ExceptionTypeFullName, string.Format("{0}", MessageID), Thread.CurrentThread.CurrentUICulture, Args);
        }
    }
}