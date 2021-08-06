using ITA.Common.UI;

namespace ITA.Common.WCF.UI
{
    public class ServiceExceptionDetailSource : IErrorSource
    {
        private ServiceExceptionDetail m_Detail;

        public ServiceExceptionDetailSource(ServiceExceptionDetail detail)
        {
            this.m_Detail = detail;
        }

        #region IErrorSource Members

        public string Type
        {
            get { return this.m_Detail.Type; }
        }

        public string Message
        {
            get { return this.m_Detail.Message; }
        }

        public string LocalizedMessage
        {
            get
            {
                if (this.m_Detail != null && this.m_Detail.IsITAException && !string.IsNullOrEmpty(this.m_Detail.ID))
                {
                    string localizedMessage = LocaleMessages.GlobalInstance.GetString(this.m_Detail.Type, this.m_Detail.ID, this.m_Detail.Args);

                    if (!string.IsNullOrEmpty(localizedMessage))
                        return localizedMessage;
                }

                return this.Message;
            }
        }

        public string HelpLink
        {
            get { return this.m_Detail.HelpLink; }
        }

        public bool HelpLinkEnabled
        {
            get { return !string.IsNullOrEmpty(this.m_Detail.HelpLink); }
        }

        public string Source
        {
            get { return Messages.I_ITA_COMMON_NONE; }
        }

        public string Data
        {
            get { return Messages.I_ITA_COMMON_NONE; }
        }

        public string TargetSite
        {
            get { return Messages.I_ITA_COMMON_NONE; }
        }

        public string StackTrace
        {
            get { return this.m_Detail.StackTrace; }
        }

        public IErrorSource InnerSource
        {
            get
            {
                if (this.m_Detail.InnerException == null)
                    return null;

                return new ServiceExceptionDetailSource(this.m_Detail.InnerException);
            }
        }

        #endregion
    }
}
