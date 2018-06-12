using Microsoft.Practices.Unity;
//using Store.Application.ServiceImplementations;
using Store.Domain.Repositories;
using Store.Domain.Services;
//using Store.Domain.Services;
using Store.Repositories.EntityFramework;
//using Store.Repositories.MongoDb;
//using Store.ServiceContracts;

namespace SqlDataImportToMongo
{

    public class UnityContainerConfig
    {
        public static IUnityContainer RegisterDependency()
        {
            IUnityContainer container = new UnityContainer();
            RegisterTypes(container);
            return container;
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            //仓储上下文
            #region EF Repository 注入
            //单例实体

            container.RegisterType<IRepositoryContext, EntityFrameworkRepositoryContext>(new ContainerControlledLifetimeManager());

            #region Domain Service
            container.RegisterType<IDomainService, DomainService>();
            #endregion

            #region Product
            container.RegisterType<IProductRepository, ProductRepository>();
            //container.RegisterType<IProductService, ProductServiceImp>();
            #endregion

            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterType<IProductCategorizationRepository, ProductCategorizationRepository>();
            #region Order
            container.RegisterType<IOrderRepository, OrderRepository>();
            //container.RegisterType<IOrderService, OrderServiceImpl>();
            #endregion
            container.RegisterType<IShoppingCartRepository, ShoppingCartRepository>();
            container.RegisterType<IShoppingCartItemRepository, ShoppingCartItemRepository>();
            #region User
            container.RegisterType<IUserRepository, UserRepository>();
            //container.RegisterType<IUserService, UserServiceImpl>();
            #endregion
            container.RegisterType<IUserRoleRepository, UserRoleRepository>();
            container.RegisterType<IRoleRepository, RoleRepository>();

            #endregion


            //#region MongoDB Repository 注入

            //container.RegisterType<IMongoDBRepositoryContextSettings, MongoDBRepositoryContextSettings>();
            //container.RegisterType<IRepositoryContext, MongoDBRepositoryContext>(new ContainerControlledLifetimeManager());
            //container.RegisterType<IProductRepository, ProductRepository>();


            //#endregion
        }
    }
}
