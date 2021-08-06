using System;
using System.Reflection;
using MethodDecorator.Fody.Interfaces.Aspects;

namespace ITA.Common.Microservices.Metrics
{
    /// <summary>
    /// Attribute for collecting metrics for synchronous methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    [ProvideAspectRole(StandardRoles.PerformanceInstrumentation)]
    [MulticastAttributeUsage(MulticastTargets.Method)]
    [Serializable]
    public sealed class MethodMetricAttribute : MethodMetricAttributeBase
    {
        public MethodMetricAttribute(MetricsMode mode, string categoryName = null)
            : base(mode, categoryName)
        {
        }

        #region Initialization 

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            if (method.IsAsync())
            {
                throw new NotSupportedException("MethodMetricAttribute is not supported for async method. Use AsyncMethodMetricAttribute instead.");
            }

            base.CompileTimeInitialize(method, aspectInfo);
        }
        
        #endregion
    }
}
