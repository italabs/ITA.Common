using System;

namespace ITA.Common.Microservices.Tracing
{
    /// <summary>
    /// Attribute used to mark the input and output parameters of the function. 
    /// If the function parameter is checked this attribute, the trace output values 
    /// of this parameter will be replaced by *******.
    /// 
    /// Usage:
    /// 
    /// [return: TraceCoreSecret]
    /// public string Test5(string p1, [TraceCoreSecret] string p2, [TraceCoreSecret] out int p3, [TraceCoreSecret] ref string p4)
    /// {
    ///     p3 = 1;
    ///     return string.Format("{0}-{1}-{2}-{3}", p1, p2, p3, p4);
    /// }
    /// 
    /// [TraceCoreSecret]
    /// public string Prop1 {get; set;}
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Property)]
    public class TraceSecretCoreAttribute : Attribute
    {
    }
}
