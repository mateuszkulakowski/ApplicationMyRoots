namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nonullablelanguagetreesharingstatus : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "LanguageID", "dbo.Languages");
            DropForeignKey("dbo.Users", "UserTreeSharingStatusID", "dbo.UserTreeSharingStatus");
            DropIndex("dbo.Users", new[] { "LanguageID" });
            DropIndex("dbo.Users", new[] { "UserTreeSharingStatusID" });
            AlterColumn("dbo.Users", "LanguageID", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "UserTreeSharingStatusID", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "LanguageID");
            CreateIndex("dbo.Users", "UserTreeSharingStatusID");
            AddForeignKey("dbo.Users", "LanguageID", "dbo.Languages", "LanguageID", cascadeDelete: true);
            AddForeignKey("dbo.Users", "UserTreeSharingStatusID", "dbo.UserTreeSharingStatus", "UserTreeSharingStatusID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "UserTreeSharingStatusID", "dbo.UserTreeSharingStatus");
            DropForeignKey("dbo.Users", "LanguageID", "dbo.Languages");
            DropIndex("dbo.Users", new[] { "UserTreeSharingStatusID" });
            DropIndex("dbo.Users", new[] { "LanguageID" });
            AlterColumn("dbo.Users", "UserTreeSharingStatusID", c => c.Int());
            AlterColumn("dbo.Users", "LanguageID", c => c.Int());
            CreateIndex("dbo.Users", "UserTreeSharingStatusID");
            CreateIndex("dbo.Users", "LanguageID");
            AddForeignKey("dbo.Users", "UserTreeSharingStatusID", "dbo.UserTreeSharingStatus", "UserTreeSharingStatusID");
            AddForeignKey("dbo.Users", "LanguageID", "dbo.Languages", "LanguageID");
        }
    }
}
