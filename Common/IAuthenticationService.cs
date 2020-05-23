using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract(CallbackContract = typeof(INotifyLogout))]
    public interface IAuthenticationService
    {
        [OperationContract]
        [FaultContract(typeof(MyException))]
        void Login(string username, string password);
        [OperationContract]
        [FaultContract(typeof(MyException))]
        void Logout(string username);
    }
}
