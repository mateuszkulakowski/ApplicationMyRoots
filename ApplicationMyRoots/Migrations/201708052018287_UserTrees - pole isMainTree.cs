namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTreespoleisMainTree : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTrees", "isMainTree", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTrees", "isMainTree");
        }
    }
}
