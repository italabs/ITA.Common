using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace ITA.Common.WCF.RestHelp.Data
{
    [DataContract]
    public class PropertyDocumentation
    {
        public PropertyInfo Property { get; set; }

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        public string DataMemberName { get; set; }

        [DataMember]
        public bool IsDataMemeber { get; set; }

        [DataMember]
        public TypeDocumentation TypeDocumentation { get; set; }

        public JToken ToJson()
        {
            return new JProperty(DataMemberName ?? Property.Name, TypeDocumentation.ToJson());
        }
    }
}