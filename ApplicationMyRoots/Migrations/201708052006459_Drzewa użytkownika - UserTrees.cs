namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DrzewaużytkownikaUserTrees : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTrees",
                c => new
                    {
                        UserTreeID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        TreeHtmlCode = c.String(),
                    })
                .PrimaryKey(t => t.UserTreeID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTrees", "UserID", "dbo.Users");
            DropIndex("dbo.UserTrees", new[] { "UserID" });
            DropTable("dbo.UserTrees");
        }
    }
}
