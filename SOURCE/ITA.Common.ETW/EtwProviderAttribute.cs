using System;

namespace ITA.Common.ETW
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EtwProviderAttribute : Attribute
    {
        private string _providerName;

        public EtwProviderAttribute(string providerName)
        {
            _providerName = providerName;
        }

        public string ProviderName { get { return _providerName; } }
    }
}
