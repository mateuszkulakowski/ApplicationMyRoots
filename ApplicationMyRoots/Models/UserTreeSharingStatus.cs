using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTreeSharingStatus
    {
        public UserTreeSharingStatus() {
            this.UsersWithStatus = new List<User>();
        }

        [Key]
        public int UserTreeSharingStatusID { get; set; }

        public string Name { get; set; }

        public virtual List<User> UsersWithStatus { get; set; }

    }
}