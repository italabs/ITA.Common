using System;
using System.ComponentModel;
using ITA.Common.Host.Interfaces;
using log4net;
using IComponent = ITA.Common.Host.Interfaces.IComponent;

namespace ITA.Common.Host.Components
{
	/// <summary>
	/// Monitoring Engine
	/// </summary>
	public class MonitoringEngine : Component, IMonitoringEngine
	{
	    private static ILog logger = Log4NetItaHelper.GetLogger(typeof (MonitoringEngine).Name);

		private EventHandlerCollection m_EventHandlers;
		private LocaleMessages m_Messages;
  
		private string m_szInstanceName;
		private string m_szEventLogName;
		private IEventLog m_EventLog;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public MonitoringEngine(IContainer container)
		{

			// Required for Windows.Forms Class Composition Designer support
			container.Add (this);
			
			InitializeComponent ();
			InitializeComponentEx ();
		}

		public MonitoringEngine ( ICommandContext context, IEventLog eventLog )
		{
			logger.Debug ( "MonitoringEngine::MonitoringEngine () >>>" );

		    m_EventLog = eventLog;

			m_szInstanceName = context.InstanceName;
			logger.DebugFormat( "Instance name: {0}", m_szInstanceName );

			m_szEventLogName = context.EventLogName;
			logger.DebugFormat( "EventLog name: {0}", m_szEventLogName );

            //Required for Windows.Forms Class Composition Designer support
			logger.Debug("Initializing component" );
			InitializeComponent ();

			logger.Debug("InitializingEx component" );
			InitializeComponentEx ();

			logger.Debug("MonitoringEngine::MonitoringEngine () <<<" );
		}
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			logger.Debug("MonitoringEngine::InitializeComponent () >>>"); 
			logger.Debug("Creating UIMessages object"); 
			m_Messages = new LocaleMessages ();

			logger.Debug("Creating event handlers collection");
			m_EventHandlers = new EventHandlerCollection ( this );

			logger.Debug("Creating event log");

		    InitializeEventLog();

            logger.Debug( "MonitoringEngine::InitializeComponent () <<<\n" );
		}
		#endregion

	    private void InitializeEventLog()
	    {
	        logger.Debug("BeginInitialize");
            m_EventLog.BeginInit();
	        
	        // 
	        // m_EventLog
	        // 
	        this.m_EventLog.Log = m_szEventLogName;

	        logger.Debug("Initializing objects by instance name");
	        m_EventLog.Source = m_szEventLogName;

            logger.Debug("EndInitialize");

	        m_EventLog.EndInit();
            
        }

        private void InitializeComponentEx()
		{

		}

		public string InstanceName
		{
			get
			{
				return m_szInstanceName;
			}
		}

		#region IMonitoringEngine Members

        public void OnEvent(IComponent Source, string ID, EEventType Type, params object[] Args)
		{
			Event e = new Event ( Source, ID, Type, Args );
            m_EventHandlers.HandleEvent(ref e);
		}

        public void OnError(IComponent Source, Exception Error)
		{
			Event e = new Event ( Source, Interfaces.Events.OnError, EEventType.Error, Error );
			m_EventHandlers.HandleEvent ( ref e );
		}

		public EventHandlerCollection EventHandlers
		{
			get { return m_EventHandlers; }
		}

        public IEventLog EventLog
		{
			get { return m_EventLog; }
		}

        public LocaleMessages Messages
		{
			get { return m_Messages; }
		}

		#endregion
	}
}
