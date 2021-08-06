using System;
using System.Runtime.Serialization;

namespace ITA.Common.Host
{
    /// <summary>
    /// The exception object derived from System.Exception
    /// Serializable
    /// </summary>
    [Serializable]
    public class HostException : ITAException
    {
        public HostException(string msg, string ID, Exception InnerEx, params object[] Args)
            :
                base(msg, ID, InnerEx, Args)
        {
        }

        public HostException(string msg, string ID, Exception InnerEx)
            :
                base(msg, ID, InnerEx)
        {
        }

        public HostException(string msg, string ID)
            :
                base(msg, ID)
        {
        }

        public HostException(SerializationInfo info, StreamingContext context)
            :
                base(info, context)
        {
        }

        #region Error Types

        public static string E_HOST_ERROR_INITIALIZING_ENGINE = "E_HOST_ERROR_INITIALIZING_ENGINE";
        public static string E_HOST_ERROR_STARTING_ENGINE = "E_HOST_ERROR_STARTING_ENGINE";
        public static string E_HOST_ERROR_PAUSING_ENGINE = "E_HOST_ERROR_PAUSING_ENGINE";
        public static string E_HOST_ERROR_RESUMING_ENGINE = "E_HOST_ERROR_RESUMING_ENGINE";

        #endregion
    }
}