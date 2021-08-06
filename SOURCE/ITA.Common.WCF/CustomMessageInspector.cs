using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace ITA.Common.WCF
{
    /// <summary>
    /// Base custom message inspector for WCF-service.
    /// This class is used for inspect or modify inbound or outbound application messages 
    /// either prior to dispatching a request message to an operation or before returning a reply message to a caller.
    /// </summary>
    public class CustomMessageInspector : IDispatchMessageInspector
    {
        public virtual object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            GetOriginalMessage(ref request);
            return null;
        }

        protected static void WriteHeaderToOperationContext(Message originalMessage, string headerId, string headerNs)
        {
            int headerIndex = originalMessage.Headers.FindHeader(headerId, headerNs);

            if (headerIndex >= 0)
            {
                var header = originalMessage.Headers.GetHeader<string>(headerIndex);
                OperationContext.Current.IncomingMessageProperties[headerId] = header;
            }
        }

        public virtual void BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        protected static Message GetOriginalMessage(ref Message request)
        {
            MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            request = buffer.CreateMessage();
            Message originalMessage = buffer.CreateMessage();
            return originalMessage;
        }
    }
}
