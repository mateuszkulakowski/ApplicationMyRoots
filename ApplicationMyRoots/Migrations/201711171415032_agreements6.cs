namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agreements6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserReceivedID", "dbo.Users");
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserSendedID", "dbo.Users");
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "UserSendedID" });
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "UserReceivedID" });
            AddColumn("dbo.UserTreeSharingAgreements", "UserSendingID", c => c.Int(nullable: false));
            AddColumn("dbo.UserTreeSharingAgreements", "UserRecivingID", c => c.Int(nullable: false));
            AddColumn("dbo.UserTreeSharingAgreements", "Accpeted", c => c.Boolean(nullable: false));
            DropColumn("dbo.UserTreeSharingAgreements", "UserSendedID");
            DropColumn("dbo.UserTreeSharingAgreements", "UserReceivedID");
            DropColumn("dbo.UserTreeSharingAgreements", "Accepted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserTreeSharingAgreements", "Accepted", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserTreeSharingAgreements", "UserReceivedID", c => c.Int(nullable: false));
            AddColumn("dbo.UserTreeSharingAgreements", "UserSendedID", c => c.Int(nullable: false));
            DropColumn("dbo.UserTreeSharingAgreements", "Accpeted");
            DropColumn("dbo.UserTreeSharingAgreements", "UserRecivingID");
            DropColumn("dbo.UserTreeSharingAgreements", "UserSendingID");
            CreateIndex("dbo.UserTreeSharingAgreements", "UserReceivedID");
            CreateIndex("dbo.UserTreeSharingAgreements", "UserSendedID");
            AddForeignKey("dbo.UserTreeSharingAgreements", "UserSendedID", "dbo.Users", "UserID", cascadeDelete: true);
            AddForeignKey("dbo.UserTreeSharingAgreements", "UserReceivedID", "dbo.Users", "UserID", cascadeDelete: true);
        }
    }
}
