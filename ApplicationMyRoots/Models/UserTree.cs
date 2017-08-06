using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTree
    {
        public int UserTreeID { get; set; }

        public int UserID { get; set; }

        public virtual User User { get; set; }

        public bool isMainTree { get; set; }

        public string TreeHtmlCode { get; set; }

        public string TransformMatrix { get; set; }
    }
}