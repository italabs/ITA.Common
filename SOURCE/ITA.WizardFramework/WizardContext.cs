using System.Collections.Generic;

namespace ITA.WizardFramework
{
    public class WizardContext
    {
        private Dictionary<string, object> m_Data = new Dictionary<string,object> ();

        public object this[string Key]
        {
            get 
            {
                if (!m_Data.ContainsKey(Key))
                {
                    return null;
                }
                return m_Data[Key]; 
            }
            set 
            { 
                m_Data[Key] = value; 
            }
        }

        public T ValueOf<T> ( string Key )
        {
            return ( T ) this [Key];
        }
    }
}
