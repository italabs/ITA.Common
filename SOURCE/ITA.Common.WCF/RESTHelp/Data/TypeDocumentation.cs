using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ITA.Common.WCF.RESTHelp.Data;
using Newtonsoft.Json.Linq;

namespace ITA.Common.WCF.RestHelp.Data
{
    [KnownType(typeof(TypeDictDocumentation))]
    [DataContract]
    public class TypeDocumentation
    {
        public Type Type { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string JsonTypeName { get; set; }

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        public bool IsDataContract { get; set; }

        [DataMember]
        public bool IsCollection { get; set; }

        [DataMember]
        public bool IsNullable { get; set; }

        [DataMember]
        public bool IsSystem { get; set; }

        [DataMember]
        public List<PropertyDocumentation> Properties { get; set; }

        public virtual string GetFullName()
        {
            return string.Format("{3}{0} ({1}{2})", JsonTypeName, Type.FullName,
                IsCollection ? "[]" : "", IsCollection ? "Array of " : string.Empty);
        }

        public virtual JToken ToJson()
        {
            if (IsCollection)
                return new JArray { ToJsonObject() };

            return ToJsonObject();
        }

        protected virtual JToken ToJsonObject()
        {
            if (Properties != null && Properties.Any())
                return new JObject(Properties.Where(p => p.IsDataMemeber).Select(p => p.ToJson()));

            return new JValue(InformationHelper.GetConstantValue(Type));
        }
    }

    [DataContract]
    public class TypeDictDocumentation : TypeDocumentation
    {
        public const string KeyName = "key";
        public const string ValueName = "value";

        public void SetDictionary(TypeDocumentation keyType, TypeDocumentation valueType)
        {
            IsCollection = true;
            Properties = new List<PropertyDocumentation>();
            Properties.Add(new PropertyDocumentation
            {
                DataMemberName = KeyName,
                IsDataMemeber = true,
                Property = null,
                Summary = keyType.Summary,
                TypeDocumentation = keyType
            });
            Properties.Add(new PropertyDocumentation
            {
                DataMemberName = ValueName,
                IsDataMemeber = true,
                Property = null,
                Summary = valueType.Summary,
                TypeDocumentation = valueType
            });
        }

        public override string GetFullName()
        {
            return Resources.HelpPageDictionary;
        }
    }
}