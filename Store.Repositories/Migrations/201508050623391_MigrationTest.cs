namespace Store.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationTest : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Products");
            AlterColumn("dbo.Products", "Id", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false, maxLength: 40));
            AlterColumn("dbo.Products", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "ImageUrl", c => c.String(maxLength: 255));
            AddPrimaryKey("dbo.Products", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Products");
            AlterColumn("dbo.Products", "ImageUrl", c => c.String());
            AlterColumn("dbo.Products", "Description", c => c.String());
            AlterColumn("dbo.Products", "Name", c => c.String());
            AlterColumn("dbo.Products", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Products", "Id");
        }
    }
}
