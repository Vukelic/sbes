using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.ServiceModel.Description;
using MyCertificates;
using System.Configuration;
using System.Diagnostics;

namespace AuthenticationService
{
    class Program
    {      
        static void Main(string[] args)
        {
            Console.WriteLine("Server started on port 4000.");

            ServiceHost hostClient = new ServiceHost(typeof(AuthenticationService));
            OpenHostForClients(hostClient);
            ServiceHost hostCredentialStore = new ServiceHost(typeof(AuthenticationService));
            OpenHostForCredentialStore(hostCredentialStore);

            Console.ReadLine();

            hostClient.Close();
            hostCredentialStore.Close();
        }
         static void OpenHostForClients(ServiceHost host)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
    
            string address = $"net.tcp://localhost:4001/AuthenticationService";
            host.AddServiceEndpoint(typeof(IAuthenticationService), binding, address);
            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            host.Open();
            Console.WriteLine("Service host for clients opened...");
        }
        static void OpenHostForCredentialStore(ServiceHost host)
        {
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:9000/AuthenticationService";

            host.AddServiceEndpoint(typeof(IAuthenticationAudit), binding, address);
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            
            host.Open();
            Console.WriteLine("Service host for credential store opened...");
        }
    }
}
