namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TabelkaFailedLogins : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FailedLogins",
                c => new
                    {
                        FailedLoginID = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        DateLogin = c.DateTime(),
                    })
                .PrimaryKey(t => t.FailedLoginID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FailedLogins");
        }
    }
}
