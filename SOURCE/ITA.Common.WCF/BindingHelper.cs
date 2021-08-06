using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace ITA.Common.WCF
{
    public static class BindingHelper
    {
        public static Binding CreateMexBindingByUri(string uri, params string[] supportedUriSchemes)
        {
            Helpers.CheckNullOrEmpty(uri, "uri");

            return CreateMexBindingByUri(new Uri(uri), supportedUriSchemes);
        }

        public static Binding CreateMexBindingByUri(Uri uri, params string[] supportedUriSchemes)
        {
            Helpers.CheckNull(uri, "uri");

            if (supportedUriSchemes.Length > 0 && !supportedUriSchemes.Contains(uri.Scheme))
            {
                throw new WCFException(WCFException.E_PROTOCOL_IS_NOT_SUPPORTED, uri.Scheme);
            }

            if (uri.Scheme == Uri.UriSchemeNetTcp)
            {
                return MetadataExchangeBindings.CreateMexTcpBinding();
            }
            if (uri.Scheme == Uri.UriSchemeNetPipe)
            {
                return MetadataExchangeBindings.CreateMexNamedPipeBinding();
            }
            if (uri.Scheme == Uri.UriSchemeHttp)
            {
                return MetadataExchangeBindings.CreateMexHttpBinding();
            }
            if (uri.Scheme == Uri.UriSchemeHttps)
            {
                return MetadataExchangeBindings.CreateMexHttpsBinding();
            }

            throw new WCFException(WCFException.E_PROTOCOL_IS_NOT_SUPPORTED, uri.Scheme);
        }

        public static Binding CreateBindingByUri(string uri, BindingOptions options, params string[] supportedUriSchemes)
        {
            Helpers.CheckNullOrEmpty(uri, "uri");
            Helpers.CheckNull(options, "options");

            return CreateBindingByUri(new Uri(uri), options, supportedUriSchemes);
        }

        public static Binding CreateBindingByUri(Uri uri, BindingOptions options, params string[] supportedUriSchemes)
        {
            Helpers.CheckNull(uri, "uri");
            Helpers.CheckNull(options, "options");

            if (supportedUriSchemes.Length > 0 && !supportedUriSchemes.Contains(uri.Scheme))
            {
                throw new WCFException(WCFException.E_PROTOCOL_IS_NOT_SUPPORTED, uri.Scheme);
            }

            if (uri.Scheme == Uri.UriSchemeNetTcp)
            {
                return CreateNetTcpBinding(options);
            }
            if (uri.Scheme == Uri.UriSchemeNetPipe)
            {
                return CreateNamedPipeBinding(options);
            }
            if (uri.Scheme == Uri.UriSchemeHttp)
            {
                return CreateBasicHttpBinding(options);
            }
            if (uri.Scheme == Uri.UriSchemeHttps)
            {
                return CreateWSHttpBinding(options);
            }

            throw new WCFException(WCFException.E_PROTOCOL_IS_NOT_SUPPORTED, uri.Scheme);
        }

        public static NetTcpBinding CreateNetTcpBinding(BindingOptions options)
        {
            Helpers.CheckNull(options, "options");

            var binding = new NetTcpBinding
            {
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                TransactionFlow = false,

                MaxBufferSize = (int)options.MaxReceivedMessageSize,
                MaxBufferPoolSize = options.MaxReceivedMessageSize,
                MaxReceivedMessageSize = options.MaxReceivedMessageSize,

                ReaderQuotas =
                {
                    MaxDepth = 400,
                    MaxStringContentLength = Int32.MaxValue,
                    MaxArrayLength = Int32.MaxValue,
                },

                OpenTimeout = options.OpenTimeout,
                SendTimeout = options.SendTimeout,
                ReceiveTimeout = options.ReceiveTimeout,

                ReliableSession =
                {
                    Enabled = options.ReliableSession,
                    InactivityTimeout = new TimeSpan(0, 2, 0, 0)
                },

                ListenBacklog = 1000,
                MaxConnections = 1000
            };

            SetSecurityType(binding, options.SecurityType);

            return binding;
        }

        public static WebHttpBinding CreateWebHttpBinding(BindingOptions options)
        {
            Helpers.CheckNull(options, "options");

            var binding = new WebHttpBinding { HostNameComparisonMode = HostNameComparisonMode.StrongWildcard };

            SetSecurityType(binding, options.SecurityType);

            binding.MaxBufferPoolSize = options.MaxReceivedMessageSize;
            binding.MaxBufferSize = (int)options.MaxReceivedMessageSize;
            binding.MaxReceivedMessageSize = options.MaxReceivedMessageSize;

            binding.ReaderQuotas.MaxDepth = 400;
            binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;

            binding.OpenTimeout = options.OpenTimeout;
            binding.SendTimeout = options.SendTimeout;
            binding.ReceiveTimeout = options.ReceiveTimeout;

            return binding;
        }

        public static BasicHttpBinding CreateBasicHttpBinding(BindingOptions options)
        {
            Helpers.CheckNull(options, "options");

            var binding = new BasicHttpBinding { HostNameComparisonMode = HostNameComparisonMode.StrongWildcard };

            SetSecurityType(binding, options.SecurityType);

            binding.MaxBufferPoolSize = options.MaxReceivedMessageSize;
            binding.MaxBufferSize = (int)options.MaxReceivedMessageSize;
            binding.MaxReceivedMessageSize = options.MaxReceivedMessageSize;

            binding.ReaderQuotas.MaxDepth = 400;
            binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;

            binding.OpenTimeout = options.OpenTimeout;
            binding.SendTimeout = options.SendTimeout;
            binding.ReceiveTimeout = options.ReceiveTimeout;

            return binding;
        }

        public static WSHttpBinding CreateWSHttpBinding(BindingOptions options)
        {
            Helpers.CheckNull(options, "options");

            var binding = new WSHttpBinding
            {
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                TransactionFlow = false,

                MaxBufferPoolSize = options.MaxReceivedMessageSize,
                MaxReceivedMessageSize = options.MaxReceivedMessageSize,

                ReaderQuotas =
                {
                    MaxDepth = 400,
                    MaxStringContentLength = Int32.MaxValue,
                    MaxArrayLength = Int32.MaxValue,
                    MaxNameTableCharCount = 16384,
                    MaxBytesPerRead = 4096
                },

                OpenTimeout = options.OpenTimeout,
                SendTimeout = options.SendTimeout,
                ReceiveTimeout = options.ReceiveTimeout,

                ReliableSession =
                {
                    Enabled = options.ReliableSession,
                    InactivityTimeout = new TimeSpan(0, 2, 0, 0)
                }
            };

            SetSecurityType(binding, options.SecurityType);

            return binding;
        }

        public static NetNamedPipeBinding CreateNamedPipeBinding(BindingOptions options)
        {
            Helpers.CheckNull(options, "options");

            return new NetNamedPipeBinding
            {
                MaxBufferPoolSize = Int32.MaxValue, // 2 TB
                MaxBufferSize = Int32.MaxValue,     // 2 TB
                MaxReceivedMessageSize = Int32.MaxValue,     // 2 TB
                ReaderQuotas = new XmlDictionaryReaderQuotas
                {
                    MaxDepth = 200,
                    MaxStringContentLength = Int32.MaxValue,
                    MaxArrayLength = Int32.MaxValue,
                    MaxNameTableCharCount = 16384,   // Default value!
                    MaxBytesPerRead = 4096   // Default value!
                },
                OpenTimeout = options.OpenTimeout,
                SendTimeout = options.SendTimeout,
                ReceiveTimeout = options.ReceiveTimeout
            };
        }

        public static WS2007FederationHttpBinding CreateFederationBinding(BindingOptions options)
        {
            Helpers.CheckNull(options, "options");

            return new WS2007FederationHttpBinding()
            {
                MaxBufferPoolSize = Int32.MaxValue, // 2 TB
                MaxReceivedMessageSize = Int32.MaxValue,     // 2 TB
                ReaderQuotas = new XmlDictionaryReaderQuotas()
                {
                    MaxDepth = 200,
                    MaxStringContentLength = Int32.MaxValue,
                    MaxArrayLength = Int32.MaxValue,
                    MaxNameTableCharCount = 16384,   // Default value!
                    MaxBytesPerRead = 4096   // Default value!
                },
                OpenTimeout = options.OpenTimeout,
                SendTimeout = options.SendTimeout,
                ReceiveTimeout = options.ReceiveTimeout
            };
        }

        #region binding securities

        private static void SetSecurityType(NetTcpBinding binding, SecurityType securityType)
        {
            if (binding != null)
            {
                switch (securityType)
                {
                    case SecurityType.Windows:
                        {
                            binding.Security.Mode = SecurityMode.Transport;
                            binding.Security.Transport = new TcpTransportSecurity
                            {
                                ClientCredentialType = TcpClientCredentialType.Windows
                            };
                            binding.Security.Message = new MessageSecurityOverTcp
                            {
                                ClientCredentialType = MessageCredentialType.Windows
                            };
                        }
                        break;
                    case SecurityType.None:
                        {
                            binding.Security.Mode = SecurityMode.None;
                        }
                        break;

                    case SecurityType.Basic:
                    case SecurityType.NTLM:
                    default:
                        {
                            throw new WCFException(WCFException.E_SECURITY_TYPE_NOT_SUPPORTED, securityType, binding.Scheme);
                        }
                }
            }
        }

        private static void SetSecurityType(WebHttpBinding binding, SecurityType securityType)
        {
            if (binding != null)
            {
                switch (securityType)
                {
                    case SecurityType.Windows:
                        {
                            binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Windows;
                        }
                        break;
                    case SecurityType.Basic:
                        {
                            binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;
                        }
                        break;
                    case SecurityType.NTLM:
                        {
                            binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
                            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Ntlm;
                        }
                        break;
                    case SecurityType.None:
                        {
                            binding.Security.Mode = WebHttpSecurityMode.None;
                        }
                        break;

                    default:
                        {
                            throw new WCFException(WCFException.E_SECURITY_TYPE_NOT_SUPPORTED, securityType, binding.Scheme);
                        }
                }
            }
        }

        private static void SetSecurityType(BasicHttpBinding binding, SecurityType securityType)
        {
            if (binding != null)
            {
                switch (securityType)
                {
                    case SecurityType.Windows:
                        {
                            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Windows;
                        }
                        break;
                    case SecurityType.Basic:
                        {
                            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;
                        }
                        break;
                    case SecurityType.NTLM:
                        {
                            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
                            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Ntlm;
                        }
                        break;
                    case SecurityType.None:
                        {
                            binding.Security.Mode = BasicHttpSecurityMode.None;
                        }
                        break;

                    default:
                        {
                            throw new WCFException(WCFException.E_SECURITY_TYPE_NOT_SUPPORTED, securityType, binding.Scheme);
                        }
                }
            }
        }

        private static void SetSecurityType(WSHttpBinding binding, SecurityType securityType)
        {
            if (binding != null)
            {
                switch (securityType)
                {
                    case SecurityType.Windows:
                        {
                            binding.Security.Mode = SecurityMode.TransportWithMessageCredential;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
                        }
                        break;
                    case SecurityType.Basic:
                        {
                            binding.Security.Mode = SecurityMode.Transport;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                            binding.Security.Message.ClientCredentialType = MessageCredentialType.None;
                        }
                        break;
                    case SecurityType.NTLM:
                        {
                            binding.Security.Mode = SecurityMode.Transport;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
                            binding.Security.Message.ClientCredentialType = MessageCredentialType.None;
                        }
                        break;
                    case SecurityType.None:
                        {
                            binding.Security.Mode = SecurityMode.None;
                        }
                        break;

                    default:
                        {
                            throw new WCFException(WCFException.E_SECURITY_TYPE_NOT_SUPPORTED, securityType, binding.Scheme);
                        }
                }
            }
        }

        #endregion
    }
}
