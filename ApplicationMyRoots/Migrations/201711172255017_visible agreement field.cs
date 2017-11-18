namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class visibleagreementfield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTreeSharingAgreements", "Visible", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTreeSharingAgreements", "Visible");
        }
    }
}
