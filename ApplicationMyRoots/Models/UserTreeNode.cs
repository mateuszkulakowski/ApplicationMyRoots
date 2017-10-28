using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTreeNode
    {
        public int UserTreeNodeID { get; set; }

        public int UserTreeID { get; set; }

        public virtual UserTree UserTree { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        [NotMapped]
        public string NameSurname { get { return Name + " " + Surname; } }

        public DateTime? DateBorn { get; set; }

        public DateTime? DateDead { get; set; }

        public byte[] Image { get; set; }

        public char? Gender { get; set; }

        [NotMapped]
        public string GenderFullName
        {
            get
            {
                if (Gender != null)
                {
                    if (Gender.ToString().ToUpper().Equals('K') || Gender.ToString().ToUpper().Equals('W')) return "Kobieta";
                    else if (Gender.ToString().ToUpper().Equals('M') || Gender.ToString().ToUpper().Equals('M')) return "Mężczyzna";
                    else return "Brak dopasowania";
                }
                else return "Nieokreślono";
            }
        }

        public string AdditionalInfo { get; set; }
    }
}