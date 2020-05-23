using Common;
using DB_Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CredentialsStore
{
    class ValidateCredentials : ICredentialAudit
    {
        public bool ValidateCredential(string username, string password)
        {
                bool retVal = false;
                User user = UserService.Instance.GetUser(username);
                if (user != null)
                {
                    if (user.Password.Equals(password))
                    {
                        Console.WriteLine($"User {username} successfully logged in!");
                        retVal = true;
                    }
                    else
                    {
                    Console.WriteLine("Wrong password");
                    }
                }
                else
                {
                Console.WriteLine("User does not exist");
                }

            return retVal;

            }              
        }
    }

