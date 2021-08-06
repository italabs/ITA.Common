using System;
using System.Runtime.InteropServices;

namespace ITA.Common.Host
{
    public enum SC_ACTION_TYPE
    {
        SC_ACTION_NONE = 0,
        SC_ACTION_RESTART = 1,
        SC_ACTION_REBOOT = 2,
        SC_ACTION_RUN_COMMAND = 3
    }

    [Serializable]
    public class SERVICE_FAILURE_ACTIONS
    {
        public UInt32 cActions;
        public UInt32 dwResetPeriod;
        public string lpCommand;
        public string lpRebootMsg;
        public SC_ACTION[] lpsaActions;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 0)]
    [Serializable]
    public struct SC_ACTION
    {
        [MarshalAs(UnmanagedType.U4)] public SC_ACTION_TYPE Type;
        public UInt32 Delay;
    }
}
