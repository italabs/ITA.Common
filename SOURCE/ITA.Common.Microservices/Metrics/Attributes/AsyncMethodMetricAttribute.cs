using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MethodDecorator.Fody.Interfaces.Aspects;
using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Metrics
{
    /// <summary>
    /// Attribute for collecting metrics for asynchronous methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    [ProvideAspectRole(StandardRoles.PerformanceInstrumentation)]
    [MulticastAttributeUsage(MulticastTargets.Method)]
    [Serializable]
    public sealed class AsyncMethodMetricAttribute : MethodMetricAttributeBase
    {
        [NonSerialized]
        private readonly AsyncLocal<MethodExecutionArgs> _args;

        public AsyncMethodMetricAttribute(MetricsMode mode, string categoryName = null)
            : base(mode, categoryName)
        {
            _args = new AsyncLocal<MethodExecutionArgs>();
        }
        
        #region Initialization 

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            if (!method.IsAsync())
            {
                throw new NotSupportedException("AsyncMethodMetricAttribute is supported for async method only. Use MethodMetricAttribute instead.");
            }

            base.CompileTimeInitialize(method, aspectInfo);
        }

        #endregion

        #region Method actions

        public override void OnEntry(MethodExecutionArgs args)
        {
            _args.Value = args;

            base.OnEntry(args);
        }

        public override void OnExit(MethodExecutionArgs args, object returnValue)
        {
            //Logger.Value.LogDebug("Skip OnExit {0} as async", args.Method.Name);
        }


        public override void OnException(MethodExecutionArgs args, Exception exception)
        {
            //Logger.Value.LogDebug("Skip OnException {0} as async", args.Method.Name);
        }

        public void OnTaskContinuation(Task task)
        {
            //Logger.Value.LogDebug("OnTaskContinuation");

            try
            {
                task?.ContinueWith(t =>
                {
                    //Logger.Value.LogDebug($"Task.ContinueWith Status={task.Status}");

                    if (t.Exception != null)
                    {
                        TickErrorCounters();

                        //Logger.Value.LogError(t.Exception, "OnTaskContinuation ContinueWith execution failed with exception");
                    }
                    else
                    {
                        TickSuccessCounters();
                    }

                    if (_args.Value != null)
                    {
                        TickExitCounters(_args.Value);
                    }

                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.Value.LogError(e, "OnTaskContinuation execution failed with exception");
            }
        }

        #endregion
    }
}