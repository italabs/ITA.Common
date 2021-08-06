using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using ITA.Common.WCF.RestHelp;
using ITA.Common.WCF.RestHelp.Views;
using ITA.Common.WCF.RESTHelp.Interfaces;

namespace ITA.Common.WCF.RESTHelp
{
    internal class HelpViewResolver : MessageFilter
    {
        private readonly Uri _endpointUri;
        private readonly IUriHelper _uriHelper;
        private readonly List<HelpViewResolverItem> _items;

        public HelpViewResolver(ServiceEndpoint endpoint, IUriHelper uriHelper)
        {
            _uriHelper = uriHelper;
            _endpointUri = endpoint.Address.Uri;

            _items = new List<HelpViewResolverItem>
            {
                new HelpViewResolverItem
                {
                    Template = _uriHelper.GetRelativeTemplateUri(HelpPageBehavior.XML_PAGE_URI),
                    View = new XmlDataHelpView(endpoint, _uriHelper)
                },
                new HelpViewResolverItem
                {
                    Template = _uriHelper.GetRelativeTemplateUri(string.Empty), 
                    View = new HtmlContractHelpView(endpoint, _uriHelper)
                },
                new HelpViewResolverItem
                {
                    Template = _uriHelper.GetRelativeTemplateUri(HelpPageBehavior.OPERATION_URI_TEMPLATE), 
                    View = new HtmlOperationHelpView(endpoint, _uriHelper)
                },
                new HelpViewResolverItem
                {
                    Template = _uriHelper.GetRelativeTemplateUri(HelpPageBehavior.FILES_TEMPLATE),
                    View = new FileHelpView(endpoint, _uriHelper)
                },
                new HelpViewResolverItem
                {
                    Template = _uriHelper.GetRelativeTemplateUri(HelpPageBehavior.IMAGES_TEMPLATE),
                    View = new FileHelpView(endpoint, _uriHelper)
                }                       
            };
        }

        public Message Resolve(Uri uri)
        {
            var item = GetTemplate(uri);
            if (item == null)
            {
                return null;
            }

            return item.View.Show(item.Template, uri);
        }

        public override bool Match(MessageBuffer buffer)
        {
            return true;
        }

        public override bool Match(Message message)
        {
            return GetTemplate(message.Headers.To) != null;
        }

        private HelpViewResolverItem GetTemplate(Uri uri)
        {
            var hostUri = new Uri(_endpointUri.GetLeftPart(UriPartial.Authority));

            var candidate = new Uri(hostUri, uri.AbsolutePath);

            foreach (var item in _items)
            {
                var matchResult = item.Template.Match(_endpointUri, candidate);
                if (matchResult != null)
                {
                    return item;
                }
            }
            return null;
        }        
    }

    internal class HelpViewResolverItem
    {        
        public UriTemplate Template { get; set; }

        public IHelpView View { get; set; }
    }
}