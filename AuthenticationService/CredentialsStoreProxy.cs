using Common;
using MyCertificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;


namespace AuthenticationService
{
    public class CredentialsStoreProxy : ChannelFactory<ICredentialAudit>, ICredentialAudit
    {
        ICredentialAudit factory;
        public CredentialsStoreProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }
        public bool ValidateCredential(string username, string password)
        {
            bool retVal = true;
            try
            {
                retVal = factory.ValidateCredential(username, password);               
            }
            catch (Exception e)
            {
                retVal = false;
                MyException ex = new MyException();
                ex.Message = e.Message;
                throw new FaultException<MyException>(ex, new FaultReason(ex.Message));
            }

            return retVal;
        }
    }
}
