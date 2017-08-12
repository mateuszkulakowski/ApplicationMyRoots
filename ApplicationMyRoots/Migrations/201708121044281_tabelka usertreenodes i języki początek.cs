namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tabelkausertreenodesijęzykipoczątek : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        LanguageID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.LanguageID);
            
            CreateTable(
                "dbo.LanguageTexts",
                c => new
                    {
                        LanguageTextID = c.Int(nullable: false, identity: true),
                        LanguageID = c.Int(nullable: false),
                        UniqueElementNumber = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.LanguageTextID)
                .ForeignKey("dbo.Languages", t => t.LanguageID, cascadeDelete: true)
                .Index(t => t.LanguageID);
            
            CreateTable(
                "dbo.UserTreeNodes",
                c => new
                    {
                        UserTreeNodeID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Surname = c.String(),
                        DateBorn = c.DateTime(nullable: false),
                        Image = c.Binary(),
                    })
                .PrimaryKey(t => t.UserTreeNodeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LanguageTexts", "LanguageID", "dbo.Languages");
            DropIndex("dbo.LanguageTexts", new[] { "LanguageID" });
            DropTable("dbo.UserTreeNodes");
            DropTable("dbo.LanguageTexts");
            DropTable("dbo.Languages");
        }
    }
}
