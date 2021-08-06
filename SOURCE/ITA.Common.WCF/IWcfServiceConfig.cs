using System;
using System.DirectoryServices.AccountManagement;

namespace ITA.Common.WCF
{
    /// <summary>
    /// Base WCF-service configuration interface
    /// </summary>
    public interface IWcfServiceConfig
    {
        /// <summary>
        /// WCF-service uri address
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// A number that specifies the number of concurrent service calls, instances and sessions
        /// </summary>
        int MaxConcurrentRequests { get; set; }

        /// <summary>
        /// MEX, WSDL metadata enabling flag
        /// </summary>
        bool Metadata { get; set; }

        /// <summary>
        /// SSL certificate thumbprint
        /// </summary>
        string Thumbprint { get; }

        /// <summary>
        /// Flag that indicates whether a reliable session is established between channel endpoints
        /// </summary>
        bool ReliableSession { get; set; }

        /// <summary>
        /// Interval of time provided for a connection to open before the transport raises an exception.
        /// </summary>
        TimeSpan OpenTimeout { get; set; }

        /// <summary>
        /// Interval of time provided for a write operation to complete before the transport raises an exception
        /// </summary>
        TimeSpan SendTimeout { get; set; }

        /// <summary>
        /// Interval of time that a connection can remain inactive, during which no application messages are received, before it is dropped
        /// </summary>
        TimeSpan ReceiveTimeout { get; set; }

        /// <summary>
        /// Authentication type
        /// </summary>
        SecurityType SecurityType { get; set; }

        /// <summary>
        /// The group, which should contains a user for performing operations.
        /// </summary>
        string AuthorizeAsGroupMember { get; set; }

        /// <summary>
        /// Store for authorization group
        /// </summary>
        ContextType AuthorizationGroupStore { get; set; }

        /// <summary>
        /// Flag that indicates whether a stack trace should be retrieved in case of ServiceExceptionDetail errors.
        /// </summary>
        bool RetrieveErrorStackTrace { get; set; }

        /// <summary>
        /// Version of the protocol
        /// </summary>
        string ProtocolVersion { get; }

        /// <summary>
        /// The maximum size (in bytes) for a received message that is processed by the binding.
        /// </summary>
        long MaxReceivedMessageSize { get; }
    }
}        
