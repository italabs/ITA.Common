using System;

namespace ITA.Common.Host
{
    internal class AdjustPrivileges
    {
        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;

        internal const int TOKEN_QUERY = 0x00000008;

        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        public static void AdjustSEShutdownPrivilege()
        {
            WinInterops.TokPriv1Luid tp;
            IntPtr hproc = WinInterops.GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;

            if (!WinInterops.OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok))
            {
                throw new Exception("Unable to open process token");
            }

            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;

            if (!WinInterops.LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid))
            {
                throw new Exception("Unable to find privilege");
            }

            if (!WinInterops.AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
            {
                throw new Exception("Unable to adjust process token");
            }
        }
    }
}