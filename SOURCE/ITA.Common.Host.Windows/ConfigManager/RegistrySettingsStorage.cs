using System;
using System.Collections;
using ITA.Common.Host.Interfaces;

using Microsoft.Win32;

namespace ITA.Common.Host.ConfigManager
{    
    /// <summary>
    /// Register settings support (HKLM by default)
    /// </summary>
    [Serializable]
    public class RegistrySettingsStorage : ComponentWithEvents, ISettingsStorage
    {
        public const string cName = "RegistrySettingsStorage";
        
        private string m_szRegRoot;
        
        private RegistryKey m_RegKeyRoot;

        public event EventHandler<ConfigurationChangedArgs> ConfigurationChanged;

        public RegistrySettingsStorage(ICommandContext context, string pathRoot)
        {
            m_szRegRoot = string.Format(pathRoot, context.InstanceName);
            m_RegKeyRoot = Registry.LocalMachine;
        }

        protected RegistrySettingsStorage(RegistryKey root, string path)
        {
            ITA.Common.Helpers.CheckNull(root, "Root");
            ITA.Common.Helpers.CheckNull(path, "Path");

            m_RegKeyRoot = root;
            m_szRegRoot = path;
        }

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
                string szKey = m_szRegRoot + "\\" + Component;
                object Val = null;
                using (RegistryKey ComponentKey = m_RegKeyRoot.OpenSubKey(szKey))
                {
                    if (null == ComponentKey)
                    {
                        return Default;
                    }

                    Val = ComponentKey.GetValue(Property, Default);
                }

                bool bBinary = false;

                if (null != Default)
                {
                    bBinary = SerializerUtils.IsBinarySerializable(Default);
                    
                    if (bBinary)
                    {
                        Val = SerializerUtils.Deserialize(Val as byte[]); 
                    }
                }

                return Val;
            }
        }

        public object this[string Component, string Property]
        {
            get
            {
                object RetVal = this[Component, Property, null];
                return RetVal;
            }
            set
            {
                bool bBinary = SerializerUtils.IsBinarySerializable(value);

                string szKey = m_szRegRoot + "\\" + Component;
                object OldValue = null;
                using (RegistryKey ComponentKey = m_RegKeyRoot.CreateSubKey(szKey))
                {
                    OldValue = ComponentKey.GetValue(Property);

                    if (bBinary)
                    {
                        ComponentKey.SetValue(Property, SerializerUtils.Serialize(value));

                        if (null != OldValue)
                        {
                            OldValue = SerializerUtils.Deserialize(OldValue as byte[]);                             
                        }
                    }
                    else
                    {
                        ComponentKey.SetValue(Property, value);
                    }
                }
                
                //
                // Compare
                //
                bool bNotEqual = false;
                if (OldValue is IList && value is IList)
                {
                    //
                    // compare lists
                    //
                    IList OldList = OldValue as IList;
                    IList NewList = value as IList;

                    if (OldList.Count != NewList.Count)
                    {
                        bNotEqual = true;
                    }
                    else
                    {
                        for (int i = 0; i < OldList.Count; i++)
                        {
                            if (!OldList[i].Equals(NewList[i]))
                            {
                                bNotEqual = true;
                                break;
                            }
                        }
                    }
                }
                else if (OldValue == null && value != null)
                {
                    bNotEqual = true;
                }
                else if (OldValue != null && value == null)
                {
                    bNotEqual = true;
                }
                else if (OldValue != null && !OldValue.Equals(value))
                {
                    bNotEqual = true;
                }
                
                //
                // Report event in case of real changes
                //
                if (bNotEqual)
                {
                    string Old = OldValue != null ? OldValue.ToString() : "<undefined value>";
                    if (OldValue is IList)
                    {
                        Old = "'";
                        foreach (object O in (IList)OldValue)
                        {
                            Old += O + ", ";
                        }
                        Old += "'";
                    }

                    string New = value != null ? value.ToString() : "<undefined value>";
                    if (value is IList)
                    {
                        New = "'";
                        foreach (object O in (IList)value)
                        {
                            New += O + ", ";
                        }
                        New += "'";
                    }
                    OnConfigurationChanged(new ConfigurationChangedArgs(Component, Property, Old, New));
                }
            }
        }

        #region Overrides of ComponentWithEvents

        public override string Name
        {
            get { return cName; }
        }
       
        #endregion
    }

    public class RegistrySettingsStorageHKLM : RegistrySettingsStorage
    {
        public RegistrySettingsStorageHKLM(string path)
            : base(Registry.LocalMachine, path)
        {
        }
    }

    public class RegistrySettingsStorageHKCU : RegistrySettingsStorage
    {
        public RegistrySettingsStorageHKCU(string path)
            : base(Registry.CurrentUser, path)
        {
        }
    }

    public class RegistrySettingsStorageHKCR : RegistrySettingsStorage
    {
        public RegistrySettingsStorageHKCR(string path)
            : base(Registry.ClassesRoot, path)
        {
        }
    }

    public class RegistrySettingsStorageHKUS : RegistrySettingsStorage
    {
        public RegistrySettingsStorageHKUS(string path)
            : base(Registry.Users, path)
        {
        }
    }
}
