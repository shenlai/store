namespace Store.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 25),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 20),
                        Password = c.String(nullable: false, maxLength: 20),
                        Email = c.String(nullable: false, maxLength: 80),
                        PhoneNumber = c.String(),
                        IsDisabled = c.Boolean(nullable: false),
                        RegisteredDate = c.DateTime(nullable: false, precision: 0, storeType: "datetime2"),
                        LastLogonDate = c.DateTime(),
                        Contact = c.String(),
                        ContactAddress_Country = c.String(),
                        ContactAddress_State = c.String(),
                        ContactAddress_City = c.String(),
                        ContactAddress_Street = c.String(),
                        ContactAddress_Zip = c.String(),
                        DeliveryAddress_Country = c.String(),
                        DeliveryAddress_State = c.String(),
                        DeliveryAddress_City = c.String(),
                        DeliveryAddress_Street = c.String(),
                        DeliveryAddress_Zip = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Roles");
        }
    }
}
