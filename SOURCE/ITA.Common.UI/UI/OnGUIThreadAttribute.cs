using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ITA.Common.Tracing;
using log4net;
using MethodDecorator.Fody.Interfaces;
using MethodDecorator.Fody.Interfaces.Aspects;

namespace ITA.Common.UI
{
    // NOTE: In case of cloning postsharp attributes DO NOT FORGET to add new attributes to Obfuscation exclusion list!!
    /// <summary>
    /// Apply this attribute to execute code on WinForms UI context. 
    /// </summary>
    /// <remarks>Class containing attributed method must be inherited from Windows.Forms.Control!</remarks>
    [Serializable]
    [ProvideAspectRole(StandardRoles.Threading)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    [MulticastAttributeUsage(MulticastTargets.Method, AllowMultiple = false)]
    public class OnGUIThreadAttribute : MethodDecoratorBase, IPartialDecoratorExit
    {
        /// <summary>
        /// <see cref="ILog"/> instance that is used for logging
        /// </summary>
        [NonSerialized]
        private ILog logger;

        protected object _returnValue;

        protected bool _invoked = false;

        /// <summary>
        /// Logger name. 
        /// </summary>
        /// <remarks>We set this field on CompileInitialize, before obfuscation methods mangle methods and class names.</remarks>
        private string loggerName;

        /// <summary>Creates log4net logger for <paramref name="method"/> method.</summary>
        /// <param name="method"><see cref="MethodBase"/> instance that contains metadata of an annotated method.</param>
        public void RuntimeInitialize(MethodBase method)
        {
            if (!typeof(Control).IsAssignableFrom(method.DeclaringType))
            {
                Trace.TraceError("OnGUIThreadAttribute can be applied only to inheritors of System.Windows.Forms.Control. Instead it was applied to {0}", method.DeclaringType);
                return;
            }

            LogHelper.UpdateLoggerName(ref loggerName, method);

            logger = LogHelper.GetLogger(loggerName, method);
        }

        public bool Init(object instance, MethodBase method, object[] args)
        {
            this.logger.DebugFormat("OnGUIThreadAttribute: Init start. ThreadId: {0}", Thread.CurrentThread.ManagedThreadId);

            var control = instance as Control;
            
            if (control == null)
            {
                this.logger.Error("OnGUIThreadAttribute applied to object which is not inherited from System.Windows.Forms.Control");
            }
            else if (!control.IsHandleCreated)
            {
                this.logger.Error(String.Format("Handle for control '{0}' is not created yet! There is a bug in program code.", control.Name));
            }
            else if (control.InvokeRequired)
            {
                this.logger.DebugFormat("OnGUIThreadAttribute: Invoke is required, calling Invoke of method {0} on myself.", method.Name);
                control.Invoke((MethodInvoker)delegate {
                    this.logger.Debug("OnGUIThreadAttribute: Method invoke begin");
                    _returnValue = method.Invoke(instance, args.Length > 0 ? args : null);
                    this.logger.Debug("OnGUIThreadAttribute: Method invoke end"); });
                _invoked = true;
                return false;
            }

            this.logger.Debug("OnGUIThreadAttribute: Init returns true");

            return true;
        }

        public void OnExit()
        {
            _invoked = false;

            this.logger.DebugFormat("OnGUIThreadAttribute: OnExit, _invoked={0}", _invoked);
        }

        public object AlterRetval(object retval)
        {
            if (_invoked)
            {
                this.logger.DebugFormat("Invoke was forced. Retval is {0}", _returnValue.ToString());
            }

            this.logger.DebugFormat("OnGUIThreadAttribute: AlterRetval, _invoked={0}, _returnValue={1}, retval={2}", _invoked, _returnValue, retval);

            return _invoked ? _returnValue : retval;
        }
    }
}
