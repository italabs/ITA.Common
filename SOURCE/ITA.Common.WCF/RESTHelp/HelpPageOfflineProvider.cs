using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using ITA.Common.WCF.RestHelp.Views;
using ITA.Common.WCF.RESTHelp.Interfaces;

namespace ITA.Common.WCF.RestHelp
{    
    public class HelpPageOfflineProvider
    {        
        public static string CONTRACT_PAGE_NAME = "Contract.html";
        public static string XML_PAGE_NAME = "ContractData.xml";
        private static string OPERATION_PAGE_NAME_PATTERN = "{0}.html";        
        private static string HTML_EXT = ".html";
        public static string DEFAULT_ENDPOINT_URL = "http://localhost:4592/RESTService";
        private static string DEFAULT_ENDPOINT_ADDRESS = "http://hostname/";
        private Uri _uri;

        public void Generate(HelpPageOfflineSettings settings)
        {
            _uri = new Uri(string.IsNullOrWhiteSpace(settings.BaseUrl) ? DEFAULT_ENDPOINT_URL : settings.BaseUrl);
            var host = new ServiceHost(settings.InterfaceImplementationType, _uri);
            var endpoint = host.AddServiceEndpoint(settings.InterfaceType, new WebHttpBinding(), "basic");
            endpoint.Behaviors.Add(new HelpPageBehavior { Enabled = true });
            host.Open();
            try
            {
                Download(settings.HelpFolderPath, endpoint);
            }
            finally
            {
                host.Close(TimeSpan.FromSeconds(30));
            }
        }

        private void Download(string folderPath, ServiceEndpoint endpoint)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var resourcesPath = Path.Combine(folderPath, "Resources\\");
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
            }

            File.WriteAllText(Path.Combine(resourcesPath, HtmlBaseHelpView.JQUERY_JS_FILE_NAME), Resources.jquery_3_2_1_min);
            File.WriteAllText(Path.Combine(resourcesPath, HtmlBaseHelpView.JQUERY_TREEVIEW_JS_FILE_NAME), Resources.jquery_treeview_js);
            File.WriteAllText(Path.Combine(resourcesPath, HtmlBaseHelpView.SCRIPT_JS_FILE_NAME), Resources.scripts);
            File.WriteAllText(Path.Combine(resourcesPath, HtmlBaseHelpView.STYLES_CSS_FILE_NAME), Resources.styles);
            File.WriteAllText(Path.Combine(resourcesPath, HtmlBaseHelpView.JQUERY_TREEVIEW_CSS_FILE_NAME), Resources.jquery_treeview_css); 

            var imagesPath = Path.Combine(resourcesPath, "Images\\");
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            Resources.file.Save(Path.Combine(imagesPath, HtmlBaseHelpView.IMAGE_FILE_NAME), ImageFormat.Gif);
            Resources.folder.Save(Path.Combine(imagesPath, HtmlBaseHelpView.IMAGE_FOLDER_NAME), ImageFormat.Gif);
            Resources.folder_closed.Save(Path.Combine(imagesPath, HtmlBaseHelpView.IMAGE_FOLDER_CLOSED_NAME), ImageFormat.Gif);
            Resources.treeview_default.Save(Path.Combine(imagesPath, HtmlBaseHelpView.IMAGE_TREEVIEW_DEFAULT_NAME), ImageFormat.Gif);
            Resources.treeview_default_line.Save(Path.Combine(imagesPath, HtmlBaseHelpView.IMAGE_TREEVIEW_DEFAULT_LINE_NAME), ImageFormat.Gif);

            File.WriteAllText(Path.Combine(folderPath, XML_PAGE_NAME), GetXmlPage(endpoint));
            File.WriteAllText(Path.Combine(folderPath, CONTRACT_PAGE_NAME), GetContractPage(endpoint));
            foreach (var pair in GetOperationPages(endpoint))
            {
                File.WriteAllText(Path.Combine(folderPath, string.Format(OPERATION_PAGE_NAME_PATTERN, pair.Key)), pair.Value);
            }
        }

        private string GetXmlPage(ServiceEndpoint endpoint)
        {
            var view = new XmlDataHelpView(endpoint, GetUriHelper());
            return view.GetXmlContent();
        }

        private string GetContractPage(ServiceEndpoint endpoint)
        {
            var view = new HtmlContractHelpView(endpoint, GetUriHelper());
            return view.GenerateContent();
        }

        private Dictionary<string, string> GetOperationPages(ServiceEndpoint endpoint)
        {
            var operations = endpoint.Contract.Operations.Select(o => o.Name);
            var view = new HtmlOperationHelpView(endpoint, GetUriHelper());
            return operations.ToDictionary(o => o, o => view.GenerateContent(o));
        }

        private IUriHelper GetUriHelper()
        {
            return new OfflineUrlHelper(_uri);
        }

        private class OfflineUrlHelper : IUriHelper
        {
            private readonly Uri _baseUri;

            public OfflineUrlHelper(Uri baseUri)
            {
                _baseUri = baseUri;
            }

            #region Implementation of IUriHelper

            public string GetHelpexRelativeUrl(string path = null)
            {
                return CONTRACT_PAGE_NAME;
            }

            public string GetRelativeUrl(string path)
            {
                if (path == HelpPageBehavior.XML_PAGE_URI)
                    return XML_PAGE_NAME;

                var operationsPath = string.Format("/{0}/", HelpPageBehavior.OPERATIONS_URI_PARAM);
                if (path.StartsWith(operationsPath))
                {
                    path = string.Format(OPERATION_PAGE_NAME_PATTERN, path.Replace(operationsPath, string.Empty));
                }

                return string.Format("{0}/{1}", ".", path != null ? path.TrimStart('/') : CONTRACT_PAGE_NAME);
            }

            public string GetHostAbsoluteUrl(string uri = null)
            {
                return string.Format("{0}/{1}/{2}", DEFAULT_ENDPOINT_ADDRESS.Trim('/'), _baseUri.PathAndQuery.Trim('/'), uri);
                
            }

            public UriTemplate GetRelativeTemplateUri(string uri)
            {
                return null;
            }

            #endregion
        }
    }

    public class HelpPageOfflineSettings
    {
        public string HelpFolderPath { get; set; }

        public string BaseUrl { get; set; }

        public Type InterfaceType { get; set; } 

        public Type InterfaceImplementationType { get; set; }
    }
}