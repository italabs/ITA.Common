using System;

namespace ITA.Common.Host
{
    public enum EChannelType : ushort
    {
        tcp = 1,
        http = 2
    }
   
    public class BinarySerializableAttribute : Attribute
    {
    }

    public interface IControlManagerConfig
    {
        string Address { get; set; }
    }

    public interface IServiceConfig
    {
        bool AutoStart { get; set; }
        bool EnableSuspend { get; set; }
        EOnPowerEventBehaviour OnBatteryAction { get; set; }
        EOnPowerEventBehaviour OnLowBatteryAction { get; set; }
        EOnPowerEventBehaviour OnSuspendAction { get; set; }

        string ID { get; set; }
    }
}
