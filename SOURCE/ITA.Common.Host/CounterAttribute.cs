using System;
using ITA.Common.Host.Enums;

namespace ITA.Common.Host
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CounterAttribute : Attribute
    {
        private string m_CounterID;
        private string m_CategoryDescription;
        private string m_CategoryName;
        private string m_CounterDescription;
        private string m_CounterName;
        private ItaPerformanceCounterType m_Type;

        public CounterAttribute(string CounterID, string CounterName, string CounterDescription, string CategoryName,
                                string CategoryDescription, ItaPerformanceCounterType CounterType)
        {
            Init(CounterID, CounterName, CounterDescription, CategoryName, CategoryDescription, CounterType, false);
        }

        public CounterAttribute(string CounterName, string CounterDescription, string CategoryName,
                                string CategoryDescription, ItaPerformanceCounterType CounterType)
        {
            Init(CounterName, CounterDescription, CategoryName, CategoryDescription, CounterType, false);
        }

        public CounterAttribute(string CounterName, string CounterDescription, string CategoryName,
                                string CategoryDescription, ItaPerformanceCounterType CounterType, bool CreateBase)
        {
            Init(CounterName, CounterDescription, CategoryName, CategoryDescription, CounterType, CreateBase);
        }

        public string CounterID
        {
            get
            {
                return m_CounterID;
            }
        }

        public string CounterName
        {
            get { return m_CounterName; }
        }

        public string CounterDescription
        {
            get { return m_CounterDescription; }
        }

        /// <summary>
        /// Category name.
        /// Note that in the runtime application prefix will be prepended to the value of this property.
        /// </summary>
        public string CategoryName
        {
            get { return m_CategoryName; }
        }

        public string CategoryDescription
        {
            get { return m_CategoryDescription; }
        }

        public ItaPerformanceCounterType CounterType
        {
            get { return m_Type; }
        }

        private void Init(string CounterName, string CounterDescription, string CategoryName, string CategoryDescription,
            ItaPerformanceCounterType CounterType, bool CreateBase)
        {
            Init(CounterName, CounterName, CounterDescription, CategoryName, CategoryDescription, CounterType, CreateBase);
        }

        private void Init(string CounterID, string CounterName, string CounterDescription, string CategoryName, string CategoryDescription,
            ItaPerformanceCounterType CounterType, bool CreateBase)
        {
            m_CounterID = CounterID;
            m_CounterName = CounterName;
            m_CounterDescription = CounterDescription;
            m_CategoryName = CategoryName;
            m_CategoryDescription = CategoryDescription;
            m_Type = CounterType;
        }
    }
}