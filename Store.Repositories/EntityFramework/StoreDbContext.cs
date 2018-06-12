using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Store.Domain.Model;
using Store.Domain.Model.ModelConfigurations;

namespace Store.Repositories.EntityFramework
{
    public sealed class StoreDbContext : DbContext
    {
        #region Ctor
        public StoreDbContext()
            //: base("Store")
            : base("BookStore")
        {
            //是否自动监测变化，默认值为 true
            this.Configuration.AutoDetectChangesEnabled = true;

            //是否启用延迟加载，默认值为 true
            //缺陷：多次与DB交互，性能降低
            //     true - 延迟加载（Lazy Loading）：获取实体时不会加载其导航属性，一旦用到导航属性就会自动加载
            //     false - 直接加载（Eager loading）：通过 Include 之类的方法显示加载导航属性，获取实体时会即时加
            this.Configuration.LazyLoadingEnabled = true;

            //打印sql log
            this.Database.Log = new Action<string>(p => System.Diagnostics.Debug.WriteLine(p));
        }
        #endregion

        #region Public Properties

        //注册Context下所有实体，EF默认使用方式在DbContext下定义实体如DbSet<Product> Products
        // 通过dbContext.Products.Where(... 方式调用，而非dbContext.Set<Product>().Where(...调用，
        // 也可在此手写方法注册，避免改动量过大 可参考  xx.framework.dal下EFDbContext

        // 所有需要关联到 Context 的类都要类似如下代码这样定义(针对每个聚合根都会定义一个DbSet的属性)
        public DbSet<Product> Products
        {
            get { return this.Set<Product>(); }
        }

        public DbSet<Category> Categories
        {
            get { return this.Set<Category>(); }
        }

        public DbSet<User> Users
        {
            get { return Set<User>(); }
        }

        public DbSet<UserRole> UserRoles
        {
            get { return Set<UserRole>(); }
        }

        public DbSet<Role> Roles
        {
            get { return Set<Role>(); }
        }

        public DbSet<ShoppingCart> ShoppingCarts
        {
            get { return Set<ShoppingCart>(); }
        }

        public DbSet<ProductCategorization> ProductCategorizations
        {
            get { return Set<ProductCategorization>(); }
        }
        public DbSet<Order> Orders
        {
            get { return Set<Order>(); }
        }
        #endregion


        #region Protected Methods
        // 
        // http://stackoverflow.com/questions/5270721/using-guid-as-pk-with-ef4-code-first
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Configurations
                .Add(new ProductTypeConfiguration())
                .Add(new UserTypeConfiguration())
                .Add(new RoleTypeConfiguration())
                .Add(new UserRoleTypeConfiguration())
                .Add(new CategoryTypeConfiguration())
                .Add(new ProductCategorizationTypeConfiguration())
                //.Add(new OrderItemTypeConfiguration())
                //.Add(new OrderTypeConfiguration())
                .Add(new ShoppingCartItemTypeConfiguration())
                .Add(new ShoppingCartTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
        #endregion

    }
}
