using System;

namespace ITA.Common.Host
{
    /// <summary>
    /// Класс-аттрибут для указания имени операции
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class OperationNameAttribute : Attribute
    {
        // The constructor is called when the attribute is set.

        // Keep a variable internally ...
        protected string m_Name;

        public OperationNameAttribute(String Name)
        {
            m_Name = Name;
        }

        // .. and show a copy to the outside world.
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
    }
}