using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace ITA.Common.WCF.RestHelp.Data
{
    [DataContract]
    public class MethodParamDocumentation
    {        
        [DataMember]
        public TypeDocumentation TypeDocumentation { get; set; }

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        public MethodParamPlace ParamPlace { get; set; }

        public Type ParameterType { get; set; }

        [DataMember]
        public string ParameterName { get; set; }

        public JToken ToJson()
        {
            if (TypeDocumentation == null)
                return null;

            return string.IsNullOrWhiteSpace(ParameterName) 
                ? TypeDocumentation.ToJson() 
                : new JProperty(ParameterName, TypeDocumentation.ToJson());
        }
    }
}
