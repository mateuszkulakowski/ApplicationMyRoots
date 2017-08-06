namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zmienionanazwapolatransformMatrix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTrees", "TransformMatrix", c => c.String());
            DropColumn("dbo.UserTrees", "TranslationMatrix");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserTrees", "TranslationMatrix", c => c.String());
            DropColumn("dbo.UserTrees", "TransformMatrix");
        }
    }
}
