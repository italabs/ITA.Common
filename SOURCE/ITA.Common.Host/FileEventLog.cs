using System;
using System.Collections.Generic;
using ITA.Common.Host.Interfaces;
using log4net;

namespace ITA.Common.Host
{
    public class FileEventLog : IEventLog
    {
        private ILog _logger = Log4NetItaHelper.GetLogger(typeof(FileEventLog).Name);
        private object _lock = new object();
        private Dictionary<string, short> _categories = new Dictionary<string, short>();

        public void AddCategory(string Component, short CatId)
        {
            lock (_lock)
            {
                if (_categories.ContainsKey(Component))
                {
                    throw new Exception($"Component '{Component}' already added.");
                }

                _categories.Add(Component, CatId);
            }
        }

        public void RemoveCategory(string Component)
        {
            lock (_lock)
            {
                if (_categories.ContainsKey(Component))
                {
                    _categories.Remove(Component);
                }
            }
        }

        public short GetCategory(string Component)
        {
            lock (_lock)
            {
                if (_categories.ContainsKey(Component))
                {
                    return _categories[Component];
                }
                return -1;
            }
        }

        public string Log { get; set; }
        public string Source { get; set; }
        public void WriteEntry(string message, EEventType eventType, int id, short category)
        {
            _logger.Debug($"EventType: {eventType}, id: {id}, category: {category}, message: {message}.");
        }

        public void WriteEntry(string message, EEventType eventType)
        {
            _logger.Debug($"EventType: {eventType}, message: {message}.");
        }

        public void BeginInit()
        {
            
        }

        public void EndInit()
        {
            
        }
    }
}
