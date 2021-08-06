namespace ITA.Common.UI
{
    /// <summary>
    /// Интнефейс-обертка (адаптер) на
    /// 1) Exception 
    /// 2) ExceptionDetail
    /// </summary>
    public interface IErrorSource
    {
        string Type { get; }

        string Message { get; }

        string LocalizedMessage { get; }

        string HelpLink { get; }

        bool HelpLinkEnabled { get; }

        string Source { get; }

        string Data { get; }

        string TargetSite { get; }

        string StackTrace { get; }

        IErrorSource InnerSource { get; }
    }
}