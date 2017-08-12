namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zmiananazwypolalanguagetext : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LanguageTexts", "UniqueElementTag", c => c.Int(nullable: false));
            DropColumn("dbo.LanguageTexts", "UniqueElementNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LanguageTexts", "UniqueElementNumber", c => c.Int(nullable: false));
            DropColumn("dbo.LanguageTexts", "UniqueElementTag");
        }
    }
}
