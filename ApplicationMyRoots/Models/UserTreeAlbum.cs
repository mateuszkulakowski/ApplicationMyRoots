using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTreeAlbum
    {
        public UserTreeAlbum()
        {
            this.Photoes = new List<UserTreePhoto>();
        }

        [Key]
        public int UserTreeAlbumID { get; set; }

        public string Name { get; set; }

        public UserTree UserTree { get; set; }

        public int UserTreeID { get; set; }

        public virtual List<UserTreePhoto> Photoes { get; set; }

    }
}