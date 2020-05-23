using Common;
using DB_Users;
using Manager;
using MyCertificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CredentialsStore
{
    public class AccountManagement : IAccountManagement, IUserAccountManagement
    {
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
        public void CreateAccount(string username, string password)
        {                  
            if (PasswordPolicy.ValidatePasswordComplex(password))
            {
                string HashPass = SecureConverter.Hash(password);
                User input = new User(username, HashPass);
                ModelPasswordHistory mPassH = new ModelPasswordHistory(username, HashPass);

                bool retval = UserService.Instance.AddUser(input);                

                if (retval == true)
                {
                    PasswordHistoryService.Instance.AddToBase(input.Username,input.Password);
                    Console.WriteLine($"User {input.Username} is successfully created");
                }
                else
                {
                    Console.WriteLine("This username is already taken");
                } 
            }
            else
            {
                Console.WriteLine("This password must contain numbers and length must be 5 characters");
            }                

        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
        public void DeleteAccount(string username,string password)
        {           
            User input = UserService.Instance.GetUser(username);
            if(input != null)
            {
                string HashPass = SecureConverter.Hash(password);
                if (input.Password == HashPass)
                {
                    UserService.Instance.DeleteUser(input);
                    PasswordHistoryService.Instance.DeleteUserFromPassHistory(input.Username);

                    string srvCertCN = "wcfservice";
                    NetTcpBinding binding = new NetTcpBinding();
                    binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                    X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
                    EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9000/AuthenticationService"),
                                              new X509CertificateEndpointIdentity(srvCert));

                    using (AuthenticationServiceAuditProxy proxy = new AuthenticationServiceAuditProxy(binding, address))
                    {
                        proxy.LogOutClient(username, "Your account has been deleted. You are logged out!");
                    }
                }
                else
                {
                    Console.WriteLine("Wrong password");
                }
            }
            else
            {
                Console.WriteLine("This user does not exist");
            }                
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
        public void ResetPassword(string username, string password)
        {
            List<string> loggedIn = new List<string>();
            User user = UserService.Instance.GetUser(username);            

            if (user != null)
            {
                
                if (PasswordPolicy.ValidatePasswordComplex(password))
                {
                    string newPass2 = SecureConverter.Hash(password);
                    if (PasswordPolicy.ValidatePasswordHistory(username, newPass2))
                    {
                        UserService.Instance.DeleteUser(user);
                        user.Password = newPass2;                  
                        user.CreatePass = DateTime.Now;
                        UserService.Instance.AddToBase(user);
                        PasswordHistoryService.Instance.AddToBase(user.Username, newPass2);

                        string srvCertCN = "wcfservice";
                        NetTcpBinding binding = new NetTcpBinding();
                        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                        X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
                        EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9000/AuthenticationService"),
                                                  new X509CertificateEndpointIdentity(srvCert));

                        using (AuthenticationServiceAuditProxy proxy = new AuthenticationServiceAuditProxy(binding, address))
                        {
                            loggedIn = proxy.GetAllLoggedUsers();
                            if (loggedIn.Contains(username))
                            {
                                proxy.LogOutClient(username, "Your password had been changed by admin. You are logged out!");
                            }                
                        }
                    }
                    else
                    {
                        Console.WriteLine("This password has been used too many times");
                    }
                }
                else
                {
                    Console.WriteLine("This password must contain numbers and length must be 5 characters");
                }
            }
            else
            {
                Console.WriteLine("User does not exist");
            }
        }

        public void ResetPassword(string username, string oldPassword, string newPassword)
        {
            List<string> loggedIn = new List<string>();
            string srvCertCN = "wcfservice";
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9000/AuthenticationService"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (AuthenticationServiceAuditProxy proxy = new AuthenticationServiceAuditProxy(binding, address))
            {
               loggedIn = proxy.GetAllLoggedUsers(); //provera da li je user ulogovan pre reset-a
            }

            if (loggedIn.Contains(username))
            {
                User user = UserService.Instance.GetUser(username);

                if (user != null)
                {
                    if (user.Password.Equals(oldPassword))
                    {
                        if (PasswordPolicy.ValidatePasswordComplex(newPassword))
                        {
                            string newPass4 = SecureConverter.Hash(newPassword);

                            if (PasswordPolicy.ValidatePasswordHistory(username, newPass4))
                            {
                                UserService.Instance.DeleteUser(user);
                                user.Password = newPass4;
                                user.CreatePass = DateTime.Now;
                                UserService.Instance.AddToBase(user);
                                PasswordHistoryService.Instance.AddToBase(user.Username, newPass4);
                            }
                            else
                            {
                                Console.WriteLine("This password has been used too many times");
                            }
                        }
                        else
                        {
                            Console.WriteLine("This password must contain numbers and length must be 5 characters");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong old password");
                    }
                }
                else
                {
                    Console.WriteLine("User does not exist");
                }
            }
            else
            {
                Console.WriteLine("User is not logged in");
            }
        }
    }
}
