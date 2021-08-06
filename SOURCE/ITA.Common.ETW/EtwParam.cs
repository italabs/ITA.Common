using System;
using System.Diagnostics.Tracing;
using System.Reflection;

namespace ITA.Common.ETW
{
    [Serializable]
    [EventData]
    public class EtwParam
    {
        [EventIgnore]
        public bool IsByRef { get; set; }

        [EventIgnore]
        public int Index { get; set; }

        [EventIgnore]
        public Type ParameterType { get; set; }

        [EventIgnore]
        public PropertyInfo[] Properties { get; set; }

        public string Name { get; set; }
        
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}=\"{1}\"", Name, Value);
        }
    }        
}