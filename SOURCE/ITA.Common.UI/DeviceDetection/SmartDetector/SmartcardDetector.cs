using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using log4net;

namespace ITA.Common.DeviceDetection
{
    internal enum SmartcardState
    {
        None = 0,
        Inserted = 1,
        Ejected = 2
    }

    internal class SmartDetector : IDeviceDetector, IDisposable
    {
        #region Member Fields

        //Shared members are lazily initialized.
        //.NET guarantees thread safety for shared initialization.
        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(SmartDetector).Name);
        private static SmartDetector _instance = null; // DO NOT CREATE INSTANCE HERE!!! 
        // It causes instance constructor to be invoked BEFORE the static construct is ended.
        // In other words: you'll be unable to use static members from instance members 'cause
        // they're not initialized yet
        private SmartcardContextSafeHandle _context;
        private SmartcardErrorCode _lastErrorCode;
        private bool _disposed = false;
        private ReaderState[] _states;
        //A thread that watches for new smart cards.
        private BackgroundWorker _worker;

        #endregion

        private string[] GetCardStates(CardState state)
        {
            List<string> states = new List<string>();

            foreach (CardState value in Enum.GetValues(typeof(CardState)))
            {
                if ((state & value) == value)
                {
                    states.Add(value.ToString());
                }
            }

            return states.ToArray();
        }

        #region Constructors

        //Make the constructor private to hide it. This class adheres to the singleton pattern.
        private SmartDetector()
        {
            logger.Debug("SmartDetector::SmartDetector () >>>");

            //Create a new SafeHandle to store the smartcard context.
            this._context = new SmartcardContextSafeHandle();
            //Establish a context with the PC/SC resource manager.
            this.EstablishContext();

            //Compose a list of the card readers which are connected to the
            //system and which will be monitored.
            ArrayList availableReaders = this.ListReaders();
            this._states = new ReaderState[availableReaders.Count];
            for (int i = 0; i <= availableReaders.Count - 1; i++)
            {
                this._states[i].Reader = availableReaders[i].ToString();
            }

            logger.InfoFormat("WaitChangeStatus detected {0} available readers", availableReaders.Count);

            //Start a background worker thread which monitors the specified
            //card readers.
            {
                this._worker = new BackgroundWorker();
                this._worker.WorkerSupportsCancellation = true;
                this._worker.DoWork += WaitChangeStatus;
                this._worker.RunWorkerAsync();
            }

            logger.Info("SmartDetector::SmartDetector () <<<");
        }

        #endregion

        #region IDisposable Support

        //IDisposable
        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).            
                }

                //Free your own state (unmanaged objects).
                //Set large fields to null.
                logger.InfoFormat("SmartDetector::Dispose (disposing={0}) >>>", disposing);

                this._states = null;
                this._worker.CancelAsync();
                this._worker.Dispose();
                this._context.Dispose();

                logger.Info("SmartDetector::Dispose () <<<");
            }
            this._disposed = true;
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        public static SmartDetector GetDetector()
        {
            if (_instance == null)
            {
                _instance = new SmartDetector();
            }
            return _instance;
        }

        private bool EstablishContext()
        {
            logger.Info("SmartDetector::EstablishContext () >>>");

            if ((this.HasContext))
            {
                return true;
            }

            logger.Info("UnsafeNativeMethods.EstablishContext () invoked");
            this._lastErrorCode =
                (SmartcardErrorCode)UnsafeNativeMethods.EstablishContext(ScopeOption.System,
                IntPtr.Zero, IntPtr.Zero, ref this._context);

            logger.InfoFormat("UnsafeNativeMethods.EstablishContext () returned {0}", this._lastErrorCode);

            bool RetVal = (this._lastErrorCode == SmartcardErrorCode.None);
            logger.InfoFormat("EstablishContext (RetVal={0}) <<<", RetVal);
            return RetVal;
        }

        private void WaitChangeStatus(object sender, DoWorkEventArgs e)
        {
            logger.Info("SmartDetector::WaitChangeStatus () >>>");

            try
            {
                if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                    Thread.CurrentThread.Name = "Detector";

                SmartcardErrorCode result = SmartcardErrorCode.None;

                bool IsCheckContext = true;

                while (!e.Cancel)
                {
                    //Obtain a lock when we use the context pointer, 
                    //which may be modified in the Dispose() method.
                    lock (this)
                    {
                        if (IsCheckContext && !this.HasContext)
                        {
                            logger.Info("Context not found, exiting...");
                            return;
                        }

                        //This thread will be executed every 1000ms. 
                        //The thread also blocks for 1000ms, meaning 
                        //that the application may keep on running for 
                        //one extra second after the user has closed 
                        //the Main Form.
                        logger.Info("UnsafeNativeMethods.GetStatusChange () invoked");
                        result =
                            (SmartcardErrorCode)UnsafeNativeMethods.GetStatusChange(
                                this._context, 1000, this._states, this._states.Length);
                    }

                    logger.InfoFormat("UnsafeNativeMethods.GetStatusChange () returned {0}", result);

                    //Refresh a list of the card readers which are connected to the system and generate events in case of their change.
                    ArrayList availableReaders = this.ListReaders();
                    logger.InfoFormat("WaitChangeStatus detected {0} available readers", availableReaders.Count);
                    logger.InfoFormat("States count: {0}", _states.Length);

                    if (availableReaders.Count != _states.Length)
                    {
                        var localOnInserted = OnInserted;
                        var localOnRemoved = OnRemoved;
                        if (availableReaders.Count > _states.Length && localOnInserted != null)
                        {
                            logger.Info("Invoking OnInserted");
                            localOnInserted(null, EventArgs.Empty);
                        }
                        if (availableReaders.Count < _states.Length && localOnRemoved != null)
                        {
                            logger.Info("Invoking OnRemoved");
                            localOnRemoved(null, EventArgs.Empty);
                        }

                        this._states = new ReaderState[availableReaders.Count];
                        for (int i = 0; i <= availableReaders.Count - 1; i++)
                        {
                            this._states[i].Reader = availableReaders[i].ToString();
                        }
                    }
                    else if (availableReaders.Count == 0) 
                    {
                        if (result == SmartcardErrorCode.ServiceStopped) //stopped service needs to re-establish context
                        {
                            logger.Info("Service is stopped. Re-establish context");

                            Thread.Sleep(1000);

                            lock (this)
                            {
                                IsCheckContext = this.RenewContext();
                            }

                            continue;                            
                        }
                        else
                        {
                            logger.Info("No readers available: timeout, continue...");
                            Thread.Sleep(1000);
                            continue;
                        }
                    }

                    if (result == SmartcardErrorCode.Timeout)
                    {
                        // Time out has passed, but there is no new info. Just go on with the loop
                        logger.Info("Timeout, continue...");
                        continue;
                    }
                    else if (result != SmartcardErrorCode.None)
                    {
                        Thread.Sleep(1000);

                        lock (this)
                        {
                            IsCheckContext = this.RenewContext();
                        }

                        continue;
                    }

                    logger.DebugFormat("States found: {0}", this._states.Length);
                    for (int i = 0; i <= this._states.Length - 1; i++)
                    {
                        if (logger.IsDebugEnabled)
                        {
                            logger.DebugFormat("Reader: {0}", this._states[i].Reader);
                            logger.DebugFormat("{0} - CurrentState:{1}", i, string.Join(",", GetCardStates(this._states[i].CurrentState)));
                            logger.DebugFormat("{0} - EventState:  {1}", i, string.Join(",", GetCardStates(this._states[i].EventState)));
                        }

                        //Check if the state changed from the last time.
                        if ((this._states[i].EventState & CardState.Changed) == CardState.Changed)
                        {
                            //Check what changed.
                            SmartcardState state = SmartcardState.None;
                            if ((this._states[i].EventState & CardState.Present) == CardState.Present
                                && (this._states[i].CurrentState & CardState.Present) != CardState.Present)
                            {
                                //The card was inserted.
                                logger.InfoFormat("The card was inserted");
                                state = SmartcardState.Inserted;
                            }
                            else if ((this._states[i].EventState & CardState.Empty) == CardState.Empty
                                && (this._states[i].CurrentState & CardState.Empty) != CardState.Empty)
                            {
                                //The card was ejected.
                                logger.InfoFormat("The card was ejected");
                                state = SmartcardState.Ejected;
                            }
                            if (state != SmartcardState.None && this._states[i].CurrentState != CardState.None)
                            {
                                switch (state)
                                {
                                    case SmartcardState.Inserted:
                                        {
                                            if (OnInserted != null)
                                            {
                                                logger.Info("Invoking OnInserted");
                                                OnInserted(null, EventArgs.Empty);
                                            }
                                            break;
                                        }
                                    case SmartcardState.Ejected:
                                        {
                                            if (OnRemoved != null)
                                            {
                                                logger.Info("Invoking OnRemoved");
                                                OnRemoved(null, EventArgs.Empty);
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }
                            }
                            //Update the current state for the next time they are checked.
                            this._states[i].CurrentState = this._states[i].EventState;
                        }
                    }
                }
            }
            catch (Exception x)
            {
                logger.ErrorFormat("Error has occurred:{0}", x.Message);
                throw;
            }
            finally
            {
                logger.Info("SmartDetector::WaitChangeStatus () <<<");
            }
        }

        private int GetReaderListBufferSize()
        {
            logger.Info("SmartDetector::GetReaderListBufferSize () >>>");
            if ((this._context.IsInvalid))
            {
                return 0;
            }

            int result = 0;

            logger.Info("UnsafeNativeMethods.ListReaders () invoked");
            this._lastErrorCode =
                (SmartcardErrorCode)UnsafeNativeMethods.ListReaders(
                    this._context, null, null, ref result);
            logger.InfoFormat("UnsafeNativeMethods.ListReaders () returned {0}", this._lastErrorCode);

            logger.InfoFormat("SmartDetector::GetReaderListBufferSize (RetVal={0}) <<<", result);
            return result;
        }

        private bool RenewContext()
        {
            this._context.Dispose();
            this._context = new SmartcardContextSafeHandle();
            return this.EstablishContext();
        }

        public ArrayList ListReaders()
        {
            logger.Info("SmartDetector::ListReaders () >>>");

            ArrayList result = new ArrayList();

            //Make sure a context has been established before 
            //retrieving the list of smartcard readers.
            if (this.EstablishContext())
            {
                //Ask for the size of the buffer first.
                int size = this.GetReaderListBufferSize();

                logger.InfoFormat("ReaderListBufferSize:'{0}'", size);

                //Allocate a string of the proper size in which 
                //to store the list of smartcard readers.
                string readerList = new string('\0', size);
                //Retrieve the list of smartcard readers.

                logger.Info("UnsafeNativeMethods.ListReaders () invoked");
                this._lastErrorCode =
                    (SmartcardErrorCode)UnsafeNativeMethods.ListReaders(this._context,
                        null, readerList, ref size);
                logger.InfoFormat("UnsafeNativeMethods.ListReaders () returned {0}", this._lastErrorCode);

                logger.InfoFormat("ReaderListBufferSize 2:'{0}'", size);

                if ((this._lastErrorCode == SmartcardErrorCode.None))
                {
                    //Extract each reader from the returned list.
                    //The readerList string will contain a multi-string of 
                    //the reader names, i.e. they are seperated by 0x00 
                    //characters.
                    string readerName = string.Empty;
                    for (int i = 0; i <= readerList.Length - 1; i++)
                    {
                        if ((readerList[i] == '\0'))
                        {
                            if ((readerName.Length > 0))
                            {
                                //We have a smartcard reader's name.
                                result.Add(readerName);
                                readerName = string.Empty;
                            }
                        }
                        else
                        {
                            //Append the found character.
                            readerName += new string(readerList[i], 1);
                        }
                    }
                }
            }

            logger.InfoFormat("SmartDetector::ListReaders (RetVal={0}) <<<", result.Count);
            return result;
        }

        #endregion

        #region Properties

        private bool HasContext
        {
            get
            {
                return (!this._context.IsInvalid);
            }
        }

        #endregion

        #region IDeviceDetector Members

        public event EventHandler OnInserted;
        public event EventHandler OnRemoved;
        public event EventHandler OnDeviceChanged;

        #endregion
    }
}
