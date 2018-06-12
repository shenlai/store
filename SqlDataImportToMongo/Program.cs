using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Practices.Unity;
using Store.Domain.Repositories;
using MongoDB.Bson;
using Store.Domain.Model;
using Store.Domain;

namespace SqlDataImportToMongo
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoDBBootstrapper.Bootstrap();
            var unityContainer = UnityContainerConfig.RegisterDependency();
            //产品
            var product = unityContainer.Resolve<IProductRepository>();
            var productlist = product.GetAll().ToList();
            ConstructMongoDocuments(productlist);

            //类别
            var category = unityContainer.Resolve<ICategoryRepository>();
            var categorylist = category.GetAll().ToList();
            ConstructMongoDocuments(categorylist);

            var order = unityContainer.Resolve<IOrderRepository>();
            //var order1 = order.GetAll();
            //var list = order1.ToList();
            var orderlist = order.GetAll().ToList();
            ConstructMongoDocuments(orderlist);

            var ShoppingCartItem = unityContainer.Resolve<IShoppingCartItemRepository>();
            var ShoppingCartItemlist = ShoppingCartItem.GetAll().ToList();
            ConstructMongoDocuments(ShoppingCartItemlist);

            var User = unityContainer.Resolve<IUserRepository>();
            var Userlist = User.GetAll().ToList();
            ConstructMongoDocuments(Userlist);

            var UserRole = unityContainer.Resolve<IUserRoleRepository>();
            var UserRolelist = UserRole.GetAll().ToList();
            ConstructMongoDocuments(UserRolelist);

            var Role = unityContainer.Resolve<IRoleRepository>();
            var Rolelist = Role.GetAll().ToList();
            ConstructMongoDocuments(Rolelist);

            var ProductCategorization = unityContainer.Resolve<IProductCategorizationRepository>();
            var ProductCategorizationlist = ProductCategorization.GetAll().ToList();
            ConstructMongoDocuments(ProductCategorizationlist);

            var ShoppingCart = unityContainer.Resolve<IShoppingCartRepository>();
            var ShoppingCartlist = ShoppingCart.GetAll().ToList();
            ConstructMongoDocuments(ShoppingCartlist);






            Console.ReadLine();

        }




        public static void ConstructMongoDocuments<T>(List<T> listAggregate) where T:IAggregateRoot
        {
            List<BsonDocument> docus = new List<BsonDocument>();
            var obj = new Object();
            foreach (var item in listAggregate)
            {
                var bsonDoc = new BsonDocument(item.ToBsonDocument());
                docus.Add(bsonDoc);
            }
            MongoHelp.InsertDocuments(docus, typeof(T));
        }
    }
}
