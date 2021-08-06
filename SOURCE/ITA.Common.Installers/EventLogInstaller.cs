using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace ITA.Common.Host
{
    /// <summary>
    /// Summary description for EventLogInstaller.
    /// </summary>
    [RunInstaller(true)]
    public class EventLogInstaller : System.Diagnostics.EventLogInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components;

        private System.Diagnostics.OverflowAction m_OverflowAction = System.Diagnostics.OverflowAction.OverwriteAsNeeded;

        private int m_iMaximumKilobytes = 512;
        private int m_iRetentionDays = 7;

        private string m_DisplayNameResourceFile = null;
        private int m_DisplayNameResourceId = -1;

        public EventLogInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the maximum event log size in kilobytes.
        /// </summary>
        public int MaximumKilobytes
        {
            get { return m_iMaximumKilobytes; }
            set { m_iMaximumKilobytes = value; }
        }

        /// <summary>
        /// The minimum number of days each event log entry is retained. 
        /// This parameter is used only if <see cref="OverflowAction"/> is set to <see cref="System.Diagnostics.OverflowAction.OverwriteOlder"/>. 
        /// </summary>
        /// <remarks>Value must be in the range 1..365 inclusive.</remarks>
        public int RetentionDays
        {
            get { return m_iRetentionDays; }
            set { m_iRetentionDays = value; }
        }

        /// <summary>
        /// The overflow behavior for writing new entries to the event log. 
        /// </summary>
        public System.Diagnostics.OverflowAction OverflowAction
        {
            get { return m_OverflowAction; }
            set { m_OverflowAction = value; }
        }

        public string DisplayNameResourceFile
        {
            get { return m_DisplayNameResourceFile; }
            set { m_DisplayNameResourceFile = value; }
        }

        public int DisplayNameResourceId
        {
            get { return m_DisplayNameResourceId; }
            set { m_DisplayNameResourceId = value; }
        }

        #region Installer overridables

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            //
            // This ugly trick is necessary to let event viewer to catch up and apply changes
            //
            int iAttemptsLeft = 5; // Preventing infinite loop by limiting the number of attempts
            while (iAttemptsLeft > 0 && !System.Diagnostics.EventLog.SourceExists(base.Source))
            {
                iAttemptsLeft--;
                Thread.Sleep(1000);
            }

            try
            {
                var log = new System.Diagnostics.EventLog(this.Log);
                log.ModifyOverflowPolicy(OverflowAction, RetentionDays);
                log.MaximumKilobytes = MaximumKilobytes;
            }
            catch (Exception x)
            {
                // ignore the errors but keep records
                Context.LogMessage(
                    string.Format(
                        "Non fatal error has occurred while installing event log '{0}'.\nUnable to configure retention policy due to the following error:\n{1}",
                        Log, x.Message));
            }

            if (!string.IsNullOrEmpty(m_DisplayNameResourceFile) && m_DisplayNameResourceId >= 0)
            {
                try
                {
                    var Log = new System.Diagnostics.EventLog(this.Log);
                    Log.RegisterDisplayName(m_DisplayNameResourceFile, m_DisplayNameResourceId);
                }
                catch (Exception x)
                {
                    // ignore the errors but keep records
                    Context.LogMessage(
                        string.Format(
                            "Non fatal error has occurred while installing event log '{0}'.\nUnable to configure display name due to the following error:\n{1}",
                            Log, x.Message));
                }
            }
        }

        #endregion

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}