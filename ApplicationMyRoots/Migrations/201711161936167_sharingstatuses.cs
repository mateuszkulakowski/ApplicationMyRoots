namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sharingstatuses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTreeSharingStatus",
                c => new
                    {
                        UserTreeSharingStatusID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.UserTreeSharingStatusID);
            
            AddColumn("dbo.Users", "UserTreeSharingStatus_UserTreeSharingStatusID", c => c.Int());
            CreateIndex("dbo.Users", "UserTreeSharingStatus_UserTreeSharingStatusID");
            AddForeignKey("dbo.Users", "UserTreeSharingStatus_UserTreeSharingStatusID", "dbo.UserTreeSharingStatus", "UserTreeSharingStatusID");
            DropColumn("dbo.Users", "Age");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Age", c => c.Int(nullable: false));
            DropForeignKey("dbo.Users", "UserTreeSharingStatus_UserTreeSharingStatusID", "dbo.UserTreeSharingStatus");
            DropIndex("dbo.Users", new[] { "UserTreeSharingStatus_UserTreeSharingStatusID" });
            DropColumn("dbo.Users", "UserTreeSharingStatus_UserTreeSharingStatusID");
            DropTable("dbo.UserTreeSharingStatus");
        }
    }
}
