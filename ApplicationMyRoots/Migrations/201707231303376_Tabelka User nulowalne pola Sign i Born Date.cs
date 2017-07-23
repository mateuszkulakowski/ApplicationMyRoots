namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TabelkaUsernulowalnepolaSigniBornDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "DateBorn", c => c.DateTime());
            AlterColumn("dbo.Users", "DateSign", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "DateSign", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "DateBorn", c => c.DateTime(nullable: false));
        }
    }
}
