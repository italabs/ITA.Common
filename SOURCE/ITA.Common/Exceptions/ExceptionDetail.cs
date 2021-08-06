using System;
using System.Runtime.Serialization;
using System.Text;

namespace ITA.Common
{
    /// <summary>
    /// Exception object wrapper
    /// </summary>
    [Serializable()]
    public class ExceptionDetail : ISerializable
    {
        private const string cz_HelpLink = "HelpLink";
        private const string cz_InnerException = "InnerException";
        private const string cz_Message = "Message";
        private const string cz_StackTrace = "StackTrace";
        private const string cz_Type = "Type";

        private string m_HelpLink = String.Empty;
        private ExceptionDetail m_InnerException = null;
        private string m_Message = String.Empty;
        private string m_StackTrace = String.Empty;
        private string m_Type = String.Empty;

        /// <summary>
        /// Contructs wrapper
        /// </summary>
        /// <param name="ex"></param>
        public ExceptionDetail(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            m_HelpLink = ex.HelpLink;
            m_Message = ex.Message;
            m_StackTrace = ex.StackTrace;
            m_Type = ex.GetType().ToString();

            if (ex.InnerException != null)
            {
                m_InnerException = new ExceptionDetail(ex.InnerException);
            }
        }

        /// <summary>
        /// Constructs wrapper
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ExceptionDetail(SerializationInfo info, StreamingContext context)
        {
            m_HelpLink = info.GetString(cz_HelpLink);
            m_InnerException = info.GetValue(cz_InnerException, typeof(ExceptionDetail)) as ExceptionDetail;
            m_Message = info.GetString(cz_Message);
            m_StackTrace = info.GetString(cz_StackTrace);
            m_Type = info.GetString(cz_Type);
        }

        /// <summary>
        /// Serializes object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(cz_HelpLink, m_HelpLink);
            info.AddValue(cz_InnerException, m_InnerException, typeof(ExceptionDetail));
            info.AddValue(cz_Message, m_Message);
            info.AddValue(cz_StackTrace, m_StackTrace);
            info.AddValue(cz_Type, m_Type);
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
        /// HelpLink of wrapped exception
        /// </summary>
        public string HelpLink
        {
            get { return m_HelpLink; }
        }

        /// <summary>
        /// Wrapped inner exception
        /// </summary>
        public ExceptionDetail InnerException
        {
            get { return m_InnerException; }
        }

        /// <summary>
        /// Message of wrapped exception
        /// </summary>
        public string Message
        {
            get { return m_Message; }
        }

        /// <summary>
        /// StackTrace of wrapped exception
        /// </summary>
        public string StackTrace
        {
            get { return m_StackTrace; }
        }

        /// <summary>
        /// Type of wrapped exception
        /// </summary>
        public string Type
        {
            get { return m_Type; }
        }
    }
}
