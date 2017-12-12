namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationmanymanyusertreeusertreenodenewtable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserTreeNodes", "UserTreeID", "dbo.UserTrees");
            DropIndex("dbo.UserTreeNodes", new[] { "UserTreeID" });
            CreateTable(
                "dbo.UserTreeUserTreeNodes",
                c => new
                    {
                        UserTreeUserTreeNodeID = c.Int(nullable: false, identity: true),
                        UserTreeID = c.Int(nullable: false),
                        UserTreeNodeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTreeUserTreeNodeID)
                .ForeignKey("dbo.UserTrees", t => t.UserTreeID, cascadeDelete: true)
                .ForeignKey("dbo.UserTreeNodes", t => t.UserTreeNodeID, cascadeDelete: true)
                .Index(t => t.UserTreeID)
                .Index(t => t.UserTreeNodeID);
            
            DropColumn("dbo.UserTreeNodes", "UserTreeID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserTreeNodes", "UserTreeID", c => c.Int(nullable: false));
            DropForeignKey("dbo.UserTreeUserTreeNodes", "UserTreeNodeID", "dbo.UserTreeNodes");
            DropForeignKey("dbo.UserTreeUserTreeNodes", "UserTreeID", "dbo.UserTrees");
            DropIndex("dbo.UserTreeUserTreeNodes", new[] { "UserTreeNodeID" });
            DropIndex("dbo.UserTreeUserTreeNodes", new[] { "UserTreeID" });
            DropTable("dbo.UserTreeUserTreeNodes");
            CreateIndex("dbo.UserTreeNodes", "UserTreeID");
            AddForeignKey("dbo.UserTreeNodes", "UserTreeID", "dbo.UserTrees", "UserTreeID", cascadeDelete: true);
        }
    }
}
