namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fieldacceptedinagreements : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTreeSharingAgreements", "Accepted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTreeSharingAgreements", "Accepted");
        }
    }
}
