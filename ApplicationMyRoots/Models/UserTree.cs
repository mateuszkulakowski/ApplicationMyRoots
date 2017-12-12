using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Models
{
    public class UserTree
    {
        public UserTree()
        {
            this.Albums = new List<UserTreeAlbum>();
            this.TreeNodes = new List<UserTreeUserTreeNode>();
        }

        [Key]
        public int UserTreeID { get; set; }

        public int UserID { get; set; }

        public virtual User User { get; set; }

        public int? UserTreeNodeID { get; set; }

        //węzeł dla który jest głównym elementem stworzonego drzewa -> dotyczy drzew isMainTree = false
        [ForeignKey("UserTreeNodeID")]
        public virtual UserTreeNode UserTreeNode { get; set; }

        //węzeł który jest partnerem stworzonego drzewa -> wyciągane imie nazwisko partnera przy wyborze 
        public int? UserTreeNodePartnerID { get; set; }

        [ForeignKey("UserTreeNodePartnerID")]
        public virtual UserTreeNode UserTreeNodePartner { get; set; }

        public bool isMainTree { get; set; }

        public string TreeHtmlCode { get; set; }

        public string TransformMatrix { get; set; }

        public virtual List<UserTreeAlbum> Albums { get; set; }

        public ICollection<UserTreeUserTreeNode> TreeNodes { get; set; }
    }
}