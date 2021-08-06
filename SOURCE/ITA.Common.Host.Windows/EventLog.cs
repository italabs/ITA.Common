using System.Collections.Specialized;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.Windows
{
    public class EventLog : System.Diagnostics.EventLog, IEventLog
    {
        private readonly ListDictionary m_Categories;

        public EventLog()
        {
            m_Categories = new ListDictionary();
        }

        public void AddCategory(string Component, short CatId)
        {
            m_Categories.Add(Component, CatId);
        }

        public void RemoveCategory(string Component)
        {
            m_Categories.Remove(Component);
        }

        public short GetCategory(string Component)
        {
            object obj = m_Categories[Component];
            return obj != null ? (short)obj : (short)0;
        }

        public void WriteEntry(string message, EEventType eventType, int id, short category)
        {
            WriteEntry(message, EventLogHelper.GetEventType(eventType), id, category);
        }

        public void WriteEntry(string message, EEventType eventType)
        {
            WriteEntry(message, EventLogHelper.GetEventType(eventType));
        }

        public new void BeginInit()
        {
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            //
            // Do not call BeginInit() on performance counters.
            // EndInit hangs under LocalSystemAccount.
            //
        }

        public new void EndInit()
        {
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            //
            // Do not call EndInit() on performance counters.
            // EndInit hangs under LocalSystemAccount.
            //
        }
    }
}
