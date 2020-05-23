using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class MyException
    {
        private string message;
        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}