using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Users
{
    public class UserService
    {
        public string path = @"C:\Users\PCX\Documents\sbes\sbes_project\DB_Users\\Users.txt";
        private UserService() { }
        private static UserService instance;

        public static UserService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserService();
                }
                return instance;
            }
        }
        public User GetUser(string username)
        {
            List<User> listUsers = new List<User>();
            User u = null;
            listUsers = ReadFromBase();
            foreach (var item in listUsers)
            {
                if (item.Username == username)
                {
                    u = item;
                    break;
                }
            }
            return u;
        }

        public bool AddUser(User user)
        {
            bool retVal = false;
            if (UserExists(user.Username))
            {
                retVal = false;
            }
            else
            {
                AddToBase(user);
                retVal = true;
            }
            return retVal;
        }

        public void AddToBase(User u)
        {

            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine($"{u.Username};{u.Password};{u.CreatePass}");
            sw.Close();
            fs.Close();
        }

        public List<User> ReadFromBase()
        {
            List<User> listUsers = new List<User>();

            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] podaci = line.Split(';');
                string Username = podaci[0];
                string Password = podaci[1];
                string Date = podaci[2];

                User u = new User();
                u.Username = Username;
                u.Password = Password;
                u.CreatePass = DateTime.Parse(Date);
                listUsers.Add(u);
            }
            sr.Close();
            fs.Close();
            return listUsers;
        }
        public bool UserExists(string username)
        {
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                }
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] podaci = line.Split(';');
                if (username == podaci[0])
                {
                    sr.Close();
                    fs.Close();
                    return true;
                }
            }

            sr.Close();
            fs.Close();
            return false;
        }

        public void DeleteUser(User u)
        {
            string[] readText = File.ReadAllLines(path);
            string retVal = $"{u.Username};{u.Password};{u.CreatePass}";
            File.WriteAllText(path, String.Empty);
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (string s in readText)
                {
                    if (!s.Equals(retVal))
                    {
                        writer.WriteLine(s);
                    }
                }
            }
        }
    }
}
