namespace Store.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderModelUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Status");
        }
    }
}
