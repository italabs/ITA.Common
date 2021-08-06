using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using ITA.Common.WCF.RestHelp.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ITA.Common.WCF.RESTHelp.Data
{
    [DataContract]
    internal sealed class OperationInformation
    {
        public OperationInformation(OperationDescription operationDescription)
        {
            Name = operationDescription.Name;
            MethodInfo = operationDescription.SyncMethod;
            Method = InformationHelper.GetOperationMethod(operationDescription);
            UriTemplate = InformationHelper.GetUriTemplateOrDefault(operationDescription);
            BodyStyle = InformationHelper.GetBodyStyle(operationDescription);
            IsRequestWrapped = BodyStyle == WebMessageBodyStyle.WrappedRequest ||
                               BodyStyle == WebMessageBodyStyle.Wrapped;
            IsResponseWrapped = BodyStyle == WebMessageBodyStyle.WrappedResponse ||
                                BodyStyle == WebMessageBodyStyle.Wrapped;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Method { get; set; }

        [DataMember]
        public string UriTemplate { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public MethodDocumentation MethodDocumentation { get; set; }

        [DataMember]
        public WebMessageBodyStyle BodyStyle { get; private set; }

        [DataMember]
        public string JsonRequestSample { get; private set; }

        [DataMember]
        public string JsonResponseSample { get; private set; }

        public bool IsRequestWrapped { get; set; }

        public bool IsResponseWrapped { get; set; }

        public MethodInfo MethodInfo { get; set; }

        public void LoadDocumentation(ICommentProvider commentProvider)
        {
            MethodDocumentation = commentProvider.GetMethodDocumentation(this);

            Description = MethodDocumentation.Summary;

            JsonRequestSample = GetJsonRequest();

            JsonResponseSample = GetJsonResponse();
        }

        private string GetJsonRequest()
        {
            var inputJsonParams = MethodDocumentation.InputParameters.Where(p => p.ParamPlace == MethodParamPlace.Body)
                .Select(p => p.ToJson()).ToArray();

            if (!inputJsonParams.Any())
            {
                return null;
            }

            return GetJsonString(inputJsonParams, IsRequestWrapped);
        }

        private string GetJsonResponse()
        {
            var outputJson = MethodDocumentation.OutputParameter.ToJson();
            if (outputJson == null)
            {
                return null;
            }
            return GetJsonString(new[] {outputJson}, false);
        }

        private string GetJsonString(JToken[] tokens, bool isWrapped)
        {
            if (tokens.Length == 1 && !isWrapped)
            {
                var token = tokens[0];
                var jproperty = token as JProperty;
                if (jproperty != null)
                {
                    return jproperty.Value.ToString(Formatting.Indented);
                }
                return token.ToString(Formatting.Indented);
            }

            var jobject = new JObject(tokens);
            return jobject.ToString(Formatting.Indented);
        }
    }
}