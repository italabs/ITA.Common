using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using ITA.Common.Tracing;

namespace ITA.Common.Host
{
    [Trace]
    public class ServiceConfig
    {
        private const int fDescription = 1;
        private const int fActions = 2;

        public static void AdjustServiceSettings(string SvcName, string Description)
        {
            AdjustServiceSettingsEx(SvcName, fDescription, Description, new SERVICE_FAILURE_ACTIONS());
        }

        public static void AdjustServiceSettings(string SvcName, SERVICE_FAILURE_ACTIONS Actions)
        {
            AdjustServiceSettingsEx(SvcName, fActions, "", Actions);
        }

        public static void AdjustServiceSettings(string SvcName, string Description, SERVICE_FAILURE_ACTIONS Actions)
        {
            AdjustServiceSettingsEx(SvcName, fActions | fDescription, Description, Actions);
        }

        private static void AdjustServiceSettingsEx(string SvcName, int Flags, string Description,
                                                    SERVICE_FAILURE_ACTIONS Actions)
        {
            IntPtr hSCManager = IntPtr.Zero;
            IntPtr hSvc = IntPtr.Zero;
            IntPtr pBuffer = IntPtr.Zero;
            try
            {
                hSCManager = OpenSCManager("", SERVICES_ACTIVE_DATABASE, SC_MANAGER_ALL_ACCESS);
                if (hSCManager == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                hSvc = OpenService(hSCManager, SvcName, SC_MANAGER_ALL_ACCESS);
                if (hSvc == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                AdjustPrivileges.AdjustSEShutdownPrivilege();

                if ((Flags & fActions) != 0)
                {
                    var ActionsPriv = new SERVICE_FAILURE_ACTIONS_PRIV();
                    ActionsPriv.cActions = Actions.cActions;
                    ActionsPriv.dwResetPeriod = Actions.dwResetPeriod;
                    ActionsPriv.lpCommand = Actions.lpCommand;
                    ActionsPriv.lpRebootMsg = Actions.lpRebootMsg;

                    int sSize = Marshal.SizeOf(typeof (SC_ACTION));
                    pBuffer = Marshal.AllocCoTaskMem(sSize*Actions.lpsaActions.Length);
                    for (int i = 0; i < Actions.lpsaActions.Length; i++)
                    {
                        IntPtr tmp;

                        if (ProcessMode.Is64Bit())
                            tmp = new IntPtr(pBuffer.ToInt64() + i * sSize);
                        else
                            tmp = new IntPtr(pBuffer.ToInt32() + i * sSize);

                        Marshal.StructureToPtr(Actions.lpsaActions[i], tmp, false);
                    }
                    ActionsPriv.lpsaActions = pBuffer;

                    if (!ChangeServiceConfig2(hSvc, SERVICE_CONFIG_FAILURE_ACTIONS, ref ActionsPriv))
                    {
                        throw new Win32Exception();
                    }
                }

                if ((Flags & fDescription) != 0)
                {
                    var Desc = new SERVICE_DESCRIPTION();
                    Desc.lpDescription = Description;
                    if (!ChangeServiceConfig2(hSvc, SERVICE_CONFIG_DESCRIPTION, ref Desc))
                    {
                        throw new Win32Exception();
                    }
                }
            }
            finally
            {
                if (pBuffer != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pBuffer);

                if (hSvc != IntPtr.Zero)
                    CloseServiceHandle(hSvc);

                if (hSCManager != IntPtr.Zero)
                    CloseServiceHandle(hSCManager);
            }
        }

        #region Interop stuff

        private const UInt32 SERVICE_CONFIG_DESCRIPTION = 1;
        private const UInt32 SERVICE_CONFIG_FAILURE_ACTIONS = 2;

        private const UInt32 SC_MANAGER_ALL_ACCESS = 0xF003F;
        private const string SERVICES_ACTIVE_DATABASE = "ServicesActive";

        [DllImport("Advapi32.dll", EntryPoint = "OpenSCManagerW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, UInt32 dwDesiredAccess);

        [DllImport("Advapi32.dll", EntryPoint = "CloseServiceHandle", CharSet = CharSet.Unicode)]
        private static extern UInt32 CloseServiceHandle(IntPtr hSCObject);

        [DllImport("Advapi32.dll", EntryPoint = "OpenServiceW", CharSet = CharSet.Unicode)]
        private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, UInt32 dwDesiredAccess);

        [DllImport("Advapi32.dll", EntryPoint = "ChangeServiceConfig2W", CharSet = CharSet.Unicode)]
        private static extern bool ChangeServiceConfig2(IntPtr Handle, UInt32 Level,
                                                        ref SERVICE_FAILURE_ACTIONS_PRIV Actions);

        [DllImport("Advapi32.dll", EntryPoint = "ChangeServiceConfig2W", CharSet = CharSet.Unicode)]
        private static extern bool ChangeServiceConfig2(IntPtr Handle, UInt32 Level, ref SERVICE_DESCRIPTION Description);

        #region Nested type: SERVICE_DESCRIPTION

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 0)]
        private struct SERVICE_DESCRIPTION
        {
            public string lpDescription;
        }

        #endregion

        #region Nested type: SERVICE_FAILURE_ACTIONS_PRIV

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 0)]
        private struct SERVICE_FAILURE_ACTIONS_PRIV
        {
            public UInt32 dwResetPeriod;
            public string lpRebootMsg;
            public string lpCommand;
            public UInt32 cActions;
            public IntPtr lpsaActions;
        }

        #endregion

        #endregion
    }
}