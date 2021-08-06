using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ITA.Common.Host.Interfaces;
using Newtonsoft.Json;

namespace ITA.Common.Host.ConfigManager
{
    /// <summary>
    /// Custom file settings storage
    /// </summary>
    [Serializable]
    public class FileSettingsStorage : ComponentWithEvents, ISettingsStorage
    {
        public const string ComponentName = "FileSettingsStorage";

        private const string DefaultSettingsFileName = "AppSettings.json";
        private static string _settingsFileName;
        private ApplicationConfig _config = new ApplicationConfig();

        private static object componentSettingsLock = new object();

        public FileSettingsStorage() : this(DefaultSettingsFileName)
        {
        }
        public FileSettingsStorage(string settingsFileName)
        {
            if (string.IsNullOrEmpty(Path.GetDirectoryName(settingsFileName)))
            {
                settingsFileName =
                    Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(FileSettingsStorage)).Location), settingsFileName);
            }
            _settingsFileName = settingsFileName;
            LoadSettingsFile();
        }

        public override string Name
        {
            get { return ComponentName; }
        }

        public event EventHandler<ConfigurationChangedArgs> ConfigurationChanged;

        public object this[string Component, string Property, object Default]
        {
            get
            {
                if (Component != null && Property != null)
                {
                    
                    var value = _config.GetSettingValue(Component, Property, Default);

                    if (value != null)
                    {
                        if (Default != null && (Default is int) && int.TryParse(value.ToString(), out int i))
                        {
                            return i;
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
                var oldValue = _config.GetSettingValue(Component, Property);
                if (oldValue != value)
                {
                    _config.SetSettingValue(Component, Property, value, componentSettingsLock);
                    SaveSettingsFile();
                    OnConfigurationChanged(new ConfigurationChangedArgs(Component, Property, oldValue != null ? oldValue.ToString() : string.Empty, value != null ? value.ToString() : string.Empty));
                }
            }
        }


        protected virtual void OnConfigurationChanged(ConfigurationChangedArgs args)
        {
            ConfigurationChanged?.Invoke(this, args);
        }

        private void SaveSettingsFile()
        {
            try
            {
                File.WriteAllText(_settingsFileName, JsonConvert.SerializeObject(_config, Formatting.Indented));
            }
            catch (Exception exc)
            {
                throw new ITAException($"An error occurred while saving settings to '{_settingsFileName}'", ITAException.E_ITA_SAVE_FILE_STORAGE_SETTINGS, exc, _settingsFileName);
            }
        }

        private void LoadSettingsFile()
        {
            try
            {
                if (File.Exists(_settingsFileName))
                {
                    _config = JsonConvert.DeserializeObject<ApplicationConfig>(File.ReadAllText(_settingsFileName));
                }
                else
                {
                    SaveSettingsFile();
                }
            }
            catch (Exception exc)
            {
                throw new ITAException($"An error occurred while loading settings from '{_settingsFileName}'", ITAException.E_ITA_LOAD_FILE_STORAGE_SETTINGS, exc, _settingsFileName);
            }
        }
    }

    internal class SettingInfo
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    internal class ComponentSettings
    {
        public string Name { get; set; }
        public ConcurrentDictionary<string, object> Settings { get; set; } = new ConcurrentDictionary<string, object>();
    }

    internal class ApplicationConfig
    {
        public string Name { get; set; }
        public List<ComponentSettings> Settings { get; set; }= new List<ComponentSettings>();

        public object GetSettingValue(string component, string property, object defaultValue = null)
        {
            var setting =
                Settings.FirstOrDefault(s => s.Name.Equals(component, StringComparison.InvariantCultureIgnoreCase));
            if (setting != null)
            {
                if (setting.Settings.TryGetValue(property, out object val))
                {
                    return val;
                }
            }

            return defaultValue;
        }

        public void SetSettingValue(string component, string property, object value, object componentSettingsLock, bool throwIfNotExists = false)
        {
            var setting = Settings.FirstOrDefault(s => s.Name.Equals(component, StringComparison.InvariantCultureIgnoreCase));

            if (setting == null)
            {
                if (throwIfNotExists)
                {
                    throw new ITAException(
                        $"Property '{property}' not found in component '{component}'.",
                        ITAException.E_ITA_COMPONENT_PROPERTY_NOT_FOUND,
                        property,
                        component);
                }

                lock (componentSettingsLock)
                {
                    setting = Settings.FirstOrDefault(s => s.Name.Equals(component, StringComparison.InvariantCultureIgnoreCase));

                    if (setting == null)
                    {
                        setting = new ComponentSettings { Name = component };
                        Settings.Add(setting);
                    }
                }
            }

            setting.Settings[property] = value;
        }
    }
}
