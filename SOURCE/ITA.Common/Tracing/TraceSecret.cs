using System;

namespace ITA.Common.Tracing
{
    /// <summary>
    /// Attribute used to mark the input and output parameters of the function. 
    /// If the function parameter is checked this attribute, the trace output values 
    /// of this parameter will be replaced by *******.
    /// 
    /// Usage:
    /// 
    /// [return: TraceSecret]
    /// public string Test5(string p1, [TraceSecret] string p2, [TraceSecret] out int p3, [TraceSecret] ref string p4)
    /// {
    ///     p3 = 1;
    ///     return string.Format("{0}-{1}-{2}-{3}", p1, p2, p3, p4);
    /// }
    /// 
    /// [TraceSecret]
    /// public string Prop1 {get; set;}
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Property)]
    public class TraceSecret : Attribute
    {
    }
}