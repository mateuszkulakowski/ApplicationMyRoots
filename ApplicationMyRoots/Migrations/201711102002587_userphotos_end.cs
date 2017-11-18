namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userphotos_end : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTreePhotoes",
                c => new
                {
                    UserTreePhotoID = c.Int(nullable: false, identity: true),
                    UserTreeID = c.Int(nullable: false),
                    Image = c.Binary()
                })
                .PrimaryKey(t => t.UserTreePhotoID)
                .ForeignKey("dbo.UserTrees", t => t.UserTreeID, cascadeDelete: true);

            //DropForeignKey("dbo.UserTreePhotoes", "UserTreeID", "dbo.UserTrees");
            //DropIndex("dbo.UserTreePhotoes", new[] { "UserTreeID" });
            //RenameColumn(table: "dbo.UserTreePhotoes", name: "UserTreeID", newName: "UserTree_UserTreeID");
            //DropPrimaryKey("dbo.UserTreePhotoes");
            //AddColumn("dbo.UserTreePhotoes", "UserTreePhotoID", c => c.Int(nullable: false, identity: true));
            //AlterColumn("dbo.UserTreePhotoes", "UserTree_UserTreeID", c => c.Int());
            //AddPrimaryKey("dbo.UserTreePhotoes", "UserTreePhotoID");
            //CreateIndex("dbo.UserTreePhotoes", "UserTree_UserTreeID");
            //AddForeignKey("dbo.UserTreePhotoes", "UserTree_UserTreeID", "dbo.UserTrees", "UserTreeID");
            //DropColumn("dbo.UserTreePhotoes", "UserTreePhotosID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTreePhotoes", "UserTreeID", "dbo.UserTrees");
            DropTable("dbo.UserTreePhotoes");
            //AddColumn("dbo.UserTreePhotoes", "UserTreePhotosID", c => c.Int(nullable: false, identity: true));
            //DropForeignKey("dbo.UserTreePhotoes", "UserTree_UserTreeID", "dbo.UserTrees");
            //DropIndex("dbo.UserTreePhotoes", new[] { "UserTree_UserTreeID" });
            //DropPrimaryKey("dbo.UserTreePhotoes");
            //AlterColumn("dbo.UserTreePhotoes", "UserTree_UserTreeID", c => c.Int(nullable: false));
            //DropColumn("dbo.UserTreePhotoes", "UserTreePhotoID");
            //AddPrimaryKey("dbo.UserTreePhotoes", "UserTreePhotosID");
            //RenameColumn(table: "dbo.UserTreePhotoes", name: "UserTree_UserTreeID", newName: "UserTreeID");
            //CreateIndex("dbo.UserTreePhotoes", "UserTreeID");
            //AddForeignKey("dbo.UserTreePhotoes", "UserTreeID", "dbo.UserTrees", "UserTreeID", cascadeDelete: true);
        }
    }
}
