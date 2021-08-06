using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using log4net;

namespace ITA.Common.WCF
{
    /// <summary>
    /// Generic wrapper class over ChannelFactory. 
    /// This class creates channels of different types that are used by clients to send messages to variously configured service endpoints
    /// </summary>
    /// <typeparam name="TChannel">Service contract class type</typeparam>
    public class ChannelFactoryWrapper<TChannel> where TChannel : class
    {
        private ChannelFactory<TChannel> _factory;

        private readonly ILog _logger = Log4NetItaHelper.GetLogger("ChannelFactoryWrapper");

        public ChannelFactoryWrapper()
        {
            _factory = new ChannelFactory<TChannel>();
        }

        public ChannelFactoryWrapper(string endpointConfigurationName)
        {
            _factory = new ChannelFactory<TChannel>(endpointConfigurationName);
        }

        public ChannelFactoryWrapper(string endpointConfigurationName, EndpointAddress endpointAddress)
        {
            _factory = new ChannelFactory<TChannel>(endpointConfigurationName, endpointAddress);
        }

        public ChannelFactoryWrapper(Binding binding, EndpointAddress endpointAddress)
        {
            _factory = new ChannelFactory<TChannel>(binding, endpointAddress);
        }

        /// <summary>
        /// Channel credentials
        /// </summary>
        public ClientCredentials Credentials
        {
            get { return _factory.Credentials; }
        }

        /// <summary>
        /// Service end-point
        /// </summary>
        public ServiceEndpoint FactoryEndpoint
        {
            get
            {
                return _factory.Endpoint;
            }
        }

        /// <summary>
        /// Service method execute with no return value.
        /// </summary>
        /// <param name="action">Action to execute</param>
        public void Execute(Action<TChannel> action)
        {
            var proxy = default(TChannel);
            try
            {
                proxy = _factory.CreateChannel();
                ((IClientChannel)proxy).Open();
                action(proxy);
                ((IClientChannel)proxy).Close();
            }
            catch (Exception)
            {
                if (proxy != null)
                {
                    ((IClientChannel) proxy).Abort();
                }
                throw;
            }
        }

        /// <summary>
        /// Service method execute with return value.
        /// </summary>
        /// <typeparam name="TResult">Return value type</typeparam>
        /// <param name="action">Action to exeute</param>
        /// <returns>Action returned value</returns>
        public TResult Execute<TResult>(Func<TChannel, TResult> action)
        {
            var proxy = default(TChannel);
            TResult result;
            try
            {
                proxy = _factory.CreateChannel();
                ((IClientChannel)proxy).Open();
                result = action(proxy);
                ((IClientChannel)proxy).Close();
            }
            catch (Exception)
            {
                if (proxy != null)
                {
                    ((IClientChannel) proxy).Abort();
                }
                throw;
            }
            return result;
        }

        /// <summary>
        /// Service channell established flag 
        /// </summary>
        public bool Connected
        {
            get
            {
                bool result = false;
                var proxy = default(TChannel);
                try
                {
                    proxy = _factory.CreateChannel();
                    _logger.Debug("Channel created");

                    ((IClientChannel)proxy).Open();
                    _logger.Debug("Channel opened");

                    ((IClientChannel)proxy).Close();
                    _logger.Debug("Channel closed");

                    result = true;
                }
                catch (Exception e)
                {
                    _logger.Error("Checking failed", e);

                    if (proxy != null)
                    {
                        ((IClientChannel)proxy).Abort();
                    }
                }
                return result;
            }
        }
    }
}
