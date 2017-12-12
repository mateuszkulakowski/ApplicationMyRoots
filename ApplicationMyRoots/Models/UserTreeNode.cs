using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTreeNode
    {
        public UserTreeNode() {
            this.NodeOtherPartners = new List<UserTree>();
            this.NodeInTrees = new List<UserTreeUserTreeNode>();
        }

        public UserTreeNode(int extid, string name, string surname, DateTime? dateborn, DateTime? datedead, string additionalinfo, int mainuser)
        {
            this.ExtID = extid;
            this.Name = name;
            this.Surname = surname;
            this.DateBorn = dateborn;
            this.DateDead = datedead;
            this.AdditionalInfo = additionalinfo;
            this.MainUser = mainuser;

            this.NodeOtherPartners = new List<UserTree>();
            this.NodeInTrees = new List<UserTreeUserTreeNode>();
        }

        public int UserTreeNodeID { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        [NotMapped]
        public string NameSurname { get { return Name + " " + Surname; } }

        // -----
        //2 pola poniższe do kopiowania drzew innych użytkowników
        [NotMapped]
        public int MainUser;

        [NotMapped]
        public int ExtID; //pole wykorzystywane przy kopiowaniu z polem mainuser tworzą klucz który informuje: mainuser-0 => extid to usertreenodeid w tabelce usertreenode, mainuser-1 => extid to userid z tabelki User
        // ------

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

        public virtual ICollection<UserTree> NodeOtherPartners { get; set; }

        //klucz z polem UserTreeNodePartner -> UserTree
        public virtual ICollection<UserTree> NodeAsNextPartner { get; set; }

        public virtual ICollection<UserTreeUserTreeNode> NodeInTrees { get; set; }
        
    }
}