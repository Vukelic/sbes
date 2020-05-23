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
    public class UserAccountManagementProxy : ChannelFactory<IUserAccountManagement>, IUserAccountManagement, IDisposable
    {
        IUserAccountManagement factory;
        public UserAccountManagementProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            this.factory = CreateChannel();
        }

        public void ResetPassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                factory.ResetPassword(username, oldPassword,newPassword);              
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
