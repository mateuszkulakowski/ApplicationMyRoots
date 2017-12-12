using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.DAL
{
    public class DbContext : System.Data.Entity.DbContext
    {
        public DbContext() : base("MyRootsDB") { }

        public DbSet<User> Users { get; set; }

        public DbSet<Error> Errors { get; set; }

        public DbSet<FailedLogin> FailedLogins { get; set; }

        public DbSet<UserTree> UserTrees { get; set; }

        public DbSet<UserTreeNode> UserTreeNodes { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<LanguageText> LanguageTexts { get; set; }
        
        public DbSet<UserTreePhoto> UserTreePhotos { get; set; }

        public DbSet<UserTreeAlbum> UserTreeAlbums { get; set; }

        public DbSet<UserTreeSharingStatus> UserTreeSharingStatuses { get; set; }

        public DbSet<UserTreeSharingAgreement> UserTreeSharingAgreements { get; set; }

        public DbSet<UserTreeUserTreeNode> UserTreesUserTreeNodes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTreeSharingAgreement>()
                .HasRequired(a => a.UserReceiving)
                .WithMany(u => u.ReceivingAgreements)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserTreeSharingAgreement>()
                .HasRequired(a => a.UserSending)
                .WithMany(u => u.SendingAgreements)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserTree>()
                .HasOptional(a => a.UserTreeNode)
                .WithMany(u => u.NodeOtherPartners)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserTree>()
                .HasOptional(a => a.UserTreeNodePartner)
                .WithMany(u => u.NodeAsNextPartner)
                .WillCascadeOnDelete(false);
        }
    }
}