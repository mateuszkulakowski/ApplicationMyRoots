namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agreements3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserReceivedID", "dbo.Users");
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserSendedID", "dbo.Users");
            DropForeignKey("dbo.UserTreeSharingAgreements", "User_UserID", "dbo.Users");
            DropForeignKey("dbo.UserTreeSharingAgreements", "User_UserID1", "dbo.Users");
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "UserSendedID" });
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "UserReceivedID" });
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "User_UserID" });
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "User_UserID1" });
            DropTable("dbo.UserTreeSharingAgreements");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserTreeSharingAgreements",
                c => new
                    {
                        UserTreeSharingAgreementID = c.Int(nullable: false, identity: true),
                        UserSendedID = c.Int(nullable: false),
                        UserReceivedID = c.Int(nullable: false),
                        User_UserID = c.Int(),
                        User_UserID1 = c.Int(),
                    })
                .PrimaryKey(t => t.UserTreeSharingAgreementID);
            
            CreateIndex("dbo.UserTreeSharingAgreements", "User_UserID1");
            CreateIndex("dbo.UserTreeSharingAgreements", "User_UserID");
            CreateIndex("dbo.UserTreeSharingAgreements", "UserReceivedID");
            CreateIndex("dbo.UserTreeSharingAgreements", "UserSendedID");
            AddForeignKey("dbo.UserTreeSharingAgreements", "User_UserID1", "dbo.Users", "UserID");
            AddForeignKey("dbo.UserTreeSharingAgreements", "User_UserID", "dbo.Users", "UserID");
            AddForeignKey("dbo.UserTreeSharingAgreements", "UserSendedID", "dbo.Users", "UserID", cascadeDelete: true);
            AddForeignKey("dbo.UserTreeSharingAgreements", "UserReceivedID", "dbo.Users", "UserID", cascadeDelete: true);
        }
    }
}
