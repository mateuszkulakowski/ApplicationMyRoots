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

        public string Name { get; set; }

        public string Surname { get; set; }

        [NotMapped]
        public string NameSurname { get { return Name + " " + Surname; } }

        public DateTime DateBorn { get; set; }

        [NotMapped]
        public int Age
        {
            get
            {
                if (DateBorn != null)
                {
                    int yearsDifference = DateTime.Now.Year - DateBorn.Year;
                    int monthsDifference = DateTime.Now.Month - DateBorn.Month;
                    int daysDifference = DateTime.Now.Day - DateBorn.Day;

                    if (monthsDifference < 0 || daysDifference < 0) yearsDifference--;

                    return yearsDifference;
                }
                else return -1;
            }
        }

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
    }
}