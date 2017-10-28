namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usertreeidwusertreenodes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTreeNodes", "UserTreeID", c => c.Int(nullable: false));
            CreateIndex("dbo.UserTreeNodes", "UserTreeID");
            AddForeignKey("dbo.UserTreeNodes", "UserTreeID", "dbo.UserTrees", "UserTreeID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTreeNodes", "UserTreeID", "dbo.UserTrees");
            DropIndex("dbo.UserTreeNodes", new[] { "UserTreeID" });
            DropColumn("dbo.UserTreeNodes", "UserTreeID");
        }
    }
}
