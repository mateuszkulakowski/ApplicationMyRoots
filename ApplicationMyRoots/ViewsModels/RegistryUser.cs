using ApplicationMyRoots.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.ViewsModels
{
    public class RegistryUser
    {
        public String Login { get; set; }

        [Display(Name = "Hasło:")]
        public String Password { get; set; }

        [Display(Name = "Imię:")]
        public String Name { get; set; }

        [Display(Name = "Nazwisko:")]
        public String Surname { get; set; }

    }
}