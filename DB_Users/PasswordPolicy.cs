using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DB_Users
{
    public class PasswordPolicy
    {
        private static bool ComplexPass;
        private static int ExpiredTime;
        private static int NumberOfRepeats;
        private static int minLength = 5;
        private static int timeCheck = 3;

        public static void AlwaysCheck()
        {
            var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), @"C:\Users\PCX\Documents\sbes\sbes_project\DB_Users\PasswordPolicyBase.txt");
            string content = null;
            string[] parts = null;

            try
            {
                content = File.ReadAllText(path);
                parts = content.Split(';');

                if (parts[0].Equals("ok"))
                {
                    ComplexPass = true;
                }
                else
                {
                    ComplexPass = false;
                }

                if (!Int32.TryParse(parts[1], out ExpiredTime))
                    ExpiredTime = 120;

                if (!Int32.TryParse(parts[2], out NumberOfRepeats))
                    NumberOfRepeats = 3;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while reading password policy base!{0}", e.Message);
                Console.WriteLine("Deafult values will be aplied.");
                ComplexPass = true;
                ExpiredTime = 300;
                NumberOfRepeats = 3;
            }
        }

        public static bool ValidatePasswordComplex(string password)
        {
            AlwaysCheck();
            bool retVal = false;
            if (ComplexPass)
            {
                if (Regex.IsMatch(password, @"[0-9]+") && password.Length >= minLength) /// da li je pass dugacak dovoljno
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
            }
            return retVal;
        }

        public static bool ValidatePasswordTime(User u)
        {
            AlwaysCheck();
            bool retVal = false;

            var CheckTime = DateTime.Now - u.CreatePass;
            int sec = CheckTime.Days * 86400 + CheckTime.Hours * 3600 + CheckTime.Minutes * 60 + CheckTime.Seconds;
            if (sec > ExpiredTime)
            {
                retVal = true;
            }

            return retVal;
        }

        public static bool ValidatePasswordHistory(string username, string password)
        {
            AlwaysCheck();
            bool retVal = true;

            if (PasswordHistoryService.Instance.CheckNumberOfPasswordRepeats(username, password) >= NumberOfRepeats)
            {
                retVal = false;
            }
            return retVal;
        }

        public static int CheckPassword()
        {
            AlwaysCheck();
            return timeCheck * 2000;
        }

    }
}

