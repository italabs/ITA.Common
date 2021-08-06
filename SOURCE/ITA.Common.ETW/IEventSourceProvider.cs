namespace ITA.Common.ETW
{
    public interface IEventSourceProvider
    {
        IStaticEventSource GetEventSource();
    } 
}
