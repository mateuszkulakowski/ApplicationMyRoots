using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class LanguageText
    {
        public int LanguageTextID { get; set; }

        public int LanguageID { get; set; }

        public int UniqueElementTag { get; set; }

        public Language Language { get; set; }

        public string Text { get; set; }
    }
}