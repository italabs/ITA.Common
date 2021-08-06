namespace ITA.Common.Host
{
    public interface IPowerEventHandler
    {
        bool IsSuspendEnabled();
        void OnBatteryLow();
        void OnBattery();
        void OnAC();
        void OnResume(EPowerResumeType Type);
        void OnSuspend();
        void OnSuspendDenied();
    }
}
