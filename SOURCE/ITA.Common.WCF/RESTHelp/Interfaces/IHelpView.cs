using System;
using System.ServiceModel.Channels;

namespace ITA.Common.WCF.RESTHelp.Interfaces
{
    internal interface IHelpView
    {
        string ContentType { get; }

        Message Show(UriTemplate template, Uri uri);
    }
}