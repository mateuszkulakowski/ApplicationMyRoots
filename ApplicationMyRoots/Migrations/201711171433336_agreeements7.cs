namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agreeements7 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.UserTreeSharingAgreements", "UserSendingID");
            CreateIndex("dbo.UserTreeSharingAgreements", "UserRecivingID");
            AddForeignKey("dbo.UserTreeSharingAgreements", "UserRecivingID", "dbo.Users", "UserID");
            AddForeignKey("dbo.UserTreeSharingAgreements", "UserSendingID", "dbo.Users", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserSendingID", "dbo.Users");
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserRecivingID", "dbo.Users");
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "UserRecivingID" });
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "UserSendingID" });
        }
    }
}
