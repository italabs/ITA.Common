using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ITA.Common.Microservices.Logging;
using MethodDecorator.Fody.Interfaces.Aspects;
using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Metrics
{
    public abstract class MethodMetricAttributeBase : OnMethodBoundaryAspect
    {
        private const string UndefinedName = "Undefined";
        private bool _initialized;
        private readonly object _syncObject = new object();

        private IReadOnlyDictionary<MetricsMode, MetricInfo> _metadata;
        private static readonly string[] MetricUnitLabelNames = { "category", "method" };
        private string[] _metricUnitLabelValues;

        [NonSerialized]
        private IMetricUnit _totalExecutedRequestsCounter;
        [NonSerialized]
        private IMetricUnit _successfullyExecutedRequestsCounter;
        [NonSerialized]
        private IMetricUnit _abandonedRequestsCounter;
        [NonSerialized]
        private IMetricUnit _lastExecutionTimeCounter;
        [NonSerialized]
        private IMetricUnit _requestsPerSecCounter;
        [NonSerialized]
        private IMetricUnit _executingRequestsCounter;
        [NonSerialized]
        private IMetricUnit _avgExecutionTimeCounter;

        private string _categoryName;
        private string _methodName;
        private readonly MetricsMode _mode;

        [NonSerialized]
        protected readonly Lazy<ILogger> Logger;

        [NonSerialized]
        private readonly Lazy<IMetricsFactory> _metricFactory;

        protected MethodMetricAttributeBase(MetricsMode mode, string categoryName = null)
        {
            Logger = new Lazy<ILogger>(CreateLogger);

            _metricFactory = new Lazy<IMetricsFactory>(CreateMetricsFactory);
            _categoryName = categoryName;
            _mode = mode;
        }


        #region Create counter metadata

        private IReadOnlyDictionary<MetricsMode, MetricInfo> CreateMetricsMetadata(MethodBase methodBase, MetricsMode modes)
        {
            _methodName = methodBase.Name;
            _metricUnitLabelValues = new[] { _categoryName, _methodName };

            var applicationMetadata = _metricFactory.Value.GetApplicationMetadata();
            var nameResolver = _metricFactory.Value.GetMetricUnitNameResolver();

            return new Dictionary<MetricsMode, MetricInfo>
            {
                {
                    MetricsMode.TotalExecutedRequestsCounter,
                    CreateCounterInfo(modes,
                        MetricsMode.TotalExecutedRequestsCounter,
                        applicationMetadata,
                        nameResolver,
                        "TotalExecutedRequests",
                        "The total number of requests executed",
                        MetricType.NumberOfItems64)
                },
                {
                    MetricsMode.AbandonedRequestsCounter,
                    CreateCounterInfo(modes,
                        MetricsMode.AbandonedRequestsCounter,
                        applicationMetadata,
                        nameResolver,
                        "AbandonedRequests",
                        "The total number of requests failed with exception",
                        MetricType.NumberOfItems64)
                },
                {
                    MetricsMode.SuccessfullyExecutedRequestsCounter,
                    CreateCounterInfo(modes,
                        MetricsMode.SuccessfullyExecutedRequestsCounter,
                        applicationMetadata,
                        nameResolver,
                        "SuccessfullyExecutedRequests",
                        "The number of requests executed normaly",
                        MetricType.NumberOfItems64)
                },
                {
                    MetricsMode.LastExecutionTimeCounter,
                    CreateCounterInfo(modes,
                        MetricsMode.LastExecutionTimeCounter,
                        applicationMetadata,
                        nameResolver,
                        "LastExecutionTime",
                        "The duration of request execution in milliseconds",
                        MetricType.ElapsedTime)
                },
                {
                    MetricsMode.RequestsPerSecCounter,
                    CreateCounterInfo(modes,
                        MetricsMode.RequestsPerSecCounter,
                        applicationMetadata,
                        nameResolver,
                        "RequestsPerSec",
                        "The number of requests processed per second",
                        MetricType.NumberOfItems64)
                },
                {
                    MetricsMode.ExecutingRequestsCounter,
                    CreateCounterInfo(modes,
                        MetricsMode.ExecutingRequestsCounter,
                        applicationMetadata,
                        nameResolver,
                        "ExecutingRequests",
                        "The number of requests executing right now",
                        MetricType.CounterDelta64)
                },
                {
                    MetricsMode.AvgExecutionTimeCounter,
                    CreateCounterInfo(modes,
                        MetricsMode.AvgExecutionTimeCounter,
                        applicationMetadata,
                        nameResolver,
                        "AvgExecutionTime",
                        "The average time of request execution in milliseconds",
                        MetricType.ElapsedTime)
                },
            };
        }

        private MetricInfo CreateCounterInfo(
            MetricsMode modes,
            MetricsMode targetMode,
            IApplicationMetadata metadata,
            IMetricUnitNameResolver nameResolver,
            string metricName,
            string description,
            MetricType type)
        {
            if ((modes & targetMode) != targetMode)
            {
                //Logger.Value.LogDebug("{0} mode does not detected", metricName);
                return null;
            }

            var metricUnitMetadata = new MetricUnitMetadata(
                metadata.ServiceName,
                _categoryName,
                _methodName,
                metricName,
                metadata.AdditionalData);

            var name = nameResolver.Resolve(metricUnitMetadata);

            //Logger.Value.LogDebug("{0} mode detected", name);

            return new MetricInfo
            {
                Name = name,
                Description = description,
                Type = type,
                LabelNames = MetricUnitLabelNames
            };
        }

        #endregion

        #region Initialization 

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            if (string.IsNullOrWhiteSpace(_categoryName))
            {
                _categoryName = method.ReflectedType?.Name ?? UndefinedName;
            }

            base.CompileTimeInitialize(method, aspectInfo);
        }

        public override void RuntimeInitialize(MethodBase methodBase)
        {
            base.RuntimeInitialize(methodBase);

            // DCL
            if (_initialized)
            {
                return;
            }

            lock (_syncObject)
            {
                if (_initialized)
                {
                    return;
                }

                //Logger.Value.LogDebug("Counters and metadata initialization of method begin");

                _metadata = CreateMetricsMetadata(methodBase, _mode);

                _totalExecutedRequestsCounter = CreateMetric(MetricsMode.TotalExecutedRequestsCounter);
                _successfullyExecutedRequestsCounter = CreateMetric(MetricsMode.SuccessfullyExecutedRequestsCounter);
                _abandonedRequestsCounter = CreateMetric(MetricsMode.AbandonedRequestsCounter);
                _lastExecutionTimeCounter = CreateMetric(MetricsMode.LastExecutionTimeCounter);
                _requestsPerSecCounter = CreateMetric(MetricsMode.RequestsPerSecCounter);
                _executingRequestsCounter = CreateMetric(MetricsMode.ExecutingRequestsCounter);
                _avgExecutionTimeCounter = CreateMetric(MetricsMode.AvgExecutionTimeCounter);

                //Logger.Value.LogDebug("Counters and metadata initialization of method has been finished");

                _initialized = true;
            }
        }

        #endregion

        #region Method actions

        public override void OnEntry(MethodExecutionArgs args)
        {
            //Logger.Value.LogDebug("OnEntry {0}", args.Method.Name);

            try
            {
                // Set begin time
                args.StartTime = Stopwatch.GetTimestamp();

                SecureIncrementCounter(_executingRequestsCounter);

                SecureIncrementCounter(_requestsPerSecCounter);
            }
            catch (Exception e)
            {
                Logger.Value.LogError(e, "OnEntry execution failed with exception");
            }


            base.OnEntry(args);
        }

        public override void OnExit(MethodExecutionArgs args, object returnValue)
        {
            //Logger.Value.LogDebug("OnExit {0}", args.Method.Name);

            try
            {
                if (!args.HasFailed)
                {
                    TickSuccessCounters();
                }
                
                TickExitCounters(args);
            }
            catch (Exception e)
            {
                Logger.Value.LogError(e, "OnExit execution failed with exception");
            }
        }


        public override void OnException(MethodExecutionArgs args, Exception exception)
        {
            //Logger.Value.LogDebug("OnException {0}", args.Method.Name);

            try
            {
                args.HasFailed = true;

                TickErrorCounters();
            }
            catch (Exception e)
            {
                Logger.Value.LogError(e, "OnException execution failed with exception");
            }

            base.OnException(args, exception);
        }

        
        #endregion

        #region Runtime

        protected  void TickErrorCounters()
        {
            SecureIncrementCounter(_abandonedRequestsCounter);
        }

        protected  void TickSuccessCounters()
        {
            SecureIncrementCounter(_successfullyExecutedRequestsCounter);
        }

        protected void TickExitCounters(MethodExecutionArgs args)
        {
            // Calculate method call duration
            var timeElapsed = GetMethodTimeElapsed(args.StartTime);

            //Logger.Value.LogDebug("Method {0} time elapsed: {1} ms", args.Method.Name, timeElapsed);

            SecureIncrementByCounter(_avgExecutionTimeCounter, (long)timeElapsed);
            SecureSetCounterValue(_lastExecutionTimeCounter, (long)timeElapsed);
            SecureDecrementCounter(_executingRequestsCounter);
            SecureIncrementCounter(_totalExecutedRequestsCounter);
        }

        private double GetMethodTimeElapsed(long start)
        {
            var elapsed = (Stopwatch.GetTimestamp() - start) * 1000;
            return (double)elapsed / Stopwatch.Frequency;
        }

        private void SecureIncrementCounter(IMetricUnit counter)
        {
            SecureTickCounter(counter, CounterAction.Increment, 0);
        }

        private void SecureDecrementCounter(IMetricUnit counter)
        {
            SecureTickCounter(counter, CounterAction.Decrement, 0);
        }

        private void SecureIncrementByCounter(IMetricUnit counter, long value)
        {
            SecureTickCounter(counter, CounterAction.IncrementBy, value);
        }

        private void SecureSetCounterValue(IMetricUnit counter, long value)
        {
            SecureTickCounter(counter, CounterAction.SetRaw, value);
        }

        private void SecureTickCounter(IMetricUnit counter, CounterAction action, long value)
        {
            if (null == counter)
            {
                //Logger.Value.LogDebug("Counter is null. Action: {0}, Value: {1}", action, value);
                return;
            }

            try
            {
                switch (action)
                {
                    case CounterAction.Increment: counter.Increment(_metricUnitLabelValues); break;
                    case CounterAction.IncrementBy: counter.IncrementBy(value, _metricUnitLabelValues); break;
                    case CounterAction.Decrement: counter.Decrement(_metricUnitLabelValues); break;
                    case CounterAction.SetRaw: counter.SetValue(value, _metricUnitLabelValues); break;
                }

                //Logger.Value.LogDebug("Counter '{0}', Value: {1}", counter.Name, value);
            }
            catch (Exception e)
            {
                Logger.Value.LogError("Error has occurred during change value of performance counter '{0}'. Description: '{1}'", counter.Name, e.Message);
            }
        }

        #endregion

        private IMetricUnit CreateMetric(MetricsMode metricMode)
        {
            try
            {
                if (_metadata.ContainsKey(metricMode))
                {
                    var info = _metadata[metricMode];
                    if (info != null)
                    {
                        return _metricFactory.Value.CreateUnit(
                            info.Name,
                            info.Description,
                            info.Type,
                            info.LabelNames);
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Logger.Value.LogDebug(e, "Creation of counter failed: MetricMode={0}", metricMode);
                return null;
            }
        }

        private IMetricsFactory CreateMetricsFactory() => MetricFactoryInstance.GetFactory();

        private ILogger CreateLogger()
        {
            var loggerFactory = AppliedLoggerFactoryInstance.GetFactory();

            return loggerFactory.CreateLogger<MethodMetricAttribute>();
        }

        private enum CounterAction
        {
            Increment,
            IncrementBy,
            Decrement,
            SetRaw
        }
    }
}