using System;
using System.Collections;
using System.Collections.Generic;

using ITA.Common.Host.ConfigManager;
using ITA.Common.Host.Interfaces;
using ITA.Common.Tracing;

namespace ITA.Common.Host.Components
{
    /// <summary>
    /// Управление настройками компонентов
    /// </summary>
    [Trace]
    public class ConfigManager : ComponentWithEvents, IConfigManager
    {
        private IDictionary m_Configs;
        private Dictionary<string, ISettingsStorage> m_allStorages = new Dictionary<string, ISettingsStorage>();
        private Dictionary<string, ISettingsStorage> m_componentStorages = new Dictionary<string, ISettingsStorage>();
        public const string cName = "ConfigurationManager";

        public ConfigManager(ISettingsStorage[] storages)
        {
            m_Configs = new Hashtable();

            foreach (ISettingsStorage settingsStorage in storages)
            {
                settingsStorage.ConfigurationChanged += settingsStorage_ConfigurationChanged;
                m_allStorages.Add(settingsStorage.Name, settingsStorage);
            }
        }

        void settingsStorage_ConfigurationChanged(object sender, ConfigurationChangedArgs e)
        {
            FireEvent(Interfaces.Events.OnConfigurationChanged, EEventType.SuccessAudit, e.Component, e.Property, e.OldValue, e.NewValue);
        }


        private ISettingsStorage GetSettingsStorage(string componentName)
        {
            if (componentName == null)
                throw new ArgumentNullException("componentName", "Argument cannot be null.");
            
            if (m_componentStorages.ContainsKey(componentName))
            {
                return m_componentStorages[componentName];
            }

            return m_allStorages[FileSettingsStorage.ComponentName];
        }

        #region IComponent Members

        public override string Name
        {
            get { return cName; }
        }
       
        #endregion

        #region IConfigManager Members
        public object this[string Component, string Property, object Default]
        {
            get
            {
                try
                {
                    return GetSettingsStorage(Component)[Component, Property, Default];
                }
                catch (Exception x)
                {
                    FireError(x);
                    throw new ITAException("Error has occurred while reading configuration parameter", ITAException.E_ITA_READCONFIGURATION, x, Component, Property);
                }
            }
        }

        public object this[string Component, string Property]
        {
            get
            {
                try
                {
                    object RetVal = GetSettingsStorage(Component)[Component, Property, null];
                    if (RetVal == null)
                        throw new ITAException("Mandatory configuration parameter '" + Component + "|" + Property + "'is absent", ITAException.E_ITA_NO_CONFIG_PARAMETER, null, Component, Property);

                    return RetVal;
                }
                catch (Exception x)
                {
                    FireError(x);
                    throw new ITAException("Error has occurred while reading configuration parameter", ITAException.E_ITA_READCONFIGURATION, x, Component, Property);
                }
            }
            set
            {
                try
                {
                    GetSettingsStorage(Component)[Component, Property] = value;
                }
                catch (Exception x)
                {
                    FireError(x);
                    throw new ITAException("Error has occurred while writing configuration parameter", ITAException.E_ITA_WRITECONFIGURATION, x, Component, Property);
                }
            }
        }


        public void AddConfig(IComponentConfig Config, ISettingsStorage storage)
        {
            if (Config != null)
            {
                m_Configs.Add(Config.Name, Config);

                if (storage != null)
                {
                    if (!m_allStorages.ContainsKey(storage.Name))
                    {
                        m_allStorages[storage.Name] = storage;
                    }
                    m_componentStorages[Config.Name] = m_allStorages[storage.Name];
                }
            }
        }

        public void DelConfig(string Name)
        {
            m_Configs.Remove(Name);
        }

        public IComponentConfig GetConfig(string Name)
        {
            return m_Configs[Name] as IComponentConfig;
        }

        #endregion
    }   
}