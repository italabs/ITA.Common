using System;

namespace ITA.Common
{
    /// <summary>
    /// Summary description for AssemblyResourceInfoAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyResourceInfoAttribute : Attribute
    {
        private readonly Type m_ExceptionType;
        private readonly string m_ResourceName;

        public AssemblyResourceInfoAttribute(Type ExType, string ResourceName)
        {
            m_ExceptionType = ExType;
            m_ResourceName = ResourceName;
        }

        public string ResourceName
        {
            get { return m_ResourceName; }
        }

        public Type ExceptionType
        {
            get { return m_ExceptionType; }
        }
    }
}