using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class FailedLogin
    {
        [Key]
        public int FailedLoginID { get; set; }

        public String Message { get; set; }

        public int UserID { get; set; }

        public DateTime? DateLogin { get; set; }
    }
}