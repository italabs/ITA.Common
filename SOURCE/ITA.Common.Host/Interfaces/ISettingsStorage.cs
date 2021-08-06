using System;

namespace ITA.Common.Host.Interfaces
{
    public interface ISettingsStorage : IComponent
    {
        event EventHandler<ConfigurationChangedArgs> ConfigurationChanged;

        object this[string Component, string Property, object Default]
        {
            get;
        }

        object this[string Component, string Property]
        {
            get;
            set;
        }
    }

    public class ConfigurationChangedArgs : EventArgs
    {
        private readonly string m_component;
        private readonly string m_property;
        private readonly string m_oldValue;
        private readonly string m_newValue;

        public ConfigurationChangedArgs(string component, string property, string oldValue, string newValue)
        {
            m_component = component;
            m_property = property;
            m_oldValue = oldValue;
            m_newValue = newValue;
        }

        public string NewValue
        {
            get { return m_newValue; }
        }

        public string OldValue
        {
            get { return m_oldValue; }
        }

        public string Property
        {
            get { return m_property; }
        }

        public string Component
        {
            get { return m_component; }
        }
    }    
}
