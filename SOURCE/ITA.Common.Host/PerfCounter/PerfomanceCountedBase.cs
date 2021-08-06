using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using ITA.Common.Host.Enums;
using ITA.Common.Host.Interfaces;
using log4net;

namespace ITA.Common.Host.PerfCounter
{
    public abstract class PerformanceCountedBase : IPerformanceCounted, IEnumerable
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PerformanceCountedBase));

        private readonly HybridDictionary m_Counters;
        private readonly string m_InstanceName;
        private readonly Type m_ObjectType;
        /// <summary>
        /// Prefix which, if defined, is added to the performance counter category name.
        /// </summary>
        private readonly string categoryPrefix;

        /// <summary>
        /// Создает коллекцию счетчиков производительности на основе метаданных указанного типа и его экземпляра.
        /// </summary>
        /// <param name="objectType">Тип, аннотированный атрибутом <see cref="CounterAttribute"/>.</param>
        /// <param name="instanceName">Экземпляр приложения - экземпляр категории счетчиков производительности будет равен ему.</param>
        public PerformanceCountedBase(Type objectType, string instanceName)
        {
            m_ObjectType = objectType;
            m_InstanceName = instanceName;
            m_Counters = new HybridDictionary();

            CreateCounters();
        }

        /// <summary>
        /// Создает коллекцию счетчиков производительности на основе метаданных указанного типа, названия приложения и его экземпляра.
        /// </summary>
        /// <param name="objectType">Тип, аннотированный атрибутом <see cref="CounterAttribute"/>.</param>
        /// <param name="appName">Название приложения, которое будет добавлено к названию категорий создаваемых счетчиков.</param>
        /// <param name="instanceName">Экземпляр приложения - экземпляр категории счетчиков производительности будет равен ему.</param>
        public PerformanceCountedBase(Type objectType, string appName, string instanceName, bool createCounters = true)
        {
            categoryPrefix = appName;

            m_ObjectType = objectType;
            m_InstanceName = instanceName;
            m_Counters = new HybridDictionary();

            if (createCounters)
            {
                CreateCounters();
            }
        }

        public Type CountedObjectType
        {
            get { return m_ObjectType; }
        }

        public string InstanceName
        {
            get { return m_InstanceName; }
        }

        #region IPerformanceCounted Members

        public ICounterUnit GetCounter(string CounterName)
        {
            return m_Counters[CounterName] as ICounterUnit;
        }

        public ICounterUnit this[string CounterID]
        {
            get { return m_Counters[CounterID] as ICounterUnit; }
        }

        public void ResetCounters()
        {
            foreach (ICounterUnit Counter in m_Counters.Values)
            {
                Counter.RawValue = 0;
            }
        }

        public bool GetTicks(out long value)
        {
            value = Stopwatch.GetTimestamp();
            return true;
        }

        public bool GetFrequency(out long value)
        {
            value = Stopwatch.Frequency;
            return true;
        }

        #endregion

        public void CreateCounters()
        {
            object[] counters = CountedObjectType.GetCustomAttributes(typeof(CounterAttribute), true);

            foreach (CounterAttribute counter in counters)
            {
                string category = ComposeCategory(counter);
                logger.DebugFormat("Creating new Performance counter with following parameters: Category={0}, CounterName={1}, InstanceName={2}",
                    category, counter.CounterName, InstanceName);

                var perfCounter = CreateCounterUnit(category, counter.CounterName, InstanceName, false);

                perfCounter.RawValue = 0;
                m_Counters.Add(counter.CounterID, perfCounter);

                switch (counter.CounterType)
                {
                    case ItaPerformanceCounterType.AverageTimer32:
                    case ItaPerformanceCounterType.AverageCount64:
                    case ItaPerformanceCounterType.CounterMultiTimer:
                    case ItaPerformanceCounterType.CounterMultiTimerInverse:
                    case ItaPerformanceCounterType.CounterMultiTimer100Ns:
                    case ItaPerformanceCounterType.CounterMultiTimer100NsInverse:
                    case ItaPerformanceCounterType.RawFraction:
                    case ItaPerformanceCounterType.SampleCounter:
                    case ItaPerformanceCounterType.SampleFraction:
                        {
                            logger.DebugFormat("Creating new Performance counter with following parameters: Category={0}, CounterName={1}, InstanceName={2}",
                                category, counter.CounterName + "Base", InstanceName);
                            perfCounter = CreateCounterUnit(category, counter.CounterName + "Base", InstanceName, false);
                            perfCounter.RawValue = 0;

                            m_Counters.Add(counter.CounterID + "Base", perfCounter);
                        }
                        break;
                }
            }
        }

        public abstract ICounterUnit CreateCounterUnit(string category, string counterName, string instanceName,
            bool readOnly);

        private string ComposeCategory(CounterAttribute counter)
        {
            string result = counter.CategoryName;
            logger.DebugFormat("Inside ComposeCategory. Original counter category name:'{0}', categoryPrefix:'{1}'", counter.CategoryName, categoryPrefix);
            if (!String.IsNullOrEmpty(categoryPrefix))
                result = categoryPrefix + ": " + result;

            logger.DebugFormat("ComposeCategory. Returning resulting category: '{0}'", result);

            return result;
        }

        public IEnumerator GetEnumerator()
        {
            return m_Counters.GetEnumerator();
        }
    }
}
