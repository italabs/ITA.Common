using System;
using System.Runtime.Serialization;

namespace ITA.Common
{
    /// <summary>
    /// Common Exception
    /// The exception object derived from System.Exception
    /// Serializable
    /// </summary>
    [Serializable]
    public class ITAException : Exception
    {
        /// <summary>
        /// The web link for more information
        /// </summary>
        private const string szHelpLink = "";

        private object[] m_Args = null;
        private string m_StrID;
		private int m_ExCode;

		protected virtual string GetHelpLink()
		{
			return szHelpLink;
		}

        /// <summary>
        /// Default deserialization constructor
        /// </summary>
        /// <param name="info">SerializationInfo object</param>
        /// <param name="context">SerializationContext object</param>
        public ITAException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            ID = info.GetString("ID");
            Args = info.GetValue("Args", typeof (Array)) as object[];
			Code = info.GetInt32("Code");
        }

        /// <summary>
        /// Constructs the exception object
        /// </summary>
        /// <param name="msg">Error context description message</param>
        /// <param name="ID">Error ID</param>
        /// <param name="InnerEx">Inner exception</param>
        /// <param name="Args">Error description message arguments</param>
        public ITAException(string msg, string ID, Exception InnerEx, params object[] Args) :
            base(msg, InnerEx)
        {
            base.HelpLink = GetHelpLink();
			m_ExCode = 0;
            m_StrID = ID;
            m_Args = Args;
        }

        /// <summary>
        /// Constructs the exception object
        /// </summary>
        /// <param name="msg">Error context description message</param>
        /// <param name="ID">Error ID</param>
        /// <param name="InnerEx">Inner exception</param>
        public ITAException(string msg, string ID, Exception InnerEx) :
            base(msg, InnerEx)
        {
			base.HelpLink = GetHelpLink();
            m_StrID = ID;
			m_ExCode = 0;
        }

		/// <summary>
		/// Constructs the exception object
		/// </summary>
		/// <param name="msg">Error context description message</param>
		/// <param name="Code">Error code</param>
		/// <param name="Args">Error description message arguments</param>
		public ITAException ( string msg, int Code, params object[] Args ):
			base ( msg )
		{
			base.HelpLink = GetHelpLink();
			m_ExCode = Code;
			m_StrID = String.Empty;
			m_Args = Args;
		}

		/// <summary>
		/// Constructs the exception object
		/// </summary>
		/// <param name="msg">Error context description message</param>
		/// <param name="Code">Error code</param>
		public ITAException ( string msg, int Code ):
			base ( msg )
		{
			base.HelpLink = GetHelpLink();
			m_ExCode = Code;
			m_StrID = "";
		}

        /// <summary>
        /// Constructs the exception object
        /// </summary>
        /// <param name="msg">Error context description message</param>
        /// <param name="ID">Error ID</param>
        /// <param name="Args">Error description message arguments</param>
        public ITAException(string msg, string ID, params object[] Args) :
            base(msg)
        {
			base.HelpLink = GetHelpLink();
            m_StrID = ID;
            m_Args = Args;
			m_ExCode = 0;
        }

        /// <summary>
        /// Constructs the exception object
        /// </summary>
        /// <param name="msg">Error context description message</param>
        /// <param name="ID">Error ID</param>
        public ITAException(string msg, string ID) :
            base(msg)
        {
			base.HelpLink = GetHelpLink();
            m_StrID = ID;
			m_ExCode = 0;
        }

        /// <summary>
        /// Error ID
        /// </summary>
        public string ID
        {
            get { return m_StrID; }
            set { m_StrID = value; }
        }

        /// <summary>
        /// Error description message arguments
        /// </summary>
        public object[] Args
        {
            get { return m_Args; }
            set { m_Args = value; }
        }

		/// <summary>
		/// Error code
		/// </summary>
		public int Code
		{
			get
			{
				return m_ExCode;
			}
			set
			{
				m_ExCode = value;
			}
		}

        /// <summary>
        /// Serializes the exception object
        /// </summary>
        /// <param name="info">SerializationInfo object</param>
        /// <param name="context">StreamingContext object</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("ID", ID);
            info.AddValue("Args", m_Args);
			info.AddValue("Code", Code);
        }

        public static string E_ITA_READCONFIGURATION = "E_ITA_READCONFIGURATION";
        public static string E_ITA_NO_CONFIG_PARAMETER = "E_ITA_NO_CONFIG_PARAMETER";
        public static string E_ITA_WRITECONFIGURATION = "E_ITA_WRITECONFIGURATION";
        public static string E_ITA_DATABASE_MANAGER_ERROR = "E_ITA_DATABASE_MANAGER_ERROR";
        public static string E_ITA_INSTALL_XML_DESERIALIZING_ERROR1 = "E_ITA_INSTALL_XML_DESERIALIZING_ERROR1";

        public static string E_ITA_SAVE_FILE_STORAGE_SETTINGS = "E_ITA_SAVE_FILE_STORAGE_SETTINGS";
        public static string E_ITA_LOAD_FILE_STORAGE_SETTINGS = "E_ITA_LOAD_FILE_STORAGE_SETTINGS";
        public static string E_ITA_COMPONENT_PROPERTY_NOT_FOUND = "E_ITA_COMPONENT_PROPERTY_NOT_FOUND";
    }
}