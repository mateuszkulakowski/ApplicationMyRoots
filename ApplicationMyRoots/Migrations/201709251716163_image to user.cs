namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imagetouser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Image", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Image");
        }
    }
}
