using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DLSM.Infrastructure.CertificateServices
{
    public interface IClientCertificateService {
        X509Certificate2 GetCertificateByThumbprint(string certificateThumbPrint,
            StoreLocation certificateStoreLocation = StoreLocation.LocalMachine);
    }
    public class ClientCertificateService : IClientCertificateService {

        public X509Certificate2 GetCertificateByThumbprint(string certificateThumbPrint, StoreLocation certificateStoreLocation = StoreLocation.LocalMachine)
        {
            X509Certificate2 certificate = null;

            X509Store certificateStore = new X509Store(certificateStoreLocation);
            certificateStore.Open(OpenFlags.ReadOnly);


            X509Certificate2Collection certCollection = certificateStore.Certificates;
            foreach (X509Certificate2 cert in certCollection)
            {
                if (cert.Thumbprint != null && cert.Thumbprint.Equals(certificateThumbPrint, StringComparison.OrdinalIgnoreCase))
                {
                    certificate = cert;
                    break;
                }
            }

            if (certificate == null)
            {
                Console.WriteLine("Certificate with thumbprint {0} not found", certificateThumbPrint);
            }

            return certificate;
        }
    }
}
