namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class readedagreement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTreeSharingAgreements", "IsReceivedUserRead", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserTreeSharingAgreements", "IsSendedUserRead", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTreeSharingAgreements", "IsSendedUserRead");
            DropColumn("dbo.UserTreeSharingAgreements", "IsReceivedUserRead");
        }
    }
}
