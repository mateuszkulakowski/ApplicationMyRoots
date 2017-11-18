namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class acceptedagreementnullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserTreeSharingAgreements", "Accpeted", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserTreeSharingAgreements", "Accpeted", c => c.Boolean(nullable: false));
        }
    }
}
