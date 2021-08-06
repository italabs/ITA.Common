namespace ITA.Common.Microservices.Metrics
{
    /// <summary>
    /// Metric types definition is clone from <a href="https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountertype?view=net-5.0">PerformanceCounterType</a> type.
    /// </summary>
    public enum MetricType
    {
        /// <summary>
        /// An instantaneous counter that shows the most recently observed value in
        /// hexadecimal format. Used, for example, to maintain a simple count of items or operations.
        /// </summary>
        NumberOfItemsHEX32 = 0,

        /// <summary>
        /// An instantaneous counter that shows the most recently observed value.
        /// Used, for example, to maintain a simple count of a very large number of
        /// items or operations. It is the same as NumberOfItemsHEX32 except that it
        /// uses larger fields to accommodate larger values.
        /// </summary>
        NumberOfItemsHEX64 = 256,

        /// <summary>
        /// An instantaneous counter that shows the most recently observed value.
        /// Used, for example, to maintain a simple count of items or operations.
        /// Counters of this type include Memory\Available Bytes.
        /// </summary>
        NumberOfItems32 = 65536,

        /// <summary>
        /// An instantaneous counter that shows the most recently observed value. Used, for example,
        /// to maintain a simple count of a very large number of items or operations.
        /// It is the same as NumberOfItems32 except that it uses larger fields to accommodate larger values.
        /// </summary>
        NumberOfItems64 = 65792,

        /// <summary>
        /// A difference counter that shows the change in the measured attribute between the two most recent sample intervals.
        /// </summary>
        CounterDelta32 = 4195328,

        /// <summary>
        /// A difference counter that shows the change in the measured attribute between the two most
        /// recent sample intervals. It is the same as the CounterDelta32 counter type except that is uses larger fields to accommodate larger values.
        /// </summary>
        CounterDelta64 = 4195584,

        /// <summary>
        /// An average counter that shows the average number of operations completed in one second.
        /// When a counter of this type samples the data, each sampling interrupt returns one or zero.
        /// The counter data is the number of ones that were sampled. It measures time in units of ticks of the system performance timer.
        /// </summary>
        SampleCounter = 4260864,

        /// <summary>
        /// An average counter designed to monitor the average length of a queue to a resource over time. It shows the difference between the queue lengths
        /// observed during the last two sample intervals divided by the duration of the interval.
        /// This type of counter is typically used to track the number of items that are queued or waiting.
        /// </summary>
        CountPerTimeInterval32 = 4523008,

        /// <summary>
        /// An average counter that monitors the average length of a queue to a resource over time.
        /// Counters of this type display the difference between the queue lengths observed during the last two sample intervals, divided by the duration of the interval.
        /// This counter type is the same as CountPerTimeInterval32 except that it uses larger fields to accommodate larger values.
        /// This type of counter is typically used to track a high-volume or very large number of items that are queued or waiting.
        /// </summary>
        CountPerTimeInterval64 = 4523264,

        /// <summary>
        /// A difference counter that shows the average number of operations completed during each second of the sample interval.
        /// Counters of this type measure time in ticks of the system clock. Counters of this type include System\ File Read Operations/sec.
        /// </summary>
        RateOfCountsPerSecond32 = 272696320,

        /// <summary>
        /// A difference counter that shows the average number of operations completed during each second of the sample interval.
        /// Counters of this type measure time in ticks of the system clock. This counter type is the same as the RateOfCountsPerSecond32 type,
        /// but it uses larger fields to accommodate larger values to track a high-volume number of items or operations per second,
        /// such as a byte-transmission rate. Counters of this type include System\ File Read Bytes/sec.
        /// </summary>
        RateOfCountsPerSecond64 = 272696576,

        /// <summary>
        /// A percentage counter that shows the average ratio of hits to all operations during the last two sample intervals. Counters of this type include Cache\Pin Read Hits %.
        /// </summary>
        RawFraction = 537003008,

        /// <summary>
        /// A percentage counter that shows the average time that a component is active as a percentage of the total sample time.
        /// </summary>
        CounterTimer = 541132032,

        /// <summary>
        /// A percentage counter that shows the active time of a component as a percentage of the total elapsed time of the sample interval.
        /// It measures time in units of 100 nanoseconds (ns). Counters of this type are designed to measure the activity
        /// of one component at a time. Counters of this type include Processor\% User Time.
        /// </summary>
        Timer100Ns = 542180608,

        /// <summary>
        /// A percentage counter that shows the average ratio of hits to all operations during the last two sample intervals. Counters of this type include Cache\Pin Read Hits %.
        /// </summary>
        SampleFraction = 549585920,

        /// <summary>
        /// A percentage counter that displays the average percentage of active time observed during sample interval.
        /// The value of these counters is calculated by monitoring the percentage of time that the service was inactive
        /// and then subtracting that value from 100 percent. This is an inverse counter type.
        /// It measures time in units of ticks of the system performance timer.
        /// </summary>
        CounterTimerInverse = 557909248,

        /// <summary>
        /// A percentage counter that shows the average percentage of active time observed during the sample interval. This is an inverse counter. Counters of this type include Processor\% Processor Time.
        /// </summary>
        Timer100NsInverse = 558957824,

        /// <summary>
        /// A percentage counter that displays the active time of one or more components as a percentage of the total time of the sample interval.
        /// Because the numerator records the active time of components operating simultaneously, the resulting percentage can exceed 100 percent.
        /// This counter type differs from CounterMultiTimer100Ns in that it measures time in units of ticks of the system performance timer,
        /// rather than in 100 nanosecond units. This counter type is a multitimer.
        /// </summary>
        CounterMultiTimer = 574686464,

        /// <summary>
        /// A percentage counter that shows the active time of one or more components as a percentage
        /// of the total time of the sample interval. It measures time in 100 nanosecond (ns) units. This counter type is a multitimer.
        /// </summary>
        CounterMultiTimer100Ns = 575735040,

        /// <summary>
        /// A percentage counter that shows the active time of one or more components as a percentage of the total time of the sample interval.
        /// It derives the active time by measuring the time that the components were not active and subtracting the result from
        /// 100 percent by the number of objects monitored. This counter type is an inverse multitimer.
        /// It differs from CounterMultiTimer100NsInverse in that it measures time in units of ticks of the system performance timer, rather than in 100 nanosecond units.
        /// </summary>
        CounterMultiTimerInverse = 591463680,

        /// <summary>
        /// A percentage counter that shows the active time of one or more components as a percentage of the total time of the sample interval.
        /// Counters of this type measure time in 100 nanosecond (ns) units. They derive the active time by measuring the time that the
        /// components were not active and subtracting the result from multiplying 100 percent by the number of objects monitored.
        /// This counter type is an inverse multitimer.
        /// </summary>
        CounterMultiTimer100NsInverse = 592512256,

        /// <summary>
        /// An average counter that measures the time it takes, on average, to complete a process or operation.
        /// Counters of this type display a ratio of the total elapsed time of the sample interval to the number
        /// of processes or operations completed during that time. This counter type measures time in ticks of
        /// the system clock. Counters of this type include PhysicalDisk\ Avg. Disk sec/Transfer.
        /// </summary>
        AverageTimer32 = 805438464,

        /// <summary>
        /// A difference timer that shows the total time between when the component or process started and the time when this value is calculated.
        /// Counters of this type include System\ System Up Time.
        /// </summary>
        ElapsedTime = 807666944,

        /// <summary>
        /// An average counter that shows how many items are processed, on average, during an operation.
        /// Counters of this type display a ratio of the items processed to the number of operations completed.
        /// The ratio is calculated by comparing the number of items processed during the last
        /// interval to the number of operations completed during the last interval.
        /// Counters of this type include PhysicalDisk\ Avg. Disk Bytes/Transfer.
        /// </summary>
        AverageCount64 = 1073874176,

        /// <summary>
        /// A base counter that stores the number of sampling interrupts taken and is used as a denominator in the sampling fraction.
        /// The sampling fraction is the number of samples that were 1 (or true) for a sample interrupt.
        /// Check that this value is greater than zero before using it as the denominator in a calculation of SampleFraction.
        /// </summary>
        SampleBase = 1073939457,

        /// <summary>
        /// A base counter that is used in the calculation of time or count averages, such as AverageTimer32
        /// and AverageCount64. Stores the denominator for calculating a counter to present "time per operation" or "count per operation".
        /// </summary>
        AverageBase = 1073939458,

        /// <summary>
        /// A base counter that stores the denominator of a counter that presents a general arithmetic fraction.
        /// Check that this value is greater than zero before using it as the denominator in a RawFraction value calculation.
        /// </summary>
        RawBase = 1073939459,

        /// <summary>
        /// A base counter that indicates the number of items sampled. It is used as the denominator in the calculations
        /// to get an average among the items sampled when taking timings of multiple, but similar items.
        /// Used with CounterMultiTimer, CounterMultiTimerInverse, CounterMultiTimer100Ns, and CounterMultiTimer100NsInverse.
        /// </summary>
        CounterMultiBase = 1107494144,
    }
}