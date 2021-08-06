using System;
using System.Collections;
using System.Threading;
using ITA.Common.Tracing;
using log4net;

namespace ITA.Common
{
	[Trace]
	public class ThreadPool
	{
		//default logger name is ThreadPool
		private ILog _logger = LogManager.GetLogger(typeof(ThreadPool));

		public readonly object SyncObject = new object();

		internal struct WorkItem
		{
			public WaitCallback Callback;
			public object Context;
		}

		private int m_MinThreads = 1;
		private int m_MaxThreads = 50;
		private int m_Free = 0;
		private int m_IdleTimeout = -1; // Milliseconds
		private string m_PoolName;

		private Queue m_Queue = Queue.Synchronized(new Queue());
		private ArrayList m_ThreadIdList = ArrayList.Synchronized(new ArrayList());
		private ManualResetEvent m_ItemQueued = new ManualResetEvent(false);
		private bool m_bStopping = false;

		private Hashtable m_Threads = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Key: ThreadID, Value: Thread
		/// </summary>
		public Hashtable Threads
		{
			get { return m_Threads; }
		}

		public ThreadPool(string PoolName, int MinimumThreads, int MaximumThreads, int IdleTimeout)
		{
			if (!string.IsNullOrEmpty(PoolName))
				_logger = Log4NetItaHelper.GetLogger(PoolName);

			m_MinThreads = MinimumThreads;
			m_MaxThreads = MaximumThreads;
			m_IdleTimeout = IdleTimeout;

			ValidateThreadParameters();
			if (string.IsNullOrEmpty(PoolName))
				throw new ArgumentException("PoolName", "The name of thread pool is invalid");

			m_Free = 0;
			m_PoolName = PoolName;
		}

		public int Available
		{
			get { return m_ThreadIdList.Count; }
		}

		public int MinThreads
		{
			get { return m_MinThreads; }
			set
			{
				m_MinThreads = value;
				ValidateThreadParameters();
			}
		}

		public int MaxThreads
		{
			get { return m_MaxThreads; }
			set
			{
				m_MaxThreads = value;
				ValidateThreadParameters();
			}
		}

		public int QueuedItems
		{
			get { return m_Queue.Count; }
		}

		public void QueueUserWorkItem(WaitCallback Callback, object Context)
		{
			if (m_bStopping)
				throw new InvalidOperationException("Can't queue work item, bacause ThreadPool is stopped");

			WorkItem I = new WorkItem();
			I.Callback = Callback;
			I.Context = Context;

			lock (SyncObject)
			{
				m_Queue.Enqueue(I);
				m_ItemQueued.Set();
				if (m_Free < m_Queue.Count && m_ThreadIdList.Count < m_MaxThreads)
				{
					AddThread();
				}
			}
		}

		public void DispatchWorkItems()
		{
			string ThreadName = BuildThreadName(Thread.CurrentThread.ManagedThreadId);

			try
			{
				while (!m_bStopping)
				{
					_logger.InfoFormat("DispatchWorkItems - BeginWorkerCycle");
					//
					// Take next item
					//
					object o = null;
					lock (SyncObject)
					{
						_logger.Info("Dequeueing item");
						o = m_Queue.Count > 0 ? m_Queue.Dequeue() : null;
						if (m_Queue.Count == 0)
						{
							m_ItemQueued.Reset();
						}
					}

					if (o == null)
					{
						_logger.Info("Waiting for next work item queued event");
						if (!m_ItemQueued.WaitOne(m_IdleTimeout, false))
						{
							//
							// There are no new items in the queue for m_IdleTimeout
							//
							lock (SyncObject)
							{
								if (Available > MinThreads)
								{
									//
									// Commit suicide if we see that there are too many threads
									//
									// it should be done in this sync scope to let others to calc Available > MinThreads correctly
									//
									DelThread();
									_logger.Info("Recycling the current thread due to idle");
									return;
								}
							}
						}
						continue;
					}

					try
					{
						lock (SyncObject)
						{
							_logger.Info("Starting work item processing, decrementing free counter");
							m_Free--;
						}
						//
						// Process the item
						//
						WorkItem I = (WorkItem)o;
						I.Callback.DynamicInvoke(new object[] { I.Context });

						// check if MaxThreads has been changed
						lock (SyncObject)
						{
							if (Available > MaxThreads)
							{
								DelThread();
								_logger.Info("Recycling the current thread due to overlimit");
								return;
							}
						}
					}
					catch (Exception x)
					{
						_logger.ErrorFormat("Error in ThreadPool::DispatchWorkItems.\n{0}", x.Message);
						//throw; ignore the error
					}
					finally
					{
						lock (SyncObject)
						{
							_logger.Info("Work item processing done, incrementing free counter");
							m_Free++;
						}
					}
				} //while
			}
			catch (ThreadInterruptedException)
			{
				_logger.Warn("Thread is interrupted");
			}
			catch (ThreadAbortException)
			{
				_logger.Warn("Thread is aborted");
			}
			catch (Exception x)
			{
				// never should occur
				_logger.ErrorFormat("Unhandled exception: {0}", x.Message);
			}
			finally
			{
				_logger.Info("Finishing thread");
				DelThread();
			}
		}

		public void Start()
		{
			lock (SyncObject)
			{
				m_bStopping = false;

				for (int i = 0; i < m_MinThreads; i++)
				{
					AddThread();
				}
			}
		}

		public void Stop()
		{
			lock (SyncObject)
			{
				//
				// We have to do clean up for pending work items (which are not yet taken for processing).
				// Others will be handled by the corresponding worker thread. 
				//
				foreach (WorkItem I in m_Queue)
				{
					try
					{
						IDisposable Disposable = I.Context as IDisposable;
						if (Disposable != null)
						{
							Disposable.Dispose();
						}
					}
					catch (Exception x)
					{
						// never should occur
						_logger.ErrorFormat("Unhandled exception while cleaning up the queue of pending work items: {0}", x.Message);
					}
				}
				m_Queue.Clear();
				//
				// Signaling to let waiting threads to determine that it's time to die
				//
				_logger.Info("Resuming waiting threads to die");
				m_bStopping = true;
				m_ItemQueued.Set();

				_logger.Info("Cleaning up");
				m_Queue.Clear();
				m_Free = 0;

				m_ThreadIdList.Clear();
			}
		}

		private string BuildThreadName(int ID)
		{
			return m_PoolName + "." + ID.ToString();
		}

		private void AddThread()
		{
			Thread T = new Thread(new ThreadStart(this.DispatchWorkItems));
			T.Name = BuildThreadName(T.ManagedThreadId);

			_logger.InfoFormat("New thread name:{0}", T.Name);

			lock (SyncObject)
			{
				m_ThreadIdList.Add(T.ManagedThreadId);
				m_Free++;

				m_Threads.Add(T.ManagedThreadId, T);
			}
			T.Start();
		}

		private void DelThread()
		{
			lock (SyncObject)
			{
				Thread This = Thread.CurrentThread;
				_logger.InfoFormat("Removing thread:{0}", This.Name);

				int i = m_ThreadIdList.Count;

				m_ThreadIdList.Remove(This.ManagedThreadId);

				m_Threads.Remove(This.ManagedThreadId);

				if (i != m_ThreadIdList.Count)
				{
					// decrement counter upon successful removal
					m_Free--;
					//
					// Workaround for a bug in .NET
					// System resources remain leaked until GC when thread is finished
					// See here for more details:
					//      http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/2c3da48c-23b2-451b-a0c6-bf8dccc6b786
					//      http://www.geekserg.com/2010/08/handling-leaking-handles.html
					//
					// Note: Of course, we do not collect resources of THIS thread but collect handled leaked from other threads
					// that are already finished. The resources of last thread finished will be cleaned either on process exit or upon 
					// next thread to die
					//
					_logger.Info("Cleaning up leaked resources due to a bug in .NET");
					GC.Collect(2, GCCollectionMode.Forced);
				}
				else
				{
					_logger.InfoFormat("Thread {0} already removed", This.Name);
				}
			}
		}

		private void ValidateThreadParameters()
		{
			if (m_MinThreads < 1)
				throw new ArgumentOutOfRangeException("MininimThreads", m_MinThreads, "Minimum number of threads for thread pool can not be less than 1");
			if (m_MaxThreads < m_MinThreads)
				throw new ArgumentOutOfRangeException("MaximumThreads", m_MaxThreads, "Maximum number of threads for thread pool can not be less than the minimum");
		}
	}
}