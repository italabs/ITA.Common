using System;
using System.Text;

namespace ITA.Common.WCF
{
    /// <summary>
    /// WCF-service base binding options
    /// </summary>
    public class BindingOptions
    {
        /// <summary>
        /// 5 Mb = 5*1024*1024;
        /// </summary>
        public const int Size_5Mb = 5242880;

        private const string TIME_FORMAT = @"hh\:mm\:ss";
        /// <summary>
        /// Flag that indicates whether a reliable session is established between channel endpoints
        /// </summary>
        public bool ReliableSession { get; set; }
        /// <summary>
        /// Interval of time provided for a connection to open before the transport raises an exception.
        /// </summary>
        public TimeSpan OpenTimeout { get; set; }
        /// <summary>
        /// Interval of time provided for a write operation to complete before the transport raises an exception
        /// </summary>
        public TimeSpan SendTimeout { get; set; }
        /// <summary>
        /// Interval of time that a connection can remain inactive, during which no application messages are received, before it is dropped
        /// </summary>
        public TimeSpan ReceiveTimeout { get; set; }
        /// <summary>
        /// Authentication type
        /// </summary>
        public SecurityType SecurityType { get; set; }
        /// <summary>
        /// The maximum size (in bytes) for a received message that is processed by the binding.
        /// </summary>
        public long MaxReceivedMessageSize { get; set; }
        /// <summary>
        /// Returns default binding options
        /// </summary>
        public static BindingOptions Default
        {
            get { return new BindingOptions(); }
        }

        public BindingOptions()
        {
            ReliableSession = false;
            OpenTimeout = TimeSpan.FromSeconds(30);
            SendTimeout = TimeSpan.FromSeconds(60);
            ReceiveTimeout = TimeSpan.FromSeconds(60);
            SecurityType = SecurityType.Windows;
            MaxReceivedMessageSize = Size_5Mb;
        }

        /// <summary>
        /// Creating copy of current binding options
        /// </summary>
        /// <returns>Copy of current binding options</returns>
        public BindingOptions Clone()
        {
            return new BindingOptions()
            {
                ReliableSession = this.ReliableSession,
                OpenTimeout = this.OpenTimeout,
                SendTimeout = this.SendTimeout,
                ReceiveTimeout = this.ReceiveTimeout,
                SecurityType = this.SecurityType,
                MaxReceivedMessageSize = this.MaxReceivedMessageSize
            };
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendFormat("ReliableSession={0}, ", ReliableSession)
                .AppendFormat("MaxReceivedMessageSize={0}, ", MaxReceivedMessageSize)
                .AppendFormat("OpenTimeout={0}, ", OpenTimeout.ToString(TIME_FORMAT))
                .AppendFormat("SendTimeout={0}, ", SendTimeout.ToString(TIME_FORMAT))
                .AppendFormat("ReceiveTimeout={0}, ", ReceiveTimeout.ToString(TIME_FORMAT))
                .AppendFormat("SecurityType={0}", SecurityType)
                .ToString();
        }
    }
}
