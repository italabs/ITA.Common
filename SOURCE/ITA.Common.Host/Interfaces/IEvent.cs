namespace ITA.Common.Host.Interfaces
{
    /// <summary>
    /// Summary description for EEventType.
    /// </summary>
    public enum EEventType
    {
        Information,
        Warning,
        Error,
        SuccessAudit,
        FailureAudit
    }

    /// <summary>
    /// Summary description for Event.
    /// </summary>
    public struct Event
    {
        public object[] Args;
        public string ID;
        public IComponent Source;
        public EEventType Type;

        public Event(IComponent Source, string ID, EEventType Type, params object[] Args)
        {
            this.Source = Source;
            this.ID = ID;
            this.Type = Type;
            this.Args = Args;
        }
    }

    /// <summary>
    /// Summary description for IEventHandler.
    /// </summary>
    public interface IEventHandler
    {
        void Handle(ref Event E);
    }

    /// <summary>
    /// Summary description for Events.
    /// </summary>
    public partial struct Events
    {
        public static string OnInitializing = "ITA.Common.Events.OnInitializing";
        public static string OnInitialized = "ITA.Common.Events.OnInitialized";
        public static string OnShuttingDown = "ITA.Common.Events.OnShuttingDown";
        public static string OnShutDown = "ITA.Common.Events.OnShutDown";
        public static string OnStarting = "ITA.Common.Events.OnStarting";
        public static string OnStarted = "ITA.Common.Events.OnStarted";
        public static string OnStopping = "ITA.Common.Events.OnStopping";
        public static string OnStopped = "ITA.Common.Events.OnStopped";
        public static string OnPausing = "ITA.Common.Events.OnPausing";
        public static string OnPaused = "ITA.Common.Events.OnPaused";
        public static string OnContinuing = "ITA.Common.Events.OnContinuing";
        public static string OnContinued = "ITA.Common.Events.OnContinued";
        public static string OnError = "ITA.Common.Events.OnError";


        public static string OnBattery = "ITA.Common.Events.OnBattery";
        public static string OnAC = "ITA.Common.Events.OnAC";
        public static string OnBatteryLow = "ITA.Common.Events.OnBatteryLow";
        public static string OnQuerySuspendDenied = "ITA.Common.Events.OnQuerySuspendDenied";
        public static string OnResumeAutomatic = "ITA.Common.Events.OnResumeAutomatic";
        public static string OnResumeCritical = "ITA.Common.Events.OnResumeCritical";
        public static string OnSuspend = "ITA.Common.Events.OnSuspend";

        public static string OnConfigurationChanged = "ITA.Common.Events.Host.ConfigManager.OnConfigurationChanged";
    }
}