namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agreements1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserSendedID", "dbo.Users");
            AddColumn("dbo.UserTreeSharingAgreements", "UserReceivedID", c => c.Int(nullable: false));
            AddColumn("dbo.UserTreeSharingAgreements", "User_UserID", c => c.Int());
            AddColumn("dbo.UserTreeSharingAgreements", "User_UserID1", c => c.Int());
            CreateIndex("dbo.UserTreeSharingAgreements", "UserReceivedID");
            CreateIndex("dbo.UserTreeSharingAgreements", "User_UserID");
            CreateIndex("dbo.UserTreeSharingAgreements", "User_UserID1");
            AddForeignKey("dbo.UserTreeSharingAgreements", "UserReceivedID", "dbo.Users", "UserID", cascadeDelete: true);
            AddForeignKey("dbo.UserTreeSharingAgreements", "User_UserID", "dbo.Users", "UserID");
            AddForeignKey("dbo.UserTreeSharingAgreements", "User_UserID1", "dbo.Users", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTreeSharingAgreements", "User_UserID1", "dbo.Users");
            DropForeignKey("dbo.UserTreeSharingAgreements", "User_UserID", "dbo.Users");
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserReceivedID", "dbo.Users");
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "User_UserID1" });
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "User_UserID" });
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "UserReceivedID" });
            DropColumn("dbo.UserTreeSharingAgreements", "User_UserID1");
            DropColumn("dbo.UserTreeSharingAgreements", "User_UserID");
            DropColumn("dbo.UserTreeSharingAgreements", "UserReceivedID");
            AddForeignKey("dbo.UserTreeSharingAgreements", "UserSendedID", "dbo.Users", "UserID", cascadeDelete: true);
        }
    }
}
