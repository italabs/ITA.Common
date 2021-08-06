using System;
using System.DirectoryServices.AccountManagement;
using System.Text;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.WCF
{
    /// <summary>
    /// Base configuration class for WCF services. 
    /// WCF-service configuration must inherit that class.
    /// </summary>
    public abstract class WcfServiceConfigBase : IWcfServiceConfig
    {
        protected static string METADATA_PARAM = "Metadata";
        protected static string ADDRESS_PARAM = "Address";
        protected static string MAX_CUNCURRENT_REQUESTS_PARAM = "MaxConcurrentRequests";
        protected static string THUMBPRINT_PARAM = "Thumbprint";
        protected static string SECURITY_TYPE_PARAM = "SecurityType";
        protected static string RELIABLE_SESSION_PARAM = "ReliableSession";
        protected static string OPEN_TIMEOUT_PARAM = "OpenTimeoutSec";
        protected static string SEND_TIMEOUT_PARAM = "SendTimeoutSec";
        protected static string RECEIVE_TIMEOUT_PARAM = "ReceiveTimeoutSec";
        protected static string AUTHORIZE_AS_GROUP_MEMBER_PARAM = "AuthorizeAsGroupMember";
        protected static string AUTHORIZATION_GROUP_STORE_PARAM = "AuthorizationGroupStore";
        protected static string RETRIEVE_ERRORS_STACK_TRACE_PARAM = "RetrieveErrorStackTrace";
        protected static string PROTOCOL_VERSION_PARAM = "ProtocolVersion";
        protected static string MAX_RECEIVED_MESSAGE_SIZE_PARAM = "MaxReceivedMessageSize";

        protected IConfigManager _configManager;

        protected WcfServiceConfigBase(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        /// <summary>
        /// Component name in the configuration storage
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Default service Uri
        /// </summary>
        public abstract string DefaultAddress { get; }
        /// <summary>
        /// Default <see cref="MaxConcurrentRequests">MaxConcurrentRequests</see> parameter value
        /// </summary>
        public virtual int DefaultMaxConcurrentRequests { get { return 1000; } }
        /// <summary>
        /// Default <see cref="Metadata">Metadata</see> parameter value
        /// </summary>
        public virtual bool DefaultMetadata { get { return false; } }
        /// <summary>
        /// Default <see cref="ReliableSession">ReliableSession</see> parameter value
        /// </summary>
        public virtual bool DefaultReliableSession { get { return false; } }
        /// <summary>
        /// Default <see cref="OpenTimeout">OpenTimeout</see> parameter value
        /// </summary>
        public virtual TimeSpan DefaultOpenTimeout { get { return TimeSpan.FromSeconds(30); } }
        /// <summary>
        /// Default <see cref="SendTimeout">SendTimeout</see> parameter value
        /// </summary>
        public virtual TimeSpan DefaultSendTimeout { get { return TimeSpan.FromSeconds(30); } }
        /// <summary>
        /// Default <see cref="ReceiveTimeout">ReceiveTimeout</see> parameter value
        /// </summary>
        public virtual TimeSpan DefaultReceiveTimeout { get { return TimeSpan.FromSeconds(30); } }
        /// <summary>
        /// Default <see cref="SecurityType">SecurityType</see> parameter value
        /// </summary>
        public virtual SecurityType DefaultSecurityType { get { return SecurityType.Windows; } }
        /// <summary>
        /// Default <see cref="AuthorizeAsGroupMember">AuthorizeAsGroupMember</see> parameter value
        /// </summary>
        public virtual string DefaultAuthorizeAsGroupMember { get { return null; } }
        /// <summary>
        /// Default <see cref="AuthorizationGroupStore">AuthorizationGroupStore</see> parameter value
        /// </summary>
        public virtual ContextType DefaultAuthorizationGroupStore { get { return ContextType.Machine; } }
        /// <summary>
        /// Default <see cref="RetrieveErrorStackTrace">RetrieveErrorStackTrace</see> parameter value
        /// </summary>
        public virtual bool DefaultRetrieveErrorStackTrace { get { return true; } }
        /// <summary>
        /// Default <see cref="ProtocolVersion">ProtocolVersion</see> parameter value
        /// </summary>
        public virtual string DefaulProtocolVersion { get { return null; } }
        /// <summary>
        /// Default <see cref="ProtocolVersion">ProtocolVersion</see> parameter value
        /// </summary>
        public virtual long DefaultMaxReceivedMessageSize { get {return BindingOptions.Size_5Mb; }}
        
        /// <summary>
        /// Write default parameters to storage if it's not exists
        /// </summary>
        public virtual void CommitRequiredParameters()
        {
            // Address
            if (_configManager[Name, ADDRESS_PARAM, null] == null)
            {
                _configManager[Name, ADDRESS_PARAM] = DefaultAddress;
            }

            // SecurityType
            if (_configManager[Name, SECURITY_TYPE_PARAM, null] == null)
            {
                _configManager[Name, SECURITY_TYPE_PARAM] = DefaultSecurityType.ToString();
            }

            // AuthorizeAsGroupMember
            if (_configManager[Name, AUTHORIZE_AS_GROUP_MEMBER_PARAM, null] == null && DefaultAuthorizeAsGroupMember != null)
            {
                _configManager[Name, AUTHORIZE_AS_GROUP_MEMBER_PARAM] = DefaultAuthorizeAsGroupMember;
            }
        }

        /// <summary>
        /// WCF-service uri address
        /// </summary>
        public string Address
        {
            get { return (string)_configManager[Name, ADDRESS_PARAM, DefaultAddress]; }
            set { _configManager[Name, ADDRESS_PARAM] = value; }
        }

        /// <summary>
        /// Specifies a number of concurrent service calls, instances and sessions
        /// </summary>
        public int MaxConcurrentRequests
        {
            get { return (int)_configManager[Name, MAX_CUNCURRENT_REQUESTS_PARAM, DefaultMaxConcurrentRequests]; }
            set { _configManager[Name, MAX_CUNCURRENT_REQUESTS_PARAM] = value; }
        }

        /// <summary>
        /// SSL certificate thumbprint
        /// </summary>
        public string Thumbprint
        {
            get
            {
                string thumbprint = (string)_configManager[Name, THUMBPRINT_PARAM, null];
                return !string.IsNullOrEmpty(thumbprint) ? thumbprint.ToUpper() : thumbprint;
            }
        }

        /// <summary>
        /// MEX, WSDL metadata enabling flag
        /// </summary>
        public bool Metadata
        {
            get { return bool.Parse((string)_configManager[Name, METADATA_PARAM, DefaultMetadata.ToString()]); }
            set { _configManager[Name, METADATA_PARAM] = value.ToString(); }
        }

        /// <summary>
        /// Flag that indicates whether a reliable session is established between channel endpoints
        /// </summary>
        public bool ReliableSession
        {
            get { return bool.Parse((string)_configManager[Name, RELIABLE_SESSION_PARAM, DefaultReliableSession.ToString()]); }
            set { _configManager[Name, RELIABLE_SESSION_PARAM] = value.ToString(); }
        }

        /// <summary>
        /// Interval of time provided for a connection to open before the transport raises an exception.
        /// </summary>
        public TimeSpan OpenTimeout
        {
            get { return TimeSpan.FromSeconds((int)_configManager[Name, OPEN_TIMEOUT_PARAM, (int)DefaultOpenTimeout.TotalSeconds]); }
            set { _configManager[Name, OPEN_TIMEOUT_PARAM] = (int)value.TotalSeconds; }
        }

        /// <summary>
        /// Interval of time provided for a write operation to complete before the transport raises an exception
        /// </summary>
        public TimeSpan SendTimeout
        {
            get { return TimeSpan.FromSeconds((int)_configManager[Name, SEND_TIMEOUT_PARAM, (int)DefaultSendTimeout.TotalSeconds]); }
            set { _configManager[Name, SEND_TIMEOUT_PARAM] = (int)value.TotalSeconds; }
        }

        /// <summary>
        /// Interval of time that a connection can remain inactive, during which no application messages are received, before it is dropped
        /// </summary>
        public TimeSpan ReceiveTimeout
        {
            get { return TimeSpan.FromSeconds((int)_configManager[Name, RECEIVE_TIMEOUT_PARAM, (int)DefaultReceiveTimeout.TotalSeconds]); }
            set { _configManager[Name, RECEIVE_TIMEOUT_PARAM] = (int)value.TotalSeconds; }
        }

        /// <summary>
        /// Authentication type
        /// </summary>
        public virtual SecurityType SecurityType
        {
            get
            {
                SecurityType securityType;
                if (!Enum.TryParse((string)_configManager[Name, SECURITY_TYPE_PARAM, DefaultSecurityType.ToString()], true, out securityType))
                {
                    throw new WCFException(WCFException.E_INVALID_SECURITY_TYPE, (string)_configManager[Name, SECURITY_TYPE_PARAM, DefaultSecurityType.ToString()]);
                }
                return securityType;
            }
            set
            {
                _configManager[Name, SECURITY_TYPE_PARAM] = value.ToString();
            }
        }

        /// <summary>
        /// The group, which should contains a user for performing operations.
        /// </summary>
        public string AuthorizeAsGroupMember
        {
            get { return (string)_configManager[Name, AUTHORIZE_AS_GROUP_MEMBER_PARAM, null]; }
            set { _configManager[Name, AUTHORIZE_AS_GROUP_MEMBER_PARAM] = value; }
        }

        /// <summary>
        /// Store for authorization group
        /// </summary>
        public ContextType AuthorizationGroupStore
        {
            get
            {
                return (ContextType)Enum.Parse(typeof(ContextType),
                    (string)_configManager[Name, AUTHORIZATION_GROUP_STORE_PARAM, DefaultAuthorizationGroupStore.ToString()]);
            }
            set
            {
                _configManager[Name, AUTHORIZATION_GROUP_STORE_PARAM] = value.ToString();
            }
        }

        /// <summary>
        /// Flag that indicates whether a stack trace should be retrieved in case of ServiceExceptionDetail errors.
        /// </summary>
        public bool RetrieveErrorStackTrace
        {
            get { return bool.Parse((string)_configManager[Name, RETRIEVE_ERRORS_STACK_TRACE_PARAM, DefaultRetrieveErrorStackTrace.ToString()]); }
            set { _configManager[Name, RETRIEVE_ERRORS_STACK_TRACE_PARAM] = value.ToString(); }
        }

        /// <summary>
        /// Version of the protocol
        /// </summary>
        public string ProtocolVersion
        {
            get { return (string)_configManager[Name, PROTOCOL_VERSION_PARAM, DefaulProtocolVersion]; }
        }

        /// <summary>
        /// The maximum size (in bytes) for a received message that is processed by the binding.
        /// </summary>
        public long MaxReceivedMessageSize
        {
            get { return long.Parse((string)_configManager[Name, MAX_RECEIVED_MESSAGE_SIZE_PARAM, DefaultMaxReceivedMessageSize.ToString()]); }
        }

        /// <summary>
        /// Binding parameters
        /// </summary>
        public BindingOptions BindingOptions
        {
            get
            {
                return new BindingOptions
                {
                    SecurityType = SecurityType,
                    ReliableSession = ReliableSession,
                    OpenTimeout = OpenTimeout,
                    SendTimeout = SendTimeout,
                    ReceiveTimeout = ReceiveTimeout,
                    MaxReceivedMessageSize = MaxReceivedMessageSize
                };
            }
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendFormat("{0}='{1}', ", ADDRESS_PARAM, Address)
                .AppendFormat("{0}={1}, ", MAX_CUNCURRENT_REQUESTS_PARAM, MaxConcurrentRequests)
                .AppendFormat("{0}={1}, ", METADATA_PARAM, Metadata)
                .AppendFormat("{0}={1}, ", RELIABLE_SESSION_PARAM, ReliableSession)
                .AppendFormat("{0}={1}sec, ", OPEN_TIMEOUT_PARAM, (int)OpenTimeout.TotalSeconds)
                .AppendFormat("{0}={1}sec, ", SEND_TIMEOUT_PARAM, (int)SendTimeout.TotalSeconds)
                .AppendFormat("{0}={1}sec, ", RECEIVE_TIMEOUT_PARAM, (int)ReceiveTimeout.TotalSeconds)
                .AppendFormat("{0}={1}, ", SECURITY_TYPE_PARAM, SecurityType)
                .AppendFormat("{0}='{1}', ", AUTHORIZE_AS_GROUP_MEMBER_PARAM, AuthorizeAsGroupMember)
                .AppendFormat("{0}={1}, ", AUTHORIZATION_GROUP_STORE_PARAM, AuthorizationGroupStore)
                .AppendFormat("{0}={1}, ", MAX_RECEIVED_MESSAGE_SIZE_PARAM, MaxReceivedMessageSize)
                .AppendFormat("{0}={1}, ", RETRIEVE_ERRORS_STACK_TRACE_PARAM, RetrieveErrorStackTrace)
                .AppendFormat("{0}={1}", PROTOCOL_VERSION_PARAM, ProtocolVersion)
                .ToString();
        }
    }
}
