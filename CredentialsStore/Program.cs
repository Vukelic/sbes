using Common;
using MyCertificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using System.IdentityModel.Policy;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using DB_Users;

namespace CredentialsStore
{
    class Program
    {

        static void Main(string[] args)
        {          
            Thread t = new Thread(ValidatePasTime);
            ServiceHost hostClient = new ServiceHost(typeof(AccountManagement));
            OpenHostForClients(hostClient);

            ServiceHost hostCredentialCheck = new ServiceHost(typeof(ValidateCredentials));
            OpenHostAS(hostCredentialCheck);

            t.Start();

            Console.ReadLine();
            hostClient.Close();           
        }

        static void OpenHostAS(ServiceHost host)
        {
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:9955/ICredentialAudit";

            host.AddServiceEndpoint(typeof(ICredentialAudit), binding, address);
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            host.Open();
            Console.WriteLine("Service host for credential store opened...");
        }
        static void OpenHostForClients(ServiceHost host)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            string address = $"net.tcp://localhost:4006/AccountManagement";
            host.AddServiceEndpoint(typeof(IAccountManagement), binding, address);
            host.AddServiceEndpoint(typeof(IUserAccountManagement), binding, address);
            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            host.Open();
            Console.WriteLine("CredentialsService host for clients opened...");
        }

        static void ValidatePasTime()
        {
            string srvCertCN = "wcfservice";
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9000/AuthenticationService"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (AuthenticationServiceAuditProxy proxy = new AuthenticationServiceAuditProxy(binding, address))
            {
                while (true)
                {
                    try
                    {
                        List<string> loggedUsers = proxy.GetAllLoggedUsers();
                        Console.WriteLine($"Ima ih {loggedUsers.Count}");
                        foreach (string user in loggedUsers)
                        {
                            if (PasswordPolicy.ValidatePasswordTime(UserService.Instance.GetUser(user)))
                            {                             
                                proxy.LogOutClient(user, "Your password has been expired.Please conntact admin.You will be logged out...");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MyException ex = new MyException();
                        ex.Message = e.Message;
                        throw new FaultException<MyException>(ex, new FaultReason(ex.Message));
                    }
                    Thread.Sleep(PasswordPolicy.CheckPassword());
                }
            }
        }
    }
}