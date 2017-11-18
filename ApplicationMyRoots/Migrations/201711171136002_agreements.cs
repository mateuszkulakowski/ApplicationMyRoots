namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agreements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTreeSharingAgreements",
                c => new
                    {
                        UserTreeSharingAgreementID = c.Int(nullable: false, identity: true),
                        UserSendedID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTreeSharingAgreementID)
                .ForeignKey("dbo.Users", t => t.UserSendedID, cascadeDelete: true)
                .Index(t => t.UserSendedID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTreeSharingAgreements", "UserSendedID", "dbo.Users");
            DropIndex("dbo.UserTreeSharingAgreements", new[] { "UserSendedID" });
            DropTable("dbo.UserTreeSharingAgreements");
        }
    }
}
