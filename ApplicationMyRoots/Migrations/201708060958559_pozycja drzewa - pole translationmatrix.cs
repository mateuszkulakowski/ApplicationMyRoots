namespace ApplicationMyRoots.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pozycjadrzewapoletranslationmatrix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTrees", "TranslationMatrix", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTrees", "TranslationMatrix");
        }
    }
}
