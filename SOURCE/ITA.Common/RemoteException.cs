using System;
using System.Runtime.Serialization;

namespace ITA.Common
{
    /// <summary>
    /// Remote exception
    /// </summary>
    [Serializable()]
    public class ITARemoteException : Exception
    {
        private ExceptionDetail m_ExceptionDetail = null;

        private void Init(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            m_ExceptionDetail = new ExceptionDetail(ex);
        }

        /// <summary>
        /// Contsructs remote exception
        /// </summary>
        /// <param name="ex"></param>
        public ITARemoteException(Exception ex)
            : base()
        {
            Init(ex);
        }

        /// <summary>
        /// Constructs remote exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public ITARemoteException(string message, Exception ex)
            : base(message)
        {
            Init(ex);
        }

        /// <summary>
        /// Constructs remote exception
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ITARemoteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_ExceptionDetail = info.GetValue("Exception", typeof(ExceptionDetail)) as ExceptionDetail;
        }

        /// <summary>
        /// Serializes exception object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Exception", m_ExceptionDetail, typeof(ExceptionDetail));
        }

        /// <summary>
        /// Wrapped exception details
        /// </summary>
        public ExceptionDetail Detail
        {
            get
            {
                return m_ExceptionDetail;
            }
        }
    }
}