using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class Error
    {
        public int ErrorID { get; set; }

        public String Message { get; set; }

        public String StackTrace { get; set; }

        public DateTime? DateThrow { get; set; }
    }
}