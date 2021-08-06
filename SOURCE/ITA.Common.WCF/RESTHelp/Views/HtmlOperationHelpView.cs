using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Xml.Linq;
using ITA.Common.WCF.RestHelp.Data;
using ITA.Common.WCF.RESTHelp.Data;
using ITA.Common.WCF.RESTHelp.Interfaces;

namespace ITA.Common.WCF.RestHelp.Views
{
    internal class HtmlOperationHelpView : HtmlBaseHelpView
    {
        private static readonly ConcurrentDictionary<Uri, string> CachedContent = new ConcurrentDictionary<Uri, string>();

        public HtmlOperationHelpView(ServiceEndpoint endpoint, IUriHelper uriHelper)
            : base(endpoint, uriHelper)
        {
        }

        public override Message Show(UriTemplate template, Uri uri)
        {
            if (WebOperationContext.Current == null)
            {
                return null;
            }
            var match = template.Match(Endpoint.Address.Uri, uri);
            if (match == null)
            {
                return null;
            }
            var operationName = match.BoundVariables[HelpPageBehavior.OPERATION_URI_PARAM];
            var content = CachedContent.GetOrAdd(uri, u => GenerateContent(operationName));
            return WebOperationContext.Current.CreateTextResponse(content, ContentType);
        }

        internal string GenerateContent(string operationName)
        {
            var document = CreateBaseDocument(Resources.HelpPageOperationTitle);
            var operation = Model.Operations.FirstOrDefault(o => o.Name == operationName);
            if (operation == null)
            {
                return string.Empty;
            }

            var operationUrl = UriHelper.GetHostAbsoluteUrl(operation.UriTemplate);

            var body = new XElement(HtmlDivElementName,
                new XAttribute(HtmlIdAttributeName, HtmlContentClass),
                new XElement(HtmlPElementName,
                    new XAttribute(HtmlClassAttributeName, HtmlHeading1Class),
                    GetString(Resources.HelpPageReferenceFor, operation.Name)),
                new XElement(HtmlAElementName,
                    new XAttribute(HtmlRelAttributeName, HtmlInputOperationClass),
                    new XAttribute(HtmlHrefAttributeName, UriHelper.GetHelpexRelativeUrl()), Resources.HelpPageOperationsAt));

            body.AddBlockIf(Resources.HelpPageOperation, operationName)                
                .AddBlockIf(Resources.HelpPageUri, operationUrl)
                .AddBlockIf(Resources.HelpPageDescription, operation.Description)
                .AddBlockIf(Resources.HelpPageMethod, operation.Method)
                .AddElement(ToInputTree(operation.MethodDocumentation))
                .AddElement(ToOutputTree(operation.MethodDocumentation))
                .AddBlock(Resources.HelpPageExamples, string.Empty);

            CreateOperationSamples(body, operation);

            document = SetBody(document, body);
            return document.ToString();
        }

        #region Operation tree view

        private XElement ToInputTree(MethodDocumentation method)
        {
            var mainUl = new XElement(HtmlUlElementName,
                new XAttribute(HtmlIdAttributeName, HtmlInputOperationClass),
                new XAttribute(HtmlClassAttributeName, HtmlFileTreeClass),
                new XElement(HtmlSpanElementName,
                    new XElement(HtmlBoldElementName, Resources.HelpPageParameters)));

            ToTree(mainUl, Resources.HelpPageInputUrlParameters,
                method.InputParameters.Where(p => p.ParamPlace == MethodParamPlace.Uri).ToArray());

            ToTree(mainUl, Resources.HelpPageInputBodyParameters,
                method.InputParameters.Where(p => p.ParamPlace == MethodParamPlace.Body).ToArray());

            ToTree(mainUl, Resources.HelpPageInputQueryParameters,
                method.InputParameters.Where(p => p.ParamPlace == MethodParamPlace.Query).ToArray());

            return new XElement(HtmlDivElementName, new XElement(HtmlPElementName, string.Empty), mainUl);
        }

        private XElement ToOutputTree(MethodDocumentation method)
        {
            if (method.OutputParameter.TypeDocumentation == null)
            {
                return new XElement(HtmlDivElementName, new XElement(HtmlPElementName, string.Empty));
            }
            var mainUl = new XElement(HtmlUlElementName,
                new XAttribute(HtmlIdAttributeName, HtmlOutputOperationClass),
                new XAttribute(HtmlClassAttributeName, HtmlFileTreeClass),
                new XElement(HtmlSpanElementName,
                    new XElement(HtmlBoldElementName, Resources.HelpPageReturnValue)));

            mainUl.Add(ToTree(method.OutputParameter));

            return new XElement(HtmlDivElementName, new XElement(HtmlPElementName, string.Empty), mainUl);
        }

        private void ToTree(XElement container, string name, MethodParamDocumentation[] parameters)
        {
            if (!parameters.Any())
                return;

            var inputParams = new XElement(HtmlLiElementName,
                new XElement(HtmlSpanElementName, name,
                    new XAttribute(HtmlClassAttributeName, HtmlFolderClass)));
            var inputParamsContainer = new XElement(HtmlUlElementName, string.Empty);
            parameters.ToList().ForEach(p => inputParamsContainer.Add(ToTree(p)));
            inputParams.Add(inputParamsContainer);
            container.Add(inputParams);
        }

        private XElement ToTree(MethodParamDocumentation param)
        {
            var li = new XElement(HtmlLiElementName,
                new XElement(HtmlSpanElementName,
                    new XElement(HtmlBoldElementName, string.IsNullOrWhiteSpace(param.ParameterName) ? string.Empty : string.Format("{0}: ", param.ParameterName)),
                    param.TypeDocumentation.GetFullName(),
                    new XAttribute(HtmlClassAttributeName, HtmlFileClass)));

            if (!string.IsNullOrWhiteSpace(param.Summary))
            {
                li.Add(new XElement(HtmlSpanElementName, param.Summary));
            }

            if (param.TypeDocumentation != null)
            {
                li.Add(ToTree(param.TypeDocumentation));
            }
            return li;
        }

        private XElement ToTree(TypeDocumentation type)
        {
            var ul = new XElement(HtmlUlElementName, new XElement(HtmlSpanElementName, string.Empty));
            if (type.IsNullable)
            {
                ul.Add(new XElement(HtmlSpanElementName,
                    new XElement(HtmlBoldElementName, Resources.HelpPageNullable)));
            }
            if (string.IsNullOrWhiteSpace(type.Summary))
            {
                ul.Add(new XElement(HtmlSpanElementName, type.Summary));
            }
            foreach (var property in type.Properties.Where(p => p.IsDataMemeber))
            {
                ul.Add(ToTree(property));
            }

            return ul;
        }

        private XElement ToTree(PropertyDocumentation property)
        {
            var name = string.IsNullOrWhiteSpace(property.DataMemberName) ? property.Property.Name : property.DataMemberName;

            var li = new XElement(HtmlLiElementName,
                new XElement(HtmlSpanElementName, new XElement(HtmlBoldElementName, string.Format("{0}: ", name)), property.TypeDocumentation.GetFullName()));

            if (!string.IsNullOrWhiteSpace(property.Summary))
            {
                li.Add(new XElement(HtmlBrElementName, new XElement(HtmlSpanElementName, property.Summary)));
            }
            if (property.TypeDocumentation != null)
            {
                li.Add(ToTree(property.TypeDocumentation));
            }
            return li;
        }

        #endregion

        #region Samples

        private static void CreateOperationSamples(XElement element, OperationInformation operationInfo)
        {
            if (!string.IsNullOrWhiteSpace(operationInfo.JsonRequestSample))
            {
                element.Add(AddSample(operationInfo.JsonRequestSample, Resources.HelpPageJsonRequest, HtmlRequestJson));
            }
            if (!string.IsNullOrWhiteSpace(operationInfo.JsonResponseSample))
            {
                element.Add(AddSample(operationInfo.JsonResponseSample, Resources.HelpPageJsonResponse, HtmlResponseJson));
            }
        }

        private static XElement AddSample(string content, string title, string label)
        {
            if (string.IsNullOrEmpty(title))
            {
                return new XElement(HtmlPElementName, new XElement(HtmlPreElementName, new XAttribute(HtmlClassAttributeName, label), content));
            }
            return new XElement(HtmlPElementName,
                new XElement(HtmlAElementName,
                    new XAttribute(HtmlNameClass, label), title),
                    new XElement(HtmlPreElementName, new XAttribute(HtmlClassAttributeName, label), content));
        }

        #endregion
    }
}