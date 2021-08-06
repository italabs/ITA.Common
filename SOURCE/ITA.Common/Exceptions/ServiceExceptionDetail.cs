using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ITA.Common
{
    /// <summary>
    /// Contract for all WCF exception with localization support.
    /// </summary>
    [DataContract]
    public class ServiceExceptionDetail
    {
        /// <summary>
        /// Parameterless constructor needed for deserialize from json
        /// </summary>
        protected ServiceExceptionDetail()
        {
        }

        public ServiceExceptionDetail(Exception ex)
            : this(ex, false)
        {
        }

        public ServiceExceptionDetail(Exception ex, bool noStackTrace)
        {
            this.Message = ex.Message;

            if (!noStackTrace)
            {
                this.StackTrace = ex.StackTrace;
            }

            this.Type = ex.GetType().FullName;

            this.HelpLink = ex.HelpLink;

            if (ex.InnerException != null)
            {
                this.InnerException = new ServiceExceptionDetail(ex.InnerException, noStackTrace);
            }

            ITAException iex = ex as ITAException;
            this.IsITAException = iex != null;

            if (iex != null)
            {
                this.ID = iex.ID;
                this.Code = iex.Code;
                
                //Convert arguments to string array to fix serialization issues.
                this.Args = iex.Args != null ? iex.Args.Select(a => a != null ? a.ToString() : null).ToArray() : null;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0}: {1}", this.Type, this.Message);
            if (this.InnerException != null)
                stringBuilder.AppendFormat(" ----> {0}", this.InnerException.ToString());
            else
                stringBuilder.Append("\n");
            stringBuilder.Append(this.StackTrace);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// English messages.
        /// </summary>
        [DataMember]
        public String Message { get; set; }

        /// <summary>
        /// Message id - from resource.
        /// </summary>
        [DataMember]
        public String ID { get; set; }

        /// <summary>
        /// Exit code.
        /// </summary>
        [DataMember]
        public int Code { get; set; }

        /// <summary>
        /// Additional arguments.
        /// </summary>
        [DataMember]
        public string[] Args { get; set; }

        /// <summary>
        /// Stack trace.
        /// </summary>
        [DataMember]
        public string StackTrace { get; set; }

        /// <summary>
        /// Exception type.
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Reference to inner exception.
        /// </summary>
        [DataMember]
        public ServiceExceptionDetail InnerException { get; set; }

        /// <summary>
        /// Help link.
        /// </summary>
        [DataMember]
        public string HelpLink { get; set; }

        /// <summary>
        /// Is it ITAException with localization support?
        /// </summary>
        [DataMember]
        public bool IsITAException { get; set; }
    }
}
