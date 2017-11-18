namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sharingstatuses1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Users", name: "UserTreeSharingStatus_UserTreeSharingStatusID", newName: "UserTreeSharingStatusID");
            RenameIndex(table: "dbo.Users", name: "IX_UserTreeSharingStatus_UserTreeSharingStatusID", newName: "IX_UserTreeSharingStatusID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Users", name: "IX_UserTreeSharingStatusID", newName: "IX_UserTreeSharingStatus_UserTreeSharingStatusID");
            RenameColumn(table: "dbo.Users", name: "UserTreeSharingStatusID", newName: "UserTreeSharingStatus_UserTreeSharingStatusID");
        }
    }
}
