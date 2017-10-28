namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additionalinfotousertreenodes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTreeNodes", "AdditionalInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTreeNodes", "AdditionalInfo");
        }
    }
}
