namespace Store.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShoppingCartModel : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Categories");
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        DispatchedDate = c.DateTime(),
                        DeliveredDate = c.DateTime(),
                        User_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Order_Id = c.Guid(),
                        Product_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .Index(t => t.Order_Id)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.ProductCategorizations",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CategoryId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShoppingCarts",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        User_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.ShoppingCartItems",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Product_Id = c.Guid(),
                        ShoopingCart_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .ForeignKey("dbo.ShoppingCarts", t => t.ShoopingCart_Id)
                .Index(t => t.Product_Id)
                .Index(t => t.ShoopingCart_Id);
            
            AlterColumn("dbo.Categories", "Id", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.Categories", "Name", c => c.String(nullable: false, maxLength: 25));
            AddPrimaryKey("dbo.Categories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingCartItems", "ShoopingCart_Id", "dbo.ShoppingCarts");
            DropForeignKey("dbo.ShoppingCartItems", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.ShoppingCarts", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Orders", "User_Id", "dbo.Users");
            DropForeignKey("dbo.OrderItems", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.OrderItems", "Order_Id", "dbo.Orders");
            DropIndex("dbo.ShoppingCartItems", new[] { "ShoopingCart_Id" });
            DropIndex("dbo.ShoppingCartItems", new[] { "Product_Id" });
            DropIndex("dbo.ShoppingCarts", new[] { "User_Id" });
            DropIndex("dbo.OrderItems", new[] { "Product_Id" });
            DropIndex("dbo.OrderItems", new[] { "Order_Id" });
            DropIndex("dbo.Orders", new[] { "User_Id" });
            DropPrimaryKey("dbo.Categories");
            AlterColumn("dbo.Categories", "Name", c => c.String());
            AlterColumn("dbo.Categories", "Id", c => c.Guid(nullable: false));
            DropTable("dbo.ShoppingCartItems");
            DropTable("dbo.ShoppingCarts");
            DropTable("dbo.ProductCategorizations");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
            AddPrimaryKey("dbo.Categories", "Id");
        }
    }
}
