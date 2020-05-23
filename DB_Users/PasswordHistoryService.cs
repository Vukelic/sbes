using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Users
{
    public class PasswordHistoryService
    {
        public string path = @"C:\Users\PCX\Documents\sbes\sbes_project\DB_Users\PasswordHistory.txt";
        private PasswordHistoryService() { }
        private static PasswordHistoryService instance;

        public static PasswordHistoryService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PasswordHistoryService();
                }
                return instance;
            }
        }

        public void AddToBase(string Username, string Password)
        {
            ModelPasswordHistory p = new ModelPasswordHistory(Username, Password);
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine($"{p.Username};{p.Password}");
            sw.Close();
            fs.Close();
        }
        public int CheckNumberOfPasswordRepeats(string username, string password)
        {
            int retVal = 0;

            List<ModelPasswordHistory> passwords = ReadFromPasswordHistoryBase();
            foreach (var item in passwords)
            {
                if (item.Username.Equals(username) && item.Password.Equals(password))
                {
                    retVal++;
                }
            }
            return retVal;
        }

        public List<ModelPasswordHistory> ReadFromPasswordHistoryBase()
        {
            List<ModelPasswordHistory> listPasswords = new List<ModelPasswordHistory>();

            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] podaci = line.Split(';');
                string Username = podaci[0];
                string Password = podaci[1];

                ModelPasswordHistory p = new ModelPasswordHistory(Username, Password);
                listPasswords.Add(p);
            }
            sr.Close();
            fs.Close();
            return listPasswords;
        }

        public void DeleteUserFromPassHistory(string username)
        {
            string[] readText = File.ReadAllLines(path);
            string retVal = $"{username}";
            File.WriteAllText(path, String.Empty);
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (string s in readText)
                {
                    string[] podaci = s.Split(';');
                    if (!podaci[0].Equals(retVal))
                    {
                        writer.WriteLine(s);
                    }
                }
            }
        }
    }
}