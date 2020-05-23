using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IUserAccountManagement
    {
        [OperationContract]
        [FaultContract(typeof(MyException))]
        void ResetPassword(string username, string oldPassword, string newPassword);
    }
}
