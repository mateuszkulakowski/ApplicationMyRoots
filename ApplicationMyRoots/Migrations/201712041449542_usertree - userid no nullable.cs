namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usertreeuseridnonullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserTrees", "UserID", "dbo.Users");
            DropIndex("dbo.UserTrees", new[] { "UserID" });
            AlterColumn("dbo.UserTrees", "UserID", c => c.Int(nullable: false));
            CreateIndex("dbo.UserTrees", "UserID");
            AddForeignKey("dbo.UserTrees", "UserID", "dbo.Users", "UserID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTrees", "UserID", "dbo.Users");
            DropIndex("dbo.UserTrees", new[] { "UserID" });
            AlterColumn("dbo.UserTrees", "UserID", c => c.Int());
            CreateIndex("dbo.UserTrees", "UserID");
            AddForeignKey("dbo.UserTrees", "UserID", "dbo.Users", "UserID");
        }
    }
}
