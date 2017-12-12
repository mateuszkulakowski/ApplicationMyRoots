namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfieldusertreenodepartnerusertreetable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTrees", "UserTreeNodePartnerID", c => c.Int());
            CreateIndex("dbo.UserTrees", "UserTreeNodePartnerID");
            AddForeignKey("dbo.UserTrees", "UserTreeNodePartnerID", "dbo.UserTreeNodes", "UserTreeNodeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTrees", "UserTreeNodePartnerID", "dbo.UserTreeNodes");
            DropIndex("dbo.UserTrees", new[] { "UserTreeNodePartnerID" });
            DropColumn("dbo.UserTrees", "UserTreeNodePartnerID");
        }
    }
}
