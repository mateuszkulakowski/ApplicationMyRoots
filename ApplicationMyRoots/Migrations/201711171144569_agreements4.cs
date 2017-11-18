namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agreements4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTreeSharingAgreements",
                c => new
                {
                    UserTreeSharingAgreementID = c.Int(nullable: false, identity: true),
                    UserSendedID = c.Int(nullable: false),
                    UserReceivedID = c.Int(nullable: false)
                })
                .PrimaryKey(t => t.UserTreeSharingAgreementID)
                .ForeignKey("dbo.Users", t => t.UserReceivedID, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.UserSendedID, cascadeDelete: false)
                .Index(t => t.UserSendedID)
                .Index(t => t.UserReceivedID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserSendedID", "dbo.Users");
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserReceivedID", "dbo.Users");
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "UserReceivedID" });
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "UserSendedID" });
            DropTable("dbo.UserTreeSharingAgreements");
        }
    }
}
