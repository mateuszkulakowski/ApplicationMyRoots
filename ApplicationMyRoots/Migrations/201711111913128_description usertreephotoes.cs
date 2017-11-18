namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class descriptionusertreephotoes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTreePhotoes", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTreePhotoes", "Description");
        }
    }
}
