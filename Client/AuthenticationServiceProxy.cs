using AuthenticationService;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class AuthenticationServiceProxy : DuplexClientBase<IAuthenticationService>, IAuthenticationService, IDisposable
    {
        IAuthenticationService factory;
        public AuthenticationServiceProxy(NetTcpBinding binding, EndpointAddress address) : base(new NotifyLogout(), binding, address)
        {
            this.factory = CreateChannel();
        }

        public void Login(string username, string password)
        {
            try
            {                
                factory.Login(username, password);              
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to RegularUser. Error message : {0}", e.Message);
            }
            catch (Exception e)
            {
                MyException ex = new MyException();
                ex.Message = e.Message;
                throw new FaultException<MyException>(ex, new FaultReason(ex.Message));              
            }
        }
        public void Logout(string username)
        {
            try
            {
                factory.Logout(username);          
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to RegularUser. Error message : {0}", e.Message);
            }
            catch (Exception e)
            {
                MyException ex = new MyException();
                ex.Message = e.Message;
                throw new FaultException<MyException>(ex, new FaultReason(ex.Message));
            }
        }
        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }

    }
}
