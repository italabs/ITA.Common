using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ITA.Common.Microservices.Logging;
using MethodDecorator.Fody.Interfaces.Aspects;
using Microsoft.Extensions.Logging;

namespace ITA.Common.Microservices.Tracing
{
    [Serializable]
    [ProvideAspectRole(StandardRoles.Tracing)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property, Inherited = false)]
    [MulticastAttributeUsage(MulticastTargets.InstanceConstructor | MulticastTargets.StaticConstructor | MulticastTargets.Method, AllowMultiple = false)]
    public sealed class TraceCoreAttribute : OnMethodBoundaryAspect
    {
        private const string GET_ACCESSOR_NAME = "get_";
        private const string SET_ACCESSOR_NAME = "set_";
        public const string SECRET_PATTERN = "*******";

        /// <summary>
        /// Enables ignoring of ThreadAbortException caused by calling Response.Redirect and other similar ASP.NET methods.
        /// </summary>
        public bool ASPNETMode { get; set; }

        public TraceCoreAttribute()
        {
            _log = new Lazy<ILogger>(CreateLogger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerName">Explicit logger name.</param>
        public TraceCoreAttribute(string loggerName)
            : this()
        {
            this._loggerName = loggerName;
        }

        /// <summary>
        /// Argument list message template.
        /// </summary>
        private string _argumentListMessage;

        /// <summary>
        /// Boolean array that contains flag if corresponding parameter is an out-parameter.
        /// </summary>
        private bool[] _isArgumentOut;

        /// <summary>
        /// Boolean array that contains flag if corresponding parameter is secret parameter.
        /// </summary>
        private bool[] _isSecretArgument;

        /// <summary>
        /// Method return value has secret attribute
        /// </summary>
        private bool _isReturnSecret;

        /// <summary>
        /// Method's entry message template.
        /// </summary>
        private string _entryMessage;

        /// <summary>
        /// Error message template.
        /// </summary>
        private string _exceptionMessage;

        /// <summary>
        /// <see cref="ILogger"/> instance that is used for logging
        /// </summary>
        [NonSerialized]
        private Lazy<ILogger> _log;

        /// <summary>
        /// Logger name. 
        /// </summary>
        /// <remarks>We set this field on CompileInitialize, before obfuscation methods mangle methods and class names.</remarks>
        private string _loggerName;

        /// <summary>
        /// Template for tracing 'out' parameters.
        /// </summary>
        private string _outResultsMessage;

        /// <summary>
        /// Template for tracing 'ref' parameters.
        /// </summary>
        private string _refResultsMessage;

        /// <summary>
        /// Method's exit message template.
        /// </summary>
        private string _successMessage;

        /// <summary>
        /// Writes to log a method's entry message and method's input arguments.
        /// </summary>
        /// <param name="inArgs"><see cref="MethodExecutionArgs"/> instance that contains current invocation state of method.</param>
        public override void OnEntry(MethodExecutionArgs inArgs)
        {
            try
            {
                if (_log == null)
                    return;

                if (_log.Value.IsEnabled(LogLevel.Trace))
                {
                    string szOnEntryMessage = _entryMessage;

                    if (inArgs.Arguments.Length != 0)
                    {
                        var argumentsList = new List<object>();

                        //RendererMap rendererMap = _log.Logger.Repository.RendererMap;
                        for (int i = 0; i < inArgs.Arguments.Length; i++)
                        {
                            string value = _isArgumentOut[i] ? "(it's an out-parameter, it doesn't have a value)" : inArgs.Arguments[i]?.ToString(); //rendererMap.FindAndRender(inArgs.Arguments[i]);
                            value = _isSecretArgument[i] ? SECRET_PATTERN : value;
                            argumentsList.Add(value);
                        }

                        szOnEntryMessage += string.Format(CultureInfo.InvariantCulture, _argumentListMessage, argumentsList.ToArray());
                    }

                    _log.Value.LogTrace(szOnEntryMessage);
                }
                else
                {
                    _log.Value.LogDebug(_entryMessage);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        /// <summary>
        /// Writes to log exception details.
        /// </summary>
        /// <param name="inArgs"><see cref="MethodExecutionArgs"/>instance that contains current invocation state of method and its exeption.</param>
        /// <param name="exception"></param>
        public override void OnException(MethodExecutionArgs inArgs, Exception exception)
        {
            try
            {
                if (_log == null)
                    return;

                if (!_log.Value.IsEnabled(LogLevel.Error))
                    return;


                if (ASPNETMode)
                {
                    ThreadAbortException e = exception as ThreadAbortException;
                    // Also we can check for e.ExceptionState != null && e.ExceptionState.GetType().Name == "CancelModuleException" 
                    // but this is also unsupported (because CancelModuleException is internal), similar to stack trace check
                    if (e != null && e.StackTrace.Contains("System.Web.HttpResponse.End()"))
                    {
                        _log.Value.LogTrace("Catched Thread.Abort exception thrown by ASP.NET engine");
                        // Almost ignoring Thread.Abort thrown by ASP.NET engine
                        return;
                    }

                }

                _log.Value.LogError(exception, _exceptionMessage);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Writes to log a method's exit message.
        /// </summary>
        /// <param name="inArgs"><see cref="MethodExecutionArgs"/> instance that contains current invocation state of method.</param>
        /// <param name="returnValue"></param>
        public override void OnExit(MethodExecutionArgs inArgs, object returnValue)
        {
            try
            {
                if (_log == null)
                    return;

                if (_log.Value.IsEnabled(LogLevel.Trace))
                {
                    string szOnSuccessMessage = _successMessage;
                    //
                    // Dump return value if any
                    //
                    var methodInfo = inArgs.Method as MethodInfo;
                    var hasReturnValue = methodInfo != null && methodInfo.ReturnType != typeof(void);
                    if (hasReturnValue)
                    {
                        var returnValueStr = returnValue ?? "(null)";
                        if (_isReturnSecret)
                        {
                            returnValueStr = SECRET_PATTERN;
                        }
                        szOnSuccessMessage += string.Format(" : {0}", returnValueStr);
                    }
                    //
                    // Dump Out and Ref parameters if any
                    // TODO: there is no need to check this in runtime.
                    //       we can find that out in compile time and be faster as most methods have no Out and Ref args.
                    //
                    if (inArgs.Arguments.Length > 0)
                    {
                        //RendererMap rendererMap = _log.Logger.Repository.RendererMap;

                        List<object> outResults = new List<object>();
                        List<object> refResults = new List<object>();

                        ParameterInfo[] parameters = inArgs.Method.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].IsOut)
                            {
                                string value = _isSecretArgument[i] ? SECRET_PATTERN : inArgs.Arguments[i]?.ToString(); //rendererMap.FindAndRender(inArgs.Arguments[i]);
                                outResults.Add(value);
                            }
                            else if (parameters[i].ParameterType.IsByRef)
                            {
                                string value = _isSecretArgument[i] ? SECRET_PATTERN : inArgs.Arguments[i]?.ToString(); //rendererMap.FindAndRender(inArgs.Arguments[i]);
                                refResults.Add(value);
                            }
                        }
                        if (outResults.Count > 0)
                        {
                            szOnSuccessMessage += string.Format(CultureInfo.InvariantCulture, _outResultsMessage, outResults.ToArray());
                        }
                        if (refResults.Count > 0)
                        {
                            szOnSuccessMessage += string.Format(CultureInfo.InvariantCulture, _refResultsMessage, refResults.ToArray());
                        }
                    }

                    _log.Value.LogTrace(szOnSuccessMessage);
                }
                else
                {
                    _log.Value.LogDebug(_successMessage);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        /// <summary>
        /// Initializes message templates that will be used for logging.
        /// </summary>
        /// <param name="inMethod"> <see cref="MethodBase"/> instance that contains metadata of an annotated method.</param>
        /// <param name="inAspectInfo">For further use.</param>
        public override void CompileTimeInitialize(MethodBase inMethod, AspectInfo inAspectInfo)
        {
            UpdateLoggerName(ref _loggerName, inMethod);

            _entryMessage = String.Format(CultureInfo.InvariantCulture, "{0} () >>>", inMethod.Name);
            _successMessage = String.Format(CultureInfo.InvariantCulture, "{0} () <<<", inMethod.Name);
            _exceptionMessage = String.Format(CultureInfo.InvariantCulture, "{0} () failed:", inMethod.Name);

            var argumentListMessageBuilder = new StringBuilder();
            argumentListMessageBuilder.Append("\n\tParameters:");
            ParameterInfo[] parameters = inMethod.GetParameters();

            _isArgumentOut = new bool[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                argumentListMessageBuilder.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "\n\t\t{0} = {{{1}}}",
                    parameters[i].Name,
                    i);

                _isArgumentOut[i] = parameters[i].IsOut;
            }
            _argumentListMessage = argumentListMessageBuilder.ToString();

            var outArgumentListMessageBuilder = new StringBuilder();
            outArgumentListMessageBuilder.Append("\n\tOut parameters results:");
            int paramIdx = 0;
            int outParamIdx = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.IsOut)
                {
                    outArgumentListMessageBuilder.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "\n\t\t{0} = {{{1}}}",
                        parameters[paramIdx].Name,
                        outParamIdx++);
                }
                paramIdx++;
            }
            _outResultsMessage = outArgumentListMessageBuilder.ToString();

            var refArgumentListMessageBuilder = new StringBuilder();
            refArgumentListMessageBuilder.Append("\n\tRef parameters results:");
            paramIdx = 0;
            int refParamIdx = 0;
            foreach (var parameter in parameters)
            {
                if (!parameter.IsOut && parameter.ParameterType.IsByRef)
                {
                    refArgumentListMessageBuilder.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "\n\t\t{0} = {{{1}}}",
                        parameters[paramIdx].Name,
                        refParamIdx++);
                }
                paramIdx++;
            }
            _refResultsMessage = refArgumentListMessageBuilder.ToString();

            _isSecretArgument = new bool[parameters.Length];
            var secretAttributeType = typeof(TraceSecretCoreAttribute);
            for (int i = 0; i < parameters.Length; i++)
            {
                var secretAttributes = parameters[i].GetCustomAttributes(secretAttributeType, false);
                _isSecretArgument[i] = secretAttributes.Any();
            }

            var methodInfo = inMethod as MethodInfo;
            if (methodInfo != null)
            {
                _isReturnSecret = methodInfo.ReturnTypeCustomAttributes.IsDefined(secretAttributeType, false);
            }

            // Check property by get_ set_ accessors
            var isGetAccessor = inMethod.Name.StartsWith(GET_ACCESSOR_NAME);
            var isSetAccessor = inMethod.Name.StartsWith(SET_ACCESSOR_NAME);
            if (inMethod.IsSpecialName && (isGetAccessor || isSetAccessor) && inMethod.DeclaringType != null)
            {
                // Get real property name
                var propName = string.Empty;
                if (isGetAccessor)
                {
                    propName = inMethod.Name.Substring(GET_ACCESSOR_NAME.Length);
                }
                if (isSetAccessor)
                {
                    propName = inMethod.Name.Substring(SET_ACCESSOR_NAME.Length);
                }

                var propInfo = inMethod.DeclaringType.GetProperties().FirstOrDefault(p => p.Name.Equals(propName));
                if (propInfo != null)
                {
                    var secretPropAttribute = propInfo.GetCustomAttributes(typeof(TraceSecretCoreAttribute), false);
                    var isPropertySecret = secretPropAttribute.Length > 0;

                    if (isSetAccessor)
                    {
                        _isSecretArgument[0] = isPropertySecret;
                    }
                    if (isGetAccessor)
                    {
                        _isReturnSecret = isPropertySecret;
                    }
                }
            }
        }

        private ILogger CreateLogger()
        {
            var loggerFactory = AppliedLoggerFactoryInstance.GetFactory();

            return !string.IsNullOrWhiteSpace(_loggerName) 
                ? loggerFactory.CreateLogger(_loggerName) 
                : loggerFactory.CreateLogger<TraceCoreAttribute>();
        }

        /// <summary>
        /// Updates logger name based on <paramref name="method"/> object.
        /// </summary>
        /// <param name="loggerName">If it is not null then method does nothing.</param>
        /// <param name="method">Method information from which type name is extracted and returned as logger name.</param>
        private static void UpdateLoggerName(ref string loggerName, MethodBase method)
        {
            if (!String.IsNullOrEmpty(loggerName))
            {
                Trace.TraceInformation("loggerName != IsNullOrEmpty, but == {0}", loggerName);
                return;
            }

            if (method != null && method.DeclaringType != null)
            {
                loggerName = method.DeclaringType.Name;
                Trace.TraceInformation("loggerName determined successfully: {0}", loggerName);
            }
            else
            {
                try
                {
                    Trace.TraceError("Either method or method.DeclaringType are null!");

                    loggerName = new StackFrame(1).GetMethod().DeclaringType.Name;
                }
                catch (Exception e)
                {
                    Trace.TraceError("Can't get type name from stack: {0}", e.Message);

                    loggerName = "Possibly_fody_attribute";
                }
            }
        }
    }
}
