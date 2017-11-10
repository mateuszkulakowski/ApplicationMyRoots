using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTreePhoto
    {
        [Key]
        public int UserTreePhotosID { get; set; }

        public UserTree UserTree { get; set; }

        public int UserTreeID { get; set; }

        public byte[] Image { get; set; }

    }
}