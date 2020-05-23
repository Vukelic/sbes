using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Users
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatePass { get; set; }
        public User()
        {
        }
        public User(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            CreatePass = DateTime.Now;
        }
    }
}

