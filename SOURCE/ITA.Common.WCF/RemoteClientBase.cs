using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using log4net;

namespace ITA.Common.WCF
{
    /// <summary>
    /// Abstract base class for wcf-service proxy client
    /// </summary>
    public abstract class RemoteClientBase
    {
        protected static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(RemoteClientBase).Name);
        /// <summary>
        /// Service uri
        /// </summary>
        protected string Uri { get; private set; }
        /// <summary>
        /// Service binding
        /// </summary>
        protected Binding BaseBinding { get; private set; }
        /// <summary>
        /// Service endpoint
        /// </summary>
        protected EndpointAddress EndPoint { get; private set; }
        /// <summary>
        /// Service connection established flag 
        /// </summary>
        public abstract bool Connected { get; }
        /// <summary>
        /// Service connection credentials
        /// </summary>
        public abstract ClientCredentials Credentials { get; }
        /// <summary>
        /// Binding and endpoint initializing by uri
        /// </summary>
        /// <param name="uri">Service uri</param>
        /// <param name="options">Binding options</param>
        protected void InitializeByUri(string uri, BindingOptions options)
        {
            logger.DebugFormat("Url: {0}", uri);
            Uri = uri;
            BaseBinding = BindingHelper.CreateBindingByUri(uri, options);
            EndPoint = new EndpointAddress(uri);
        }
    }
    /// <summary>
    /// Generic base class for wcf-service proxy client
    /// </summary>
    /// <typeparam name="T">Wcf-service contract type</typeparam>
    public class RemoteClientBase<T> : RemoteClientBase where T : class
    {
        protected ChannelFactoryWrapper<T> m_factoryWrapper;

        public RemoteClientBase(string uri)
            : this(uri, BindingOptions.Default)
        {
        }

        public RemoteClientBase(string uri, BindingOptions options)
        {
            InitializeByUri(uri, options);
            m_factoryWrapper = new ChannelFactoryWrapper<T>(BaseBinding, EndPoint);
        }
        /// <summary>
        /// Service connection established flag 
        /// </summary>
        public override bool Connected
        {
            get { return m_factoryWrapper.Connected; }
        }
        /// <summary>
        /// Service connection credentials
        /// </summary>
        public override ClientCredentials Credentials
        {
            get { return m_factoryWrapper.Credentials; }
        }

        /// <summary>
        /// Service contract operation executing with validation
        /// </summary>
        /// <typeparam name="TResult">Operation return type</typeparam>
        /// <param name="action">Operation for execute</param>
        /// <returns>The value of operation return type.</returns>
        protected TResult DoWithValidation<TResult>(Func<T, TResult> action)
        {
            try
            {
                return m_factoryWrapper.Execute(action);
            }
            catch (TimeoutException timeProblem)
            {
                logger.Error("TimeOut:", timeProblem);
                throw new WCFException(WCFException.E_CLIENT_TIMEOUT_ERROR, timeProblem);
            }
            catch (FaultException<ServiceExceptionDetail> faultException)
            {
                if (faultException.Detail != null)
                {
                    logger.Error("Inner fault exception: " + faultException.Detail.Message);

                    if (faultException.Detail.StackTrace != null)
                    {
                        logger.Error(faultException.Detail.StackTrace);
                    }

                    throw;
                }
                else
                {
                    logger.Error("Fault exception: ", faultException);
                    throw;
                }
            }
            catch (FaultException x)
            {
                logger.Error("Generic FaultException", x);
                throw;
            }
            catch (CommunicationException commProblem)
            {
                logger.Error("Communication Error", commProblem);
                throw new WCFException(WCFException.E_CLIENT_CONNECTION_ERROR, commProblem);
            }
        }
        /// <summary>
        /// Service contract operation executing with validation included product specifically validation
        /// </summary>
        /// <typeparam name="TProductExceptionDetail">Product specifically detailed exception class</typeparam>
        /// <typeparam name="TResult">Operation return type</typeparam>
        /// <param name="action">Operation for execute</param>
        /// <returns>The value of operation return type.</returns>
        protected TResult DoWithValidation<TProductExceptionDetail, TResult>(Func<T, TResult> action)
            where TProductExceptionDetail : class 
        {
            try
            {
                return m_factoryWrapper.Execute(action);
            }
            catch (TimeoutException timeProblem)
            {
                logger.Error("TimeOut:", timeProblem);
                throw new WCFException(WCFException.E_CLIENT_TIMEOUT_ERROR, timeProblem);
            }
            catch (FaultException<ServiceExceptionDetail> faultException)
            {
                if (faultException.Detail != null)
                {
                    logger.Error("Inner fault exception: " + faultException.Detail.Message);

                    if (faultException.Detail.StackTrace != null)
                    {
                        logger.Error(faultException.Detail.StackTrace);
                    }

                    throw;
                }
                else
                {
                    logger.Error("Fault exception: ", faultException);
                    throw;
                }
            }
            catch (FaultException<TProductExceptionDetail> x)
            {
                logger.Error(string.Format("{0} FaultException", typeof(TProductExceptionDetail).Name), x);
                throw;
            }
            catch (FaultException x)
            {
                logger.Error("Generic FaultException", x);
                throw;
            }
            catch (CommunicationException commProblem)
            {
                logger.Error("Communication Error", commProblem);
                throw new WCFException(WCFException.E_CLIENT_CONNECTION_ERROR, commProblem);
            }
        }
        /// <summary>
        /// Service contract no value returned operation executing with validation
        /// </summary>
        /// <param name="action">Operation for execute</param>
        protected void DoWithValidation(Action<T> action)
        {
            try
            {
                m_factoryWrapper.Execute(action);
            }
            catch (TimeoutException timeProblem)
            {
                logger.Error("TimeOut:", timeProblem);
                throw new WCFException(WCFException.E_CLIENT_TIMEOUT_ERROR, timeProblem);
            }
            catch (FaultException<ServiceExceptionDetail> faultException)
            {
                if (faultException.Detail != null)
                {
                    logger.Error("Inner fault exception: " + faultException.Detail.Message);

                    if (faultException.Detail.StackTrace != null)
                    {
                        logger.Error(faultException.Detail.StackTrace);
                    }

                    throw;
                }
                else
                {
                    logger.Error("Fault exception: ", faultException);
                    throw;
                }
            }
            catch (FaultException x)
            {
                logger.Error("Generic FaultException", x);

                throw;
            }
            catch (CommunicationException commProblem)
            {
                logger.Error("Communication Error", commProblem);
                throw new WCFException(WCFException.E_CLIENT_CONNECTION_ERROR, commProblem);
            }
        }
    }
}
