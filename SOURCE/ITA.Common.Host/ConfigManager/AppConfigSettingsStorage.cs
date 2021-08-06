using System;
using System.Configuration;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.ConfigManager
{
    /// <summary>
    /// AppConfig settings support.
    /// </summary>
    [Serializable]
    public class AppConfigSettingsStorage : ComponentWithEvents, ISettingsStorage
    {
        public const string cName = "AppConfigSettingsStorage";

        public event EventHandler<ConfigurationChangedArgs> ConfigurationChanged;

        protected virtual void OnConfigurationChanged(ConfigurationChangedArgs args)
        {
            if (ConfigurationChanged != null)
            {
                ConfigurationChanged(this, args);
            }
        }
        
        public object this[string Component, string Property, object Default]
        {
            get
            {                               
                if (Component != null && Property != null)
                {
                    string keyName = GetKeyName(Component, Property);

                    var value = ConfigurationManager.AppSettings[keyName];

                    if (value != null)
                    {
                        if (Default != null && (Default is int))
                        {
                            int i = 0;
                            if (int.TryParse(value, out i))
                            {
                                return i;
                            }
                        }

                        return value;
                    }
                }
                
                return Default;
            }
        }

        public object this[string Component, string Property]
        {
            get
            {
                return this[Component, Property, null];
            }
            set
            {
                string keyName = GetKeyName(Component, Property);
                string keyValue = value != null ? value.ToString() : string.Empty;

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var valueItem = config.AppSettings.Settings[keyName];

                if (valueItem != null)
                {
                    if (valueItem.Value != keyValue)
                    {
                        string oldValue = valueItem.Value;

                        //Change parameter.
                        config.AppSettings.Settings[keyName].Value = keyValue;
                        
                        OnConfigurationChanged(new ConfigurationChangedArgs(Component, Property, oldValue, keyValue));
                    }
                }
                else
                {
                    //Add new parameter.
                    config.AppSettings.Settings.Add(keyName, keyValue);
                    OnConfigurationChanged(new ConfigurationChangedArgs(Component, Property, "''", keyValue));
                }
                
                config.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        private string GetKeyName(string Component, string Property)
        {
            string keyName = string.Format("{0}_{1}", Component, Property);
            return keyName;
        }        

        #region Overrides of ComponentWithEvents

        public override string Name
        {
            get { return cName; }
        }

        #endregion
    }
}
