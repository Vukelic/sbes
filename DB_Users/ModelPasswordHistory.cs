using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Users
{
    public class ModelPasswordHistory
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public ModelPasswordHistory(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public ModelPasswordHistory()
        {

        }
    }
}
