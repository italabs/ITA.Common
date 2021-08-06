using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ITA.Common.Host.Enums;
using ITA.Common.Host.Interfaces;
using ITA.Common.Host.PerfCounter;
using log4net;
using MethodDecorator.Fody.Interfaces.Aspects;

namespace ITA.Common.Host
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [ProvideAspectRole(StandardRoles.PerformanceInstrumentation)]
    [MulticastAttributeUsage(MulticastTargets.Method)]
    [Serializable]
    public sealed class MethodPerfCountedAttribute : OnMethodBoundaryAspect
    {
        /// <summary>
        /// В случае использования конструтора MethodPerfCountedAttribute(ECountingMode modes)
        /// в переменную должен передаваться префикс приложения для формирования категории счетчика.
        /// Устанавливается в конструкторе хоста.
        /// </summary>
        public static string GlobalApplicationPrefix = "Undefined";

        private static readonly ILog Logger = LogManager.GetLogger(typeof(MethodPerfCountedAttribute));

        private bool _initialized = false;

        //todo In case of adding new counter DO NOT FORGET to add new record to Obfuscation exclusion list !!

        private PerformanceCounterInfo _TotalExecutedRequestsInfo;
        private PerformanceCounterInfo _AbandonedRequestsInfo;
        private PerformanceCounterInfo _SuccessfullyExecutedRequestsInfo;
        private PerformanceCounterInfo _LastExecutionTimeInfo;
        private PerformanceCounterInfo _RequestsPerSecInfo;
        private PerformanceCounterInfo _ExecutingRequestsInfo;
        private PerformanceCounterInfo _AvgExecutionTimeInfo;

        [NonSerialized] private ICounterUnit _TotalExecutedRequestsCounter;
        [NonSerialized] private ICounterUnit _SuccessfullyExecutedRequestsCounter;
        [NonSerialized] private ICounterUnit _AbandonedRequestsCounter;
        [NonSerialized] private ICounterUnit _LastExecutionTimeCounter;
        [NonSerialized] private ICounterUnit _RequestsPerSecCounter;
        [NonSerialized] private ICounterUnit _ExecutingRequestsCounter;
        [NonSerialized] private ICounterUnit _AvgExecutionTimeCounter;
        [NonSerialized] private ICounterUnit _AvgExecutionTimeCounterBase;

        private string _applicationPrefix;
        private string _categoryName;
        private const string _instanceName = "Default";

        public MethodPerfCountedAttribute(string applicationPrefix, ECountingMode modes)
        {            
            Helpers.CheckNullOrEmpty(applicationPrefix, "ApplicationPrefix");
            Logger.DebugFormat("Mode: {0}", modes);

            InitializeCountersInfo(modes);
            
            _applicationPrefix = applicationPrefix;
            Logger.DebugFormat("Application prefix: '{0}'", _applicationPrefix ?? "NULL");
        }

        public MethodPerfCountedAttribute(string applicationPrefix, string category, ECountingMode modes) : this(applicationPrefix, modes)
        {
            Helpers.CheckNullOrEmpty(category, "Category");
            _categoryName = category;
            Logger.DebugFormat("Given category: {0}", category);
        }

        /// <summary>
        /// ctor. Префикс приложения для создания счетчика будет получен из переменной <see cref="GlobalApplicationPrefix"/>
        /// </summary>
        public MethodPerfCountedAttribute(ECountingMode modes)
        {
            Logger.DebugFormat("GlobalApplicationPrefix: {0} Mode: {1}", GlobalApplicationPrefix, modes);

            InitializeCountersInfo(modes);
        }
        
        public MethodPerfCountedAttribute(ECountingMode modes, string category)
        {
            Helpers.CheckNullOrEmpty(category, "category");

            Logger.DebugFormat("GlobalApplicationPrefix: {0} Mode: {1} Category: {2}", GlobalApplicationPrefix, modes, category);

            InitializeCountersInfo(modes);

            _categoryName = category;
        }

        public string ApplicationPrefix
        {
            get { return _applicationPrefix; }
        }

        public string CategoryName
        {
            get { return _categoryName; }
        }

        public IEnumerable<PerformanceCounterInfo> CountersInfos
        {
            get { return GetCounterInfos(); }
        }

        #region PostSharp initialization 

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            Logger.DebugFormat("CompileTimeInitialize {0}", method.Name);

            if (_applicationPrefix != null && _categoryName == null)
            {
                _categoryName = PerfCounterHelper.BuildCountersCategoryName(method.ReflectedType.Name, _applicationPrefix, Logger);
            }

            base.CompileTimeInitialize(method, aspectInfo);
        }        

        public override void RuntimeInitialize(MethodBase methodBase)
        {
            base.RuntimeInitialize(methodBase);
            
            Logger.DebugFormat("RuntimeInitialize of '{0}'", methodBase.Name);
            Logger.DebugFormat("GlobalApplicationPrefix: '{0}'", GlobalApplicationPrefix ?? "NULL");
            Logger.DebugFormat("Application prefix: '{0}'", _applicationPrefix ?? "NULL");
            Logger.DebugFormat("Category name: '{0}'", _categoryName ?? "NULL");

            // Если префикс явно установлен, используем его
            _applicationPrefix = _applicationPrefix ?? GlobalApplicationPrefix;

            if (_applicationPrefix == null)
            {
                Logger.Error("_applicationPrefix is null");
                return;
            }

            // Определяем категорию
             _categoryName = PerfCounterHelper.BuildCountersCategoryName(_categoryName ?? methodBase.ReflectedType.Name, _applicationPrefix, Logger);

            if (_categoryName == null)
            {
                Logger.Error("_categoryName is null");
                return;
            }

            //DumpCounterInfos(methodBase);

            try
            {
                CreatePerformanceCountersInstances(methodBase.Name);
            }
            catch (Exception e)
            {
                Logger.Error("Initialization of perf counters failed", e);
            }
        }

        private void DumpCounterInfos(MethodBase methodBase)
        {
            var lst = CountersInfos;

            foreach (PerformanceCounterInfo info in lst)
            {
                Logger.DebugFormat("\tCounter: '{0}' of '{1}' of '{2}' enabled", info.CounterName, methodBase.Name, methodBase.ReflectedType.FullName);
            }
        }

        #endregion

        #region Method actions

        public override void OnEntry(MethodExecutionArgs args)
        {
            Logger.DebugFormat("OnEntry {0}", args.Method.Name);

            try
            {
                //
                // Засекаем время входа
                //
                args.StartTime = GetTicks();

                SecureIncrementCounter(_ExecutingRequestsCounter);
                SecureIncrementCounter(_RequestsPerSecCounter);
            }
            catch (Exception e)
            {
                Logger.Error("OnEntry execution failed with exception", e);
            }


            base.OnEntry(args);
        }

        public override void OnExit(MethodExecutionArgs args, object returnValue)
        {
            Logger.DebugFormat("OnExit {0}", args.Method.Name);

            try
            {
                if (!args.HasFailed)
                {
                    SecureIncrementCounter(_SuccessfullyExecutedRequestsCounter);
                }

                //
                // Длительность вызова 
                //
                var timeElapsed = GetMethodTimeElapsed(args.StartTime);

                Logger.DebugFormat("Method {0} time elapsed: {1} ms", args.Method.Name, timeElapsed);

                TickExitCounters(timeElapsed);
            }
            catch (Exception e)
            {
                Logger.Error("OnExit execution failed with exception", e);
            }
        }


        public override void OnException(MethodExecutionArgs args, Exception exception)
        {
            Logger.DebugFormat("OnException {0}", args.Method.Name);

            try
            {
                args.HasFailed = true;

                SecureIncrementCounter(_AbandonedRequestsCounter);
            }
            catch (Exception e)
            {
                Logger.Error("OnException execution failed with exception", e);
            }

            base.OnException(args, exception);
        }

        #endregion

        private void AddCounterInfoIfNeed(List<PerformanceCounterInfo> lst, PerformanceCounterInfo info)
        {
            if (info != null)
            {
                lst.Add(info);
            }
        }

        private void TickExitCounters(double timeElapsed)
        {
            SecureIncrementByCounter(_AvgExecutionTimeCounter, (long)timeElapsed);
            SecureIncrementCounter(_AvgExecutionTimeCounterBase);

            SecureSetCounterValue(_LastExecutionTimeCounter, (long)timeElapsed);
            SecureDecrementCounter(_ExecutingRequestsCounter);
            SecureIncrementCounter(_TotalExecutedRequestsCounter);
        }

        private double GetMethodTimeElapsed(long start)
        {
            long finish = GetTicks();
            long freq = GetFrequency();

            long elapsed = (finish - start) * 1000;
            double timeElapsed = ((double)elapsed / freq);
            return timeElapsed;
        }

        private void CreatePerformanceCountersInstances(string methodName)
        {
            if (_initialized)
            {
                return;
            }            

            if (!PerfCounterAdapter.PerformanceCounterCategoryExists(_categoryName))
            {
                throw new Exception(string.Format("Performance counter category '{0}' is not exist", _categoryName));
            }

            _TotalExecutedRequestsCounter        = InitCounterInstance(methodName, _categoryName, _TotalExecutedRequestsInfo);
            _SuccessfullyExecutedRequestsCounter = InitCounterInstance(methodName, _categoryName, _SuccessfullyExecutedRequestsInfo);
            _AbandonedRequestsCounter            = InitCounterInstance(methodName, _categoryName, _AbandonedRequestsInfo);
            _LastExecutionTimeCounter            = InitCounterInstance(methodName, _categoryName, _LastExecutionTimeInfo);
            _RequestsPerSecCounter               = InitCounterInstance(methodName, _categoryName, _RequestsPerSecInfo);
            _ExecutingRequestsCounter            = InitCounterInstance(methodName, _categoryName, _ExecutingRequestsInfo);
            _AvgExecutionTimeCounter             = InitCounterInstance(methodName, _categoryName, _AvgExecutionTimeInfo, ref _AvgExecutionTimeCounterBase);

            _initialized = true;

            Logger.DebugFormat("Counters initialization of method {0} has been finished", methodName);
        }

        private static ICounterUnit InitCounterInstance(string methodName, string category, PerformanceCounterInfo counterInfo)
        {
            try
            {
                ICounterUnit basePc = null;
                return InitCounterInstance(methodName, category, counterInfo, ref basePc);
            }
            catch (Exception e)
            {
                Logger.DebugFormat("Creation of couner failed: Category={0}, methodName={1}. Detail: {2}", category, methodName, e.Message);
                return null;
            }
        }

        private static ICounterUnit InitCounterInstance(string methodName, string category, PerformanceCounterInfo counterInfo, ref ICounterUnit baseCounter)
        {
            if (counterInfo == null)
            {
                return null;
            }

            var cName = BuildCounterName(methodName, counterInfo.CounterName);
            var counter = PerfCounterAdapter.CreateCounterUnit(category, cName, counterInfo.CounterType, _instanceName, false);

            Logger.DebugFormat("Created new counter with following parameters: Category={0}, CounterName={1}", category, counter.CounterName);

            switch (counterInfo.CounterType)
            {
                case ItaPerformanceCounterType.AverageTimer32:
                case ItaPerformanceCounterType.AverageCount64:
                case ItaPerformanceCounterType.CounterMultiTimer:
                case ItaPerformanceCounterType.CounterMultiTimerInverse:
                case ItaPerformanceCounterType.CounterMultiTimer100Ns:
                case ItaPerformanceCounterType.CounterMultiTimer100NsInverse:
                case ItaPerformanceCounterType.RawFraction:
                case ItaPerformanceCounterType.SampleCounter:
                case ItaPerformanceCounterType.SampleFraction:
                    {
                        var baseCounterName = counter.CounterName + "Base";
                        baseCounter = PerfCounterAdapter.CreateCounterUnit(category, baseCounterName, counterInfo.CounterType, _instanceName, false);
                        Logger.DebugFormat("Created new base counter with following parameters: Category={0}, CounterName={1}", category, counter.CounterName + "Base");
                        break;
                    }
            }

            return counter;
        }

        public static string BuildCounterName(string methodName, string counterName)
        {
            return string.Format("{0} {1}", methodName, counterName);
        }

        private void SecureIncrementCounter(ICounterUnit counter)
        {
            SecureTickCouner(counter, CounterAction.Increment, 0);
        }

        private void SecureDecrementCounter(ICounterUnit counter)
        {
            SecureTickCouner(counter, CounterAction.Decrement, 0);
        }

        private void SecureIncrementByCounter(ICounterUnit counter, long value)
        {
            SecureTickCouner(counter, CounterAction.IncrementBy, value);
        }

        private void SecureSetCounterValue(ICounterUnit counter, long value)
        {
            SecureTickCouner(counter, CounterAction.SetRaw, value);
        }

        internal enum CounterAction
        {
            Increment,
            IncrementBy,
            Decrement,
            SetRaw
        }

        private void SecureTickCouner(ICounterUnit counter, CounterAction action, long value)
        {
            if (null != counter)
            {
                try
                {
                    switch (action)
                    {
                        case CounterAction.Increment: counter.Increment(); break;
                        case CounterAction.IncrementBy: counter.IncrementBy(value); break;
                        case CounterAction.Decrement: counter.Decrement(); break;
                        case CounterAction.SetRaw: counter.RawValue = value; break;
                    }

                    Logger.DebugFormat("Counter '{0}', Value: {1}", counter.CounterName, counter.RawValue);
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("Error has occurred during change value of performance counter '{0}'. Description: '{1}'", counter.CounterName, e.Message);
                }
            }
            else
            {
                Logger.DebugFormat("Counter is null. Action: {0}, Value: {1}", action, value);
            }
        }

        private void InitializeCountersInfo(ECountingMode modes)
        {
            InitCounterInfo(ref _TotalExecutedRequestsInfo, modes, ECountingMode.TotalExecutedRequestsCounter, "TotalExecutedRequests", "The total number of requests executed", ItaPerformanceCounterType.NumberOfItems64);
            InitCounterInfo(ref _AbandonedRequestsInfo, modes, ECountingMode.AbandonedRequestsCounter, "AbandonedRequests", "The total number of requests failed with exception", ItaPerformanceCounterType.NumberOfItems64);
            InitCounterInfo(ref _SuccessfullyExecutedRequestsInfo, modes, ECountingMode.SuccessfullyExecutedRequestsCounter, "SuccessfullyExecutedRequests", "The number of requests executed normaly", ItaPerformanceCounterType.NumberOfItems64);
            InitCounterInfo(ref _LastExecutionTimeInfo, modes, ECountingMode.LastExecutionTimeCounter, "LastExecutionTime", "The duration of request execution in milliseconds", ItaPerformanceCounterType.NumberOfItems64);
            InitCounterInfo(ref _RequestsPerSecInfo, modes, ECountingMode.RequestsPerSecCounter, "RequestsPerSec", "The number of requests processed per second", ItaPerformanceCounterType.RateOfCountsPerSecond64);
            InitCounterInfo(ref _ExecutingRequestsInfo, modes, ECountingMode.ExecutingRequestsCounter, "ExecutingRequests", "The number of requests executing right now", ItaPerformanceCounterType.NumberOfItems64);
            InitCounterInfo(ref _AvgExecutionTimeInfo, modes, ECountingMode.AvgExecutionTimeCounter, "AvgExecutionTime", "The average time of request execution in milliseconds", ItaPerformanceCounterType.AverageCount64);
        }

        private void InitCounterInfo(ref PerformanceCounterInfo counter, ECountingMode modes, ECountingMode targetMode, string name, string desc, ItaPerformanceCounterType cType)
        {
            if ((modes & targetMode) == targetMode)
            {
                counter = new PerformanceCounterInfo()
                {
                    CounterName = name,
                    CounterDecription = desc,
                    CounterType = cType
                };

                Logger.DebugFormat("{0} mode detected", name);
            }
            else
            {
                Logger.DebugFormat("{0} mode does not detected", name);
            }
        }

        private List<PerformanceCounterInfo> GetCounterInfos()
        {
            var lst = new List<PerformanceCounterInfo>();

            AddCounterInfoIfNeed(lst, _TotalExecutedRequestsInfo);
            AddCounterInfoIfNeed(lst, _AbandonedRequestsInfo);
            AddCounterInfoIfNeed(lst, _SuccessfullyExecutedRequestsInfo);
            AddCounterInfoIfNeed(lst, _LastExecutionTimeInfo);
            AddCounterInfoIfNeed(lst, _RequestsPerSecInfo);
            AddCounterInfoIfNeed(lst, _ExecutingRequestsInfo);
            AddCounterInfoIfNeed(lst, _AvgExecutionTimeInfo);

            return lst;
        }

        private long GetTicks()
        {
            return Stopwatch.GetTimestamp();
        }

        private long GetFrequency()
        {
            return Stopwatch.Frequency;
        }
    }
}