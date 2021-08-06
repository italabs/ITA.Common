using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using log4net;

namespace ITA.Common.WCF
{
    public static class ServiceHostExtensions
    {
        public static void SetCertificate(this ServiceHost serviceHost, string thumbPrint, ILog logger, 
            StoreLocation sl = StoreLocation.LocalMachine, StoreName sn = StoreName.My, X509FindType ft = X509FindType.FindByThumbprint)
        {
            Helpers.CheckNull(thumbPrint, "thumbPrint");

            if (logger != null)
            {
                logger.Debug("SetCertificate");

                logger.DebugFormat("\tStoreLocation: {0}", sl);
                logger.DebugFormat("\tStoreName: {0}", sn);
                logger.DebugFormat("\tX509FindType: {0}", ft);
                logger.DebugFormat("\tThumbprint: {0}", thumbPrint);
            }

            serviceHost.Credentials.ServiceCertificate.SetCertificate(sl, sn, ft, thumbPrint);

            if (logger != null)
            {
                logger.Debug("SetCertificate succeeded");
            }
        }

        public static void CloseAndDispose(this ServiceHost serviceHost, ILog logger)
        {
            if (serviceHost != null)
            {
                try
                {
                    if (serviceHost.State != CommunicationState.Closed)
                    {
                        serviceHost.Close();
                        if (logger != null)
                        {
                            logger.Debug("ServiceHost is closed.");
                        }
                    }
                    ((IDisposable)serviceHost).Dispose();
                }
                catch (Exception ex)
                {
                    // только логгируем: host.Dispose() вызывает почему-то исключение, если хост в faulted state
                    if (logger != null)
                    {
                        logger.Warn("Error when disposing ServiceHost.", ex);
                    }
                }
            }
        }
    }
}
