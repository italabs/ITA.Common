using System.Runtime.InteropServices;

namespace ITA.Common.Host
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SYSTEM_POWER_STATUS
    {
        public byte ACLineStatus;
        public byte BatteryFlag;
        public byte BatteryLifePercent;
        public byte Reserved1;
        public int BatteryLifeTime;
        public int BatteryFullLifeTime;
    }
}
