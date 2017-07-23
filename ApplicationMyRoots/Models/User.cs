using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class User
    {
        public User() { }

        public User(String Login, String Password, String Name, String Surname)
        {
            this.Login = Login;
            this.Password = Password;
            this.Name = Name;
            this.Surname = Surname;
        }

        public int UserID { get; set; }

        public String Login { get; set; }

        public String Password { get; set; }

        public byte[] PasswordEncoded { get; set; }

        public String Name { get; set; }

        public String Surname { get; set; }

        public int Age { get; set; }

        public DateTime? DateBorn { get; set; }

        public DateTime? DateSign { get; set; }

        public String City { get; set; }

    }
}