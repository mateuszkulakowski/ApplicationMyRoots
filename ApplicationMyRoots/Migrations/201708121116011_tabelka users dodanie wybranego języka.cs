namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tabelkausersdodaniewybranegojęzyka : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LanguageID", c => c.Int(nullable: true));
            CreateIndex("dbo.Users", "LanguageID");
            AddForeignKey("dbo.Users", "LanguageID", "dbo.Languages");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "LanguageID", "dbo.Languages");
            DropIndex("dbo.Users", new[] { "LanguageID" });
            DropColumn("dbo.Users", "LanguageID");
        }
    }
}
