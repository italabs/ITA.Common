using System;
using System.Runtime.InteropServices;

namespace ITA.Common
{
    /// <summary>
    /// Summary description for SingleInstance.
    /// </summary>
    public class SingleInstance : IDisposable
    {
        #region Delegates

        public delegate void ActivateInstanceEventHandler(SingleInstance Source, EventArgs args);

        #endregion

        private const int ERROR_ALREADY_EXISTS = 183;
        private readonly bool m_bGlobal;

        private IntPtr m_hHandle = IntPtr.Zero;
        private string m_szName = "";

        public SingleInstance(string Name, bool Global)
        {
            m_szName = Name;
            m_bGlobal = Global;
        }

        public SingleInstance(string Name)
        {
            m_szName = Name;
        }

        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Unregister();
        }

        #endregion

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateEvent(
            [In, MarshalAs(UnmanagedType.I4)] int SecurityAttributes,
            [In, MarshalAs(UnmanagedType.Bool)] bool ManualReset,
            [In, MarshalAs(UnmanagedType.Bool)] bool InitialState,
            [In, MarshalAs(UnmanagedType.LPTStr)] string Name
            );

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);

        public bool Register()
        {
            Unregister();

            m_hHandle = CreateEvent(0, false, false, CreateObjName());
            int i = Marshal.GetLastWin32Error();
            if (i == ERROR_ALREADY_EXISTS)
            {
                return false;
            }

            return true;
        }

        public void Unregister()
        {
            if (m_hHandle != IntPtr.Zero)
            {
                CloseHandle(m_hHandle);
                m_hHandle = IntPtr.Zero;
            }
        }

        protected string CreateObjName()
        {
            if (m_bGlobal)
            {
                //
                // Determine the W2K and higher (terminal services support)
                //
                if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                    Environment.OSVersion.Version.Major >= 5)
                {
                    return "Global\\" + m_szName;
                }
            }
            return m_szName;
        }
    }
}