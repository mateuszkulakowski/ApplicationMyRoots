using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class Language
    {
        [Key]
        public int LanguageID { get; set; }

        public string Name { get; set; }

        [ForeignKey("User")]
        public virtual IEnumerable<User> Users { get; set; }
    }
}