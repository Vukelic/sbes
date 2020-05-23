using Common;
using MyCertificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class AuthenticationService : IAuthenticationService, IAuthenticationAudit
    {
       private static Dictionary<string, INotifyLogout> clients = new Dictionary<string, INotifyLogout>();     

        public List<string> GetAllLoggedUsers() //uzeli ulogovane usere
        {
            return clients.Keys.ToList();
        }
        
        [PrincipalPermission(SecurityAction.Demand, Role = "studenti")]
        public void Login(string username, string password)
        { 
            string srvCertCN = "wcfservice2";
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9955/ICredentialAudit"),
                                      new X509CertificateEndpointIdentity(srvCert));

            if (!clients.ContainsKey(username)) //pita da li je u listi ulogovanih
            {
                using (CredentialsStoreProxy proxy = new CredentialsStoreProxy(binding, address))
                {
                    try
                    {
                        if (proxy.ValidateCredential(username, password))
                        {
                            
                            INotifyLogout CallbackService = OperationContext.Current.GetCallbackChannel<INotifyLogout>(); 
                            clients.Add(username,CallbackService);
                        }
                        else
                        {
                            Console.WriteLine("User not found");
                        }
                    }
                    catch (Exception e)
                    {
                        MyException ex = new MyException();
                        ex.Message = e.Message;
                        throw new FaultException<MyException>(ex, new FaultReason(e.Message));
                    }               
                }
            }
            else
            {
                Console.WriteLine("User is already logged in");             
            }
        
         }

        [PrincipalPermission(SecurityAction.Demand, Role = "studenti")]
        public void Logout(string username)
        {
            if (clients.ContainsKey(username))
            {
                clients.Remove(username); //sklonjen nevalidan user
                Console.WriteLine($"User {username} successfully logged out!");
                INotifyLogout CallbackService = OperationContext.Current.GetCallbackChannel<INotifyLogout>();
                CallbackService.NotifyClient("You are successfully logged out!");
            }
            else
            {
                Console.WriteLine("User is already logged out");      
            }
        }

        public void LogOutClient(string username, string message)
        {
            if (clients.ContainsKey(username))
            {
                clients[username].NotifyClient(message);        //u slucaju da je user obrisan ili da mu je istekla sifra
                try
                {
                    clients.Remove(username);
                }
                catch (Exception e)
                {
                    MyException ex = new MyException();
                    ex.Message = e.Message;
                    throw new FaultException<MyException>(ex, new FaultReason(ex.Message));
                    // console.writeline("klijent se izlogovao u medjuvremenu");
                }
            }
            else
            {
                Console.WriteLine("this user is deleted");        //u slucaju da je obrisan a nije ulogovan     
            }

        }
    }
}
