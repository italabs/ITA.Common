using System;
using System.Collections;
using ITA.Common.Host.Interfaces;
using log4net;

// ---------- ATTENTION ---------
// Whole purpose of those three classes is to remove dependency on ContextBoundObject. 
// These classes must be used by components that don't get published via remoting and are generic or contain generic methods.
// ---------- ATTENTION ---------

namespace ITA.Common.Host
{
    /// <summary>
    /// Used instead of classic <see cref="CompaundComponent"/> when you need to make your class generic or create a generic method inside of it.
    /// </summary>
    public abstract class UnboundCompaundComponent : IComponent
    {
        protected ArrayList m_Components;
        protected ILog m_Logger;

        protected EComponentStatus m_Status = EComponentStatus.Stopped;

        public UnboundCompaundComponent()
        {
            m_Components = new ArrayList();
        }

        public abstract string PublishedAs { get; }

        public abstract IPerformanceCounted Counters
        {
            get;
        }

        public ILog Logger
        {
            get
            {
                if (m_Logger == null)
                {
                    m_Logger = Log4NetItaHelper.GetLogger(Name);
                }
                return m_Logger;
            }
        }

        #region IComponent Members

        public abstract string Name { get; }
        public abstract IComponentConfig Config { get; }

        public EComponentStatus Status
        {
            get { return m_Status; }
        }

        public virtual void Initialize()
        {
            foreach (IComponent C in m_Components)
            {
                C.Initialize();
            }
        }

        public virtual void Start()
        {
            foreach (IComponent C in m_Components)
            {
                C.Start();
            }
        }

        public virtual void Stop()
        {
            for (int i = m_Components.Count - 1; i >= 0; i--)
            {
                ((IComponent)m_Components[i]).Stop();
            }
        }

        public virtual void Shutdown()
        {
            for (int i = m_Components.Count - 1; i >= 0; i--)
            {
                ((IComponent)m_Components[i]).Shutdown();
            }
        }

        public virtual void Continue()
        {
            foreach (IComponent C in m_Components)
            {
                C.Continue();
            }
        }

        public virtual void Pause()
        {
            for (int i = m_Components.Count - 1; i >= 0; i--)
            {
                ((IComponent)m_Components[i]).Pause();
            }
        }

        #endregion

        protected void SetStatus(EComponentStatus Value)
        {
            m_Status = Value;
        }

        public virtual IComponent Get(string Name)
        {
            foreach (IComponent C in m_Components)
            {
                if (C.Name == Name)
                {
                    return C;
                }
            }
            return null;
        }

        public virtual void Add(IComponent C)
        {
            if (C != this && C != null)
            {
                m_Components.Add(C);
            }
        }

        public virtual void Remove(IComponent C)
        {
            m_Components.Remove(C);
        }

        public virtual void RemoveAll()
        {
            ArrayList tmp = (ArrayList)m_Components.Clone();

            foreach (IComponent component in tmp)
            {
                this.Remove(component);
            }

            tmp = null;
        }
    }

    /// <summary>
    /// Used instead of classic <see cref="CompaundComponentWithEvents"/> when you need to make your class generic 
    /// or create a generic method inside of it.
    /// </summary>
    public abstract class UnboundCompaundComponentWithEvents : UnboundCompaundComponent, IComponentWithEvents
    {
        #region IComponentWithEvents Members

        public void FireEvent(string ID, EEventType Type, params object[] Args)
        {
            if (null != OnEvent)
                OnEvent(this, ID, Type, Args);
        }

        public void FireError(Exception x)
        {
            if (null != OnError)
                OnError(this, x);
        }

        public void FireFatalError(Exception x)
        {
            if (null != OnFatalError)
                OnFatalError(this, x);
        }

        public virtual string[] Events
        {
            get
            {
                return new[]
                           {
                               Interfaces.Events.OnInitializing,
                               Interfaces.Events.OnInitialized,
                               Interfaces.Events.OnStarting,
                               Interfaces.Events.OnStarted,
                               Interfaces.Events.OnStopping,
                               Interfaces.Events.OnStopped,
                               Interfaces.Events.OnShuttingDown,
                               Interfaces.Events.OnShutDown,
                               Interfaces.Events.OnPausing,
                               Interfaces.Events.OnPaused,
                               Interfaces.Events.OnContinuing,
                               Interfaces.Events.OnContinued,
                               Interfaces.Events.OnError
                           };
            }
        }

        public event OnIComponentError OnError;
        public event OnIComponentEvent OnEvent;
        public event OnIComponentError OnFatalError;

        #endregion

        public override void Add(IComponent C)
        {
            base.Add(C);

            var E = C as IComponentWithEvents;
            if (null != E)
            {
                E.OnEvent += OnEvent;
                E.OnError += OnError;
                E.OnFatalError += OnFatalError;
            }
        }

        public override void Remove(IComponent C)
        {
            var E = C as IComponentWithEvents;
            if (null != E)
            {
                E.OnEvent -= OnEvent;
                E.OnError -= OnError;
                E.OnFatalError -= OnFatalError;
            }

            m_Components.Remove(C);
        }
    }

    /// <summary>
    /// Used instead of classic <see cref="ComponentWithEvents"/> when you need to make your class generic or create a generic method inside of it.
    /// </summary>
    public abstract class UnboundComponentWithEvents : IComponentWithEvents
    {
        protected ILog m_Logger;
        protected EComponentStatus m_Status = EComponentStatus.Stopped;

        //public virtual IPerformanceCounted Counters
        //{
        //    get { return null; }
        //}

        public virtual string PublishedAs
        {
            get
            {
                return null;
            }
        }

        public virtual ILog Logger
        {
            get
            {
                if (m_Logger == null)
                {
                    m_Logger = Log4NetItaHelper.GetLogger(Name);
                }
                return m_Logger;
            }
        }

        #region IComponentWithEvents Members

        public abstract string Name { get; }

        public virtual IComponentConfig Config
        {
            get { return null; }
        }

        public EComponentStatus Status
        {
            get { return m_Status; }
        }

        public void Initialize()
        {
            Logger.DebugFormat("{0} is initializing", Name);

            try
            {
                if (m_Status != EComponentStatus.Running && m_Status != EComponentStatus.Starting &&
                    m_Status != EComponentStatus.Paused)
                {
                    FireEvent(Interfaces.Events.OnInitializing, EEventType.Information, Name);

                    OnInitialize();

                    FireEvent(Interfaces.Events.OnInitialized, EEventType.Information, Name);
                }
            }
            catch (Exception ex)
            {
                m_Status = EComponentStatus.Error;
                Logger.ErrorFormat("{0} failed to initialize: {1}", Name, ex);
                throw new HostException(Messages.I_ITA_ERROR_ON_INIT, HostException.E_HOST_ERROR_INITIALIZING_ENGINE,
                                        ex, Name);
            }

            Logger.InfoFormat("{0} initialized.", Name);
        }

        public void Start()
        {
            Logger.DebugFormat("{0} is starting", Name);

            try
            {
                if (m_Status != EComponentStatus.Running && m_Status != EComponentStatus.Starting &&
                    m_Status != EComponentStatus.Paused)
                {
                    FireEvent(Interfaces.Events.OnStarting, EEventType.Information, Name);
                    m_Status = EComponentStatus.Starting;

                    OnStart();

                    m_Status = EComponentStatus.Running;
                    FireEvent(Interfaces.Events.OnStarted, EEventType.Information, Name);
                }
            }
            catch (Exception ex)
            {
                m_Status = EComponentStatus.Error;
                Logger.ErrorFormat("{0} failed to start: {1}", Name, ex);
                FireError(ex);
                throw new HostException(ITA.Common.Messages.I_ITA_ERROR_ON_START, HostException.E_HOST_ERROR_STARTING_ENGINE, ex,
                                        Name);
            }

            Logger.InfoFormat("{0} started.", Name);
        }

        public void Stop()
        {
            Logger.DebugFormat("{0} is stopping", Name);

            try
            {
                if (m_Status == EComponentStatus.Running || m_Status == EComponentStatus.Starting ||
                    m_Status == EComponentStatus.Paused)
                {
                    FireEvent(Interfaces.Events.OnStopping, EEventType.Information, Name);

                    OnStop();

                    m_Status = EComponentStatus.Stopped;
                    FireEvent(Interfaces.Events.OnStopped, EEventType.Information, Name);
                }
            }
            catch (Exception ex)
            {
                m_Status = EComponentStatus.Error;
                Logger.ErrorFormat("{0} failed to stop: {1}", Name, ex);
                FireError(ex);

                // throw new HostException();  Let other components to stop
            }

            Logger.DebugFormat("{0} stopped.", Name);
        }

        public void Shutdown()
        {
            Logger.DebugFormat("{0} is shutting down", Name);

            try
            {
                FireEvent(Interfaces.Events.OnShuttingDown, EEventType.Information, Name);

                OnShutdown();

                FireEvent(Interfaces.Events.OnShutDown, EEventType.Information, Name);
            }
            catch (Exception ex)
            {
                m_Status = EComponentStatus.Error;
                Logger.ErrorFormat("{0} failed to shutdown: {1}", Name, ex);
                FireError(ex);

                // throw new HostException();  Let other components to shut down
            }

            Logger.DebugFormat("{0} shut down.", Name);
        }

        public void Pause()
        {
            Logger.DebugFormat("{0} is pausing", Name);

            try
            {
                if (m_Status == EComponentStatus.Running || m_Status == EComponentStatus.Starting)
                {
                    FireEvent(Interfaces.Events.OnPausing, EEventType.Information, Name);

                    OnPause();

                    m_Status = EComponentStatus.Paused;
                    FireEvent(Interfaces.Events.OnPaused, EEventType.Information, Name);
                }
            }
            catch (Exception ex)
            {
                m_Status = EComponentStatus.Error;
                Logger.ErrorFormat("{0} failed to pause: {1}", Name, ex);
                FireError(ex);
                throw new HostException(ITA.Common.Messages.I_ITA_ERROR_ON_PAUSE, HostException.E_HOST_ERROR_PAUSING_ENGINE, ex,
                                        Name);
            }

            Logger.DebugFormat("{0} paused.", Name);
        }

        public void Continue()
        {
            Logger.DebugFormat("{0} is resuming", Name);

            try
            {
                if (m_Status == EComponentStatus.Paused)
                {
                    FireEvent(Interfaces.Events.OnContinuing, EEventType.Information, Name);

                    OnContinue();

                    m_Status = EComponentStatus.Running;
                    FireEvent(Interfaces.Events.OnContinued, EEventType.Information, Name);
                }
            }
            catch (Exception ex)
            {
                m_Status = EComponentStatus.Error;
                Logger.ErrorFormat("{0} failed to resume: {1}", Name, ex);
                FireError(ex);
                throw new HostException(ITA.Common.Messages.I_ITA_ERROR_ON_CONTINUE, HostException.E_HOST_ERROR_RESUMING_ENGINE,
                                        ex, Name);
            }

            Logger.DebugFormat("{0} resumed.", Name);
        }

        public void FireEvent(string ID, EEventType Type, params object[] Args)
        {
            if (null != OnEvent)
                OnEvent(this, ID, Type, Args);
        }

        public void FireError(Exception x)
        {
            if (null != OnError)
                OnError(this, x);
        }

        public void FireFatalError(Exception x)
        {
            m_Status = EComponentStatus.Error;

            if (null != OnFatalError)
                OnFatalError(this, x);
        }

        public virtual string[] Events
        {
            get
            {
                return new[]
                           {
                               Interfaces.Events.OnStarting,
                               Interfaces.Events.OnStarted,
                               Interfaces.Events.OnStopping,
                               Interfaces.Events.OnStopped,
                               Interfaces.Events.OnPausing,
                               Interfaces.Events.OnPaused,
                               Interfaces.Events.OnContinuing,
                               Interfaces.Events.OnContinued,
                               Interfaces.Events.OnError
                           };
            }
        }

        public event OnIComponentError OnError;
        public event OnIComponentEvent OnEvent;
        public event OnIComponentError OnFatalError;

        #endregion

        public virtual void OnInitialize()
        {
        }

        public virtual void OnShutdown()
        {
        }

        public virtual void OnPause()
        {
        }

        public virtual void OnContinue()
        {
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnStop()
        {
        }

        protected void SetStatus(EComponentStatus Value)
        {
            m_Status = Value;
        }
    }
}