using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class User
    {
        public int UserID { get; set; }

        public String Login { get; set; }

        public byte[] Password { get; set; }

        public String Name { get; set; }

        public String Surname { get; set; }

        public int Age { get; set; }

        public DateTime DateBorn { get; set; }

        public DateTime DateSign { get; set; }

        public String City { get; set; }

    }
}