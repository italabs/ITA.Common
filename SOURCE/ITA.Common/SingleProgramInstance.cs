using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ITA.Common
{    
    /// <summary>
    /// SingleProgamInstance uses a mutex synchronization 
    /// object to ensure that only one copy of process is running 
    /// at a particular time.  It also allows for UI identification
    /// of the intial process by bringing that window to the foreground.
    /// </summary>
    public class SingleProgramInstance : IDisposable
    {
        private const int BSM_APPLICATIONS = 0x00000008;
        private const int BSF_IGNORECURRENTTASK = 0x00000002;
        private const int BSF_POSTMESSAGE = 0x00000010;

        private readonly bool _global;
        private readonly int _message;
        private bool _owned;
        private Mutex _processSync;

        public SingleProgramInstance(string identifier) :
            this(identifier, false)
        {
        }

        public SingleProgramInstance(string identifier, bool global)
        {
            _global = global;

            //
            // Initialize a named mutex and attempt to get ownership immediately.
            //
            _processSync = new Mutex(
                true, // desire intial ownership
                global ? "Global\\" + identifier : identifier,
                out _owned);

            _message = RegisterWindowMessage(identifier);
            if (_message == 0)
            {
                throw new SystemException();
            }
        }

        public int Message
        {
            get { return _message; }
        }

        public bool Global
        {
            get { return _global; }
        }

        public bool IsSingleInstance
        {
            //If we don't own the mutex than
            // we are not the first instance.
            get { return _owned; }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        private static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U4)]
        private static extern int BroadcastSystemMessage(Int32 dwFlags, ref Int32 lpdwRecipients, Int32 uiMessage,
                                                         IntPtr wParam, IntPtr lParam);

        public static bool Exists(string identifier, bool global)
        {
            bool Found = false;
            try
            {
                using (Mutex Existing = Mutex.OpenExisting(global ? "Global\\" + identifier : identifier))
                {
                    Found = true; // Do nothing, just make sure it exists and close it instantly
                }
            }
            catch
            {
                //
                // This is not really right condition. The trick is that we have no idea of what to do if Access Denied happened.
                // So let's treat all issues as Does not Exist
                //
                // catch(WaitHandleCannotBeOpenedException)
                //{
                //    Found = false;
                //}
                //catch(UnauthorizedAccessException ex)
                //{
                //    Unauthorized = true;
                //}

                Found = false;
            }

            return Found;
        }

        ~SingleProgramInstance()
        {
            Dispose(false);
        }

        public void NotifyOwnerProcess()
        {
            int Recipients = BSM_APPLICATIONS;
            BroadcastSystemMessage(BSF_IGNORECURRENTTASK | BSF_POSTMESSAGE, ref Recipients, _message, IntPtr.Zero, IntPtr.Zero);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ; // free managed resources
            }

            if (_processSync != null)
            {
                _processSync.Close();
                _processSync = null;
                _owned = false;
            }
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            //release mutex (if necessary) and notify 
            // the garbage collector to ignore the destructor
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}