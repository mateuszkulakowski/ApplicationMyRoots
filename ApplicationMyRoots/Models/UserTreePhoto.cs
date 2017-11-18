using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTreePhoto
    {

        public UserTreePhoto() { }

        [Key]
        public int UserTreePhotoID { get; set; }

        public UserTreeAlbum UserTreeAlbum { get; set; }

        public int UserTreeAlbumID { get; set; }

        public string Description { get; set; }

        public byte[] Image { get; set; }
    }
}