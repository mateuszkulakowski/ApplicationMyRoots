namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateagreement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTreeSharingAgreements", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTreeSharingAgreements", "Date");
        }
    }
}
