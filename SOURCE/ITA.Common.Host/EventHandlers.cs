using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml.Serialization;
using ITA.Common.Host.Interfaces;
using log4net;

namespace ITA.Common.Host
{
    internal class EventInfo
    {
        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(EventInfo).Name);

        public static string Build(LocaleMessages Messages, Event E, out int ID)
        {
            logger.DebugFormat("Build() begin.");

            try
            {
                ID = 0;
                logger.DebugFormat("E.GetType():{0}", E.GetType());
                logger.DebugFormat("E.Type:{0}", E.Type);
                logger.DebugFormat("E.Source.GetType():{0}", E.Source.GetType());
                logger.DebugFormat("E.ID:{0}", E.ID);
                if (E.Args != null)
                {
                    logger.Debug("E.Args:");
                    foreach (object o in E.Args)
                    {
                        logger.DebugFormat("arg:{0}", o);
                    }
                }

                // it doesn't work as expected. E.GetType() == typeof(Event) so we can register custom events only in assembly where Event is declared.
                string szMessage = Messages.GetString(E.GetType(), E.ID, E.Args);

                logger.DebugFormat("szMessage:{0}", szMessage);

                if (szMessage != null && szMessage.StartsWith("&"))
                {
                    //
                    // Message may be prefixed with &id; 
                    // For example:  &33;
                    //
                    int i = szMessage.IndexOf(";");
                    if (i >= 2)
                    {
                        string szID = szMessage.Substring(1, i - 1);
                        logger.DebugFormat("szID:{0}", szID);

                        try
                        {
                            ID = int.Parse(szID);
                            logger.DebugFormat("ID:{0}", ID);

                            szMessage = szMessage.Substring(i + 1);

                            logger.DebugFormat("szMessage:{0}", szMessage);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e);
                        }
                    }
                }

                if (E.Type == EEventType.FailureAudit)
                {
                    if (E.Args.Length > 0 && E.Args[0] is Exception)
                    {
                        szMessage += "\n";
                        szMessage += ExceptionInfo.Build(Messages, E.Args[0] as Exception);
                    }
                }

                logger.DebugFormat("szMessage:{0}", szMessage);
                return szMessage;
            }
            finally
            {
                logger.DebugFormat("Build() end.");
            }
        }
    }

    internal class ExceptionInfo
    {
        public static string Build(LocaleMessages Messages, Exception Error)
        {
            // if we have ITAException then try to get localized string
            string LocalizedMessage = null;
            string NonLocalizedMessage = Error.Message;

            if (Error is ITAException)
            {
                var OurEx = Error as ITAException;
                LocalizedMessage = Messages.GetString(Error.GetType(), OurEx.ID, OurEx.Args);
            }

            string Format = (LocalizedMessage != null && NonLocalizedMessage != null) ? "{0}\n{1}" : "{0} {1}";
            string szMessage = string.Format(Format, NonLocalizedMessage, LocalizedMessage);

            if (null != Error.InnerException)
            {
                szMessage += "\n";
                szMessage += Build(Messages, Error.InnerException);
            }

            return szMessage;
        }
    }

    /// <summary>
    /// Summary description for RunApplicationHandler.
    /// </summary>
    [Serializable]
    public class RunApplicationHandler : IEventHandler
    {
        private readonly IMonitoringEngine m_Engine;
        private readonly ProcessStartInfo m_pStartInfo;

        public RunApplicationHandler(IMonitoringEngine Engine, string szFileName, string szArguments,
                                     string szWorkingDirectory)
        {
            m_Engine = Engine;
            m_pStartInfo = new ProcessStartInfo(szFileName, szArguments);
            m_pStartInfo.WorkingDirectory = szWorkingDirectory;
        }

        [XmlAttribute]
        public string CommandLine
        {
            get { return ""; }
            set { ; }
        }

        #region IEventHandler Members

        public void Handle(ref Event E)
        {
            try
            {
                Process.Start(m_pStartInfo);
            }
            catch (Exception x)
            {
                string szMessage = String.Format("Error while executing command line.\n{0}",
                                                 ExceptionInfo.Build(m_Engine.Messages, x));
                m_Engine.EventLog.WriteEntry(szMessage, EEventType.Error);
            }
        }

        #endregion
    }

    /// <summary>
    /// Summary description for WriteToEventLog.
    /// </summary>
    [Serializable]
    public class WriteToEventLogHandler : IEventHandler
    {
        private readonly IMonitoringEngine m_Engine;

        public WriteToEventLogHandler(IMonitoringEngine Engine)
        {
            m_Engine = Engine;
        }

        #region IEventHandler Members

        public void Handle(ref Event E)
        {
            short Category = m_Engine.EventLog.GetCategory(E.Source.Name);

            int ID = 0;
            string szMessage =
                E.ID == Events.OnError
                    ? ExceptionInfo.Build(m_Engine.Messages, E.Args[0] as Exception)
                    : EventInfo.Build(m_Engine.Messages, E, out ID);

            if (szMessage == null)
                szMessage = E.ID;

            if (szMessage == null)
                szMessage = "";

            m_Engine.EventLog.WriteEntry(szMessage, E.Type, ID, Category);
        }

        #endregion
    }

    /// <summary>
    /// Summary description for TraceEventHandler.
    /// </summary>
    [Serializable]
    public class TraceEventHandler : IEventHandler
    {
        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(TraceEventHandler).Name);

        private readonly IMonitoringEngine m_Engine;

        public TraceEventHandler(IMonitoringEngine Engine)
        {
            m_Engine = Engine;
        }

        #region IEventHandler Members

        public void Handle(ref Event E)
        {
            logger.DebugFormat("Handle() begin.");
            try
            {
                logger.DebugFormat("E.ID:{0}", E.ID);
                int ID = 0;
                string szMessage =
                    E.ID == Events.OnError
                        ? ExceptionInfo.Build(m_Engine.Messages, E.Args[0] as Exception)
                        : EventInfo.Build(m_Engine.Messages, E, out ID);
                logger.DebugFormat("ID:{0}", ID);
                logger.DebugFormat("szMessage:{0}", szMessage);

                if (szMessage == null)
                    szMessage = E.ID;

                if (szMessage == null)
                    szMessage = "";

                Trace.WriteLine(szMessage, E.Source.Name);

                if (E.ID == Events.OnError)
                    Trace.WriteLine((E.Args[0] as Exception).StackTrace, E.Source.Name);
            }
            catch (Exception x)
            {
                logger.Error(x);
                string szMessage = String.Format("Error while writing trace.\n{0}",
                                                 ExceptionInfo.Build(m_Engine.Messages, x));
                m_Engine.EventLog.WriteEntry(szMessage, EEventType.Error);
            }
            finally
            {
                logger.DebugFormat("Handle() end.");
            }
        }

        #endregion
    }

    /// <summary>
    /// Summary description for EventHandlerCollection.
    /// </summary>
    public class EventHandlerCollection
    {
        private readonly HybridDictionary m_EventHandlers;
        private IMonitoringEngine m_Engine;

        public EventHandlerCollection(IMonitoringEngine Engine)
        {
            m_Engine = Engine;
            m_EventHandlers = new HybridDictionary();
        }

        public HybridDictionary EventHandlers
        {
            get { return m_EventHandlers; }
        }

        public void HandleEvent(ref Event E)
        {
            //
            // Look for handlers assigned to all components
            //
            var List = m_EventHandlers[typeof (IMonitoringEngine)] as IDictionary;
            if (null != List)
            {
                //
                // Look for handlers assigned to all events
                //
                var Handlers = List[-1] as IEventHandler[];
                if (null != Handlers)
                    ProcessHandlers(Handlers, ref E);
                //
                // Look for handlers assigned to specific event
                //
                Handlers = List[E.ID] as IEventHandler[];
                if (null != Handlers)
                    ProcessHandlers(Handlers, ref E);
            }
            //
            // Look for handlers assigned to specific component
            //
            List = m_EventHandlers[E.Source.GetType()] as IDictionary;
            if (null != List)
            {
                //
                // Look for handlers assigned to all events
                //
                var Handlers = List[-1] as IEventHandler[];
                if (null != Handlers)
                    ProcessHandlers(Handlers, ref E);
                //
                // Look for handlers assigned to specific event
                //
                Handlers = List[E.ID] as IEventHandler[];
                if (null != Handlers)
                    ProcessHandlers(Handlers, ref E);
            }
        }

        private void ProcessHandlers(IEventHandler[] Handlers, ref Event E)
        {
            foreach (IEventHandler H in Handlers)
            {
                try
                {
                    H.Handle(ref E);
                }
                catch
                {
                }
            }
        }
    }
}