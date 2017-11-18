namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class albumy : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserTreePhotoes", "UserTreeID", "dbo.UserTrees");
            DropIndex("dbo.UserTreePhotoes", new[] { "UserTreeID" });
            CreateTable(
                "dbo.UserTreeAlbums",
                c => new
                    {
                        UserTreeAlbumID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserTreeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTreeAlbumID)
                .ForeignKey("dbo.UserTrees", t => t.UserTreeID, cascadeDelete: true)
                .Index(t => t.UserTreeID);
            
            AddColumn("dbo.UserTreePhotoes", "UserTreeAlbumID", c => c.Int(nullable: false));
            CreateIndex("dbo.UserTreePhotoes", "UserTreeAlbumID");
            AddForeignKey("dbo.UserTreePhotoes", "UserTreeAlbumID", "dbo.UserTreeAlbums", "UserTreeAlbumID", cascadeDelete: true);
            DropColumn("dbo.UserTreePhotoes", "UserTreeID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserTreePhotoes", "UserTreeID", c => c.Int(nullable: false));
            DropForeignKey("dbo.UserTreeAlbums", "UserTreeID", "dbo.UserTrees");
            DropForeignKey("dbo.UserTreePhotoes", "UserTreeAlbumID", "dbo.UserTreeAlbums");
            DropIndex("dbo.UserTreePhotoes", new[] { "UserTreeAlbumID" });
            DropIndex("dbo.UserTreeAlbums", new[] { "UserTreeID" });
            DropColumn("dbo.UserTreePhotoes", "UserTreeAlbumID");
            DropTable("dbo.UserTreeAlbums");
            CreateIndex("dbo.UserTreePhotoes", "UserTreeID");
            AddForeignKey("dbo.UserTreePhotoes", "UserTreeID", "dbo.UserTrees", "UserTreeID", cascadeDelete: true);
        }
    }
}
