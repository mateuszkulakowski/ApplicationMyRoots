namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class poleuserIDwtabelcefailedlogin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FailedLogins", "UserID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FailedLogins", "UserID");
        }
    }
}
