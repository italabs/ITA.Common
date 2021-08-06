using System;

namespace ITA.Common.ETW
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false, AllowMultiple = false)]
    public class EtwTraceParamAttribute : Attribute
    {
        private readonly string _fieldNames;

        public EtwTraceParamAttribute(string fieldNames)
        {
            _fieldNames = fieldNames;
        }

        public string FieldNames
        {
            get 
            {
                return _fieldNames;
            }
        }
    }
}
