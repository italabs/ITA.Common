using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using ITA.Common.WCF.RESTHelp;

namespace ITA.Common.WCF.RestHelp
{
    internal class HelpPageInvoker : IOperationInvoker
    {
        private readonly HelpViewResolver _viewResolver;

        public HelpPageInvoker(HelpViewResolver resolver)
        {
            _viewResolver = resolver;
        }

        public object[] AllocateInputs()
        {
            return new object[1];
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = new object[0];
            var inputMessage = (Message) inputs[0];
            return _viewResolver.Resolve(inputMessage.Headers.To);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw new NotSupportedException();
        }

        public bool IsSynchronous
        {
            get { return true; }
        }
    }
}
