using Common;
using MyCertificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Security;


namespace CredentialsStore
{
    public class AuthenticationServiceAuditProxy : ChannelFactory<IAuthenticationAudit>, IAuthenticationAudit
    {
        IAuthenticationAudit factory;
        public AuthenticationServiceAuditProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
                      
            this.factory = CreateChannel();          
        }

        public List<string> GetAllLoggedUsers()
        {
            return factory.GetAllLoggedUsers();
        }
        public void LogOutClient(string username, string message)
        {
            factory.LogOutClient(username, message);
        }
    }
}
