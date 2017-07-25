namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TabelkaErrorszbłędamipodczaswykonywaniaprogramu : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Errors",
                c => new
                    {
                        ErrorID = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        StackTrace = c.String(),
                        DateThrow = c.DateTime(),
                    })
                .PrimaryKey(t => t.ErrorID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Errors");
        }
    }
}
