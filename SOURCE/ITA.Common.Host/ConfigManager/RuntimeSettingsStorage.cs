using System;
using System.Collections;
using System.Collections.Generic;

using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.ConfigManager
{
    [Serializable]
    public class RuntimeSettingsStorage : ComponentWithEvents, ISettingsStorage
    {
        public const string cName = "RuntimeSettingsStorage";

        private readonly Dictionary<string, Hashtable> m_storage = new Dictionary<string, Hashtable>();

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
                if (Component != null && m_storage.ContainsKey(Component))
                {
                    var hashtable = m_storage[Component];
                    if (Property != null && hashtable != null)
                    {
                        var value = hashtable[Property];

                        if (null != Default)
                        {
                            bool bBinary = SerializerUtils.IsBinarySerializable(Default);

                            if (bBinary)
                            {
                                value = SerializerUtils.Deserialize(value as byte[]);                                
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
            get { return this[Component, Property, null]; }
            set
            {
                bool bBinary = SerializerUtils.IsBinarySerializable(value);

                object OldValue;
                OldValue = this[Component, Property];


                if (!m_storage.ContainsKey(Component))
                {
                    m_storage[Component] = new Hashtable();
                }
                var hashtable = m_storage[Component];
                
                if (bBinary)
                {
                    hashtable[Property] = SerializerUtils.Serialize(value);
                }
                else
                {
                    hashtable[Property] = value;
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
}
