namespace ITA.Common.UI
{
    /// <summary>
    /// Класс - обертка на ExceptionDetail, переданный при ремотинге удаленным сервером 
    /// в FaultException
    /// </summary>
    public class ExceptionDetailSource : IErrorSource
    {
        private readonly ExceptionDetail _ex;
        private IErrorSource _innerSource;

        public ExceptionDetailSource(ExceptionDetail ex)
        {
            _ex = ex;
        }

        #region IErrorSource Members

        public string Type
        {
            get { return _ex.Type; }
        }

        public string Message
        {
            get { return _ex.Message; }
        }

        public string LocalizedMessage
        {
            get { return string.Empty; }
        }

        public string HelpLink
        {
            get { return !string.IsNullOrEmpty(_ex.HelpLink) ? _ex.HelpLink : Messages.I_ITA_COMMON_NONE; }
        }

        public bool HelpLinkEnabled
        {
            get { return !string.IsNullOrEmpty(_ex.HelpLink); }
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
            get { return _ex.StackTrace; }
        }

        public IErrorSource InnerSource
        {
            get
            {
                if (_innerSource != null)
                    return _innerSource;

                if (_ex.InnerException == null)
                {
                    return null;
                }

                _innerSource = new ExceptionDetailSource(_ex.InnerException);

                return _innerSource;
            }
        }

        #endregion
    }
}