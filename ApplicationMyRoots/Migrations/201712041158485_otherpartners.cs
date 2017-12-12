namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class otherpartners : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTrees", "UserTreeNodeID", c => c.Int());
            CreateIndex("dbo.UserTrees", "UserTreeNodeID");
            AddForeignKey("dbo.UserTrees", "UserTreeNodeID", "dbo.UserTreeNodes", "UserTreeNodeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTrees", "UserTreeNodeID", "dbo.UserTreeNodes");
            DropIndex("dbo.UserTrees", new[] { "UserTreeNodeID" });
            DropColumn("dbo.UserTrees", "UserTreeNodeID");
        }
    }
}
