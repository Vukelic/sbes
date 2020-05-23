using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IAuthenticationAudit
    {
        [OperationContract]
        [FaultContract(typeof(MyException))]
        void LogOutClient(string username, string message);
        [OperationContract]
        List<string> GetAllLoggedUsers();

    }
}
