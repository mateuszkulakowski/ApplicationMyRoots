using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTreeUserTreeNode
    {
        public UserTreeUserTreeNode() { }

        [Key]
        public int UserTreeUserTreeNodeID { get; set; }

        public int UserTreeID { get; set; }

        [ForeignKey("UserTreeID")]
        public virtual UserTree UserTree { get; set; }

        public int UserTreeNodeID { get; set; }

        [ForeignKey("UserTreeNodeID")]
        public virtual UserTreeNode UserTreeNode { get; set; }
    }
}