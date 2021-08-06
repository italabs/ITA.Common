namespace ITA.Common.Host.Interfaces
{
    public interface ICounterUnit
    {
        string CounterName { get; }
        
        long RawValue { get; set; }

        object GetUnit();

        void Increment();
        
        void IncrementBy(long value);
        
        void Decrement();
    }
}
