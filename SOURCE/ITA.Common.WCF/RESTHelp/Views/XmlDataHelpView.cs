using System;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using ITA.Common.WCF.RESTHelp.Interfaces;

namespace ITA.Common.WCF.RestHelp.Views
{
    internal class XmlDataHelpView : BaseHelpView
    {
        private string _xmlContext;

        public XmlDataHelpView(ServiceEndpoint endpoint, IUriHelper uriHelper)
            : base(endpoint, uriHelper)
        {
        }

        public override string ContentType { get { return Resources.HelpPageXml; } }

        public override Message Show(UriTemplate template, Uri uri)
        {
            if (_xmlContext == null)
            {
                _xmlContext = GetXmlContent();
            }
            return WebOperationContext.Current.CreateTextResponse(_xmlContext, ContentType);
        }

        internal string GetXmlContent()
        {
            var serializer = new DataContractSerializer(Model.GetType());
            var xmlWriterSettings = new XmlWriterSettings { Indent = true };
            var builder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(builder, xmlWriterSettings))
            {
                serializer.WriteObject(xmlWriter, Model);
            }
            return builder.ToString();
        }
    }
}