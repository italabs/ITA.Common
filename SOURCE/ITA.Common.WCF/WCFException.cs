using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace ITA.Common.WCF
{
    /// <summary>
    /// ITA WCF-service exception
    /// Serializable
    /// </summary>
    [Serializable]
    public class WCFException : ITAException
    {
        public WCFException(string id, Exception innerEx, params object[] args)
            : base(id, id, innerEx, args)
        {
        }

        public WCFException(string id, params object[] args)
            : base(id, id, args)
        {
        }

        public WCFException(string id, Exception innerEx)
            : base(id, id, innerEx)
        {
        }

        public WCFException(string id)
            : base(id, id)
        {
        }

        public WCFException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        static WCFException()
        {
            LocaleMessages.GlobalInstance.RegisterAssembly(Assembly.GetExecutingAssembly());
        }

        public override string Message
        {
            get
            {
                string localizedMessage = LocaleMessages.GlobalInstance.GetString(GetType(), ID, Args);

                if (string.IsNullOrEmpty(localizedMessage))
                {
                    localizedMessage = ID;
                }
                return localizedMessage;
            }
        }

        #region Error Types

        public static string E_PROTOCOL_IS_NOT_SUPPORTED = "E_PROTOCOL_IS_NOT_SUPPORTED";
        public static string E_CLIENT_TIMEOUT_ERROR = "E_CLIENT_TIMEOUT_ERROR";
        public static string E_CLIENT_CONNECTION_ERROR = "E_CLIENT_CONNECTION_ERROR";
        public static string E_SECURITY_TYPE_NOT_SUPPORTED = "E_SECURITY_TYPE_NOT_SUPPORTED";
        public static string E_INVALID_SECURITY_TYPE = "E_INVALID_SECURITY_TYPE";
        public static string E_REST_HELP_INVALID_SCHEME = "E_REST_HELP_INVALID_SCHEME";
        public static string E_REST_HELP_INVALID_MESSAGE_VERSION = "E_REST_HELP_INVALID_MESSAGE_VERSION";
        public static string E_REST_HELP_NEED_MANUAL_ADDRESSING = "E_REST_HELP_NEED_MANUAL_ADDRESSING";
        public static string E_REST_HELP_CANNOT_HAVE_MESSAGE_HEADER = "E_REST_HELP_CANNOT_HAVE_MESSAGE_HEADER";
        public static string E_REST_HELP_ENDPOINT_IS_NULL = "E_REST_HELP_ENDPOINT_IS_NULL";
        
        #endregion
    }
}
