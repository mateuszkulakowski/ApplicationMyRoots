namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TabelkaUserdodanie : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Password = c.String(),
                        PasswordEncoded = c.Binary(),
                        Name = c.String(),
                        Surname = c.String(),
                        Age = c.Int(nullable: false),
                        DateBorn = c.DateTime(nullable: false),
                        DateSign = c.DateTime(nullable: false),
                        City = c.String(),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
