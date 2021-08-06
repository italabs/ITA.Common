namespace ITA.Common.WCF.RESTHelp.Interfaces
{
    public interface IHelpPageSettings
    {
        bool Enabled { get; set; }

        string BaseHelpUri { get; }
    }
}