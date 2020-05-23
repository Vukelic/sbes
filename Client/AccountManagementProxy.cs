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
    public class AccountManagementProxy : ChannelFactory<IAccountManagement>, IAccountManagement, IDisposable
    {
        IAccountManagement factory;
        public AccountManagementProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            this.factory = CreateChannel();
        }
        public void CreateAccount(string username, string password)
        {
            try
            {
                factory.CreateAccount(username, password);               
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to Administrator. Error message : {0}", e.Message);
            }
            catch (Exception e)
            {
                MyException ex = new MyException();
                ex.Message = e.Message;
                throw new FaultException<MyException>(ex, new FaultReason(ex.Message));              
            }
        }
        public void DeleteAccount(string username,string password)
        {
            try
            {
                factory.DeleteAccount(username,password);
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to Administrator. Error message : {0}", e.Message);
            }
            catch (Exception e)
            {
                MyException ex = new MyException();
                ex.Message = e.Message;
                throw new FaultException<MyException>(ex, new FaultReason(ex.Message));              
            }
        }
        public void ResetPassword(string username, string password)
        {
            try
            {
                factory.ResetPassword(username, password);      
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to Administrator. Error message : {0}", e.Message);
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