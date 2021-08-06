using System;
using System.Collections;

namespace ITA.Common.UI
{
    /// <summary>
    /// Класс - обертка на стантартный Exception
    /// </summary>
    public class ExceptionSource : IErrorSource
    {
        private readonly Exception _ex;

        private IErrorSource _innerSource;

        public ExceptionSource(Exception ex)
        {
            _ex = ex;
        }

        #region IErrorSource Members

        public string Type
        {
            get { return _ex.GetType().ToString(); }
        }

        public string Message
        {
            get { return _ex.Message; }
        }

        public string LocalizedMessage
        {
            get
            {
                if (_ex != null && _ex is ITAException)
                {
                    // if we have our Exception then try to get localized string
                    var OurEx = _ex as ITAException;
                    return LocaleMessages.GlobalInstance.GetString(_ex.GetType(), OurEx.ID, OurEx.Args);
                }
                else
                    return string.Empty;
            }
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
            get
            {
                //
                // .NET bug
                // crashes when trying to get Source property of ExternalException type
                //

                string source = string.Empty;
                try
                {
                    source = !string.IsNullOrEmpty(_ex.Source) ? _ex.Source : Messages.I_ITA_COMMON_NONE;
                }
                catch (Exception Unexpected)
                {
                    source = string.Format(Messages.I_ITA_COMMON_ERROR_DESCR, Unexpected.Message);
                }
                return source;
            }
        }

        public string Data
        {
            get
            {
                string data = "";
                try
                {
                    if (_ex.Data != null)
                    {
                        foreach (DictionaryEntry P in _ex.Data)
                        {
                            data += string.Format("\n\t\t", P.Key != null ? P.Key.ToString() : "<null>",
                                                  P.Value != null ? P.Value.ToString() : "<null>");
                        }
                    }
                }
                catch (Exception Unexpected)
                {
                    data = string.Format(Messages.I_ITA_COMMON_ERROR_DESCR, Unexpected.Message);
                }

                return data;
            }
        }

        public string TargetSite
        {
            get
            {
                //
                // .NET bug
                // TargetSite property requires the presense of remote assembly which has thrown this exception
                //

                string site;
                try
                {
                    site = _ex.TargetSite != null ? _ex.TargetSite.ToString() : Messages.I_ITA_COMMON_NONE;
                }
                catch (Exception Unexpected)
                {
                    site = string.Format(Messages.I_ITA_COMMON_ERROR_DESCR, Unexpected.Message);
                }

                return site;
            }
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

                _innerSource = new ExceptionSource(_ex.InnerException);

                return _innerSource;
            }
        }

        #endregion
    }
}