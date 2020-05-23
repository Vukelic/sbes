using Common;
using Manager;
using MyCertificates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        public static bool IsUserInGroup()
        {
            bool retVal = false;
            var identity = WindowsIdentity.GetCurrent();
            foreach (var item in identity.Groups)
            {
                var group = item.Translate(typeof(NTAccount));
                if (group.ToString().Contains("studenti"))
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }
        static void Main(string[] args)
        {
            if (!IsUserInGroup())
            {               
                NetTcpBinding myBinding = new NetTcpBinding();
                myBinding.Security.Mode = SecurityMode.Transport;
                myBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                myBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                string address = $"net.tcp://localhost:4006/AccountManagement";
                bool exit = false;

                using (AccountManagementProxy proxy = new AccountManagementProxy(myBinding, new EndpointAddress(new Uri(address))))
                {                    
                    while (!exit)
                    {
                        Console.WriteLine("\n Choose option: \n");
                        Console.WriteLine("1.Create Account \n");
                        Console.WriteLine("2.Delete Account \n");
                        Console.WriteLine("3.Reset Password \n");
                        string operation = Console.ReadLine();

                        switch (operation)
                        {
                            case "1":                               
                                Console.WriteLine("Enter username:");
                                string username = Console.ReadLine();
                                Console.WriteLine("Enter password:");
                                string pas1 = Console.ReadLine();                               
                                proxy.CreateAccount(username, pas1);
                                break;
                            case "2":
                                Console.WriteLine("Enter username:");
                                string un = Console.ReadLine();
                                Console.WriteLine("Enter password");
                                string pd = Console.ReadLine();
                                proxy.DeleteAccount(un, pd);
                                break;
                            case "3":
                                Console.WriteLine("Enter username:");
                                string us = Console.ReadLine();
                                Console.WriteLine("Enter new password");
                                string pas = Console.ReadLine();                                
                                proxy.ResetPassword(us, pas);
                                break;
                            default:
                                exit = true;
                                Console.Clear();
                                Console.WriteLine("You choose to exit \n");
                                break;
                        }
                    }
                }
            }
                else
                {
                NetTcpBinding myBinding = new NetTcpBinding();
                myBinding.Security.Mode = SecurityMode.Transport;
                myBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                myBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                string address = $"net.tcp://localhost:4001/AuthenticationService";
                bool exit = false;
                using (AuthenticationServiceProxy proxy = new AuthenticationServiceProxy(myBinding, new EndpointAddress(new Uri(address))))
                {
                    NetTcpBinding myBindingManagement = new NetTcpBinding();
                    myBinding.Security.Mode = SecurityMode.Transport;
                    myBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                    myBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                    string addressManagement = $"net.tcp://localhost:4006/AccountManagement";
                    using (UserAccountManagementProxy proxyAccManagement = new UserAccountManagementProxy(myBindingManagement, new EndpointAddress(new Uri(addressManagement))))
                    {
                        while (!exit)
                        {
                            Console.WriteLine("\n Choose option: \n");
                            Console.WriteLine("1.Login \n");
                            Console.WriteLine("2.Reset password \n");
                            Console.WriteLine("3.Logout \n");
                            string operation = Console.ReadLine();

                            switch (operation)
                            {
                                case "1":
                                    string username = WindowsIdentity.GetCurrent().Name;
                                    string[] pharse = username.Split('\\');
                                    Console.WriteLine($"Username is {pharse[1]}");
                                    string pass = WritePassword();
                                    string newPass = SecureConverter.Hash(pass);
                                    proxy.Login(pharse[1], newPass);
                                    break;
                                case "2":
                                    string my_username = WindowsIdentity.GetCurrent().Name;
                                    string[] pharse_user = my_username.Split('\\');
                                    Console.WriteLine($"Username is {pharse_user[1]}");
                                    Console.WriteLine("Enter old password:");
                                    string old_password = Console.ReadLine();
                                    string newPass3 = SecureConverter.Hash(old_password);
                                    Console.WriteLine("Enter new password");
                                    string new_password = Console.ReadLine();                                   
                                    proxyAccManagement.ResetPassword(pharse_user[1], newPass3, new_password);                                    
                                    break;
                                case "3":
                                    string name = WindowsIdentity.GetCurrent().Name;
                                    string[] pharse1 = name.Split('\\');
                                    Console.WriteLine($"Username is {pharse1[1]}");
                                    proxy.Logout(pharse1[1]);
                                    break;
                                default:
                                    exit = true;
                                    Console.Clear();
                                    Console.WriteLine("You choose to exit \n");
                                    break;
                            }
                        }
                    }
                       
                    }      
                }         
            
               Console.ReadKey();
        }

        public static string WritePassword()
        {
            Console.WriteLine("Enter your password: ");
            string pwd = "";
            ConsoleKeyInfo input;
            do
            {
                input = Console.ReadKey(true);

                if (input.Key != ConsoleKey.Backspace && input.Key != ConsoleKey.Enter)
                {
                    pwd += input.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (input.Key == ConsoleKey.Backspace && pwd.Length > 0)
                    {
                        pwd = pwd.Substring(0, (pwd.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            while (input.Key != ConsoleKey.Enter);
            return pwd;
        }
    }
}

    
