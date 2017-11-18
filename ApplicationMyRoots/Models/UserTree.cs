using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTree
    {
        public UserTree()
        {
            this.Nodes = new List<UserTreeNode>();
            this.Albums = new List<UserTreeAlbum>();
        }

        [Key]
        public int UserTreeID { get; set; }

        public int UserID { get; set; }

        public virtual User User { get; set; }

        public bool isMainTree { get; set; }

        public string TreeHtmlCode { get; set; }

        public string TransformMatrix { get; set; }

        public virtual List<UserTreeNode> Nodes { get; set; }

        public virtual List<UserTreeAlbum> Albums { get; set; }


    }
}