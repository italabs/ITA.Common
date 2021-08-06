using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using ITA.Common.WCF.RestHelp.Data;

namespace ITA.Common.WCF.RESTHelp.Data
{
    [DataContract]
    internal sealed class ContractInformation : ICommentProvider
    {
        private static readonly ConcurrentDictionary<Type, TypeDocumentation> TypeCache = new ConcurrentDictionary<Type, TypeDocumentation>();

        private readonly List<AssemblyDocumentation> _assemblies;

        public ContractInformation(ContractDescription contractDescription)
        {
            _assemblies = contractDescription.Operations.SelectMany(o => o.SyncMethod.GetParameters())
                .Select(p => p.ParameterType.Assembly).Union(new[] { contractDescription.ContractType.Assembly })
                .Distinct().Select(a => new AssemblyDocumentation(a)).ToList();

            ContractDocumentation = GetDocumentation(contractDescription.ContractType);
            Operations = new List<OperationInformation>();
            foreach (var operation in contractDescription.Operations)
            {
                var operationInfo = new OperationInformation(operation);
                operationInfo.LoadDocumentation(this);
                Operations.Add(operationInfo);
            }           
        }

        [DataMember]
        public TypeDocumentation ContractDocumentation { get; set; }

        [DataMember]
        public List<OperationInformation> Operations { get; private set; }

        public MethodDocumentation GetMethodDocumentation(OperationInformation operation)
        {
            var template = operation.UriTemplate != operation.Name
                ? new UriTemplate(operation.UriTemplate)
                : null;
            var method = operation.MethodInfo;
            var type = method.DeclaringType;
            var parameters = method.GetParameters();
            var methodParams = string.Join(",", parameters.Select(p => p.ParameterType.FullName));
            var methodKey = string.IsNullOrWhiteSpace(methodParams)
                ? string.Format("M:{0}.{1}", type.FullName, method.Name)
                : string.Format("M:{0}.{1}({2})", type.FullName, method.Name, methodParams);

            var methodComments = GetMemberDoc(methodKey);
            var documentation = new MethodDocumentation
            {
                Method = method,
                Summary = methodComments == null ? string.Empty : methodComments.Summary,
                Returns = methodComments == null ? string.Empty : methodComments.Returns
            };

            documentation.InputParameters = parameters
                .Select(p => GetMethodParamaterDocumentation(methodComments, p.Name, p.ParameterType, template))
                .ToList();

            var returnType = operation.MethodInfo.ReturnType;

            documentation.OutputParameter = GetMethodParamaterDocumentation(methodComments, string.Empty, returnType, null);
            documentation.OutputParameter.Summary = documentation.Returns;

            return documentation;
        }
      
        private TypeDocumentation GetDocumentation(Type type)
        {
            return TypeCache.GetOrAdd(type, t =>
            {
                if (type == typeof(void))
                    return null;

                var isDictionary = typeof(IDictionary).IsAssignableFrom(type);
                var isCollection = type.IsArray;
                var isDataContract = Attribute.GetCustomAttribute(type, typeof(DataContractAttribute)) != null;
                var isList = typeof(IList).IsAssignableFrom(type);
                var isNullable = IsNullable(type);

                var realType = type.IsGenericType && (isList || isNullable) ? GetGenericTypeArgument(type) : type;
                realType = isCollection ? type.GetElementType() : realType;

                var isSystemType = realType.Assembly.GetName().Name == "mscorlib";
                var isKeyValuePair = realType.Name == typeof(KeyValuePair<,>).Name;

                var typeKey = string.Format("T:{0}", realType.FullName);
                var typeInfo = GetMemberDoc(typeKey);

                if (isDictionary || isKeyValuePair)
                {
                    var dictType = isDictionary ? type : realType;
                    var typeDictDocumentation = new TypeDictDocumentation();
                    var types = dictType.GetGenericArguments();
                    typeDictDocumentation.SetDictionary(GetDocumentation(types[0]), GetDocumentation(types[1]));
                    return typeDictDocumentation;
                }

                return new TypeDocumentation
                {
                    Type = realType,
                    IsDataContract = isDataContract,
                    Name = typeInfo == null ? realType.FullName : typeInfo.Name,
                    Summary = typeInfo == null ? string.Empty : typeInfo.Summary,
                    Properties = !isSystemType ? GetPropertyDocumentation(realType) : new List<PropertyDocumentation>(),
                    IsCollection = isCollection || isList,
                    JsonTypeName = SystemTypeToJsonType(realType),
                    IsNullable = isNullable,
                    IsSystem = isSystemType
                };
            });
        }

        private Type GetGenericTypeArgument(Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                return type.GetGenericArguments().FirstOrDefault();
            }
            return null;
        }

        private bool IsNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        private string SystemTypeToJsonType(Type type)
        {            
            if (type == typeof(string))
            {
                return Resources.HelpPageString;
            }
            if (type == typeof(bool))
            {
                return Resources.HelpPageBoolean;
            }
            if (type == typeof(int) || type == typeof(short))
            {
                return Resources.HelpPageNumeric;
            }
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                return Resources.HelpPageFloatNumeric;
            }
            return Resources.HelpPageJsonObject;
        }

        private List<PropertyDocumentation> GetPropertyDocumentation(Type type)
        {
            var result = new List<PropertyDocumentation>();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var realType = property.DeclaringType ?? type;
                var propKey = string.Format("P:{0}.{1}", realType.FullName, property.Name);
                var propInfo = GetMemberDoc(propKey);
                var attribute = Attribute.GetCustomAttribute(property, typeof(DataMemberAttribute)) as DataMemberAttribute;
                result.Add(new PropertyDocumentation
                {
                    Property = property,
                    Summary = propInfo == null ? string.Empty : propInfo.Summary,
                    IsDataMemeber = attribute != null,
                    DataMemberName = attribute != null ? attribute.Name : string.Empty,
                    TypeDocumentation = GetDocumentation(property.PropertyType)
                });
            }

            return result;
        }        

        private MethodParamDocumentation GetMethodParamaterDocumentation(MemberComment methodComments, string name, Type type , UriTemplate template)
        {            
            var summaryInfo = methodComments != null
                ? methodComments.Parameters.FirstOrDefault(p => p.Name == name)
                : null;

            var place = MethodParamPlace.Body;
            if (template != null)
            {
                if (template.PathSegmentVariableNames.Contains(name.ToUpper()))
                {
                    place = MethodParamPlace.Uri;                    
                }
                if (template.QueryValueVariableNames.Contains(name.ToUpper()))
                {
                    place = MethodParamPlace.Query;                    
                }
            }

            return new MethodParamDocumentation
            {                
                ParameterName = name,
                ParameterType = type,
                TypeDocumentation = GetDocumentation(type),
                Summary = summaryInfo != null ? summaryInfo.Description : null,                
                ParamPlace = place 
            };
        }

        private MemberComment GetMemberDoc(string key)
        {
            return _assemblies.Select(assembly => assembly.GetMemeberDoc(key)).FirstOrDefault(res => res != null);
        }
    }

    internal interface ICommentProvider
    {
        MethodDocumentation GetMethodDocumentation(OperationInformation operation);
    }
}
