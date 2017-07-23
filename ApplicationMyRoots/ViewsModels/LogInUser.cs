using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class LogInUser
    {
        [Required]
        [Display(Name = "Login:")]
        public String Login { get; set; }

        [Required]
        [Display(Name ="Hasło:")]
        public String Password { get; set; }

        [Display(Name = "Zapamiętać?*")]
        public bool Remember { get; set; }
    }
}