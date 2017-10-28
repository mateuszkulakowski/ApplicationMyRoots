namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nulowalnydatetimeusertresnodes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTreeNodes", "DateDead", c => c.DateTime());
            AlterColumn("dbo.UserTreeNodes", "DateBorn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserTreeNodes", "DateBorn", c => c.DateTime(nullable: false));
            DropColumn("dbo.UserTreeNodes", "DateDead");
        }
    }
}
