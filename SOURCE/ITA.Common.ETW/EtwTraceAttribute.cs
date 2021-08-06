using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using MethodDecorator.Fody.Interfaces.Aspects;

namespace ITA.Common.ETW
{
    [Serializable]
    [ProvideAspectRole(StandardRoles.Tracing)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    [MulticastAttributeUsage(MulticastTargets.InstanceConstructor | MulticastTargets.StaticConstructor | MulticastTargets.Method, AllowMultiple = false)]
    public sealed class EtwTraceAttribute : OnMethodBoundaryAspect
    {
        #region Consts
        
        private const string NULL_VALUE = "(null)";
        private const string VOID_VALUE = "void";
        private const string INPUT_VALUE = " >>> ";
        private const string OUTPUT_VALUE = " <<< ";
        private const string KEY_VALUE_FORMAT = "{0}=\"{1}\" ";
        private const string PARAM_TYPE_FORMAT = "Type=\"{0}\" ";

        #endregion

        /// <summary>
        /// Input parameters without "out"
        /// </summary>
        private EtwParamCollection _inParams = new EtwParamCollection();

        /// <summary>
        /// Out parameters and return value
        /// </summary>
        private EtwReturnParamCollection _outParams = new EtwReturnParamCollection();

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var currentParam = parameters[i];
                var etwParam = new EtwParam
                {
                    Index = i,
                    Name = parameters[i].Name,
                    IsByRef = parameters[i].ParameterType.IsByRef
                };

                if (!currentParam.IsOut)
                {
                    _inParams.Parameters.Add(etwParam);

                    if (etwParam.IsByRef)
                    {
                        _outParams.Parameters.Add(etwParam);
                    }
                }
                else
                {
                    _outParams.Parameters.Add(etwParam);
                }

                // Check EtwTraceParamAttribute                
                var etwFieldsAttributes = currentParam.GetCustomAttributes(typeof(EtwTraceParamAttribute), false);
                if (etwFieldsAttributes != null && etwFieldsAttributes.Any())
                {
                    var propertyNames = string.Join(";", etwFieldsAttributes.OfType<EtwTraceParamAttribute>().Select(a => a.FieldNames)).Split(';');

                    etwParam.ParameterType = currentParam.ParameterType;
                    etwParam.Properties = propertyNames.Select(name => etwParam.ParameterType.GetProperty(name)).ToArray();
                }
            }
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            try
            {
                var eventSource = GetEventSource(args);
                var isVerbose = eventSource.IsEnabled(EventLevel.Verbose, StaticEventSource.Keywords.Trace);
                if (isVerbose)
                {
                    UpdateEtwParametersValue(_inParams, args);
                    eventSource.Start(args.Method.Name, _inParams.ToString());
                }
                else
                {
                    eventSource.Start(args.Method.Name, INPUT_VALUE);
                }                
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public override void OnExit(MethodExecutionArgs args, object returnValue)
        {
            var eventSource = GetEventSource(args);
            var isVerbose = eventSource.IsEnabled(EventLevel.Verbose, StaticEventSource.Keywords.Trace);
            if (isVerbose)
            {
                var methodInfo = args.Method as MethodInfo;
                var hasReturnValue = methodInfo != null && methodInfo.ReturnType != typeof(void);
                if (hasReturnValue)
                {
                    _outParams.ReturnValue = GetArgumentValue(returnValue);
                }
                else
                {
                    _outParams.ReturnValue = VOID_VALUE;
                }

                UpdateEtwParametersValue(_outParams, args);

                eventSource.Stop(args.Method.Name, _outParams.ToString());
            }
            else
            {
                eventSource.Stop(args.Method.Name, OUTPUT_VALUE);
            }
        }

        public override void OnException(MethodExecutionArgs args, Exception exception)
        {
            try
            {
                var eventSource = GetEventSource(args);
                var isVerbose = eventSource.IsEnabled(EventLevel.Verbose, StaticEventSource.Keywords.Trace);
                var exceptionInfo = new EtwExceptionInfo
                {
                    Message = exception.Message,
                    StackTrace = isVerbose ? exception.StackTrace : string.Empty
                };
                eventSource.Fail(args.Method.Name, exceptionInfo.ToString());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        private IStaticEventSource GetEventSource(MethodExecutionArgs args)
        {
            var source = args.Instance as IEventSourceProvider;

            return source != null ? source.GetEventSource() : StaticEventSource.Log;
        }

        private void UpdateEtwParametersValue(EtwParamCollection list, MethodExecutionArgs args)
        {
            foreach (var item in list.Parameters)
            {
                item.Value = GetArgumentValue(args.Arguments[item.Index], item);
            }
        }

        private string GetArgumentValue(object value, EtwParam param = null)
        {
            if (value == null)
            {
                return NULL_VALUE;
            }

            var etwInfo = value as IEtwInformation;
            if (etwInfo != null)
            {
                return etwInfo.GetEtwInformation();
            }

            if (param == null)
            {
                return value.ToString();
            }

            if (param.Properties != null)
            {
                var builder = new StringBuilder();
                builder.AppendFormat(PARAM_TYPE_FORMAT, param.ParameterType.Name);
                foreach (var property in param.Properties)
                {
                    var propValue = property.GetValue(value, null);
                    builder.AppendFormat(KEY_VALUE_FORMAT, property.Name, propValue == null ? NULL_VALUE : propValue.ToString());
                }
                return builder.ToString();
            }
            else
            {
                return value.ToString();
            }
        }
    }
}