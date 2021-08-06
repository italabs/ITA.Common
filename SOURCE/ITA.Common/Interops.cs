using System;
using System.Runtime.InteropServices;
using ITA.Common.Host;

namespace ITA.Common
{
    public static class WinInterops
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len,
            IntPtr prev, IntPtr relen);

        #region Nested type: TokPriv1Luid

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        #endregion

        [DllImport("Kernel32.dll", EntryPoint = "AllocConsole")]
        public static extern bool AllocConsole();

        [DllImport("Kernel32.dll", EntryPoint = "AttachConsole")]
        public static extern bool AttachConsole(uint dwProcessId);

        [DllImport("Kernel32.dll", EntryPoint = "GetLastError")]
        public static extern uint GetLastError();

        [DllImport("Kernel32.dll", EntryPoint = "GetSystemPowerStatus")]
        public static extern bool GetSystemPowerStatus(ref SYSTEM_POWER_STATUS SystemPowerStatus);
    }
}
